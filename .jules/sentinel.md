## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-06-05 - Insecure CORS Wildcard
**Vulnerability:** The CORS policy configuration in `Program.cs` used `.SetIsOriginAllowed(origin => true)` alongside `.AllowCredentials()`. This explicitly allowed any origin to make cross-origin requests with credentials (cookies, auth headers), completely bypassing the intended explicit whitelist defined in `AllowedOrigins` and opening the application to Cross-Origin Resource Sharing bypass vulnerabilities.
**Learning:** In ASP.NET Core, `.SetIsOriginAllowed(origin => true)` behaves functionally identical to a wildcard `*` but is permitted when used with `.AllowCredentials()`. This combination defeats the purpose of maintaining an allowed origins list and introduces a critical security flaw.
**Prevention:** Always restrict `.WithOrigins` to specific, trusted domains when using `.AllowCredentials()`. Do not use `.SetIsOriginAllowed(origin => true)` globally alongside credentials.
