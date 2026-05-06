## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2025-02-20 - Missing Authorization on Read-Only Endpoints Returning PII
**Vulnerability:** Read-only endpoints (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) in `TurnosController` returned DTOs containing Personally Identifiable Information (PII) like `Cliente.Telefono` but lacked explicit authorization attributes (`[Authorize]`), exposing this sensitive data to unauthenticated users.
**Learning:** Even read-only (GET) endpoints require strict authorization when the returned data structure (e.g., `TurnoResponseDto`) includes sensitive or PII fields. Broken Access Control isn't limited to endpoints that modify state.
**Prevention:** Always verify the structure of DTOs returned by API endpoints. If a DTO contains PII, ensure the corresponding endpoints are protected with appropriate `[Authorize]` or `[Authorize(Roles = "...")]` attributes based on who should legitimately access that data.
