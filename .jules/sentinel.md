## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-29 - Missing Authorization and Broken Access Control
**Vulnerability:** Endpoints `GetTurnosHoy`, `GetPorPagar`, and `GetCola` in `TurnosController` lacked method-level `[Authorize]` attributes. Because they return DTOs with Personally Identifiable Information (PII) like `Cliente.Telefono`, unauthenticated users could access this data, resulting in both Broken Access Control and Information Exposure.
**Learning:** In ASP.NET Core, simply omitting authorization attributes on controllers without global filters leaves endpoints public by default. Additionally, read-only GET endpoints must be authorized if they expose sensitive data or PII, not just those that modify data.
**Prevention:** Default to applying `[Authorize]` at the controller level or employ a secure-by-default global policy, explicitly opting-in to `[AllowAnonymous]` only for truly public endpoints. Ensure that DTOs returned by endpoints align with the least-privilege principle.

## 2024-05-29 - CORS Misconfiguration
**Vulnerability:** The CORS configuration in `Program.cs` incorrectly combined `.WithOrigins(allowedOrigins)` with `.SetIsOriginAllowed(origin => true)`. This allowed any origin to connect, effectively bypassing the `allowedOrigins` whitelist while also enabling credentials (`.AllowCredentials()`), leading to potential Cross-Origin request forgery risks.
**Learning:** The `.SetIsOriginAllowed(origin => true)` method overrides other origin restrictions, accepting any origin. It should not be used in combination with `.AllowCredentials()` unless explicitly required for a strictly controlled internal API, as it fundamentally bypasses origin-based security checks.
**Prevention:** Strictly rely on explicitly configured allowed origins (`.WithOrigins(...)`) and avoid using wildcards or `.SetIsOriginAllowed(origin => true)` when credentials are permitted.
