## 2026-03-05 - [Missing Authorization on Admin Endpoints]
**Vulnerability:** Administrative endpoints (creating/editing/deleting barbers, registering turns as admin, getting turns to pay) lack `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated users to perform administrative actions.
**Learning:** Endpoints meant for admin users must have explicit authorization attributes; relying only on frontend routing leaves the API exposed.
**Prevention:** Always apply the principle of least privilege by decorating sensitive API endpoints with appropriate `[Authorize]` constraints based on roles.
