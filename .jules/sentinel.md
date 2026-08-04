## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-07-08 - Missing Authorization Leading to PII Exposure
**Vulnerability:** Endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) lacked `[Authorize]` attributes, exposing the `TurnoResponseDto` which contains PII (client phone numbers) to unauthenticated users.
**Learning:** Returning DTOs that include sensitive information like phone numbers necessitates strict endpoint authorization. Even if endpoints are only consumed by authenticated frontend components, lacking backend validation means anyone can access the API and harvest PII.
**Prevention:** Apply `[Authorize]` attributes (and specific roles if needed) to all endpoints that expose PII, and consider whether PII is strictly necessary for all responses or if it can be omitted or masked.

## 2026-08-04 - Insecure Direct Object Reference (IDOR) in TurnosController
**Vulnerability:** The endpoints `EnSilla` and `Finalizar` in `TurnosController.cs` allowed any authenticated user (e.g., any barber) to modify the state of a `Turno`, even if it was assigned to another barber, by manipulating the `TurnoId` in the request body.
**Learning:** When verifying resource ownership in backend controllers to prevent IDOR, we must extract the authenticated user's ID using `User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value` and parse it to an integer. Return `Forbid()` (or `Unauthorized()` if unparseable) if the resource owner ID does not match the parsed ID and the user lacks the 'Admin' role.
**Prevention:** Ensure that any endpoint that updates or deletes a specific resource includes authorization logic to check that the current user owns that resource (or has sufficient admin privileges to override the check).
