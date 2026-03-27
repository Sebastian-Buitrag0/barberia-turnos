## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-30 - [Missing Authorization on Sensitive Endpoints]
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController.cs` were returning sensitive Personally Identifiable Information (PII), such as `Telefono`, without any authentication or authorization checks.
**Learning:** Read-only API endpoints (GET requests) that return Data Transfer Objects (DTOs) with PII must be explicitly protected, even if they don't modify data. Unauthenticated access can lead to mass data scraping and privacy violations.
**Prevention:** Always apply the `[Authorize]` attribute to any endpoint returning sensitive user data. Use `[Authorize(Roles = "...")]` to restrict access to specific roles (like Admin) based on business logic and the frontend architecture's view mapping.
