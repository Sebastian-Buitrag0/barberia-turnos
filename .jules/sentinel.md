## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Authorization on Read-Only Endpoints Exposing PII
**Vulnerability:** Several read-only GET endpoints (e.g., `GetTurnosHoy`, `GetCola`, `GetPorPagar` in `TurnosController`) returning Data Transfer Objects (DTOs) with Personally Identifiable Information (PII) like phone numbers lacked `[Authorize]` attributes.
**Learning:** Returning DTOs that include PII requires explicit protection, even if the endpoints only perform non-destructive read operations.
**Prevention:** Apply strict authorization (`[Authorize]` or `[Authorize(Roles = "...")]`) to any endpoint that exposes PII to prevent Broken Access Control and unauthenticated data leakage.
