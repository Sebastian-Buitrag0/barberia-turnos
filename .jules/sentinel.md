## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-04-28 - Missing Resource Ownership Validation in State Changes
**Vulnerability:** Endpoints modifying specific resources (like `EnSilla` and `Finalizar` in `TurnosController`) lacked resource ownership authorization checks. An authenticated user (e.g. any barbero) could manipulate state for turnos they did not own, presenting an Insecure Direct Object Reference (IDOR) vulnerability.
**Learning:** Relying solely on the `[Authorize]` attribute ensures the user is authenticated but does not verify that the user is authorized to perform actions on a specific resource.
**Prevention:** When verifying resource ownership in backend controllers to prevent IDOR, extract the authenticated user's ID using `User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value` and parse it. Return `Forbid()` if the resource owner ID does not match the parsed ID and the user lacks elevated privileges (e.g., the 'Admin' role).
