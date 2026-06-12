## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Insecure Direct Object References (IDOR) on Turnos Status Modifiers
**Vulnerability:** The endpoints `EnSilla` and `Finalizar` in `TurnosController` were protected with `[Authorize]`, but lacked resource-level ownership validation. Any authenticated barber could alter the status of any client turn, even if the turn was assigned to another barber.
**Learning:** Method-level `[Authorize]` attributes verify authentication and global roles, but do not automatically perform resource-level authorization. Leaving this out leads to IDOR vulnerabilities on state-modifying actions.
**Prevention:** Always verify resource ownership (e.g., matching the authenticated user's ID against `Turno.BarberoId`) on endpoints that modify specific resources, granting bypasses only for administrative roles (e.g. `Admin`).
