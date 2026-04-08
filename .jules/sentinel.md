## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Authorization on Read Endpoints Returning PII
**Vulnerability:** Read-only API endpoints (e.g., `GetTurnosHoy`, `GetCola`, `GetPorPagar` in `TurnosController`) lacked authorization, allowing unauthenticated users to access PII (Personally Identifiable Information) such as client phone numbers via the `TurnoResponseDto`.
**Learning:** Endpoints returning sensitive information, even read-only GET requests, must be explicitly protected with appropriate `[Authorize]` attributes (often requiring specific roles like 'Admin') to prevent unauthorized data exposure.
**Prevention:** Review all returned DTOs for PII. If PII is present, ensure the endpoint enforcing its return is protected by an `[Authorize]` attribute corresponding to the intended audience (e.g., Admin, Barbero).
