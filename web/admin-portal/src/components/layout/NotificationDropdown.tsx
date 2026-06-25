"use client"

import { useRouter } from "next/navigation"
import { CheckCircle2, Bell, Inbox } from "lucide-react"
import { Dropdown } from "@/components/ui/Dropdown"
import { NotificationItem } from "@/lib/api"

function relativeTime(iso: string): string {
  const diff = Date.now() - new Date(iso).getTime()
  const mins = Math.floor(diff / 60_000)
  if (mins < 1) return "Just now"
  if (mins < 60) return `${mins} min${mins === 1 ? "" : "s"} ago`
  const hours = Math.floor(mins / 60)
  if (hours < 24) return `${hours} hour${hours === 1 ? "" : "s"} ago`
  return new Date(iso).toLocaleDateString(undefined, { month: "short", day: "numeric" })
}

interface Props {
  isOpen: boolean
  onClose: () => void
  notifications: NotificationItem[]
  unreadCount: number
  loading: boolean
  onMarkRead: (id: string) => Promise<void>
  onMarkAllRead: () => Promise<void>
}

export function NotificationDropdown({
  isOpen, onClose, notifications, unreadCount, loading, onMarkRead, onMarkAllRead,
}: Props) {
  const router = useRouter()

  const handleItemClick = (n: NotificationItem) => {
    if (!n.isRead) onMarkRead(n.id)
    onClose()
    router.push(`/notifications?selected=${n.id}`)
  }

  return (
    <Dropdown isOpen={isOpen} onClose={onClose} className="w-80 py-0 overflow-hidden">
      {/* Header */}
      <div className="flex items-center justify-between px-4 py-3 border-b border-gray-100 dark:border-gray-800">
        <div className="flex items-center gap-2">
          <span className="text-sm font-semibold text-gray-900 dark:text-white">Notifications</span>
          {unreadCount > 0 && (
            <span className="inline-flex items-center justify-center min-w-[20px] h-5 px-1.5 text-[10px] font-bold bg-brand-500 text-white rounded-full">
              {String(unreadCount).padStart(2, "0")}
            </span>
          )}
        </div>
        {unreadCount > 0 && (
          <button
            onClick={onMarkAllRead}
            className="text-xs text-brand-500 hover:text-brand-600 font-medium transition-colors"
          >
            Mark all read
          </button>
        )}
      </div>

      {/* List */}
      <div className="max-h-80 overflow-y-auto divide-y divide-gray-50 dark:divide-gray-800/60">
        {loading && notifications.length === 0 && (
          <div className="py-8 text-center text-sm text-gray-400">Loading…</div>
        )}

        {!loading && notifications.length === 0 && (
          <div className="py-10 flex flex-col items-center gap-2 text-gray-400">
            <Bell size={22} className="opacity-40" />
            <span className="text-sm">No notifications</span>
          </div>
        )}

        {notifications.map((n) => (
          <button
            key={n.id}
            onClick={() => handleItemClick(n)}
            className={`w-full text-left flex items-start gap-3 px-4 py-3.5 transition-colors hover:bg-gray-50 dark:hover:bg-gray-800/50 ${
              !n.isRead ? "bg-blue-50/60 dark:bg-blue-950/20" : ""
            }`}
          >
            {/* Icon avatar */}
            <div className="shrink-0 mt-0.5 flex items-center justify-center w-9 h-9 rounded-full bg-green-100 dark:bg-green-900/30">
              <CheckCircle2 size={18} className="text-green-600 dark:text-green-400" />
            </div>

            {/* Content */}
            <div className="flex-1 min-w-0">
              <p className={`text-sm leading-snug truncate ${!n.isRead ? "font-semibold text-gray-900 dark:text-white" : "font-medium text-gray-700 dark:text-gray-300"}`}>
                {n.title}
              </p>
              <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5 line-clamp-2 leading-relaxed">
                {n.message}
              </p>
              <p className="text-[11px] text-gray-400 dark:text-gray-500 mt-1">
                {relativeTime(n.createdAt)}
              </p>
            </div>

            {/* Unread dot */}
            {!n.isRead && (
              <div className="shrink-0 mt-2 w-2 h-2 rounded-full bg-brand-500" />
            )}
          </button>
        ))}
      </div>

      {/* Footer: link to full inbox */}
      <div className="border-t border-gray-100 dark:border-gray-800">
        <button
          onClick={() => { onClose(); router.push("/notifications") }}
          className="w-full flex items-center justify-center gap-2 px-4 py-2.5 text-xs font-medium text-brand-500 hover:bg-gray-50 dark:hover:bg-gray-800/50 transition-colors"
        >
          <Inbox size={13} />
          View all in Inbox
        </button>
      </div>
    </Dropdown>
  )
}
