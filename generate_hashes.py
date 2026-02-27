import hashlib
import binascii
import os

def generate_hash(password, salt=None):
    if salt is None:
        salt = os.urandom(16)

    # PBKDF2 with SHA256, 100000 iterations, 32 bytes hash
    dk = hashlib.pbkdf2_hmac('sha256', password.encode('utf-8'), salt, 100000, dklen=32)

    salt_b64 = binascii.b2a_base64(salt).decode('utf-8').strip()
    hash_b64 = binascii.b2a_base64(dk).decode('utf-8').strip()

    return f"{salt_b64}.{hash_b64}"

print(f"0000: {generate_hash('0000')}")
print(f"1111: {generate_hash('1111')}")
print(f"2222: {generate_hash('2222')}")
