## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Authorization on Data Exposure Endpoints
**Vulnerability:** Several endpoints returning TurnoResponseDto (which contains PII like Cliente.Telefono) in `TurnosController` were not protected. `GetTurnosHoy` and `GetPorPagar` needed `[Authorize(Roles = "Admin")]` and `GetCola` needed `[Authorize]` to prevent unauthorized extraction of phone numbers.
**Learning:** Implicit exposure of Personally Identifiable Information (PII) occurs when Response DTOs contain sensitive data but are returned by unauthenticated endpoints. Overlooking endpoint-level authorization can quickly lead to Broken Access Control.
**Prevention:** Implement endpoint-level authorization by explicitly setting the `[Authorize]` and role constraints on every method that interacts with sensitive data, even if the primary use case is internal administration.
