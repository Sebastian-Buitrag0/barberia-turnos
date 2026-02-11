namespace BarberiaTurnos.Models;

public class Turno
{
    public int Id { get; set; }
    public int TurnoDiario { get; set; }
    public string Estado { get; set; } = "EnCola"; // EnCola, Llamado, EnSilla, PorPagar, Finalizado
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public int? BarberoId { get; set; }
    public Usuario? Barbero { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaInicioAtencion { get; set; } // When Barber sets "En Silla"
    public int NivelAviso { get; set; } = 0; // 0=None, 1=Almost, 2=Next
    public decimal? Total { get; set; }
    public List<TurnoDetalle> Detalles { get; set; } = new();
}
