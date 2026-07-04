## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Authorization on Turnos Endpoints (PII Leak)
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` were completely public due to missing `[Authorize]` attributes. These endpoints return `TurnoResponseDto`, which includes `Cliente.Telefono` (PII), resulting in a critical data leak where anyone could dump customer phone numbers.
**Learning:** In ASP.NET Core, an entire controller missing a class-level `[Authorize]` attribute means all its endpoints are unauthenticated by default unless specified. In this project, `AdminView.vue` and `BarberoView.vue` implicitly required these routes, but the backend didn't enforce it.
**Prevention:** Apply `[Authorize]` globally or at the controller level to default to a secure posture. Ensure that endpoints returning PII like `Telefono` are strictly restricted to authorized roles (e.g., `Admin` or authenticated staff).
