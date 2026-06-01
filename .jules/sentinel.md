## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-29 - Missing Resource Ownership Authorization (IDOR)
**Vulnerability:** The endpoints `/api/turnos/ensilla` and `/api/turnos/finalizar` allowed any authenticated user to modify turn states by simply providing a valid `TurnoId`. There was no check to ensure the user making the request actually owned the Turno or was an administrator.
**Learning:** Adding the `[Authorize]` attribute ensures the user is authenticated, but it does NOT guarantee authorization to access specific resources. When modifying specific entities, the user's ID or roles must be validated against the entity's owner.
**Prevention:** Always extract the authenticated user's ID from claims (`User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)`) and verify it matches the resource owner's ID (e.g., `turno.BarberoId == userId`), optionally allowing access for specific roles like 'Admin'. Return `Forbid()` if the check fails.
