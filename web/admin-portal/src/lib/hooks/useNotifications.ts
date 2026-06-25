import { useState, useEffect, useCallback, useRef } from "react"
import { adminApi, NotificationItem } from "@/lib/api"

const POLL_INTERVAL = 30_000

export interface UseNotificationsResult {
  notifications: NotificationItem[]
  unreadCount: number
  loading: boolean
  markRead: (id: string) => Promise<void>
  markAllRead: () => Promise<void>
  refetch: () => Promise<void>
}

export const useNotifications = (): UseNotificationsResult => {
  const [notifications, setNotifications] = useState<NotificationItem[]>([])
  const [unreadCount, setUnreadCount] = useState(0)
  const [loading, setLoading] = useState(false)
  const intervalRef = useRef<ReturnType<typeof setInterval> | null>(null)

  const fetchNotifications = useCallback(async () => {
    // Skip if no token (not logged in)
    if (typeof window === "undefined" || !localStorage.getItem("litxus_token")) return
    try {
      const res = await adminApi.getNotifications()
      setNotifications(res.data.data)
      setUnreadCount(res.data.unreadCount)
    } catch {
      // Silently fail — notifications are non-critical
    }
  }, [])

  const refetch = useCallback(async () => {
    setLoading(true)
    await fetchNotifications()
    setLoading(false)
  }, [fetchNotifications])

  useEffect(() => {
    refetch()

    intervalRef.current = setInterval(fetchNotifications, POLL_INTERVAL)

    const onFocus = () => fetchNotifications()
    window.addEventListener("focus", onFocus)

    return () => {
      if (intervalRef.current) clearInterval(intervalRef.current)
      window.removeEventListener("focus", onFocus)
    }
  }, [fetchNotifications, refetch])

  const markRead = useCallback(async (id: string) => {
    // Optimistic update
    setNotifications(prev => prev.map(n => n.id === id ? { ...n, isRead: true } : n))
    setUnreadCount(prev => Math.max(0, prev - 1))
    try {
      await adminApi.markNotificationRead(id)
    } catch {
      // Revert on failure by refetching
      fetchNotifications()
    }
  }, [fetchNotifications])

  const markAllRead = useCallback(async () => {
    // Optimistic update
    setNotifications(prev => prev.map(n => ({ ...n, isRead: true })))
    setUnreadCount(0)
    try {
      await adminApi.markAllNotificationsRead()
    } catch {
      fetchNotifications()
    }
  }, [fetchNotifications])

  return { notifications, unreadCount, loading, markRead, markAllRead, refetch }
}
