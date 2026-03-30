## 2024-05-30 - Overly Permissive CORS Configuration
**Vulnerability:** The CORS configuration used `.SetIsOriginAllowed(origin => true)` alongside `.AllowCredentials()`, effectively allowing any origin to make cross-origin requests with credentials, bypassing the `AllowedOrigins` restriction.
**Learning:** In ASP.NET Core, combining `.SetIsOriginAllowed(origin => true)` with credentials negates any specific origin restrictions set via `.WithOrigins()`, leading to a severe security risk where malicious sites can perform authenticated requests.
**Prevention:** Strictly rely on the `AllowedOrigins` list configured via `.WithOrigins(allowedOrigins)` and avoid using `.SetIsOriginAllowed(origin => true)` unless explicitly required for a highly controlled, dynamic origin scenario (which is rare).

## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.
