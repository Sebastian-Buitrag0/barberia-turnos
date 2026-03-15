## 2024-03-15 - Missing Authentication on Sensitive Endpoints
**Vulnerability:** The endpoints `/api/turnos/hoy`, `/api/turnos/cola`, and `/api/turnos/porpagar` in `TurnosController.cs` were returning unmasked sensitive data (like `ClienteTelefono` and `ClienteNombre`) within `TurnoResponseDto` without requiring authentication.
**Learning:** This is a clear case of Broken Access Control (Insecure Direct Object Reference or Unauthenticated Access), allowing anyone to scrape customer phone numbers for the day.
**Prevention:** Always apply the `[Authorize]` attribute to any API endpoint that returns Personally Identifiable Information (PII) or sensitive business state, unless the endpoint is specifically designed to be public and the data is masked.
