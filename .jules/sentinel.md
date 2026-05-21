## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - IDOR in Turno State Modifications
**Vulnerability:** The `EnSilla` and `Finalizar` endpoints in `TurnosController` only required `[Authorize]` (any authenticated user), allowing any barber to modify the state and services of appointments assigned to other barbers.
**Learning:** Checking for an authenticated session is not enough for endpoints that modify specific resources. A resource ownership check must be performed to prevent Insecure Direct Object References (IDOR).
**Prevention:** Always verify that the authenticated user (e.g., by checking their ID from JWT claims) matches the resource owner (e.g., `Turno.BarberoId`) or has administrative privileges before allowing state modifications.