## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-29 - Overly Permissive CORS with Credentials
**Vulnerability:** The CORS configuration in `Program.cs` explicitly used `.SetIsOriginAllowed(origin => true)` alongside `.AllowCredentials()`. This overrode the intended restriction to `AllowedOrigins` and allowed any website to make authenticated cross-origin requests, leading to potential Cross-Site Request Forgery (CSRF) via CORS bypass or unauthorized data exfiltration.
**Learning:** In ASP.NET Core, `.SetIsOriginAllowed(origin => true)` bypasses explicit origin lists and returns `Access-Control-Allow-Origin: <Request-Origin>`. When combined with `.AllowCredentials()`, it fully defeats CORS security boundaries.
**Prevention:** Remove `.SetIsOriginAllowed(origin => true)` when using `.AllowCredentials()` and rely solely on an explicit list of `.WithOrigins(...)`.
