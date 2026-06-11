## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2025-02-27 - [Fix IDOR in Turnos endpoints]
**Vulnerability:** Insecure Direct Object Reference (IDOR) in Turnos controller endpoints EnSilla and Finalizar. The endpoints accepted a TurnoId but did not verify if the current authenticated user was authorized to manage that specific Turno.
**Learning:** Even if an endpoint uses the [Authorize] attribute, it's necessary to ensure that the authenticated user is accessing or modifying records that belong to them (or they have sufficient permissions like Admin).
**Prevention:** Always verify resource ownership by extracting the user ID from claims (User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value) and comparing it to the owner of the resource being modified.
