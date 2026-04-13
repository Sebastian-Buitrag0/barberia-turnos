## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-04-13 - Insecure Direct Object Reference (IDOR) on Appointment State Endpoints
**Vulnerability:** The `EnSilla` and `Finalizar` endpoints in `TurnosController` allowed any authenticated user to modify the state of any appointment, even those assigned to other barbers. An attacker could manipulate the appointment flow or add arbitrary services to other users' appointments.
**Learning:** The endpoints lacked resource ownership validation. While they checked if the user was authenticated, they failed to verify if the authenticated user was authorized to act on the specific `Turno` ID provided in the request payload.
**Prevention:** Always extract the authenticated user's context (e.g., ID from claims) and verify ownership of the requested resource against the database before performing any state modifications or returning sensitive data.
