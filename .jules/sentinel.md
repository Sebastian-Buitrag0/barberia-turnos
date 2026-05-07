## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-05-07 - Missing Authorization Exposing PII
**Vulnerability:** The endpoints `GetTurnosHoy` and `GetPorPagar` in `TurnosController` were missing `[Authorize(Roles = "Admin")]`, and `GetCola` was missing `[Authorize]`. This allowed unauthenticated users to access these endpoints, potentially exposing Personally Identifiable Information (PII) such as the client's phone number (`Cliente.Telefono`) via the returned `TurnoResponseDto`.
**Learning:** Read-only API endpoints (GET requests) returning Data Transfer Objects (DTOs) with Personally Identifiable Information (PII) must be explicitly protected with `[Authorize]` attributes to prevent unauthorized data exposure.
**Prevention:** Review all GET endpoints returning DTOs with PII and ensure they have the appropriate `[Authorize]` attributes based on the intended audience and sensitivity of the data. Always apply the principle of least privilege.
