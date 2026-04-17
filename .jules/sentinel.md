## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-04-17 - Missing Authorization Exposing PII Data
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` were missing `[Authorize]` attributes. These endpoints return `TurnoResponseDto` which contains PII (Client phone numbers), leading to unauthenticated exposure of sensitive data.
**Learning:** Read-only API endpoints (GET requests) returning Data Transfer Objects (DTOs) with Personally Identifiable Information (PII) must be explicitly protected with `[Authorize]` attributes to prevent unauthorized data exposure.
**Prevention:** Always verify what data is included in DTOs returned by endpoints, and apply appropriate authorization attributes based on the sensitivity of the exposed data.
