## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2025-02-23 - Insecure Direct Object Reference (IDOR) in Turnos State Transitions
**Vulnerability:** The endpoints for state transitions (`EnSilla` and `Finalizar` in `TurnosController`) only required a valid `TurnoId`, without validating if the authenticated Barbero actually owned the shift. This allowed any authenticated Barbero to arbitrarily modify the state of other Barberos' shifts.
**Learning:** Endpoints that modify specific resources must verify resource ownership in addition to authentication. Relying solely on `[Authorize]` is insufficient if it doesn't map to the specific resource.
**Prevention:** Verify resource ownership for modification endpoints by extracting the authenticated user ID and comparing it to the resource's owner ID.
