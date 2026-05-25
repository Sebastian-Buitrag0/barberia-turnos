## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-05-25 - Missing Authorization on TurnosController GET Endpoints
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController.cs` lacked explicit authorization attributes (`[Authorize]`). These endpoints return `TurnoResponseDto`, which includes PII (Personally Identifiable Information) such as the client's `Telefono`. This constituted a Broken Access Control vulnerability allowing unauthenticated users to access sensitive data.
**Learning:** Returning objects with PII from endpoints requires rigorous access control. In ASP.NET Core controllers lacking class-level authorization, it's easy to overlook method-level attributes, unintentionally leaving data publicly accessible.
**Prevention:** Apply class-level `[Authorize]` attributes to controllers handling sensitive data, or ensure strict code review verifies that every endpoint returning PII has the correct `[Authorize]` or `[Authorize(Roles="...")]` attributes.
