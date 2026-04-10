## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-04-10 - Broken Access Control Leading to PII Exposure in Turnos Endpoints
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController.cs` were unauthenticated, exposing the `TurnoResponseDto` which contains Personally Identifiable Information (PII) like the client's phone number.
**Learning:** Read-only API endpoints returning DTOs that include sensitive data can be a source of data leakage if not explicitly protected, even if they don't modify state.
**Prevention:** Enforce strict `[Authorize]` or `[Authorize(Roles = "Admin")]` attributes on any endpoint returning PII data, aligning with the expected frontend usage and access roles.
