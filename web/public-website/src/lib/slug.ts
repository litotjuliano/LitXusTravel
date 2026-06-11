import { createCipheriv, createDecipheriv, createHash } from "crypto"

// SECRET must be set in .env.local for production — never commit real keys
const SECRET = process.env.SLUG_SECRET ?? "nexustravel-default-slug-secret-32!"

// 32-byte AES key derived from secret
const KEY = createHash("sha256").update(SECRET).digest()

// Deterministic IV derived from the input — same ID always → same token
// This is intentional: stable URLs for SEO and link sharing
function deriveIv(id: string): Buffer {
  return createHash("md5").update(id + SECRET).digest() // 16 bytes
}

/** Encrypt a plain ID into a URL-safe Base64 token */
export function encryptSlug(id: string): string {
  const iv = deriveIv(id)
  const cipher = createCipheriv("aes-256-cbc", KEY, iv)
  const encrypted = Buffer.concat([cipher.update(id, "utf8"), cipher.final()])
  // Prepend IV so decrypt doesn't need it stored separately
  return Buffer.concat([iv, encrypted]).toString("base64url")
}

/** Decrypt a URL-safe Base64 token back to the original ID.
 *  Returns null if the token is invalid or tampered. */
export function decryptSlug(token: string): string | null {
  try {
    const buf = Buffer.from(token, "base64url")
    if (buf.length < 17) return null // IV (16) + at least 1 byte of ciphertext
    const iv = buf.subarray(0, 16)
    const enc = buf.subarray(16)
    const decipher = createDecipheriv("aes-256-cbc", KEY, iv)
    const decrypted = Buffer.concat([decipher.update(enc), decipher.final()])
    return decrypted.toString("utf8")
  } catch {
    return null
  }
}
