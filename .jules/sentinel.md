## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-29 - Missing Resource Ownership Authorization (IDOR)
**Vulnerability:** In `TurnosController.cs`, endpoints like `EnSilla` and `Finalizar` were protected with `[Authorize]`, requiring authentication, but did not check if the requesting user (Barbero) actually owned the appointment being modified. Any authenticated user could modify appointments belonging to others.
**Learning:** `[Authorize]` only proves identity, not ownership. When a user requests to modify a specific resource by its ID (e.g., modifying `dto.TurnoId`), explicit code must verify that the resource's owner (e.g., `turno.BarberoId`) matches the authenticated user's ID, or that the user has administrative privileges.
**Prevention:** Implement resource-based authorization checks in endpoints that act on specific resources by retrieving the resource, verifying ownership against the `ClaimTypes.NameIdentifier` of the current user, and returning `Forbid()` if the check fails.
