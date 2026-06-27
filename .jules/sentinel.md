## 2026-06-27 - Insecure Direct Object References in TurnosController
**Vulnerability:** The endpoints `EnSilla` and `Finalizar` in `TurnosController` lacked resource ownership checks, meaning any authenticated user could modify the state of any appointment, potentially leading to unauthorized data manipulation or service disruption.
**Learning:** When modifying resources in EF Core, the existence of a valid JWT token (`[Authorize]`) does not guarantee the user has the right to modify the specific resource they are requesting. Extracting the authenticated user's ID via `User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value` and checking if they are the owner of the resource or hold an administrative role is crucial.
**Prevention:** Always verify that the authenticated user's ID matches the owner of the resource they are attempting to modify. For operations restricted to resource owners, return `Forbid()` (or `Unauthorized()`) if the IDs do not match and the user lacks admin privileges.

## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.
