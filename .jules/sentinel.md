## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-20 - Unauthenticated PII Exposure in API Endpoints
**Vulnerability:** Read-only API endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) lacked `[Authorize]` attributes, exposing `TurnoResponseDto` which contains Personally Identifiable Information (PII) like client phone numbers to unauthenticated users.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers often forget to secure read-only (GET) endpoints that retrieve data but don't modify it, inadvertently leaking PII.
**Prevention:** Explicitly secure every sensitive endpoint at the method level using `[Authorize]` or `[Authorize(Roles = "...")]` attributes, especially those returning DTOs that contain PII, to prevent Broken Access Control.
