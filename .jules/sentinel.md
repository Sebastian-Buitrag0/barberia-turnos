## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-06-16 - CORS Misconfiguration Allowing Credentials with Any Origin
**Vulnerability:** The `Program.cs` CORS configuration combined `SetIsOriginAllowed(origin => true)` with `AllowCredentials()`. This combination allows any origin to make authenticated requests via cookies or other credentials, nullifying the security benefit of the explicit `AllowedOrigins` list and opening up the application to Cross-Origin Resource Sharing bypass vulnerabilities.
**Learning:** `SetIsOriginAllowed(origin => true)` overrides any explicitly defined origins and allows all origins. Using it together with `AllowCredentials()` is a dangerous pattern that can expose applications to CSRF-like attacks and credential leakage to malicious domains.
**Prevention:** Never use `SetIsOriginAllowed(origin => true)` in combination with `AllowCredentials()`. Always strictly define the `WithOrigins(...)` list when allowing credentials.
