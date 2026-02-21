namespace BarberiaTurnos.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty; // "Admin" o "Barbero"
    public bool IsAvailable { get; set; } = false; // true = En turno, false = Fuera de turno
}
