using Microsoft.EntityFrameworkCore;
using BarberiaTurnos.Data;
using BarberiaTurnos.Hubs;
using BarberiaTurnos.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// EF Core + PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// SignalR
builder.Services.AddSignalR();

// WhatsApp Service (Singleton because it manages its own DB scopes via IServiceScopeFactory)
builder.Services.AddSingleton<IWhatsAppService, TwilioWhatsAppService>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// CORS (permitir frontend en desarrollo y producción)
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                     ?? new[] { "http://localhost:5173", "http://127.0.0.1:5173", "http://localhost" };

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .SetIsOriginAllowed(origin => true) // Allows any origin to work with credentials explicitly
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR/Cookies if used
    });
});

var app = builder.Build();

// Auto-apply migrations with retry logic
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var maxRetries = 5;
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            logger.LogInformation($"Attempting to connect to the database and run migrations. Attempt {i + 1}/{maxRetries}");
            db.Database.Migrate();
            logger.LogInformation("Database connected and migrations applied successfully.");
            break; // Success
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, $"Database connection/migration failed on attempt {i + 1}");
            if (i == maxRetries - 1)
            {
                logger.LogError(ex, "Failed to connect to the database after multiple attempts.");
                throw; // Re-throw on last attempt if we absolutely must fail
            }
            Thread.Sleep(TimeSpan.FromSeconds(5)); // Wait before retrying
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication(); // Important: Auth before Authorization
app.UseAuthorization();
app.MapControllers();
app.MapHub<TurnosHub>("/hubs/turnos");

app.Run();
