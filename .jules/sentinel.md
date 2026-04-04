## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.
## 2026-04-04 - [Missing Authorization on Sensitive Endpoints]
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController.cs` return `TurnoResponseDto` containing PII (like the client's phone number) but lacked any authorization checks, exposing this data to unauthenticated users.
**Learning:** Read-only GET endpoints that return DTOs with PII must be explicitly protected with `[Authorize]` or `[Authorize(Roles = "Admin")]` depending on who is intended to access them. The lack of these attributes leads to Broken Access Control and data leakage.
**Prevention:** Always verify the authorization requirements of GET endpoints, especially those returning data entities containing PII, during development and code review.
