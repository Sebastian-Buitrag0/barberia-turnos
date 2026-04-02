## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Unauthenticated Endpoints Leaking PII
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` were unauthenticated and returned `TurnoResponseDto`, which includes PII (client phone numbers), exposing user data to unauthenticated scraping.
**Learning:** Returning DTOs containing PII from unauthenticated GET requests exposes the application to data leakage. Endpoints must be explicitly evaluated for the data they expose and protected accordingly.
**Prevention:** Apply strict `[Authorize]` and role-based access control (e.g., `[Authorize(Roles = "Admin")]`) to any endpoint that returns sensitive user data or PII, aligning with the principle of least privilege.
