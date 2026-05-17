## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Insecure Direct Object Reference (IDOR) on Turno Modification
**Vulnerability:** The endpoints `EnSilla` and `Finalizar` in `TurnosController` accepted a `TurnoId` from the request body but did not verify if the authenticated user (barber) actually owned the given Turno before modifying its state. This allowed any authenticated user to potentially modify or finalize turns assigned to other barbers.
**Learning:** Endpoints that modify specific resources must verify resource ownership to prevent Insecure Direct Object References (IDOR), especially when the resource identifier is provided directly by the client.
**Prevention:** Always extract the authenticated user's ID from the token claims and compare it against the resource owner's ID (e.g., `Turno.BarberoId`) before proceeding with the modification, granting fallback access only to authorized roles like `Admin`.
