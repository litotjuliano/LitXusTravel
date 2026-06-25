"use client"

import { createContext, useContext, ReactNode } from "react"
import { useNotifications, UseNotificationsResult } from "@/lib/hooks/useNotifications"

const NotificationsContext = createContext<UseNotificationsResult | null>(null)

export function NotificationsProvider({ children }: { children: ReactNode }) {
  const value = useNotifications()
  return (
    <NotificationsContext.Provider value={value}>
      {children}
    </NotificationsContext.Provider>
  )
}

export function useNotificationsContext(): UseNotificationsResult {
  const ctx = useContext(NotificationsContext)
  if (!ctx) throw new Error("useNotificationsContext must be used within NotificationsProvider")
  return ctx
}
