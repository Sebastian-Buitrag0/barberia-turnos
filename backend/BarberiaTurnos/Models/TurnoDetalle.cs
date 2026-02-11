namespace BarberiaTurnos.Models;

public class TurnoDetalle
{
    public int Id { get; set; }
    public int TurnoId { get; set; }
    public Turno Turno { get; set; } = null!;
    public int ServicioId { get; set; }
    public Servicio Servicio { get; set; } = null!;
    public decimal PrecioCobrado { get; set; }
}
