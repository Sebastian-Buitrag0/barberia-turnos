## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2026-04-23 - Broken Access Control on PII Exposing Endpoints
**Vulnerability:** The endpoints `GetTurnosHoy`, `GetPorPagar`, and `GetCola` in `TurnosController` were missing `[Authorize]` attributes. These endpoints return DTOs containing Personally Identifiable Information (PII) such as the client's phone number, leading to unauthorized data exposure.
**Learning:** Read-only API endpoints (GET requests) returning Data Transfer Objects (DTOs) with Personally Identifiable Information (PII) must be explicitly protected. In this application architecture, frontend APIs are segmented by role, meaning backend endpoints must be secured to match the frontend expectations.
**Prevention:** Ensure that all endpoints returning PII enforce strict `[Authorize]` attributes, differentiating between `Admin` only endpoints and shared `Barbero` access.
