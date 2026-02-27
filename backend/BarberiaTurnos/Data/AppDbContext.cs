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
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id = 1, Nombre = "Admin", Pin = "/p09UPopWPFoxMD4dSE1Bw==.ovZz1gBUYfR77CEjdIfAaVpPNeTofnpzSGmVlW4upyo=", Rol = "Admin" },
            new Usuario { Id = 2, Nombre = "Barbero 1", Pin = "5lh567xfkkl2xYEfR/fOkA==.xVhyAWJWFOLKO1kebfjodq0lKbZYpRFEVmZGZa/L2S0=", Rol = "Barbero" },
            new Usuario { Id = 3, Nombre = "Barbero 2", Pin = "x7oXPyKRHUikgTAaj9q22A==.HI3Dd0dgahtKj/HaywAbX7u95Lh1lMLEY2tglXgpqG4=", Rol = "Barbero" }
        );
    }
}
