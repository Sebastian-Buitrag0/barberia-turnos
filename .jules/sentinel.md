## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-30 - PII Exposure via Unauthenticated API Endpoints
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetCola`, and `GetPorPagar` in `TurnosController` lacked `[Authorize]` attributes, allowing unauthenticated users to access sensitive data, such as client names, phone numbers, and barber assignments.
**Learning:** Endpoints returning Data Transfer Objects (DTOs) with Personally Identifiable Information (PII) must be explicitly protected, even if they are read-only (GET requests). A missing attribute leaves them entirely public.
**Prevention:** Apply the `[Authorize]` attribute to all endpoints that handle sensitive information or user-specific data to enforce authentication. Consider implementing endpoint scanning or integration tests that assert 401/403 responses for unauthenticated requests.
