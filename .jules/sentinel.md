## 2025-05-22 - [Plaintext PIN Storage]
**Vulnerability:** User PINs were stored in plaintext in the database, allowing direct access to credentials if the database is compromised.
**Learning:** Legacy systems or simple apps often overlook hashing for "PINs" thinking they are not "passwords", but they serve the same authentication purpose.
**Prevention:** Always hash any secret used for authentication (PINs, passwords, API keys) using a strong algorithm like PBKDF2, Argon2, or BCrypt before storage.
