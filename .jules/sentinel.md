## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Information Exposure (PII) via Unprotected GET Endpoints
**Vulnerability:** Read-only API endpoints (GET requests) returning Data Transfer Objects (DTOs) with Personally Identifiable Information (PII), such as client phone numbers, were exposed publicly in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`).
**Learning:** Returning PII in responses requires explicit access controls, even on GET requests that don't modify data. In ASP.NET Core, an absence of `[Authorize]` means public access, creating a data exposure vulnerability.
**Prevention:** Apply strict `[Authorize]` attributes selectively (`Admin` role for `GetTurnosHoy` and `GetPorPagar`, and general auth for `GetCola`) based on the necessary access level for the endpoints returning PII data.
