## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-05-12 - PII Exposure in Read-Only API Endpoints
**Vulnerability:** Read-only GET endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) were lacking `[Authorize]` attributes while returning DTOs that contain Personally Identifiable Information (PII), specifically `Cliente.Telefono`.
**Learning:** Returning objects with PII on public endpoints leads to unauthorized data exposure, even if the operation is read-only.
**Prevention:** Always ensure that endpoints returning PII are properly protected with `[Authorize]` or role-based authorization attributes, matching the required access level for the data being exposed.
