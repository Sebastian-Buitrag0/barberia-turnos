## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-05-24 - Overly Permissive CORS Configuration Vulnerability
**Vulnerability:** The backend's CORS configuration in `Program.cs` used `.SetIsOriginAllowed(origin => true)` alongside `.AllowCredentials()`. This configuration effectively negates the `WithOrigins(allowedOrigins)` restriction, permitting any origin to perform cross-origin requests with credentials (cookies, auth headers).
**Learning:** Combining a wildcard-like origin policy (`.SetIsOriginAllowed(origin => true)`) with credentials enabled opens the application to Cross-Origin Resource Sharing bypass vulnerabilities, which can lead to data exposure and unauthorized actions by malicious sites.
**Prevention:** Always restrict CORS explicitly to known and trusted `AllowedOrigins` when credentials are required. Never use wildcard origins (`*` or equivalent functions) in tandem with `.AllowCredentials()`.
