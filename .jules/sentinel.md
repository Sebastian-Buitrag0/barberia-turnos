## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2023-10-24 - [Broken Access Control on Turnos Controller]
**Vulnerability:** Endpoints returning sensitive PII (like phone numbers) in `TurnosController` were accessible unauthenticated.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. The DTO design allowed PII to be leaked because public endpoints were returning the full DTO structure.
**Prevention:** Always explicitly secure every sensitive endpoint at the method level if the controller lacks a class-level authorization attribute, and evaluate the specific data being exposed in DTOs.
