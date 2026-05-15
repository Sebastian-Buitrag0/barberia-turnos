## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Authorization on Endpoints Exposing PII
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` were returning `TurnoResponseDto` which contained Personally Identifiable Information (PII), specifically `Cliente.Telefono`. These endpoints lacked `[Authorize]` attributes, making them accessible to unauthenticated users and leading to PII exposure (Broken Access Control).
**Learning:** Read-only API endpoints (GET requests) returning DTOs that contain PII must be explicitly protected with `[Authorize]` attributes to prevent unauthorized data exposure.
**Prevention:** Regularly audit DTOs for PII. Ensure that any endpoint returning these DTOs is explicitly secured with the appropriate `[Authorize]` attributes based on the intended audience (e.g., `[Authorize(Roles = "Admin")]` for admin-only data).
