## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-30 - Missing Authorization on PII Endpoints
**Vulnerability:** Endpoints retrieving sensitive queue or appointment states (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) lacked `[Authorize]` attributes, allowing unauthenticated attackers to access Personally Identifiable Information (PII) like phone numbers. This is a Broken Access Control vulnerability.
**Learning:** Read-only API endpoints (GET requests) returning Data Transfer Objects (DTOs) with Personally Identifiable Information (PII) must be explicitly protected with `[Authorize]` attributes to prevent unauthorized data exposure.
**Prevention:** Always verify that API endpoints handling sensitive data or PII are protected by appropriate `[Authorize]` attributes, specifying required roles if necessary.
