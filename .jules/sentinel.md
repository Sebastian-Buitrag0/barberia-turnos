## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-06-13 - Broken Access Control / PII Exposure in Turnos Endpoints
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` were missing authorization attributes. This allowed unauthenticated actors to access these endpoints and view PII like `Cliente.Telefono` for all appointments.
**Learning:** Default unauthenticated controller access leaves potentially sensitive DTO fields (like client phone numbers used in responses) exposed unless explicit `[Authorize]` attributes are added to the endpoints or globally.
**Prevention:** Audit all endpoints returning potentially sensitive models or DTOs to ensure they have the appropriate `[Authorize]` attributes applied, matching the required access level (e.g. Admin vs Staff/Barbero).
