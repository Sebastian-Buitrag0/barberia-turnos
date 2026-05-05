## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-05-05 - Missing Authorization on Read-Only API Endpoints Leaking PII
**Vulnerability:** Several read-only endpoints (e.g., `GetTurnosHoy`, `GetCola`, `GetPorPagar` in `TurnosController`) returning `TurnoResponseDto` which contains Personally Identifiable Information (PII) like `Cliente.Telefono` lacked `[Authorize]` attributes. This allowed unauthenticated users to access this sensitive data, a form of Broken Access Control.
**Learning:** Returning Data Transfer Objects (DTOs) with PII from endpoints, even read-only (GET) ones, must be explicitly protected with `[Authorize]` attributes to prevent unauthorized data exposure.
**Prevention:** Always verify the data being returned by an endpoint (e.g., inspecting the DTO) and ensure appropriate authorization attributes (`[Authorize]`, `[Authorize(Roles = "...")]`) are applied to endpoints that expose sensitive data or PII.
