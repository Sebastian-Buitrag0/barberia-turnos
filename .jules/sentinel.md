## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - PII Exposure via Unsecured Endpoints
**Vulnerability:** Endpoints returning sensitive information like PII (e.g., `GetTurnosHoy`, `GetCola`, `GetPorPagar` in `TurnosController`) lacked `[Authorize]` attributes, allowing unauthenticated users to access this data.
**Learning:** Returning DTOs with PII (like `Cliente.Telefono`) requires strict access control. Without controller-level authorization, each endpoint must be individually reviewed for data exposure risks.
**Prevention:** Always evaluate the data returned by an endpoint. If it includes PII or sensitive business data, explicitly apply `[Authorize]` with the appropriate roles.
