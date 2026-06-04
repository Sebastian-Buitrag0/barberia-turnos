## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-06-04 - Insecure Direct Object Reference (IDOR) on Turno Modification
**Vulnerability:** The `EnSilla` and `Finalizar` endpoints in `TurnosController` lacked resource ownership validation, allowing any authenticated user (even non-admins or other barbers) to modify the state and details of a `Turno` assigned to someone else.
**Learning:** Checking `[Authorize]` at the controller/method level only confirms the user is logged in. It does not ensure they own or have rights to modify the specific resource being requested, leading to IDOR.
**Prevention:** For endpoints that modify specific resources, always retrieve the user's ID via `User.FindFirst(ClaimTypes.NameIdentifier)` and compare it against the resource owner's ID (e.g., `turno.BarberoId`). Return `Forbid()` if there's a mismatch and the user lacks elevated privileges like the 'Admin' role.
