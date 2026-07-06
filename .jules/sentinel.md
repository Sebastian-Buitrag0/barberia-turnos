## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-07-06 - Insecure Direct Object Reference (IDOR) on Turno State Changes
**Vulnerability:** The `EnSilla` and `Finalizar` endpoints in `TurnosController` modified the state of a `Turno` without verifying if the requesting user (barber) was the assigned owner (`BarberoId`).
**Learning:** Relying only on `[Authorize]` and trusting client-provided `TurnoId` allows authenticated users to modify resources belonging to others. Resource ownership must be explicitly verified against the authenticated user's ID.
**Prevention:** Extract the authenticated user's ID using `User.FindFirst(ClaimTypes.NameIdentifier)` and verify it against the target resource's owner ID (e.g., `Turno.BarberoId`) or ensure the user has an 'Admin' role before permitting state modifications.
