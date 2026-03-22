## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-29 - Missing Authorization on DTO Endpoints Exposing PII
**Vulnerability:** Endpoints returning lists of aggregated DTOs (e.g., `GetTurnosHoy` and `GetCola` in `TurnosController`) lacked `[Authorize]` attributes. Because `TurnoResponseDto` included personally identifiable information (PII) like phone numbers, any unauthenticated attacker could scrape phone numbers of clients.
**Learning:** Read-only (GET) endpoints returning objects with sensitive data (even seemingly benign data like phone numbers used for notifications) must be protected. The framework does not automatically map DTO field sensitivity; it's the developer's responsibility to restrict access to those endpoints.
**Prevention:** Always evaluate the contents of DTOs returned by public endpoints. If PII is included, either remove the fields from the DTO, create a public-safe version of the DTO, or explicitly protect the endpoint with `[Authorize]`.
