## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2025-02-21 - Broken Access Control on PII Exposing Endpoints
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetPorPagar`, and `GetCola` in `TurnosController` returned `TurnoResponseDto` which includes Personally Identifiable Information (PII) such as the client's phone number (`ClienteTelefono`), but lacked authorization attributes. This allowed unauthenticated users to view sensitive data about other users.
**Learning:** Read-only API endpoints (GET requests) returning Data Transfer Objects (DTOs) with PII must be explicitly protected with `[Authorize]` or role-based authorization attributes to prevent unauthorized data exposure and Broken Access Control vulnerabilities.
**Prevention:** Always verify the contents of DTOs returned by endpoints, especially those that include PII. Apply appropriate `[Authorize]` or `[Authorize(Roles = "...")]` attributes to these endpoints based on their intended audience (e.g., Admin, Barbero).
