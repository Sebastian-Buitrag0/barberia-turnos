namespace BarberiaTurnos.Services;

public interface IWhatsAppService
{
    Task SendTurnCallNotification(string telefono, int turnoDiario, string barberoNombre);
    Task SendNextInLineNotification(string telefono, int turnoDiario);
    Task SendApproachingNotification(string telefono, int turnoDiario);
}
