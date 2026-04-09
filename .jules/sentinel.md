## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2025-02-20 - Missing Authorization on Sensitive DTO Endpoints Leaking PII
**Vulnerability:** Read-only endpoints (`GetTurnosHoy`, `GetCola`, `GetPorPagar`) returning `TurnoResponseDto` were unauthenticated. Since the DTO includes the client's phone number (`Cliente.Telefono`), this allowed unauthenticated users to access Personally Identifiable Information (PII), resulting in a Broken Access Control vulnerability.
**Learning:** Endpoints that only return data (GET requests) are often overlooked for authorization compared to state-modifying endpoints. Even if an endpoint seems harmless, the data shape (DTO) it returns must be analyzed for sensitive fields like PII.
**Prevention:** Always evaluate the contents of returned DTOs. If a DTO contains sensitive data, the corresponding endpoint must be explicitly protected with `[Authorize]` and role-based restrictions appropriate for the data's sensitivity.
