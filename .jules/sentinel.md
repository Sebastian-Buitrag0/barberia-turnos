## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.
## 2024-05-28 - Missing Authorization on Read-Only Endpoints Exposing PII
**Vulnerability:** Read-only endpoints (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) in `TurnosController` returned `TurnoResponseDto` objects containing Personally Identifiable Information (PII) like `Cliente.Telefono` without enforcing authentication, leading to unauthenticated data exposure.
**Learning:** Returning detailed DTOs on public, unauthenticated GET requests can leak sensitive user information. Endpoints returning sensitive information or PII must be protected with `[Authorize]` attributes.
**Prevention:** Apply strict authorization (`[Authorize]` or `[Authorize(Roles = "...")]`) to any endpoint returning DTOs that contain PII. Segment public views to only use endpoints that expose safe, non-sensitive summary data.
