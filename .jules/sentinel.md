## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-04-29 - Broken Access Control / Data Exposure on Read Endpoints
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` were completely public despite returning PII (client phone numbers) in the `TurnoResponseDto`. Unauthenticated attackers could scrape sensitive appointment data.
**Learning:** Just because an endpoint is read-only (GET) does not mean it is safe to leave public. If the DTO contains PII, it must be protected. The frontend already separated these calls into authenticated views (`AdminView.vue`, `BarberoView.vue`), so adding backend enforcement did not break the public interface (`ClienteView.vue`).
**Prevention:** Always review DTO properties returned by an endpoint. If PII is included, explicitly enforce `[Authorize]` at the method level.
