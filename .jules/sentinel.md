## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Authorization on Sensitive Data Endpoints
**Vulnerability:** Endpoints retrieving aggregate turn data (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) lacked authorization, potentially exposing Personally Identifiable Information (PII) such as the client's phone number (`Cliente.Telefono`) present in the `TurnoResponseDto`.
**Learning:** Broken Access Control vulnerabilities can arise when endpoints intended only for administrative or staff views are inadvertently left public. A deep understanding of frontend access patterns is necessary to determine the correct authorization level for backend endpoints.
**Prevention:** Strictly enforce authorization attributes on endpoints returning sensitive data, correlating the required roles with the intended client application access levels.
