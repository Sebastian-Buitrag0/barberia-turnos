## 2025-05-20 - Plaintext PINs & Architecture Gap
**Vulnerability:** PINs were stored in plaintext in the database and login relied on exact PIN match via SQL query.
**Learning:** The "Login by PIN" architecture without usernames creates a unique challenge: secure hashing (with random salts) prevents simple "lookup user by credential" SQL queries.
**Prevention:** When designing "credential-only" login systems, consider deterministic hashing or prepare for O(N) verification if the user base is small. Always use PBKDF2 or similar for credentials.
