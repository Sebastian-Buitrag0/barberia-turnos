using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.EntityFrameworkCore;
using BarberiaTurnos.Data;
using BarberiaTurnos.Models;
using Microsoft.AspNetCore.SignalR;
using BarberiaTurnos.Hubs;

namespace BarberiaTurnos.Services;

public class TwilioWhatsAppService : IWhatsAppService
{
    private readonly IConfiguration _config;
    private readonly ILogger<TwilioWhatsAppService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<TurnosHub> _hub;

    public TwilioWhatsAppService(
        IConfiguration config,
        ILogger<TwilioWhatsAppService> logger,
        IServiceScopeFactory scopeFactory,
        IHubContext<TurnosHub> hub)
    {
        _config = config;
        _logger = logger;
        _scopeFactory = scopeFactory;
        _hub = hub;
    }

    // ─── Outbound Notifications ─────────────────────────────────────────────
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

    // ─── Chatbot State Machine ───────────────────────────────────────────────
    public async Task<string> ProcessIncomingMessageAsync(string telefono, string body)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var input = body.Trim().ToLower();

        // ── Global command: "cancelar" at any point in the conversation ──────
        if (input.Contains("cancelar") || input.Contains("cancela"))
        {
            return await HandleCancelar(telefono, db);
        }

        // Get or create conversation state for this phone number
        var state = await db.WhatsAppStates.FirstOrDefaultAsync(w => w.Telefono == telefono);
        if (state == null)
        {
            state = new WhatsAppState { Telefono = telefono, EstadoActual = "Inicio" };
            db.WhatsAppStates.Add(state);
        }

        state.UltimaInteraccion = DateTime.UtcNow;

        string respuesta;

        switch (state.EstadoActual)
        {
            case "Inicio":
                respuesta = await HandleInicio(state, input, db);
                break;
            case "EsperandoRespuestaCorte":
                respuesta = await HandleRespuestaCorte(state, input, db);
                break;
            case "EsperandoBarbero":
                respuesta = await HandleBarbero(state, input, db);
                break;
            case "EsperandoDia":
                respuesta = await HandleDia(state, input, db);
                break;
            case "EsperandoFechaEspecifica":
                respuesta = await HandleFechaEspecifica(state, input, db);
                break;
            case "EsperandoNombre":
                respuesta = await HandleNombre(state, input, db);
                break;
            default:
                state.EstadoActual = "Inicio";
                respuesta = await HandleInicio(state, input, db);
                break;
        }

