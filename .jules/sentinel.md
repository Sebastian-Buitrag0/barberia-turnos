## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2025-05-08 - Exposure of PII via Unprotected Read-Only Endpoints
**Vulnerability:** Read-only API endpoints returning DTOs with Personally Identifiable Information (PII) like `Cliente.Telefono` (e.g., `GetTurnosHoy`, `GetCola`, `GetPorPagar` in `TurnosController`) lacked explicit authorization attributes, allowing public access to sensitive data and resulting in Broken Access Control.
**Learning:** In ASP.NET Core, even read-only (GET) endpoints must be explicitly protected with `[Authorize]` attributes if they return sensitive data or PII, as default behavior leaves them public. The lack of protection allows unauthenticated users to enumerate or view data they should not have access to.
**Prevention:** Always evaluate the data returned by endpoints (specifically DTOs) for PII. Apply appropriate authorization attributes (e.g., `[Authorize]`, `[Authorize(Roles = "Admin")]`) based on the required access level for the data being exposed.
