## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-29 - PII Exposure via Unprotected Endpoints
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` returned `TurnoResponseDto`, which includes `Cliente.Telefono` (PII), but lacked `[Authorize]` attributes. This allowed unauthenticated access to sensitive user data.
**Learning:** Endpoints that return DTOs containing PII must have explicit authorization checks. Returning rich DTOs in public endpoints without filtering sensitive fields is a critical Broken Access Control vulnerability.
**Prevention:** Apply `[Authorize]` attributes to all endpoints exposing PII, and ensure role-based access control (e.g., `[Authorize(Roles = "Admin")]`) is applied based on the principle of least privilege.
