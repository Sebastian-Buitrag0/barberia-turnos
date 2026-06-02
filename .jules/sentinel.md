## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - CORS Bypass Vulnerability via SetIsOriginAllowed
**Vulnerability:** The CORS configuration in `Program.cs` used `.SetIsOriginAllowed(origin => true)` alongside `.AllowCredentials()`, overriding the restricted `allowedOrigins` list and permitting any domain to make authenticated cross-origin requests.
**Learning:** Using `.SetIsOriginAllowed(origin => true)` with `.AllowCredentials()` creates a critical CORS bypass vulnerability, allowing malicious sites to read sensitive responses if a user is authenticated.
**Prevention:** Never use `.SetIsOriginAllowed(origin => true)` with `.AllowCredentials()`. Always explicitly specify the allowed origins using `.WithOrigins(...)` when credentials are permitted.
