## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.
## 2026-05-30 - Fix Broken Access Control on Turnos endpoints
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` lacked `[Authorize]` attributes, exposing PII (Personally Identifiable Information like client phone numbers) to unauthenticated users.
**Learning:** In ASP.NET Core, methods without authorization attributes are public by default. Any endpoint returning sensitive data must be explicitly protected with the appropriate roles or basic authorization, even if the frontend attempts to hide the UI elements.
**Prevention:** Apply `[Authorize(Roles = "Admin")]` or `[Authorize]` to sensitive endpoints according to the access they require. Alternatively, apply global authorization at the controller level and explicitly use `[AllowAnonymous]` for public endpoints.
