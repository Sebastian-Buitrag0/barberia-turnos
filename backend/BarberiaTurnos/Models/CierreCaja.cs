namespace BarberiaTurnos.Models;

public class CierreCaja
{
    public int Id { get; set; }
    
    // The calendar date the closure represents (e.g. 2026-02-20)
    public DateTime FechaFiltro { get; set; }
    
    // The exact moment the admin clicked "Close Box"
    public DateTime FechaCierre { get; set; } = DateTime.UtcNow;
    
    // Absolute snapshot of total money from "Finalizado" shifts that day
    public decimal TotalRecaudado { get; set; }
    
    // Amount of completed shifts that day
    public int CantidadTurnos { get; set; }
    
    // Which Administrator closed it
    public int AdminId { get; set; }
    public Usuario Admin { get; set; } = null!;
}
