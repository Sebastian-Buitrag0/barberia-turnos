## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Resource Ownership Verification in Stateful Turno Operations
**Vulnerability:** The `EnSilla` and `Finalizar` endpoints in `TurnosController` accepted a `TurnoId` and modified the state/services of that Turno without verifying if the authenticated user (the Barber) was actually assigned to that Turno. This allowed any authenticated user to modify or finalize other barbers' turnos (Insecure Direct Object Reference / IDOR).
**Learning:** Having an `[Authorize]` attribute only guarantees the user is logged in, not that they have the right to modify the specific resource requested. In multi-tenant or multi-user applications, resource ownership must be explicitly verified in the controller logic.
**Prevention:** Always extract the authenticated user's ID (`User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value`) and compare it against the resource owner's ID (`Turno.BarberoId`). Return `Forbid()` if they do not match, unless the user has administrative privileges.
