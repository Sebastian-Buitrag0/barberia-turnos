## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Unauthenticated Exposure of PII via GET Endpoints
**Vulnerability:** Endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController.cs` were returning `TurnoResponseDto` without requiring authentication. This DTO contained Personally Identifiable Information (PII) like the client's phone number (`ClienteTelefono`), making it publicly accessible and causing a Broken Access Control vulnerability.
**Learning:** Returning objects with PII on public endpoints must be avoided. Data Transfer Objects (DTOs) with sensitive data need their accessing routes heavily secured with `[Authorize]` attributes.
**Prevention:** Apply `[Authorize]` attributes to all endpoints returning data belonging to internal systems or customers, especially if it contains phone numbers, emails, or personal identifiers.
