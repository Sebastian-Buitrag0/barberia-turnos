## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Resource Ownership Verification (IDOR)
**Vulnerability:** The endpoints `/api/turnos/ensilla` and `/api/turnos/finalizar` allowed any authenticated Barbero to modify any `Turno` by providing a valid `TurnoId`, without verifying if the Turno was actually assigned to them.
**Learning:** In controllers, applying `[Authorize]` simply ensures the user is authenticated. When an endpoint modifies a specific resource, it is necessary to fetch the resource and enforce ownership by comparing the authenticated user's ID with the resource owner's ID.
**Prevention:** Always extract the authenticated user's ID (`User.FindFirst(ClaimTypes.NameIdentifier)`) and ensure it matches the entity's owner ID (or check if the user is an Admin) before permitting state-changing actions on a specific entity. Return `Forbid()` if authorization fails.
