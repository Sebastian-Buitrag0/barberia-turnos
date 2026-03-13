## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.
## 2024-03-13 - [Missing Authorization on Sensitive Endpoints]
**Vulnerability:** Several GET endpoints in `TurnosController` (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) were returning `TurnoResponseDto` containing PII (customer names, phone numbers) without requiring authentication.
**Learning:** Read-only API endpoints returning DTOs with PII might be overlooked during security reviews if they only fetch data, but they must be explicitly protected with `[Authorize]` attributes to prevent unauthorized data exposure.
**Prevention:** Ensure that all endpoints returning PII, regardless of HTTP method, have explicit `[Authorize]` attributes. In controllers handling mixed access roles, apply `[Authorize(Roles = "...")]` attributes explicitly at the method level.
