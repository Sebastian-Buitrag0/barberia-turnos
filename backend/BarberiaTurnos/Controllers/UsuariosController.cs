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
    private readonly IPasswordHasher _passwordHasher;

    public UsuariosController(AppDbContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    // GET: api/usuarios/barberos
    [HttpGet("barberos")]
    public async Task<ActionResult<List<UsuarioAdminResponseDto>>> GetBarberosAdmin()
    {
        var barberos = await _db.Usuarios
            .Where(u => u.Rol == "Barbero")
            .ToListAsync();

        // Return placeholder for PIN to avoid exposing hash
        var dtos = barberos.Select(u => new UsuarioAdminResponseDto(u.Id, u.Nombre, u.Rol, "****", u.IsAvailable)).ToList();
        
        return Ok(dtos);
    }

    // POST: api/usuarios/barberos
    [HttpPost("barberos")]
    public async Task<ActionResult<UsuarioAdminResponseDto>> CrearBarbero([FromBody] CrearModificarBarberoDto dto)
    {
        // Check uniqueness by iterating all users and verifying the PIN
        var usuarios = await _db.Usuarios.ToListAsync();
        if (usuarios.Any(u => _passwordHasher.Verify(dto.Pin, u.Pin)))
            return BadRequest(new { message = "El PIN ya está en uso." });

        var nuevoBarbero = new Usuario
        {
            Nombre = dto.Nombre,
            Pin = _passwordHasher.Hash(dto.Pin),
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

        // Handle PIN update only if it's not the placeholder
        if (dto.Pin != "****")
        {
            // Check if PIN matches existing hash (in case they typed the same pin again, no need to change or re-hash if verifying works)
            bool pinChanged = !_passwordHasher.Verify(dto.Pin, barbero.Pin);

            if (pinChanged)
            {
                // Check uniqueness against other users
                var usuarios = await _db.Usuarios.Where(u => u.Id != id).ToListAsync();
                if (usuarios.Any(u => _passwordHasher.Verify(dto.Pin, u.Pin)))
                    return BadRequest(new { message = "El PIN ya está en uso por otro usuario." });

                barbero.Pin = _passwordHasher.Hash(dto.Pin);
            }
        }

        barbero.Nombre = dto.Nombre;

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
