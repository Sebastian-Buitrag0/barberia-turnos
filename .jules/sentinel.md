## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2025-02-28 - Missing Authorization on PII Endpoints
**Vulnerability:** Broken Access Control leading to PII exposure. Endpoints returning client phone numbers (`TurnoResponseDto`) like `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` were accessible unauthenticated.
**Learning:** Returning DTOs that include sensitive data like `Cliente.Telefono` without enforcing strict authorization (e.g., `[Authorize(Roles = "Admin")]` for admin endpoints) allows attackers to iterate and scrape PII.
**Prevention:** Always verify that endpoints returning sensitive/PII data are protected with the appropriate `[Authorize]` attribute corresponding to the roles that need access.
