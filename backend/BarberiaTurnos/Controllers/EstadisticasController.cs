using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberiaTurnos.Data;
using BarberiaTurnos.Models;
using System.Security.Claims;

namespace BarberiaTurnos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadisticasController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<EstadisticasController> _logger;

    public EstadisticasController(AppDbContext db, ILogger<EstadisticasController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // GET /api/estadisticas/resumen-hoy
    [Authorize]
    [HttpGet("resumen-hoy")]
    public async Task<ActionResult> GetResumenHoy()
    {
        var hoy = DateTime.UtcNow.Date;

        // Check if there's already a closing for today
        var cierreCaja = await _db.CierresCaja
            .Include(c => c.Admin)
            .FirstOrDefaultAsync(c => c.FechaFiltro == hoy);

        // Calculate current metrics
        var turnosFinalizados = await _db.Turnos
            .Where(t => t.FechaCreacion.Date == hoy && (t.Estado == "Finalizado" || t.Estado == "PorPagar"))
            .ToListAsync();

        decimal totalRecaudado = turnosFinalizados.Sum(t => t.Total ?? 0);
        int cantidadTurnos = turnosFinalizados.Count;

        // Top barberos hoy
        var barberosMetrics = await _db.Turnos
            .Where(t => t.FechaCreacion.Date == hoy && (t.Estado == "Finalizado" || t.Estado == "PorPagar") && t.BarberoId != null)
            .GroupBy(t => t.BarberoId)
            .Select(g => new
            {
                BarberoId = g.Key,
                TotalGenerado = g.Sum(x => x.Total ?? 0),
                CantidadTurnos = g.Count()
            })
            .ToListAsync();

        var barberosDetails = new List<object>();
        foreach(var m in barberosMetrics)
        {
            var b = await _db.Usuarios.FindAsync(m.BarberoId);
            if (b != null)
            {
                barberosDetails.Add(new { Nombre = b.Nombre, Total = m.TotalGenerado, Turnos = m.CantidadTurnos });
            }
        }

        return Ok(new
        {
            fecha = hoy.ToString("yyyy-MM-dd"),
            totalRecaudado,
            cantidadTurnos,
            ticketPromedio = cantidadTurnos > 0 ? totalRecaudado / cantidadTurnos : 0,
            rendimientoBarberos = barberosDetails.OrderByDescending(b => ((dynamic)b).Total),
            cajaCerrada = cierreCaja != null,
            detalleCierre = cierreCaja != null ? new { 
                horaCierre = cierreCaja.FechaCierre,
                admin = cierreCaja.Admin?.Nombre,
                totalCerrado = cierreCaja.TotalRecaudado
            } : null
        });
    }

    // POST /api/estadisticas/cerrar-caja
    [Authorize(Roles = "Admin")]
    [HttpPost("cerrar-caja")]
    public async Task<ActionResult> CerrarCaja()
    {
        var hoy = DateTime.UtcNow.Date;
        
        var existeCierre = await _db.CierresCaja.AnyAsync(c => c.FechaFiltro == hoy);
        if (existeCierre)
        {
            return BadRequest(new { message = "La caja ya fue cerrada el día de hoy." });
        }

        // Get admin ID
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int adminId)) return Unauthorized();

        var turnosHoy = await _db.Turnos
            .Where(t => t.FechaCreacion.Date == hoy && (t.Estado == "Finalizado" || t.Estado == "PorPagar"))
            .ToListAsync();

        decimal total = turnosHoy.Sum(t => t.Total ?? 0);

        var cierre = new CierreCaja
        {
            FechaFiltro = hoy,
            FechaCierre = DateTime.UtcNow,
            TotalRecaudado = total,
            CantidadTurnos = turnosHoy.Count,
            AdminId = adminId
        };

        _db.CierresCaja.Add(cierre);
        
        // Mark all 'PorPagar' as 'Finalizado' to ensure data integrity
        foreach (var t in turnosHoy.Where(t => t.Estado == "PorPagar"))
        {
            t.Estado = "Finalizado";
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation("Cierre de caja completado para {Fecha} por Admin {AdminId}. Total: {Total}", hoy, adminId, total);

        return Ok(new { message = "Caja cerrada exitosamente.", total, turnos = turnosHoy.Count });
    }

    // GET /api/estadisticas/historico
    [Authorize(Roles = "Admin")]
    [HttpGet("historico")]
    public async Task<ActionResult> GetHistorico()
    {
        var cierres = await _db.CierresCaja
            .Include(c => c.Admin)
            .OrderByDescending(c => c.FechaFiltro)
            .Take(30) // Last 30 days
            .Select(c => new
            {
                c.Id,
                Fecha = c.FechaFiltro.ToString("yyyy-MM-dd"),
                c.FechaCierre,
                c.TotalRecaudado,
                c.CantidadTurnos,
                AdminNombre = c.Admin.Nombre
            })
            .ToListAsync();

        return Ok(cierres);
    }
}
