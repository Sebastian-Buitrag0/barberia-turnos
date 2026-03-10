## 2025-02-28 - [Critical] Missing Authorization on Admin Endpoints in UsuariosController
**Vulnerability:** Missing authorization on `api/usuarios/barberos` endpoints allowed unauthenticated users to fetch, create, edit, and delete barber profiles.
**Learning:** Administrative endpoints were not uniformly protected by the `[Authorize(Roles = "Admin")]` attribute, leading to a Broken Access Control vulnerability. The controller level was unauthenticated, and endpoints relied on individual attributes that were missing.
**Prevention:** Consistently apply `[Authorize(Roles = "Admin")]` to all endpoints intended for administrative actions, or apply it at the controller level and explicitly use `[AllowAnonymous]` for public endpoints. Always verify authorization on CRUD endpoints.
