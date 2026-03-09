using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberiaTurnos.Data;
using BarberiaTurnos.DTOs;

namespace BarberiaTurnos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiciosController : ControllerBase
{
    private readonly AppDbContext _db;

    public ServiciosController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<ServicioDto>>> GetAll()
    {
        var servicios = await _db.Servicios
            .Where(s => s.Activo)
            .Select(s => new ServicioDto(s.Id, s.Nombre, s.Precio))
            .ToListAsync();

        return Ok(servicios);
    }
}
