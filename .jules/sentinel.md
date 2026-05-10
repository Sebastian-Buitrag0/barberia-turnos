## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - PII Exposure via Unprotected GET Endpoints
**Vulnerability:** Read-only GET endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) returned `TurnoResponseDto` which includes sensitive Personally Identifiable Information (PII) like `Cliente.Telefono`. These endpoints were missing `[Authorize]` attributes, allowing unauthenticated users to scrape customer phone numbers and names.
**Learning:** Developers often remember to secure endpoints that modify state (POST/PUT/DELETE) but neglect GET endpoints because they assume "read-only" operations are safe. However, if the read operations return PII, they pose a critical data leak risk.
**Prevention:** Apply `[Authorize]` at the controller level by default to secure all endpoints, and only explicitly opt-out with `[AllowAnonymous]` for truly public data. Review all DTOs returned by endpoints to ensure they do not contain sensitive data if the endpoint must remain public.
