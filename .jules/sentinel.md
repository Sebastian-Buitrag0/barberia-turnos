## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Authorization on Sensitive Info Endpoints
**Vulnerability:** Several sensitive read endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) lacked `[Authorize]` attributes. These endpoints return `TurnoResponseDto` which includes Personally Identifiable Information (PII) like `Cliente.Telefono`, exposing this data to unauthenticated users.
**Learning:** In ASP.NET Core, even if endpoints are intended for internal use by a frontend (like `AdminView.vue` or `BarberoView.vue`), they must be explicitly protected with `[Authorize]` at the API level if they contain sensitive data. The frontend logic does not protect the underlying API.
**Prevention:** Always verify that endpoints returning sensitive information or PII are protected by `[Authorize]` and specific roles if applicable to ensure least privilege access. Consider using controller-level `[Authorize]` to enforce security by default.
