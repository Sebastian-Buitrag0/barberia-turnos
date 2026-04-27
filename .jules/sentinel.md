## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-04-27 - PII Exposure via Missing Authorization on Turno Endpoints
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` were missing `[Authorize]` attributes, exposing Personally Identifiable Information (PII) like the client's `Telefono` to unauthenticated requests.
**Learning:** Read-only endpoints that return DTOs containing sensitive information (such as `TurnoResponseDto`) must be explicitly protected with authorization, even if they don't modify state. The frontend separation between public and authenticated interfaces can obscure which endpoints are publicly accessible if the backend does not enforce it.
**Prevention:** Always verify what data is included in DTOs returned by endpoints. If PII or sensitive data is present, apply appropriate `[Authorize]` and `[Authorize(Roles = "...")]` attributes to ensure only authorized users (e.g., Admins, Barbers) can access the data.
