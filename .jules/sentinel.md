## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Authorization Exposing PII Data
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController.cs` were returning `TurnoResponseDto` which includes PII (`Cliente.Telefono`), but lacked authorization attributes (`[Authorize]`), allowing unauthenticated attackers to scrape customer phone numbers.
**Learning:** Endpoints that return Personally Identifiable Information (PII) must always be explicitly protected with appropriate authorization attributes. Do not assume endpoints are safe just because they are primarily consumed by authenticated frontend components.
**Prevention:** Apply `[Authorize]` or `[Authorize(Roles="...")]` to any endpoint that fetches or manipulates PII data, and implement explicit ownership checks where relevant.
