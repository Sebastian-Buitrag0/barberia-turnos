## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Insecure Direct Object Reference (IDOR) in TurnosController
**Vulnerability:** The `EnSilla` and `Finalizar` endpoints in `TurnosController` allowed any authenticated Barbero to modify the state of any `Turno` by providing an arbitrary `TurnoId`, without verifying if the Turno was actually assigned to them.
**Learning:** Even when endpoints are protected with `[Authorize]`, they remain vulnerable to IDOR if they modify specific resources without verifying that the authenticated user owns or has sufficient privileges over that specific resource.
**Prevention:** Always verify resource ownership in endpoints that modify or access specific entities (e.g., check `turno.BarberoId == currentUserId || User.IsInRole("Admin")`) before allowing the operation to proceed. Return `Forbid()` if the check fails.
