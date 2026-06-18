## 2024-05-28 - Missing Authorization on Admin Endpoints
**Vulnerability:** Several administrative endpoints (e.g., managing barbers in `UsuariosController`, manually registering turns in `TurnosController`) lacked `[Authorize(Roles = "Admin")]` attributes, allowing unauthenticated or under-privileged users to perform administrative actions.
**Learning:** In ASP.NET Core, controllers without class-level `[Authorize]` attributes leave their endpoints public by default. Developers must explicitly secure every sensitive endpoint to prevent Broken Access Control.
**Prevention:** Implement a secure-by-default approach by applying `[Authorize]` at the controller level or enforce authorization checks globally, then selectively use `[AllowAnonymous]` for public endpoints.

## 2024-05-28 - Overly Permissive CORS and Missing Endpoint Authorization
**Vulnerability:** The application was vulnerable to broken access control and data exposure due to a combination of an overly permissive CORS configuration (`SetIsOriginAllowed(origin => true)` combined with `AllowCredentials()`) and unauthenticated endpoints that returned sensitive Personal Identifiable Information (PII) like phone numbers.
**Learning:** Using `SetIsOriginAllowed(origin => true)` essentially nullifies the `WithOrigins` restrictions and allows any cross-origin request to succeed, which is extremely dangerous when combined with `AllowCredentials()`. Additionally, any endpoint returning PII must explicitly implement role-based access control.
**Prevention:** Strictly define allowed origins in CORS policies and remove `SetIsOriginAllowed` unless dynamically validated against a known whitelist. Always use explicit `[Authorize]` tags on all endpoints that deal with PII.