        await db.SaveChangesAsync();
        return respuesta;
    }

    // ─── Step Handlers ───────────────────────────────────────────────────────

    private Task<string> HandleInicio(WhatsAppState state, string input, AppDbContext db)
    {
        state.EstadoActual = "EsperandoRespuestaCorte";
        // Reset any leftover temp data
        state.NombreTemporal = null;
        state.BarberoIdTemporal = null;
        state.DiaTurnoTemporal = null;

        return Task.FromResult(
            "✂️ ¡Hola! Bienvenido a *Barbería*. ¿Necesitas un turno para tu corte?\n\n" +
            "Responde:\n*1* - Sí, quiero un turno\n*2* - No, gracias\n\n" +
            "_En cualquier momento escribe *cancelar* para anular tu turno activo._");
    }

    private async Task<string> HandleRespuestaCorte(WhatsAppState state, string input, AppDbContext db)
    {
        if (input is "2" or "no")
        {
            state.EstadoActual = "Inicio";
            return "¡Hasta luego! Cuando quieras puedes escribirme de nuevo. 👋";
        }

        if (input is not "1" and not "si" and not "sí")
        {
            return "Por favor responde *1* (Sí) o *2* (No).";
        }

        // Fetch available barbers
        var barberos = await db.Usuarios
            .Where(u => u.Rol == "Barbero" && u.IsAvailable)
            .OrderBy(u => u.Nombre)
            .ToListAsync();

        state.EstadoActual = "EsperandoBarbero";

        if (barberos.Count == 0)
        {
            // No barbers available, skip barber selection
            state.BarberoIdTemporal = null;
            state.EstadoActual = "EsperandoDia";
            return "Actualmente no hay barberos disponibles en este momento, pero podemos agendarte el turno. ¿Para qué día lo necesitas?\n\n*1* - Hoy\n*2* - Otro día";
        }

        var opciones = string.Join("\n", barberos.Select((b, i) => $"*{i + 1}* - {b.Nombre}"));
        return $"¿Con qué barbero deseas tu corte?\n\n{opciones}\n*0* - Cualquiera está bien";
    }

    private async Task<string> HandleBarbero(WhatsAppState state, string input, AppDbContext db)
    {
        var barberos = await db.Usuarios
            .Where(u => u.Rol == "Barbero" && u.IsAvailable)
            .OrderBy(u => u.Nombre)
            .ToListAsync();

        if (input == "0")
        {
            state.BarberoIdTemporal = null;
        }
        else if (int.TryParse(input, out int idx) && idx >= 1 && idx <= barberos.Count)
        {
            state.BarberoIdTemporal = barberos[idx - 1].Id;
        }
        else
        {
            var opciones = string.Join("\n", barberos.Select((b, i) => $"*{i + 1}* - {b.Nombre}"));
            return $"Opción no válida. Por favor elige:\n\n{opciones}\n*0* - Cualquiera está bien";
        }

        state.EstadoActual = "EsperandoDia";
        return "¿Para qué día necesitas el turno?\n\n*1* - Hoy\n*2* - Otro día";
    }

    private Task<string> HandleDia(WhatsAppState state, string input, AppDbContext db)
    {
        if (input == "1" || input.Contains("hoy"))
        {
            state.DiaTurnoTemporal = DateTime.UtcNow.Date;
            state.EstadoActual = "EsperandoNombre";
            return Task.FromResult("¡Perfecto! Por último, ¿cuál es tu nombre? (Para que el barbero te llame)");
        }
        else if (input == "2" || input.Contains("otro") || input.Contains("especif"))
        {
            state.EstadoActual = "EsperandoFechaEspecifica";
            return Task.FromResult("¿Para qué fecha? Escríbela en formato *dd/mm/aaaa* (Ej: *15/03/2026*)");
        }
        else
        {
            return Task.FromResult("Por favor responde *1* (Hoy) o *2* (Otro día).");
        }
    }

    private Task<string> HandleFechaEspecifica(WhatsAppState state, string input, AppDbContext db)
    {
        // Accept d/M/yyyy, dd/MM/yyyy, d-M-yyyy, dd-MM-yyyy variants
        var normalized = input.Replace("-", "/");
        string[] formats = { "d/M/yyyy", "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy" };

        if (DateTime.TryParseExact(normalized, formats,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out DateTime fecha))
        {
            if (fecha.Date < DateTime.UtcNow.Date)
            {
                return Task.FromResult("❌ Esa fecha ya pasó. Por favor escribe una fecha futura (formato *dd/mm/aaaa*).");
            }

            state.DiaTurnoTemporal = fecha.Date;
            state.EstadoActual = "EsperandoNombre";
            return Task.FromResult($"✅ Perfecto, agendamos para el *{fecha:dd/MM/yyyy}*.\n¿Cuál es tu nombre?");
        }

        return Task.FromResult("No entendí la fecha 😅 Escríbela así: *09/03/2026* (día/mes/año)");
    }

    private async Task<string> HandleNombre(WhatsAppState state, string input, AppDbContext db)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
        {
            return "Por favor escribe tu nombre completo.";
        }

        var nombre = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        var diaTurno = state.DiaTurnoTemporal ?? DateTime.UtcNow.Date;

        // Find or create client — never overwrite existing name
        var cliente = await db.Clientes.FirstOrDefaultAsync(c => c.Telefono == state.Telefono);
        if (cliente == null)
        {
            cliente = new Cliente { Nombre = nombre, Telefono = state.Telefono };
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();
        }
        // If the client already exists we keep their stored name

        // Check for existing active turn on that day
        var turnoExistente = await db.Turnos
            .FirstOrDefaultAsync(t =>
                t.ClienteId == cliente.Id &&
                t.DiaTurno == diaTurno &&
                t.Estado != "Finalizado");

        if (turnoExistente != null)
        {
            state.EstadoActual = "Inicio";
            return $"⚠️ {nombre}, ya tienes un turno agendado para ese día (Turno #{turnoExistente.TurnoDiario}). ¡Te avisaremos cuando sea tu momento!\n\n_Escribe *cancelar* si quieres anularlo._";
        }

        // Assign daily turn number
        var ultimoTurno = await db.Turnos
            .Where(t => t.DiaTurno == diaTurno)
            .MaxAsync(t => (int?)t.TurnoDiario) ?? 0;

        var turno = new Turno
        {
            TurnoDiario = ultimoTurno + 1,
            Estado = "EnCola",
            ClienteId = cliente.Id,
            BarberoId = state.BarberoIdTemporal,
            FechaCreacion = DateTime.UtcNow,
            DiaTurno = diaTurno
        };

        db.Turnos.Add(turno);
        await db.SaveChangesAsync();

        // 🔴 Notify all connected dashboard clients via SignalR
        await _hub.Clients.All.SendAsync("QueueUpdated");

        // Get barber name if selected
        string barberoTexto = "cualquier barbero";
        if (state.BarberoIdTemporal.HasValue)
        {
            var barbero = await db.Usuarios.FindAsync(state.BarberoIdTemporal.Value);
            if (barbero != null) barberoTexto = barbero.Nombre;
        }

        // Reset state
        state.EstadoActual = "Inicio";
        state.NombreTemporal = null;
        state.BarberoIdTemporal = null;
        state.DiaTurnoTemporal = null;

        return $"✅ ¡Listo, {nombre}! Tu turno ha sido agendado:\n\n" +
               $"🔢 Turno: *#{turno.TurnoDiario}*\n" +
               $"📅 Día: *{diaTurno:dd/MM/yyyy}*\n" +
               $"💈 Barbero: *{barberoTexto}*\n\n" +
               $"Te avisaremos por aquí cuando se acerque tu turno. ¡Hasta pronto! 🙌\n\n" +
               $"_Si necesitas cancelar escribe *cancelar*._";
    }

    // ─── Cancelar turno activo ────────────────────────────────────────────────
    private async Task<string> HandleCancelar(string telefono, AppDbContext db)
    {
        var hoy = DateTime.UtcNow.Date;

        var turno = await db.Turnos
            .Include(t => t.Cliente)
            .FirstOrDefaultAsync(t =>
                t.Cliente.Telefono == telefono &&
                t.DiaTurno == hoy &&
                (t.Estado == "EnCola" || t.Estado == "Llamado"));

        if (turno == null)
        {
            return "ℹ️ No encontré un turno activo para cancelar hoy.";
        }

        var numeroCancelado = turno.TurnoDiario;
        db.Turnos.Remove(turno);
        await db.SaveChangesAsync();

        // 🔴 Notify all connected dashboard clients via SignalR
        await _hub.Clients.All.SendAsync("QueueUpdated");

        return $"✅ Tu turno *#{numeroCancelado}* ha sido cancelado. Si deseas un nuevo turno, escríbeme cuando quieras. 👋";
    }

    // ─── Core Send Logic ─────────────────────────────────────────────────────

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
