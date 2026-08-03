## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-07-08 - Missing Authorization Leading to PII Exposure
**Vulnerability:** Endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) lacked `[Authorize]` attributes, exposing the `TurnoResponseDto` which contains PII (client phone numbers) to unauthenticated users.
**Learning:** Returning DTOs that include sensitive information like phone numbers necessitates strict endpoint authorization. Even if endpoints are only consumed by authenticated frontend components, lacking backend validation means anyone can access the API and harvest PII.
**Prevention:** Apply `[Authorize]` attributes (and specific roles if needed) to all endpoints that expose PII, and consider whether PII is strictly necessary for all responses or if it can be omitted or masked.

## 2025-02-28 - Fail-Open Webhook Signature Validation
**Vulnerability:** The `TwilioWebhookController` had a fail-open signature validation implementation. If `Twilio:AuthToken` was missing (e.g., in a production environment due to a misconfiguration), it bypassed the signature check completely and allowed any request to trigger the webhook.
**Learning:** Local development fallbacks (bypassing authentication or signature checks) can easily leak into production environments if they are not explicitly gated behind environment checks.
**Prevention:** Always gate development bypasses with strict environment checks (e.g., `_env.IsDevelopment()`) and ensure that the default behavior in non-development environments is to fail securely (fail closed).
