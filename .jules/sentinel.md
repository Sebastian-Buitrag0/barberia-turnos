## 2025-03-09 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., `UsuariosController` for barberos CRUD and `TurnosController` for `RegistrarAdmin`) lacked proper role-based authorization attributes.
**Learning:** This missing authorization allows any unauthenticated or unauthorized user to access and modify administrative data, bypassing security controls.
**Prevention:** Always verify and enforce role-based access control (RBAC) via `[Authorize(Roles = "Admin")]` on all controller endpoints that modify or retrieve sensitive/administrative data. Use integration tests to ensure that these endpoints return `401 Unauthorized` or `403 Forbidden` for non-admin users.
