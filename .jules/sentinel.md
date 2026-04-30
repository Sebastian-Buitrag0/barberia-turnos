## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - IDOR Vulnerability in Resource Modification Endpoints
**Vulnerability:** In `TurnosController.cs`, endpoints that modified specific resources like `EnSilla` and `Finalizar` lacked resource ownership authorization checks. Any authenticated Barbero could modify `Turno` records belonging to other Barberos.
**Learning:** Checking for authentication (`[Authorize]`) is not enough for state-modifying endpoints. Direct references to objects (like `TurnoId`) must be validated against the authenticated user's claims to ensure they own or have permission to modify that specific resource.
**Prevention:** Always extract the authenticated user's ID using `User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value` and verify it against the resource owner's ID (or verify the user is an Admin) before permitting state-modifying operations.
