## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Insecure CORS Misconfiguration
**Vulnerability:** The backend's CORS configuration in `Program.cs` used `.SetIsOriginAllowed(origin => true)` alongside `.AllowCredentials()`. This explicitly allows any origin (bypassing the `AllowedOrigins` list) to make authenticated requests. This is a severe misconfiguration that defeats CORS protections and can lead to CSRF or data exposure.
**Learning:** When using `.AllowCredentials()`, ASP.NET Core explicitly prevents `.AllowAnyOrigin()` to enforce security. However, developers sometimes bypass this restriction using `.SetIsOriginAllowed(origin => true)`, which is just as dangerous.
**Prevention:** Never use `.SetIsOriginAllowed(origin => true)` with `.AllowCredentials()`. Always explicitly define the allowed origins using `.WithOrigins(...)` when credentials/cookies are involved.
