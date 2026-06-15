## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.
## 2026-06-15 - [Broken Access Control in TurnosController]
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController.cs` were missing `[Authorize]` attributes, exposing PII (client names and phone numbers) to unauthenticated users.
**Learning:** In ASP.NET Core, methods without class-level or method-level `[Authorize]` attributes are public by default. If a controller returns PII, every endpoint must be explicitly secured to prevent Broken Access Control.
**Prevention:** Apply `[Authorize]` or `[Authorize(Roles = "...")]` to sensitive endpoints and verify authorization requirements during code review when adding new routes.
