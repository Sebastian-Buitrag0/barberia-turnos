using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using BarberiaTurnos.Data;
using BarberiaTurnos.Models;
using BarberiaTurnos.DTOs;
using BarberiaTurnos.Hubs;
using BarberiaTurnos.Services;
using Microsoft.AspNetCore.Authorization;

namespace BarberiaTurnos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TurnosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IHubContext<TurnosHub> _hub;
    private readonly IWhatsAppService _whatsApp;

    public TurnosController(AppDbContext db, IHubContext<TurnosHub> hub, IWhatsAppService whatsApp)
    {
        _db = db;
        _hub = hub;
        _whatsApp = whatsApp;
    }

    // GET /api/turnos/hoy
    [HttpGet("hoy")]
    public async Task<ActionResult<List<TurnoResponseDto>>> GetTurnosHoy()
    {
        var hoy = DateTime.UtcNow.Date;
        var turnos = await _db.Turnos
            .Include(t => t.Cliente)
            .Include(t => t.Barbero)
            .Include(t => t.Detalles).ThenInclude(d => d.Servicio)
            .Where(t => t.FechaCreacion.Date == hoy) // All turns for today
            .OrderByDescending(t => t.TurnoDiario)
            .Select(t => new TurnoResponseDto(
                t.Id,
                t.TurnoDiario,
                t.Estado,
                t.Cliente.Nombre,
                t.Cliente.Telefono,
                t.Barbero != null ? t.Barbero.Nombre : null,
                t.Total,
                t.FechaCreacion,
                t.Detalles.Select(d => new DetalleDto(d.Servicio.Nombre, d.PrecioCobrado)).ToList()
            ))
            .ToListAsync();

        return Ok(turnos);
    }

    // GET /api/turnos/cola
    [HttpGet("cola")]
    public async Task<ActionResult<List<TurnoResponseDto>>> GetCola()
    {
        var hoy = DateTime.UtcNow.Date;
        var turnos = await _db.Turnos
            .Include(t => t.Cliente)
            .Include(t => t.Barbero)
            .Include(t => t.Detalles).ThenInclude(d => d.Servicio)
            .Where(t => t.FechaCreacion.Date == hoy && t.Estado != "Finalizado")
            .OrderBy(t => t.TurnoDiario)
            .Select(t => new TurnoResponseDto(
                t.Id,
                t.TurnoDiario,
                t.Estado,
                t.Cliente.Nombre,
                t.Cliente.Telefono,
                t.Barbero != null ? t.Barbero.Nombre : null,
                t.Total,
                t.FechaCreacion,
                t.Detalles.Select(d => new DetalleDto(d.Servicio.Nombre, d.PrecioCobrado)).ToList()
            ))
            .ToListAsync();

        return Ok(turnos);
    }

    // GET /api/turnos/porpagar
    [HttpGet("porpagar")]
    public async Task<ActionResult<List<TurnoResponseDto>>> GetPorPagar()
    {
        var hoy = DateTime.UtcNow.Date;
        var turnos = await _db.Turnos
            .Include(t => t.Cliente)
            .Include(t => t.Barbero)
            .Include(t => t.Detalles).ThenInclude(d => d.Servicio)
            .Where(t => t.FechaCreacion.Date == hoy && t.Estado == "PorPagar")
            .OrderBy(t => t.TurnoDiario)
            .Select(t => new TurnoResponseDto(
                t.Id,
                t.TurnoDiario,
                t.Estado,
                t.Cliente.Nombre,
                t.Cliente.Telefono,
                t.Barbero != null ? t.Barbero.Nombre : null,
                t.Total,
                t.FechaCreacion,
                t.Detalles.Select(d => new DetalleDto(d.Servicio.Nombre, d.PrecioCobrado)).ToList()
            ))
            .ToListAsync();

        return Ok(turnos);
    }

    // POST /api/turnos/registrar (cliente se auto-registra via QR o formulario)
    [HttpPost("registrar")]
    public async Task<ActionResult<TurnoResponseDto>> Registrar([FromBody] RegistrarTurnoDto dto)
    {
        // Buscar o crear cliente
        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Telefono == dto.Telefono);
        if (cliente == null)
        {
            cliente = new Cliente { Nombre = dto.Nombre, Telefono = dto.Telefono };
            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync();
        }
        else
        {
            cliente.Nombre = dto.Nombre;
        }

        // Verificar si ya tiene turno activo hoy
        var hoy = DateTime.UtcNow.Date;
        var turnoExistente = await _db.Turnos
            .FirstOrDefaultAsync(t => t.ClienteId == cliente.Id 
                && t.FechaCreacion.Date == hoy
                && t.Estado != "Finalizado");

        if (turnoExistente != null)
        {
            return BadRequest(new { message = "Ya tienes un turno activo para hoy.", turnoId = turnoExistente.Id, turnoDiario = turnoExistente.TurnoDiario });
        }

        // Calcular consecutivo diario
        var ultimoTurno = await _db.Turnos
            .Where(t => t.FechaCreacion.Date == hoy)
            .MaxAsync(t => (int?)t.TurnoDiario) ?? 0;

        var turno = new Turno
        {
            TurnoDiario = ultimoTurno + 1,
            Estado = "EnCola",
            ClienteId = cliente.Id,
            FechaCreacion = DateTime.UtcNow
        };

        _db.Turnos.Add(turno);
        await _db.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("QueueUpdated");

        return Ok(new TurnoResponseDto(
            turno.Id, turno.TurnoDiario, turno.Estado,
            cliente.Nombre, cliente.Telefono, null, null, turno.FechaCreacion, null
        ));
    }

    // POST /api/turnos/registrar-admin (admin agrega a la fila manualmente)
    [HttpPost("registrar-admin")]
    public async Task<ActionResult<TurnoResponseDto>> RegistrarAdmin([FromBody] RegistrarTurnoDto dto)
    {
        // Mismo flujo que registrar, pero sin validación de turno existente
        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Telefono == dto.Telefono);
        if (cliente == null)
        {
            cliente = new Cliente { Nombre = dto.Nombre, Telefono = dto.Telefono };
            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync();
        }
        else
        {
            cliente.Nombre = dto.Nombre;
        }

        var hoy = DateTime.UtcNow.Date;
        var ultimoTurno = await _db.Turnos
            .Where(t => t.FechaCreacion.Date == hoy)
            .MaxAsync(t => (int?)t.TurnoDiario) ?? 0;

        var turno = new Turno
        {
            TurnoDiario = ultimoTurno + 1,
            Estado = "EnCola",
            ClienteId = cliente.Id,
            FechaCreacion = DateTime.UtcNow
        };

        _db.Turnos.Add(turno);
        await _db.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("QueueUpdated");

        return Ok(new TurnoResponseDto(
            turno.Id, turno.TurnoDiario, turno.Estado,
            cliente.Nombre, cliente.Telefono, null, null, turno.FechaCreacion, null
        ));
    }

    // POST /api/turnos/llamar (barbero llama al siguiente)
    [Authorize]
    [HttpPost("llamar")]
    public async Task<ActionResult<TurnoResponseDto>> Llamar([FromBody] LlamarTurnoDto dto)
    {
        // Get ID from JWT Token
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int barberoId)) return Unauthorized();

        var barbero = await _db.Usuarios.FindAsync(barberoId);
        if (barbero == null) return NotFound(new { message = "Barbero no encontrado" });

        var hoy = DateTime.UtcNow.Date;
        var turno = await _db.Turnos
            .Include(t => t.Cliente)
            .Where(t => t.FechaCreacion.Date == hoy && t.Estado == "EnCola")
            .OrderBy(t => t.TurnoDiario)
            .FirstOrDefaultAsync();

        if (turno == null) return NotFound(new { message = "No hay turnos en cola" });

        turno.Estado = "Llamado";
        turno.BarberoId = barberoId;
        await _db.SaveChangesAsync();

        // Enviar WhatsApp
        _ = _whatsApp.SendTurnCallNotification(turno.Cliente.Telefono, turno.TurnoDiario, barbero.Nombre);

        // Check queue notifications for next in line
        _ = CheckQueueNotifications();

        await _hub.Clients.All.SendAsync("QueueUpdated");

        return Ok(new TurnoResponseDto(
            turno.Id, turno.TurnoDiario, turno.Estado,
            turno.Cliente.Nombre, turno.Cliente.Telefono,
            barbero.Nombre, null, turno.FechaCreacion, null
        ));
    }

    // POST /api/turnos/ensilla (barbero confirma que el cliente pasó a la silla)
    [Authorize]
    [HttpPost("ensilla")]
    public async Task<ActionResult> EnSilla([FromBody] CobrarTurnoDto dto)
    {
        var turno = await _db.Turnos.FindAsync(dto.TurnoId);
        if (turno == null) return NotFound();

        turno.Estado = "EnSilla";
        turno.FechaInicioAtencion = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        // Check notifications (maybe others are waiting)
        _ = CheckQueueNotifications();

        await _hub.Clients.All.SendAsync("QueueUpdated");
        return Ok(new { message = "Turno en silla" });
    }

    // POST /api/turnos/finalizar (añade servicios y calcula total)
    [Authorize]
    [HttpPost("finalizar")]
    public async Task<ActionResult> Finalizar([FromBody] FinalizarTurnoDto dto)
    {
        var turno = await _db.Turnos.Include(t => t.Detalles).FirstOrDefaultAsync(t => t.Id == dto.TurnoId);
        if (turno == null) return NotFound();

        var servicios = await _db.Servicios
            .Where(s => dto.ServicioIds.Contains(s.Id) && s.Activo)
            .ToListAsync();

        decimal total = 0;
        foreach (var servicio in servicios)
        {
            turno.Detalles.Add(new TurnoDetalle
            {
                ServicioId = servicio.Id,
                PrecioCobrado = servicio.Precio
            });
            total += servicio.Precio;
        }

        turno.Total = total;
        turno.Estado = "PorPagar";
        await _db.SaveChangesAsync();

        _ = CheckQueueNotifications();

        await _hub.Clients.All.SendAsync("QueueUpdated");
        return Ok(new { message = "Turno finalizado", total });
    }

    // POST /api/turnos/cobrar (admin cobra y cierra el turno)
    [Authorize(Roles = "Admin")]
    [HttpPost("cobrar")]
    public async Task<ActionResult> Cobrar([FromBody] CobrarTurnoDto dto)
    {
        var turno = await _db.Turnos.FindAsync(dto.TurnoId);
        if (turno == null) return NotFound();

        turno.Estado = "Finalizado";
        await _db.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("QueueUpdated");
        return Ok(new { message = "Turno cobrado y cerrado" });
    }

    // GET /api/turnos/mi-turno/{telefono}
    [HttpGet("mi-turno/{telefono}")]
    public async Task<ActionResult> MiTurno(string telefono)
    {
        var hoy = DateTime.UtcNow.Date;
        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Telefono == telefono);
        if (cliente == null) return NotFound(new { message = "No registrado" });

        var turno = await _db.Turnos
            .Include(t => t.Barbero)
            .FirstOrDefaultAsync(t => t.ClienteId == cliente.Id
                && t.FechaCreacion.Date == hoy
                && t.Estado != "Finalizado");

        if (turno == null) return NotFound(new { message = "No tienes turno activo" });

        var personasDelante = await _db.Turnos
            .CountAsync(t => t.FechaCreacion.Date == hoy
                && t.Estado == "EnCola"
                && t.TurnoDiario < turno.TurnoDiario);

        return Ok(new
        {
            turnoId = turno.Id,
            turnoDiario = turno.TurnoDiario,
            estado = turno.Estado,
            personasDelante,
            barberoNombre = turno.Barbero?.Nombre
        });
    }

    private async Task CheckQueueNotifications()
    {
        var hoy = DateTime.UtcNow.Date;
        var cola = await _db.Turnos
            .Include(t => t.Cliente)
            .Where(t => t.FechaCreacion.Date == hoy && t.Estado == "EnCola")
            .OrderBy(t => t.TurnoDiario)
            .Take(2)
            .ToListAsync();

        if (cola.Count == 0) return;

        // Position 1 (Next)
        var next = cola[0];
        if (next.NivelAviso < 2)
        {
             await _whatsApp.SendNextInLineNotification(next.Cliente.Telefono, next.TurnoDiario);
             next.NivelAviso = 2;
        }

        // Position 2 (Approaching)
        if (cola.Count > 1)
        {
             var second = cola[1];
             if (second.NivelAviso < 1)
             {
                 var busyBarbers = await _db.Turnos
                     .Where(t => t.FechaCreacion.Date == hoy && t.Estado == "EnSilla" && t.FechaInicioAtencion != null)
                     .ToListAsync();

                 // Check if any barber has been working > 20 mins
                 var anyLongRunning = busyBarbers.Any(t => (DateTime.UtcNow - t.FechaInicioAtencion.Value).TotalMinutes >= 20);

                 if (anyLongRunning)
                 {
                      await _whatsApp.SendApproachingNotification(second.Cliente.Telefono, second.TurnoDiario);
                      second.NivelAviso = 1;
                 }
             }
        }
        await _db.SaveChangesAsync();
    }
}
