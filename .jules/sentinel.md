## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-07-08 - Missing Authorization Leading to PII Exposure
**Vulnerability:** Endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) lacked `[Authorize]` attributes, exposing the `TurnoResponseDto` which contains PII (client phone numbers) to unauthenticated users.
**Learning:** Returning DTOs that include sensitive information like phone numbers necessitates strict endpoint authorization. Even if endpoints are only consumed by authenticated frontend components, lacking backend validation means anyone can access the API and harvest PII.
**Prevention:** Apply `[Authorize]` attributes (and specific roles if needed) to all endpoints that expose PII, and consider whether PII is strictly necessary for all responses or if it can be omitted or masked.

## 2026-07-08 - Missing Resource Ownership Validation Leading to IDOR
**Vulnerability:** The endpoints `EnSilla` and `Finalizar` in `TurnosController` accepted `TurnoId` from the request body but failed to verify if the authenticated user (`Barbero`) modifying the state was actually the one assigned to that specific `Turno`. This constituted an Insecure Direct Object Reference (IDOR), allowing any authenticated user to modify another user's turn states.
**Learning:** Checking for authentication (`[Authorize]`) is not enough for endpoints modifying specific resources. Ownership of the resource must be verified against the authenticated context (the currently logged-in user).
**Prevention:** Implement resource ownership checks in endpoints modifying state by extracting the authenticated user's ID (`User.FindFirst(ClaimTypes.NameIdentifier)?.Value`), parsing it, and ensuring it matches the entity owner's ID unless the user has administrative privileges (`User.IsInRole("Admin")`).
