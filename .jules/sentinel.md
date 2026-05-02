## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Authorization on Sensitive Read Endpoints
**Vulnerability:** Several read endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) lacked `[Authorize]` attributes. These endpoints return DTOs containing Personally Identifiable Information (PII) such as client phone numbers, making them accessible to any unauthenticated user.
**Learning:** Returning PII in DTOs requires strict access controls. By default, endpoints in ASP.NET Core are public unless explicitly protected. The lack of authorization allows unauthorized users to scrape sensitive client data.
**Prevention:** Apply the principle of least privilege. Use explicit `[Authorize]` attributes (often with specific roles, like `Admin`) on endpoints returning sensitive data. For a secure-by-default architecture, consider applying `[Authorize]` at the controller level and explicitly allowing anonymous access only where intended.
