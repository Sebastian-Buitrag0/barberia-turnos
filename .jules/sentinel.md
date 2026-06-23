## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-10-26 - IDOR and Insecure CORS Configuration
**Vulnerability:** Found Broken Object Level Authorization (IDOR) in `TurnosController` (`EnSilla` and `Finalizar` methods) allowing users to manipulate other barbers' appointments. Additionally, found an insecure CORS configuration using `.SetIsOriginAllowed(origin => true)` alongside `.AllowCredentials()`, which bypassed explicit origin restrictions.
**Learning:** Resource modification endpoints must explicitly verify resource ownership by matching the resource's owner ID to the authenticated user's ID. In ASP.NET Core CORS, using `SetIsOriginAllowed` with a permissive lambda defeats origin validation when credentials are allowed.
**Prevention:** Always extract `ClaimTypes.NameIdentifier` and verify it matches the target entity's owner ID before modification, returning `Forbid()` if it doesn't match. Restrict CORS origins strictly to an explicit allowlist and avoid dynamic true-returning lambdas.
