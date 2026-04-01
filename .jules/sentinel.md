## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-06-05 - Overly Permissive CORS Configuration
**Vulnerability:** The CORS configuration in `Program.cs` used `.SetIsOriginAllowed(origin => true)` alongside `.AllowCredentials()`. This bypasses origin checks, allowing any website to make authenticated cross-origin requests, leading to severe CSRF and data exposure risks.
**Learning:** ASP.NET Core requires a specific, limited list of allowed origins when `.AllowCredentials()` is enabled. Permitting all origins while accepting credentials negates CORS protections.
**Prevention:** Never use `.SetIsOriginAllowed(origin => true)` or wildcard (`*`) origins when `.AllowCredentials()` is present. Always validate origins against an explicitly defined, trusted list.
