## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Authorization on Read-Only API Endpoints Leaking PII
**Vulnerability:** Read-only API endpoints returning DTOs with PII (`GetTurnosHoy`, `GetCola`, `GetPorPagar` returning `TurnoResponseDto` which contains client phone numbers) were lacking explicit `[Authorize]` attributes. This exposed Personally Identifiable Information (PII) to unauthenticated users via Broken Access Control.
**Learning:** In ASP.NET Core, even read-only (GET) endpoints require explicit `[Authorize]` attributes if they return sensitive data or PII, especially when the controller lacks class-level default security.
**Prevention:** Always verify the data being returned in DTOs and apply appropriate method-level `[Authorize]` or `[Authorize(Roles="...")]` attributes to secure endpoints based on the sensitivity of the data.
