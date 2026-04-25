## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Authorization on Endpoints Exposing PII
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` returned `TurnoResponseDto`, which includes `Cliente.Telefono` (PII). These endpoints lacked `[Authorize]` attributes, leaving them publicly accessible and causing a severe data exposure vulnerability.
**Learning:** Read-only API endpoints (GET requests) that return Data Transfer Objects (DTOs) containing Personally Identifiable Information (PII) must be explicitly protected with `[Authorize]` attributes to prevent unauthorized data exposure. The presence of PII requires strict access control, even for endpoints that do not modify data.
**Prevention:** Always review the models/DTOs returned by API endpoints. If any model contains PII (e.g., phone numbers, emails, addresses), the endpoint must be secured with the appropriate `[Authorize]` attributes, matching the required access level for that data.
