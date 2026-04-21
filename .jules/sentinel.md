## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2025-02-28 - Missing Authorization on Read-Only DTOs Exposing PII
**Vulnerability:** The GET endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController.cs` were public and returned lists of `TurnoResponseDto`, which include the `ClienteTelefono` (phone number). This allowed unauthenticated users to scrape personally identifiable information (PII) of clients.
**Learning:** Read-only API endpoints (GET requests) are often incorrectly perceived as safe and left unsecured. However, if the DTOs they return contain PII, they must be explicitly protected with appropriate authorization attributes.
**Prevention:** Review all DTOs returned by GET endpoints to identify sensitive data (PII). Ensure that any endpoint returning such data enforces strict authorization (`[Authorize]` or `[Authorize(Roles = "...")]`) to prevent data exposure.
