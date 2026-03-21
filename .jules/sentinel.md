## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2025-03-21 - Fix Missing Authentication on Turnos Controller Endpoints
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController.cs` lacked authentication (missing `[Authorize]` attributes). This allowed unauthenticated users to access sensitive queue and appointment data, including Personally Identifiable Information (PII) like customer phone numbers.
**Learning:** Even read-only API endpoints (GET requests) that return Data Transfer Objects (DTOs) with PII must be protected. The omission of `[Authorize]` can easily lead to Broken Access Control and unauthenticated data exposure.
**Prevention:** Always verify that endpoints returning sensitive data have appropriate `[Authorize]` attributes applied. Review DTO contents during endpoint creation to ensure no PII is accidentally exposed to public or unauthenticated scopes.
