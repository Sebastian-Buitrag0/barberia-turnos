## 2025-05-24 - Plaintext PIN Storage
**Vulnerability:** User PINs were stored in plaintext in the database (`Usuario.Pin`) and used as the primary identifier for login.
**Learning:** The application relies on PIN uniqueness but did not enforce it securely (direct SQL query). Hashing PINs prevents direct SQL lookups for login, necessitating a full table scan (`ToListAsync`) followed by in-memory verification (`PasswordHasher.Verify`). This is acceptable for low-volume apps like this barbershop but would be a scaling bottleneck.
**Prevention:** Use a separate unique identifier (e.g., Username or Email) for login lookups, allowing efficient database indexing, while keeping the password/PIN hashed.
