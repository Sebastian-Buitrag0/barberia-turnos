## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-07-01 - Overly Permissive CORS Configuration
**Vulnerability:** The backend's CORS configuration used `.SetIsOriginAllowed(origin => true)` alongside `.AllowCredentials()`. This bypasses the intended `WithOrigins` restrictions, allowing any arbitrary website to make authenticated cross-origin requests to the API, creating a severe CORS bypass and Cross-Site Request Forgery (CSRF) risk.
**Learning:** In ASP.NET Core, `.SetIsOriginAllowed(origin => true)` overrides `.WithOrigins(...)`, dynamically permitting any origin. Combining this with credentials support (`.AllowCredentials()`) violates security best practices and exposes the application to cross-origin attacks.
**Prevention:** Never use `.SetIsOriginAllowed(origin => true)` globally, especially with `.AllowCredentials()`. Always restrict `.WithOrigins()` to a strictly defined, configuration-driven list of trusted origins (e.g., specific frontend domains).
