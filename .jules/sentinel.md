## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2025-02-23 - Missing Authorization exposing PII
**Vulnerability:** The endpoints `GetTurnosHoy` and `GetPorPagar` in `TurnosController` exposed Personally Identifiable Information (PII) like phone numbers without proper authorization controls, making them accessible to unauthenticated users or lower privilege users. `GetCola` also exposed PII to unauthenticated users.
**Learning:** Endpoints that return Data Transfer Objects (DTOs) with PII must be explicitly protected, especially when the controller lacks a class-level `[Authorize]` attribute.
**Prevention:** Always verify that endpoints returning sensitive data have the appropriate `[Authorize]` attribute corresponding to the required access level.
