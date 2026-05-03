## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-05-03 - PII Exposure via Missing Authorization
**Vulnerability:** Unprotected endpoints (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) returning `TurnoResponseDto` leaked PII (client phone numbers) to unauthenticated users.
**Learning:** Read-only API endpoints returning Data Transfer Objects (DTOs) with Personally Identifiable Information (PII) must be explicitly protected with `[Authorize]` attributes to prevent unauthorized data exposure.
**Prevention:** Regularly review DTOs returned by endpoints to identify PII, and ensure any endpoint returning such data is secured with appropriate role-based or general authorization.
