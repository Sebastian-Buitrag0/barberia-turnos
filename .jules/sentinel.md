## 2025-02-18 - [Fix Plaintext PIN Storage]
**Vulnerability:** User PINs were stored in plaintext in the database and used as the primary identifier for login.
**Learning:** Using a secret (PIN/Password) as the only identifier complicates secure storage because you cannot efficiently look up a user by a salted hash.
**Prevention:** Store a non-secret identifier (like username or email) alongside the secret. If restricted to PIN-only authentication, accept the trade-off of iterating through all users to verify the hash (acceptable for small datasets) or use a deterministic index (less secure).
