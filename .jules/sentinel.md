## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Unauthenticated PII Exposure in Read-Only Endpoints
**Vulnerability:** Read-only GET endpoints (like `GetTurnosHoy`, `GetCola`, and `GetPorPagar`) returning `TurnoResponseDto` which contains Personally Identifiable Information (PII) such as the client's phone number, were missing `[Authorize]` attributes. This exposed sensitive data to unauthenticated requests.
**Learning:** Endpoints that return DTOs containing PII must be explicitly protected with authorization attributes, even if they do not modify state. The frontend structure, with separate authenticated views, often relies on backend restrictions.
**Prevention:** Apply strict authorization constraints on endpoints returning data with PII. Use `[Authorize]` or role-specific attributes (`[Authorize(Roles = "Admin")]`) explicitly to protect sensitive data exposure in compliance with privacy practices.
