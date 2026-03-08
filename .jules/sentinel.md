## 2024-03-08 - [Missing Authorization on Administrative Endpoints]
**Vulnerability:** Several administrative endpoints in `UsuariosController` (GET, POST, PUT, DELETE for barbers) and `TurnosController` (POST registrar-admin) lacked the `[Authorize(Roles = "Admin")]` attribute, allowing unauthenticated or unauthorized users to perform admin actions.
**Learning:** In ASP.NET Core, it's easy to overlook adding authorization attributes to individual actions or the entire controller. If a controller mixes public and admin endpoints, each admin endpoint must be explicitly secured.
**Prevention:** Always verify that endpoints intended for specific roles have the appropriate `[Authorize(Roles = "...")]` attribute applied. Use integration tests to ensure unauthorized requests return 401/403.
