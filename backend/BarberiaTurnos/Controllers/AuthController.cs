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
    private readonly IPasswordHasher _passwordHasher;

    public AuthController(AppDbContext db, IJwtService jwt, IPasswordHasher passwordHasher)
    {
        _db = db;
        _jwt = jwt;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("login")]
    public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
    {
        // Fetch all users to verify hashed PINs
        // Note: This is acceptable for a small number of users.
        var usuarios = await _db.Usuarios.ToListAsync();
        var usuario = usuarios.FirstOrDefault(u => _passwordHasher.Verify(dto.Pin, u.Pin));

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
