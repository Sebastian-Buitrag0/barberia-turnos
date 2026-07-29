## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-07-08 - Missing Authorization Leading to PII Exposure
**Vulnerability:** Endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) lacked `[Authorize]` attributes, exposing the `TurnoResponseDto` which contains PII (client phone numbers) to unauthenticated users.
**Learning:** Returning DTOs that include sensitive information like phone numbers necessitates strict endpoint authorization. Even if endpoints are only consumed by authenticated frontend components, lacking backend validation means anyone can access the API and harvest PII.
**Prevention:** Apply `[Authorize]` attributes (and specific roles if needed) to all endpoints that expose PII, and consider whether PII is strictly necessary for all responses or if it can be omitted or masked.

## 2024-05-30 - Twilio Webhook Fail-Open and Wildcard CORS with Credentials
**Vulnerability:**
1. The `TwilioWebhookController` bypassed signature validation if the `Twilio:AuthToken` configuration was missing, leading to a fail-open scenario in production where attackers could spoof incoming WhatsApp messages.
2. The CORS policy in `Program.cs` used `.SetIsOriginAllowed(origin => true)` alongside `.AllowCredentials()`. This explicitly bypassed the `AllowedOrigins` list, essentially creating a wildcard origin with credentials allowed, leading to severe Cross-Origin Request vulnerabilities.
**Learning:**
1. Failsafes meant for local development (like skipping external webhook validation) must be strictly gated using `IWebHostEnvironment.IsDevelopment()` to prevent them from executing in production if configuration secrets are accidentally missing.
2. In ASP.NET Core, `.SetIsOriginAllowed(origin => true)` overrides specific origin lists. It must never be used in conjunction with `.AllowCredentials()` in production environments, as it nullifies origin restrictions.
**Prevention:**
1. Always inject `IWebHostEnvironment` and check `IsDevelopment()` before bypassing critical security checks or validations. Fail closed (deny access) if required configuration is missing in production.
2. Rely strictly on explicit `WithOrigins(...)` arrays for CORS policies that require credentials.
