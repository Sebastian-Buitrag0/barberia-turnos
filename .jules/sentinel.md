## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-06-14 - Missing Authorization on Turnos Endpoints
**Vulnerability:** Several endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) exposed personally identifiable information (PII) like the client's phone number and the barber's name. These endpoints lacked `[Authorize]` attributes, allowing unauthenticated users to access this sensitive data, which represents a critical Broken Access Control vulnerability.
**Learning:** Endpoints that return Personally Identifiable Information (PII) must always be protected. In ASP.NET Core, methods in controllers that do not have class-level `[Authorize]` attributes are public by default.
**Prevention:** Implement a secure-by-default approach by explicitly securing every endpoint that exposes sensitive data with method-level `[Authorize]` attributes, or consider applying it globally.
