using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BarberiaTurnos.Data;
using BarberiaTurnos.Models;
using BarberiaTurnos.DTOs;

namespace BarberiaTurnos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsuariosController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/usuarios/barberos
    [Authorize(Roles = "Admin")]
    [HttpGet("barberos")]
    public async Task<ActionResult<List<UsuarioAdminResponseDto>>> GetBarberosAdmin()
    {
        var barberos = await _db.Usuarios
            .Where(u => u.Rol == "Barbero")
            .Select(u => new UsuarioAdminResponseDto(u.Id, u.Nombre, u.Rol, u.IsAvailable))
            .ToListAsync();
        
        return Ok(barberos);
    }

    // POST: api/usuarios/barberos
    [Authorize(Roles = "Admin")]
    [HttpPost("barberos")]
    public async Task<ActionResult<UsuarioAdminResponseDto>> CrearBarbero([FromBody] CrearModificarBarberoDto dto)
    {
        if (string.IsNullOrEmpty(dto.Pin))
            return BadRequest(new { message = "El PIN es obligatorio." });

        if (await _db.Usuarios.AnyAsync(u => u.Pin == dto.Pin))
            return BadRequest(new { message = "El PIN ya está en uso." });

        var nuevoBarbero = new Usuario
        {
            Nombre = dto.Nombre,
            Pin = dto.Pin,
            Rol = "Barbero"
        };

        _db.Usuarios.Add(nuevoBarbero);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBarberosAdmin), new { id = nuevoBarbero.Id }, 
            new UsuarioAdminResponseDto(nuevoBarbero.Id, nuevoBarbero.Nombre, nuevoBarbero.Rol, nuevoBarbero.IsAvailable));
    }

    // PUT: api/usuarios/barberos/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("barberos/{id}")]
    public async Task<IActionResult> EditarBarbero(int id, [FromBody] CrearModificarBarberoDto dto)
    {
        var barbero = await _db.Usuarios.FindAsync(id);
        
        if (barbero == null || barbero.Rol != "Barbero")
            return NotFound(new { message = "Barbero no encontrado." });

        if (!string.IsNullOrEmpty(dto.Pin))
        {
            if (barbero.Pin != dto.Pin && await _db.Usuarios.AnyAsync(u => u.Pin == dto.Pin))
                return BadRequest(new { message = "El PIN ya está en uso por otro usuario." });

            barbero.Pin = dto.Pin;
        }

        barbero.Nombre = dto.Nombre;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/usuarios/barberos/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("barberos/{id}")]
    public async Task<IActionResult> EliminarBarbero(int id)
    {
        var barbero = await _db.Usuarios.FindAsync(id);
        
        if (barbero == null || barbero.Rol != "Barbero")
            return NotFound(new { message = "Barbero no encontrado." });

        // Desvincular turnos del barbero antes de eliminar para no romper la llave foranea
        var turnosAfectados = await _db.Turnos.Where(t => t.BarberoId == id).ToListAsync();
        foreach (var turno in turnosAfectados)
        {
            turno.BarberoId = null;
        }

        _db.Usuarios.Remove(barbero);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/usuarios/me/disponibilidad
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Barbero")]
    [HttpPost("me/disponibilidad")]
    public async Task<ActionResult> ToggleDisponibilidad()
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var barbero = await _db.Usuarios.FindAsync(userId);
        if (barbero == null) return NotFound("Barbero no encontrado");

        barbero.IsAvailable = !barbero.IsAvailable;
        await _db.SaveChangesAsync();

        return Ok(new { isAvailable = barbero.IsAvailable });
    }
}
