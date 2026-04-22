## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-04-22 - Insecure Direct Object Reference (IDOR) in Turno State Updates
**Vulnerability:** The `EnSilla` and `Finalizar` endpoints in `TurnosController` lacked resource ownership validation. Any authenticated barber could modify the state of turnos assigned to other barbers.
**Learning:** Adding the `[Authorize]` attribute only validates that a user is logged in, but it does not check if the user has permission to modify the specific resource requested in the payload.
**Prevention:** Always implement explicit authorization checks inside endpoints that modify resources. Compare the resource's owner ID against the authenticated user's ID extracted from the JWT token (`User.FindFirst(ClaimTypes.NameIdentifier)?.Value`).
