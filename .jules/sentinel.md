## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-29 - Insecure Direct Object Reference / Missing Authorization on Read Endpoints
**Vulnerability:** Read-only API endpoints (`GetTurnosHoy`, `GetCola`, `GetPorPagar` in `TurnosController`) were returning full `TurnoResponseDto` objects containing Personally Identifiable Information (PII) like customer phone numbers, without any `[Authorize]` attributes.
**Learning:** Even read-only HTTP GET endpoints can pose severe security risks if they return sensitive data like PII without proper authorization checks, leading to Information Disclosure vulnerabilities. Public APIs must carefully curate data exposure using specific DTOs or enforce proper authorization.
**Prevention:** Always apply the principle of least privilege. Any endpoint exposing PII or financial states must have explicitly defined `[Authorize]` or `[Authorize(Roles = "...")]` attributes. Implement integration tests to verify that sensitive endpoints return `401 Unauthorized` for anonymous requests.
