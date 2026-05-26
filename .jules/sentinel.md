## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Resource Ownership Verification (IDOR) on Turno Modifications
**Vulnerability:** Several endpoints modifying specific turnos (`EnSilla` and `Finalizar` in `TurnosController`) relied only on the generic `[Authorize]` attribute, meaning any authenticated user could modify any turno by supplying a different `TurnoId`.
**Learning:** In backend controllers, verifying authentication isn't enough when dealing with resource modification. Broken access control vulnerabilities (specifically IDOR) occur when resource ownership isn't enforced at the controller level.
**Prevention:** Always extract the authenticated user's ID (`User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value`) and verify it against the owner ID associated with the requested resource, throwing a `Forbid()` response if they don't match (unless overriding with an Admin role).
