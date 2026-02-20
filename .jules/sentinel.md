## 2025-02-12 - Critical: Plaintext PIN Exposure in Admin API

**Vulnerability:** The `GET /api/usuarios/barberos` endpoint returned a list of barbers including their plaintext PINs. The endpoint was also publicly accessible without authentication.
**Learning:** Default ASP.NET Core controllers are public. Returning full entity models or DTOs with sensitive fields (even if intended for admins) creates a massive information disclosure risk if access controls fail.
**Prevention:**
1. Always apply `[Authorize]` to sensitive controllers.
2. Never include passwords/PINs in Response DTOs, even for Admin APIs.
3. Use separate DTOs for Create/Update vs Read operations.
