## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-30 - PII Exposure in Unauthenticated Turno Endpoints
**Vulnerability:** Endpoints retrieving lists of turnos (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) were publicly accessible and returned `TurnoResponseDto`, which includes Personally Identifiable Information (PII) like `Cliente.Telefono`. This allowed any user to extract phone numbers of all clients in the queue.
**Learning:** When creating DTOs for frontend consumption, it is easy to accidentally leak sensitive fields if the DTO is shared between authenticated and unauthenticated contexts. If an endpoint must be public, it must use a sanitized DTO (e.g., omitting the phone number). Since these specific endpoints are only used by authenticated Staff/Admins, they must be explicitly protected.
**Prevention:** Always verify if a returned DTO contains PII. If it does, strictly enforce `[Authorize]` (or role-based authorization) on all endpoints returning it, or create a separate, sanitized public DTO.
