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
    public DbSet<WhatsAppState> WhatsAppStates => Set<WhatsAppState>();
    public DbSet<CierreCaja> CierresCaja => Set<CierreCaja>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Primary key for WhatsAppState
        modelBuilder.Entity<WhatsAppState>()
            .HasKey(w => w.Telefono);
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
            new Servicio { Id = 3, Nombre = "Corte y Barba Marcada", Precio = 23000m, Activo = true },
            new Servicio { Id = 4, Nombre = "Corte y Barba", Precio = 25000m, Activo = true },
            new Servicio { Id = 5, Nombre = "Corte y Cejas", Precio = 20000m, Activo = true },
            new Servicio { Id = 6, Nombre = "Corte y Raya", Precio = 19000m, Activo = true },
            new Servicio { Id = 7, Nombre = "Corte y Figura", Precio = 22000m, Activo = true },
            new Servicio { Id = 8, Nombre = "Despunte de Cabello", Precio = 10000m, Activo = true },
            new Servicio { Id = 9, Nombre = "Barba", Precio = 8000m, Activo = true },
            new Servicio { Id = 10, Nombre = "Cejas con Cuchilla", Precio = 5000m, Activo = true },
            new Servicio { Id = 11, Nombre = "Cejas con Cera", Precio = 10000m, Activo = true }
        );

        // Seed Usuarios (Solo Admin)
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id = 1, Nombre = "Admin", Pin = "0000", Rol = "Admin" }
        );
    }
}
