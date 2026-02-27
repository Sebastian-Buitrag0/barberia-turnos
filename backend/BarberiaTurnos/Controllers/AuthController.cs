using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberiaTurnos.Data;
using BarberiaTurnos.DTOs;
using BarberiaTurnos.Services;

namespace BarberiaTurnos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtService _jwt;
    private readonly IPasswordHasher _hasher;

    public AuthController(AppDbContext db, IJwtService jwt, IPasswordHasher hasher)
    {
        _db = db;
        _jwt = jwt;
        _hasher = hasher;
    }

    [HttpPost("login")]
    public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
    {
        // Recuperar todos los usuarios para verificar el PIN (ya que está hasheado)
        // Nota: En un sistema más grande, se debería usar username/email para filtrar primero.
        var usuarios = await _db.Usuarios.ToListAsync();
        var usuario = usuarios.FirstOrDefault(u => _hasher.VerifyPassword(dto.Pin, u.Pin));

        if (usuario == null)
            return Unauthorized(new { message = "PIN incorrecto" });

        var token = _jwt.GenerateToken(usuario);

        return Ok(new 
        { 
            user = new UsuarioResponseDto(usuario.Id, usuario.Nombre, usuario.Rol, usuario.IsAvailable),
            token 
        });
    }
}
