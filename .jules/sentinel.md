## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-30 - Insecure Direct Object Reference (IDOR) in TurnosController
**Vulnerability:** The `EnSilla` and `Finalizar` endpoints in `TurnosController` modified `Turno` states based on user input without verifying if the authenticated user owned the resource.
**Learning:** Relying solely on `[Authorize]` is insufficient for endpoints modifying specific resources. An authenticated user could modify another user's resources if ownership is not checked.
**Prevention:** Explicitly verify resource ownership in controller endpoints by matching the authenticated user's ID (`User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value`) with the resource's owner ID (e.g., `Turno.BarberoId`), allowing exceptions for 'Admin' roles. Return `Forbid()` on mismatch.
