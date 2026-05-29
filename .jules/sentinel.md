## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-29 - Insecure CORS Configuration
**Vulnerability:** The CORS configuration used `.SetIsOriginAllowed(origin => true)` alongside `.AllowCredentials()`. This effectively bypassed the intended `allowedOrigins` whitelist by reflecting any requested Origin header, allowing any origin to perform credentialed cross-origin requests, leading to potential CSRF or cross-origin data exposure.
**Learning:** When using `.AllowCredentials()`, combining it with a blanket `.SetIsOriginAllowed(origin => true)` defeats the entire purpose of CORS origin restrictions, effectively mimicking `Access-Control-Allow-Origin: *` but for credentialed requests.
**Prevention:** Never use `.SetIsOriginAllowed(origin => true)` with `.AllowCredentials()`. Always rely strictly on `.WithOrigins(allowedOrigins)` to explicitly whitelist trusted origins for credentialed access.
