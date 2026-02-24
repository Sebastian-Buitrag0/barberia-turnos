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
        var usuarios = await _db.Usuarios.ToListAsync();
        var usuario = usuarios.FirstOrDefault(u => PasswordHasher.Verify(u.Pin, dto.Pin));

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
