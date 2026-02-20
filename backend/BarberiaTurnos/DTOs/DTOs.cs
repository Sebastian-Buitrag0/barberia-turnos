namespace BarberiaTurnos.DTOs;

public record RegistrarTurnoDto(string Nombre, string Telefono, int? BarberoId);
public record LlamarTurnoDto(int BarberoId);
public record FinalizarTurnoDto(int TurnoId, List<int> ServicioIds);
public record CobrarTurnoDto(int TurnoId);
public record CancelarTurnoDto(string Telefono);
public record LoginDto(string Pin);

public record TurnoResponseDto(
    int Id,
    int TurnoDiario,
    string Estado,
    string ClienteNombre,
    string ClienteTelefono,
    string? BarberoNombre,
    decimal? Total,
    DateTime FechaCreacion,
    List<DetalleDto>? Detalles
);

public record DetalleDto(string ServicioNombre, decimal PrecioCobrado);

public record UsuarioResponseDto(int Id, string Nombre, string Rol);
public record UsuarioAdminResponseDto(int Id, string Nombre, string Rol);

public record ServicioDto(int Id, string Nombre, decimal Precio);
public record CrearModificarBarberoDto(string Nombre, string Pin);
