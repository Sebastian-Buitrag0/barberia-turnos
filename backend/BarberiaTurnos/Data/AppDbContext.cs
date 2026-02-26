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
        // PINs are hashed using PBKDF2 (SHA256, 100000 iterations, 16 bytes salt)
        // "0000" -> Pb8StHPke9OZdLu/QMRAxQ==.iLjwEUoohb3fGjiTkgjFxy/zlmctR1jYaZjsDPK+fL0=
        // "1111" -> cFlY1/cPziXSQYjBoQeIXQ==.2TGMzTPUVC3n5UhhDuPwTR9gupUmbW41xQdbzIkXcLo=
        // "2222" -> DJEYx0i7dXu9Wz6eL8kg4g==.TVnZ5CZhIP3Y5VLuBnJnjLemxRnYjOhaZodsYzIkEUQ=
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id = 1, Nombre = "Admin", Pin = "Pb8StHPke9OZdLu/QMRAxQ==.iLjwEUoohb3fGjiTkgjFxy/zlmctR1jYaZjsDPK+fL0=", Rol = "Admin" },
            new Usuario { Id = 2, Nombre = "Barbero 1", Pin = "cFlY1/cPziXSQYjBoQeIXQ==.2TGMzTPUVC3n5UhhDuPwTR9gupUmbW41xQdbzIkXcLo=", Rol = "Barbero" },
            new Usuario { Id = 3, Nombre = "Barbero 2", Pin = "DJEYx0i7dXu9Wz6eL8kg4g==.TVnZ5CZhIP3Y5VLuBnJnjLemxRnYjOhaZodsYzIkEUQ=", Rol = "Barbero" }
        );
    }
}
