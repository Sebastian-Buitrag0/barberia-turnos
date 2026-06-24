## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-06-24 - Missing Authorization checks on Appointments
**Vulnerability:** IDOR vulnerability discovered where barbers could modify appointments assigned to other barbers in `TurnosController` (`EnSilla` and `Finalizar` endpoints) because there was no check to ensure the authenticated user owned the resource.
**Learning:** Endpoints that modify specific resources must verify that the authenticated user owns the resource or has sufficient privileges (e.g., Admin role). Merely requiring authentication via `[Authorize]` is insufficient.
**Prevention:** Implement resource ownership verification by extracting the authenticated user's ID (`User.FindFirst(ClaimTypes.NameIdentifier)?.Value`) and comparing it against the resource owner's ID (`Turno.BarberoId`). Return `Forbid()` or `Unauthorized()` if they do not match and the user lacks elevated privileges.
