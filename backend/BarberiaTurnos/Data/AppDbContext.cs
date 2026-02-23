using Microsoft.EntityFrameworkCore;
using BarberiaTurnos.Models;

namespace BarberiaTurnos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<Turno> Turnos => Set<Turno>();
    public DbSet<TurnoDetalle> TurnoDetalles => Set<TurnoDetalle>();
    public DbSet<CierreCaja> CierresCaja => Set<CierreCaja>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique index on Cliente.Telefono
        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Telefono)
            .IsUnique();

        // Turno relationships
        modelBuilder.Entity<Turno>()
            .HasOne(t => t.Cliente)
            .WithMany()
            .HasForeignKey(t => t.ClienteId);

        modelBuilder.Entity<Turno>()
            .HasOne(t => t.Barbero)
            .WithMany()
            .HasForeignKey(t => t.BarberoId)
            .IsRequired(false);

        modelBuilder.Entity<TurnoDetalle>()
            .HasOne(td => td.Turno)
            .WithMany(t => t.Detalles)
            .HasForeignKey(td => td.TurnoId);

        modelBuilder.Entity<TurnoDetalle>()
            .HasOne(td => td.Servicio)
            .WithMany()
            .HasForeignKey(td => td.ServicioId);

        // Seed Servicios
        modelBuilder.Entity<Servicio>().HasData(
            new Servicio { Id = 1, Nombre = "Corte Clásico", Precio = 15000m, Activo = true },
            new Servicio { Id = 2, Nombre = "Corte Desvanecido", Precio = 17000m, Activo = true },
            new Servicio { Id = 3, Nombre = "Corte y Barba", Precio = 25000m, Activo = true },
            new Servicio { Id = 4, Nombre = "Cejas Hombre", Precio = 3000m, Activo = true },
            new Servicio { Id = 5, Nombre = "Barba", Precio = 8000m, Activo = true }
        );

        // Seed Usuarios (Admin + 2 Barberos)
        // PINs are hashed using PBKDF2 (SHA256, 100000 iterations)
        // 0000 -> Y9Vj6LPdjfNzr7NJ2CFuBw==.UAa4BXP60rwyxn/Y3l1exdhNzluv7XIWzLi+VR+mulY=
        // 1111 -> aa7DsBH4T/r3+sAVkd/7yA==.9uyXeRuanReUYvOaTnU9bQyuP6uTpbMpGIHRewlKiDg=
        // 2222 -> xfC39ipE1KkemplJw7kPlw==.x6wwLvSpLs1+LJbLN0Go7XCcCL8Uw5+goo950FP0frY=
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id = 1, Nombre = "Admin", Pin = "Y9Vj6LPdjfNzr7NJ2CFuBw==.UAa4BXP60rwyxn/Y3l1exdhNzluv7XIWzLi+VR+mulY=", Rol = "Admin" },
            new Usuario { Id = 2, Nombre = "Barbero 1", Pin = "aa7DsBH4T/r3+sAVkd/7yA==.9uyXeRuanReUYvOaTnU9bQyuP6uTpbMpGIHRewlKiDg=", Rol = "Barbero" },
            new Usuario { Id = 3, Nombre = "Barbero 2", Pin = "xfC39ipE1KkemplJw7kPlw==.x6wwLvSpLs1+LJbLN0Go7XCcCL8Uw5+goo950FP0frY=", Rol = "Barbero" }
        );
    }
}
