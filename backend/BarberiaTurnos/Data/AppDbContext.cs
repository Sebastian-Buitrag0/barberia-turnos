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

        // Seed Usuarios (Admin + 2 Barberos) - PINs are hashed (PBKDF2)
        // 0000 -> x9X3VhoQbtpKPoEFTBa9Jg==.ZTD76QHcYwdtqgS3lZMmt8YM2B0wHtcjzOr+QtxMaak=
        // 1111 -> xbi/qzvW133SbCAC2wwrfQ==.NRHoKtUjfWP3659XDRbw2cw1V1LQ/l83L2TInVok6EA=
        // 2222 -> Ad39b3TAlOI6bVo5b5ljgQ==.d/ADKEn0cw3q201UNPDjLxJr2cIpdqqyAU5DFhneVvU=
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id = 1, Nombre = "Admin", Pin = "x9X3VhoQbtpKPoEFTBa9Jg==.ZTD76QHcYwdtqgS3lZMmt8YM2B0wHtcjzOr+QtxMaak=", Rol = "Admin" },
            new Usuario { Id = 2, Nombre = "Barbero 1", Pin = "xbi/qzvW133SbCAC2wwrfQ==.NRHoKtUjfWP3659XDRbw2cw1V1LQ/l83L2TInVok6EA=", Rol = "Barbero" },
            new Usuario { Id = 3, Nombre = "Barbero 2", Pin = "Ad39b3TAlOI6bVo5b5ljgQ==.d/ADKEn0cw3q201UNPDjLxJr2cIpdqqyAU5DFhneVvU=", Rol = "Barbero" }
        );
    }
}
