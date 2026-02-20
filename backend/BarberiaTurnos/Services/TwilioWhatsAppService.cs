using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace BarberiaTurnos.Services;

public class TwilioWhatsAppService : IWhatsAppService
{
    private readonly IConfiguration _config;
    private readonly ILogger<TwilioWhatsAppService> _logger;

    public TwilioWhatsAppService(IConfiguration config, ILogger<TwilioWhatsAppService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendTurnCallNotification(string telefono, int turnoDiario, string barberoNombre)
    {
        await SendMessageAsync(telefono, $"🪒 ¡Es tu turno! Turno #{turnoDiario}. {barberoNombre} te está esperando. ¡Pasa a la silla!");
    }

    public async Task SendFirstInLineNotification(string telefono, int turnoDiario)
    {
        await SendMessageAsync(telefono, $"🚨 ¡Estás de primero en la fila (Turno #{turnoDiario})! Atento al próximo llamado del barbero.");
    }

    public async Task SendNextInLineNotification(string telefono, int turnoDiario)
    {
        await SendMessageAsync(telefono, $"⏳ ¡Prepárate! Eres el siguiente (Turno #{turnoDiario}). Por favor acércate a la zona de espera.");
    }

    public async Task SendApproachingNotification(string telefono, int turnoDiario)
    {
        await SendMessageAsync(telefono, $"👀 Atento: Tu turno #{turnoDiario} se aproxima. Los barberos están terminando sus cortes.");
    }

    private async Task SendMessageAsync(string telefono, string body)
    {
        try
        {
            var accountSid = _config["Twilio:AccountSid"];
            var authToken = _config["Twilio:AuthToken"];
            var fromNumber = _config["Twilio:FromNumber"];

            if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromNumber))
            {
                _logger.LogWarning("Twilio no configurado. Mensaje no enviado a {Telefono}", telefono);
                return;
            }

            TwilioClient.Init(accountSid, authToken);

            var message = await MessageResource.CreateAsync(
                to: new PhoneNumber($"whatsapp:+{telefono.TrimStart('+')}"),
                from: new PhoneNumber($"whatsapp:{fromNumber}"),
                body: body
            );

            _logger.LogInformation("WhatsApp enviado a {Telefono}. SID: {Sid}", telefono, message.Sid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando WhatsApp a {Telefono}", telefono);
        }
    }
}
