## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Authorization on Sensitive Queue State Endpoints
**Vulnerability:** Read-only API endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) lacked `[Authorize]` attributes, exposing sensitive queue states, daily totals, and PII (e.g., phone numbers, client names) to unauthenticated users.
**Learning:** Endpoints that return Data Transfer Objects (DTOs) containing sensitive information or business data (like daily revenue and customer PII) must be explicitly protected, even if they are only handling HTTP GET requests.
**Prevention:** Always verify that endpoints returning sensitive DTOs have the appropriate `[Authorize]` attributes applied to prevent unauthorized data exposure and Broken Access Control.
