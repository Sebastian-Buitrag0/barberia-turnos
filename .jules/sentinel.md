## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-06-18 - Missing Authorization Exposing PII
**Vulnerability:** Endpoints `GetTurnosHoy`, `GetPorPagar`, and `GetCola` in `TurnosController` lacked `[Authorize]` attributes, exposing PII (`Cliente.Telefono`) to unauthenticated users.
**Learning:** Endpoints that return sensitive data or PII must be explicitly protected with appropriate `[Authorize]` attributes based on their intended consumer roles (e.g., Admin or authenticated staff).
**Prevention:** Always verify that endpoints returning sensitive data have method-level or controller-level authorization attributes applied, and rely on role-based access control where necessary.
