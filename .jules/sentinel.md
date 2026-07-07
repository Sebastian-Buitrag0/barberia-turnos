## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-29 - Fail-Open Webhook Validation
**Vulnerability:** The `TwilioWebhookController`'s signature validation bypassed the check (fail-open) if the `Twilio:AuthToken` was missing, allowing unauthenticated attackers to spoof webhook requests in production if the environment was misconfigured.
**Learning:** Security validations should never fail-open in production based on missing configuration secrets. This creates a critical vulnerability when a deployment environment is missing a required secret.
**Prevention:** Explicitly check the environment (e.g. using `IWebHostEnvironment.IsDevelopment()`) before allowing bypasses for local development, and default to failing securely in all other environments.
