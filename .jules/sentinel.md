## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-29 - Missing Authorization on Endpoints Returning PII
**Vulnerability:** Endpoints such as `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` returned Data Transfer Objects (DTOs) that included Personally Identifiable Information (PII) like `Cliente.Telefono` without adequate access control, allowing potential Broken Access Control and data exposure.
**Learning:** Endpoints that return DTOs containing sensitive user data must enforce explicit authorization to prevent data exposure. The default public nature of controllers without class-level `[Authorize]` attributes requires careful consideration for each method.
**Prevention:** Thoroughly review the data models and DTOs returned by each endpoint. Apply appropriate `[Authorize]` attributes (e.g., `[Authorize]` or `[Authorize(Roles = "Admin")]`) based on the frontend consumer requirements and data sensitivity to restrict access to authorized personnel only.
