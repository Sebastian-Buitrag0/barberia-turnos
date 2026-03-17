## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-29 - Missing Authorization on Data Endpoints
**Vulnerability:** Endpoints returning sensitive Personally Identifiable Information (PII) like phone numbers and names (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) in `TurnosController` were public, allowing unauthenticated access to PII and leading to Broken Access Control.
**Learning:** Read-only data endpoints (GET) that return DTOs with PII must explicitly be protected with `[Authorize]` attributes to prevent data exposure, as they are not implicitly secure.
**Prevention:** Apply `[Authorize]` at the controller level or endpoint level whenever returning PII data and review endpoints to ensure unauthenticated users only access public data (like `getBarberos`).
