namespace BarberiaTurnos.Models;

public class WhatsAppState
{
    public string Telefono { get; set; } = string.Empty; // Primary key (e.g., "+573001234567")
    public string EstadoActual { get; set; } = "Inicio"; 
    // Estados: Inicio, EsperandoRespuestaCorte, EsperandoBarbero, EsperandoDia, EsperandoFechaEspecifica, EsperandoNombre
    public string? NombreTemporal { get; set; }
    public int? BarberoIdTemporal { get; set; }
    public DateTime? DiaTurnoTemporal { get; set; }
    
    // Almacena los IDs de los turnos que el usuario puede cancelar, separados por coma (ej: "12,15")
    public string? CancelacionPendienteIds { get; set; }
    
    public DateTime UltimaInteraccion { get; set; } = DateTime.UtcNow;
}
