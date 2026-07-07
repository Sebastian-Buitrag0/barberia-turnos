using Microsoft.AspNetCore.Mvc;
using BarberiaTurnos.Services;
using Twilio.Security;

namespace BarberiaTurnos.Controllers;

[ApiController]
[Route("api/twilio")]
public class TwilioWebhookController : ControllerBase
{
    private readonly IWhatsAppService _whatsApp;
    private readonly IConfiguration _config;
    private readonly ILogger<TwilioWebhookController> _logger;
    private readonly IWebHostEnvironment _env;

    public TwilioWebhookController(
        IWhatsAppService whatsApp,
        IConfiguration config,
        ILogger<TwilioWebhookController> logger,
        IWebHostEnvironment env)
    {
        _whatsApp = whatsApp;
        _config = config;
        _logger = logger;
        _env = env;
    }

    /// <summary>
    /// Twilio calls this endpoint every time someone sends a WhatsApp message.
    /// Configure this URL in the Twilio console under WhatsApp → Sandboxes → "When a message comes in".
    /// </summary>
    [HttpPost("webhook")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Webhook([FromForm] IFormCollection form)
    {
        // Validate that the request comes from Twilio
        if (!ValidateTwilioRequest())
        {
            _logger.LogWarning("Request to Twilio webhook failed signature validation.");
            return Forbid();
        }

        var from = form["From"].ToString(); // e.g. "whatsapp:+573001234567"
        var body = form["Body"].ToString();

        _logger.LogInformation("Incoming WhatsApp message from {From}: {Body}", from, body);

        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(body))
        {
            return Ok(); // Ignore empty messages
        }

        // Normalize the phone number — strip the "whatsapp:" prefix
        var telefono = from.Replace("whatsapp:", "").Trim();

        try
        {
            // Process the message through the state machine and get the reply
            var reply = await _whatsApp.ProcessIncomingMessageAsync(telefono, body);

            // Respond to Twilio using TwiML so Twilio sends the reply back immediately
            var twiml = $"""
                <?xml version="1.0" encoding="UTF-8"?>
                <Response>
                  <Message>{System.Net.WebUtility.HtmlEncode(reply)}</Message>
                </Response>
                """;

            return Content(twiml, "application/xml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico procesando mensaje de WhatsApp de {From}", from);
            
            var errorTwiml = """
                <?xml version="1.0" encoding="UTF-8"?>
                <Response>
                  <Message>⚠️ Lo sentimos, ocurrió un error procesando tu mensaje. Por favor intenta de nuevo más tarde o escribe *hola* para reiniciar.</Message>
                </Response>
                """;
            return Content(errorTwiml, "application/xml");
        }
    }

    private bool ValidateTwilioRequest()
    {
        var authToken = _config["Twilio:AuthToken"];
        if (string.IsNullOrEmpty(authToken))
        {
            if (_env.IsDevelopment())
            {
                // In development, if Twilio isn't configured, skip validation (local dev fallback)
                return true;
            }

            // In production, missing token is a configuration error and should fail securely
            _logger.LogError("Twilio:AuthToken is missing in production. Failing webhook validation securely.");
            return false;
        }

        try
        {
            var validator = new RequestValidator(authToken);
            var url = $"{Request.Scheme}://{Request.Host}{Request.Path}";

            var parameters = Request.Form.ToDictionary(
                x => x.Key,
                x => x.Value.ToString());

            var signature = Request.Headers["X-Twilio-Signature"].ToString();

            return validator.Validate(url, parameters, signature);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Twilio signature");
            return false;
        }
    }
}
