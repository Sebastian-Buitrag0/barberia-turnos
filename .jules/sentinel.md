## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - PII Exposure via Missing Authorization on Read Endpoints
**Vulnerability:** Read-only API endpoints (`GetTurnosHoy`, `GetCola`, `GetPorPagar` in `TurnosController`) returning `TurnoResponseDto` were unauthenticated. Since this DTO contains Personally Identifiable Information (PII) like `Cliente.Telefono`, this exposed sensitive data to unauthenticated external access.
**Learning:** Developers often remember to secure endpoints that modify data (POST/PUT/DELETE) but may forget to protect GET requests. If a GET request returns a DTO containing sensitive information (PII, credentials, etc.), it must be explicitly protected with the appropriate `[Authorize]` attribute based on the application's roles.
**Prevention:** Always verify what data is returned in DTOs for GET requests. Use explicit `[Authorize]` attributes (or configure a secure-by-default global policy) to ensure that endpoints exposing PII are only accessible to the correct authenticated roles.
