## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-29 - Missing Authorization on Sensitive DTO Endpoints
**Vulnerability:** Endpoints retrieving appointments (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) lacked authorization checks, exposing Personally Identifiable Information (PII) like `Cliente.Telefono` from `TurnoResponseDto` to unauthenticated users.
**Learning:** Returning detailed DTOs on public or unauthenticated endpoints creates a serious data leakage risk. Any GET endpoint exposing PII or sensitive operational states must have explicit `[Authorize]` attributes matching the frontend role consumption.
**Prevention:** Apply strict `[Authorize]` (and role-based `[Authorize(Roles = "...")]` where appropriate) on every endpoint returning PII data, and implement separate DTOs for public vs authenticated access to ensure zero-trust data exposure.
