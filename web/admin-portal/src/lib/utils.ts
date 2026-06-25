import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export interface TokenClaims {
  sub?: string
  email?: string
  role?: string
  tenantId?: string
}

export function getTokenClaims(): TokenClaims {
  if (typeof window === "undefined") return {}
  try {
    const token = localStorage.getItem("litxus_token")
    if (!token) return {}
    const payload = token.split(".")[1]
    const decoded = JSON.parse(atob(payload.replace(/-/g, "+").replace(/_/g, "/")))
    return {
      sub: decoded.sub,
      email: decoded.email,
      role: decoded.role,
      tenantId: decoded.tenantId,
    }
  } catch {
    return {}
  }
}

export function formatCurrency(amount: number, currency = "RM") {
  return `${currency} ${amount.toLocaleString()}`
}

export function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString("en-MY", {
    day: "numeric", month: "short", year: "numeric",
  })
}

export { getStatusColor } from "@/lib/statuses"
