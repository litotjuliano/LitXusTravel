"use client"

import { useEffect } from "react"

/**
 * Clears authentication on app startup (development only)
 * This ensures users are logged out when the dev server restarts
 */
export function AppInitializer() {
  useEffect(() => {
    // Only run on initial app load using a session marker
    const hasRunOnThisSession = sessionStorage.getItem("app_initialized")

    if (!hasRunOnThisSession) {
      // First time this session - clear old tokens from previous sessions
      localStorage.removeItem("nexus_token")
      sessionStorage.setItem("app_initialized", "true")

      // Redirect to login if on a protected route
      if (typeof window !== "undefined" && !window.location.pathname.startsWith("/auth")) {
        window.location.href = "/auth/login"
      }
    }
  }, [])

  return null
}
