## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-07-08 - Missing Authorization Leading to PII Exposure
**Vulnerability:** Endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) lacked `[Authorize]` attributes, exposing the `TurnoResponseDto` which contains PII (client phone numbers) to unauthenticated users.
**Learning:** Returning DTOs that include sensitive information like phone numbers necessitates strict endpoint authorization. Even if endpoints are only consumed by authenticated frontend components, lacking backend validation means anyone can access the API and harvest PII.
**Prevention:** Apply `[Authorize]` attributes (and specific roles if needed) to all endpoints that expose PII, and consider whether PII is strictly necessary for all responses or if it can be omitted or masked.

## 2024-05-29 - Fail-Open Authorization Logic Bypass
**Vulnerability:** A fail-open vulnerability existed in `TwilioWebhookController.ValidateTwilioRequest` where missing Twilio configuration (`Twilio:AuthToken`) bypassed security signature validation unconditionally, applying development fallback behavior even in production.
**Learning:** Security fallback logic designed for local development must always be explicitly guarded by environment checks (e.g., `_env.IsDevelopment()`). Failing to do so creates a state where unconfigured production deployments run without security controls, allowing unauthorized requests to pass.
**Prevention:** Never use missing configuration as a trigger for bypassing security checks unless explicitly verifying a safe environment. Production systems should default to "fail securely" (e.g., returning 403 Forbidden) when required security configuration is absent.

## 2024-05-30 - Insecure Direct Object Reference (IDOR) on Turno State Changes
**Vulnerability:** The `EnSilla` and `Finalizar` endpoints in `TurnosController` accepted a `TurnoId` from authenticated users and modified its state without verifying if the requested `Turno` belonged to the authenticated barbero. This allowed any authenticated user to arbitrarily change the state of any appointment.
**Learning:** Authenticating a user is not the same as authorizing them to perform an action on a specific resource. Endpoint-level `[Authorize]` only ensures the user is logged in, but business logic must explicitly verify resource ownership to prevent IDOR (Insecure Direct Object Reference).
**Prevention:** Always verify resource ownership in backend controllers. Extract the authenticated user's ID from claims (`User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value`) and ensure it matches the owner ID of the target resource. Return `Forbid()` (or `Unauthorized()`) if they do not match and the user lacks elevated privileges (e.g., 'Admin' role).
