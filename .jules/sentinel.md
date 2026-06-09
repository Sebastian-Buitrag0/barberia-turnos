## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Overly Permissive CORS Configuration
**Vulnerability:** The backend CORS configuration used `.SetIsOriginAllowed(origin => true)` alongside `.AllowCredentials()`. This bypasses origin restrictions, allowing any domain to send cross-origin requests with credentials (cookies, auth headers), potentially enabling CSRF and data exposure.
**Learning:** Using `.SetIsOriginAllowed(origin => true)` effectively defeats the security purpose of CORS restrictions when combined with `.AllowCredentials()`, as it dynamically echoes back any provided origin.
**Prevention:** Strictly rely on the explicit list of allowed origins defined in `.WithOrigins(...)` when credentialed requests are required.
