## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2025-02-28 - Missing Authorization on PII Endpoints
**Vulnerability:** Endpoints retrieving appointments and queue state (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) in `TurnosController` lacked `[Authorize]` attributes, exposing Personally Identifiable Information (PII) like client phone numbers to unauthenticated users.
**Learning:** Returning DTOs containing sensitive PII data requires explicit authorization checks. Controllers with mixed access levels must ensure all non-public data endpoints are protected.
**Prevention:** Always evaluate the data returned by an endpoint. If it contains PII or sensitive business data, enforce strict `[Authorize]` attributes. Consider separating public and private DTOs.