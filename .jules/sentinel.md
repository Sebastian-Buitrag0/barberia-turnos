## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Missing Resource Ownership Validation on Turnos
**Vulnerability:** The `EnSilla` and `Finalizar` endpoints in `TurnosController` lacked resource ownership validation, resulting in an Insecure Direct Object Reference (IDOR) / Broken Access Control. Any authenticated user (e.g., any barbero) could move the state of *any* appointment, rather than just the ones assigned to them.
**Learning:** Checking that a user is authenticated (`[Authorize]`) does not guarantee they are authorized to act upon specific resources. Ownership must be explicitly validated.
**Prevention:** In backend controllers, extract the authenticated user's ID using `User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value` and explicitly check that the parsed ID matches the resource's owner ID (e.g., `turno.BarberoId`), unless the user possesses an overriding role (e.g., `Admin`). Return `Forbid()` upon failure.
