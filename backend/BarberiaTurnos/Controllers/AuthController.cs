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

    public AuthController(AppDbContext db, IJwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("login")]
    public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
    {
        // 🛡️ Sentinel: Fetch all users to verify hashed PINs (PIN is the only credential)
        var users = await _db.Usuarios.ToListAsync();
        var usuario = users.FirstOrDefault(u => PasswordHasher.Verify(dto.Pin, u.Pin));

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
