using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberiaTurnos.Data;
using BarberiaTurnos.Models;
using BarberiaTurnos.DTOs;
using BarberiaTurnos.Services;

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
    [HttpGet("barberos")]
    public async Task<ActionResult<List<UsuarioAdminResponseDto>>> GetBarberosAdmin()
    {
        var barberos = await _db.Usuarios
            .Where(u => u.Rol == "Barbero")
            .Select(u => new UsuarioAdminResponseDto(u.Id, u.Nombre, u.Rol, "****", u.IsAvailable))
            .ToListAsync();
        
        return Ok(barberos);
    }

    // POST: api/usuarios/barberos
    [HttpPost("barberos")]
    public async Task<ActionResult<UsuarioAdminResponseDto>> CrearBarbero([FromBody] CrearModificarBarberoDto dto)
    {
        var usuarios = await _db.Usuarios.ToListAsync();
        if (usuarios.Any(u => PasswordHasher.Verify(dto.Pin, u.Pin)))
            return BadRequest(new { message = "El PIN ya está en uso." });

        var nuevoBarbero = new Usuario
        {
            Nombre = dto.Nombre,
            Pin = PasswordHasher.Hash(dto.Pin),
            Rol = "Barbero"
        };

        _db.Usuarios.Add(nuevoBarbero);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBarberosAdmin), new { id = nuevoBarbero.Id }, 
            new UsuarioAdminResponseDto(nuevoBarbero.Id, nuevoBarbero.Nombre, nuevoBarbero.Rol, "****", nuevoBarbero.IsAvailable));
    }

    // PUT: api/usuarios/barberos/{id}
    [HttpPut("barberos/{id}")]
    public async Task<IActionResult> EditarBarbero(int id, [FromBody] CrearModificarBarberoDto dto)
    {
        var barbero = await _db.Usuarios.FindAsync(id);
        
        if (barbero == null || barbero.Rol != "Barbero")
            return NotFound(new { message = "Barbero no encontrado." });

        barbero.Nombre = dto.Nombre;

        if (dto.Pin != "****" && !string.IsNullOrWhiteSpace(dto.Pin))
        {
             var usuarios = await _db.Usuarios.Where(u => u.Id != id).ToListAsync();
             if (usuarios.Any(u => PasswordHasher.Verify(dto.Pin, u.Pin)))
                 return BadRequest(new { message = "El PIN ya está en uso por otro usuario." });

             barbero.Pin = PasswordHasher.Hash(dto.Pin);
        }

        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/usuarios/barberos/{id}
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
