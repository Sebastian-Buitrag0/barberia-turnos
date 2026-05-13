## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-05-13 - Missing Authorization on TurnosController GET endpoints exposing PII
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` were unprotected and publicly accessible. These endpoints return `TurnoResponseDto`, which contains Personally Identifiable Information (PII) such as the client's phone number.
**Learning:** Read-only API endpoints (GET requests) returning DTOs with PII must be explicitly protected with `[Authorize]` attributes to prevent unauthorized data exposure.
**Prevention:** Ensure that all endpoints returning PII enforce strict authorization logic or restrict DTO mapping to only expose public information to unauthenticated users.
