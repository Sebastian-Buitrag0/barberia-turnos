## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-07-08 - Missing Authorization Leading to PII Exposure
**Vulnerability:** Endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) lacked `[Authorize]` attributes, exposing the `TurnoResponseDto` which contains PII (client phone numbers) to unauthenticated users.
**Learning:** Returning DTOs that include sensitive information like phone numbers necessitates strict endpoint authorization. Even if endpoints are only consumed by authenticated frontend components, lacking backend validation means anyone can access the API and harvest PII.
**Prevention:** Apply `[Authorize]` attributes (and specific roles if needed) to all endpoints that expose PII, and consider whether PII is strictly necessary for all responses or if it can be omitted or masked.

## 2024-05-18 - Prevent CORS bypass in .NET Backend
**Vulnerability:** The backend's CORS configuration used `.SetIsOriginAllowed(origin => true)` alongside `.AllowCredentials()`. This explicitly bypassed the `AllowedOrigins` array, allowing any origin to make authenticated cross-origin requests, which creates a high-risk CSRF vector.
**Learning:** In ASP.NET Core, `.SetIsOriginAllowed(origin => true)` overrides specific origin restrictions. When paired with `.AllowCredentials()`, it effectively acts as a wildcard origin for credentials, which is normally blocked by modern browsers for security reasons.
**Prevention:** Never use `.SetIsOriginAllowed(origin => true)` with `.AllowCredentials()` unless you intend to completely disable origin-based CORS restrictions. Always explicitly define `AllowedOrigins` when credentials are required.
