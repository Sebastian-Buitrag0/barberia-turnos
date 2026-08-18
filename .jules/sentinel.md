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

## 2024-05-30 - Insecure Direct Object Reference (IDOR) on Resource State Modification
**Vulnerability:** The endpoints `EnSilla` and `Finalizar` in `TurnosController.cs` lacked resource ownership validation. Although they required authentication (`[Authorize]`), any authenticated barber could submit a valid `TurnoId` and modify the state of another barber's appointment.
**Learning:** Authentication is not authorization. Simply verifying that a user is logged in (even with a specific role like "Barbero") is insufficient when an endpoint operates on a specific resource identifier provided by the client. The backend must always verify that the authenticated user is authorized to perform the requested action on that *specific* resource.
**Prevention:** For any endpoint modifying a specific resource, extract the authenticated user's ID from the current context (e.g., `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`) and verify it against the resource's owner ID (e.g., `turno.BarberoId`). Return `Forbid()` (403) if they do not match, unless an administrative override (e.g., `User.IsInRole("Admin")`) is applicable.
