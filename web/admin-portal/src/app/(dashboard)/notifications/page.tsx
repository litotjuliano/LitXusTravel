"use client"

import { useState, useEffect, useRef, Suspense } from "react"
import { useSearchParams } from "next/navigation"
import { CheckCircle2, Bell, CheckCheck, Loader } from "lucide-react"
import { useNotificationsContext } from "@/context/NotificationsContext"
import { NotificationItem } from "@/lib/api"

function relativeTime(iso: string): string {
  const diff = Date.now() - new Date(iso).getTime()
  const mins = Math.floor(diff / 60_000)
  if (mins < 1) return "Just now"
  if (mins < 60) return `${mins} min${mins === 1 ? "" : "s"} ago`
  const hours = Math.floor(mins / 60)
  if (hours < 24) return `${hours} hour${hours === 1 ? "" : "s"} ago`
  const days = Math.floor(hours / 24)
  if (days < 7) return `${days} day${days === 1 ? "" : "s"} ago`
  return new Date(iso).toLocaleDateString(undefined, { month: "short", day: "numeric", year: "numeric" })
}

function NotificationCard({
  n,
  isSelected,
  onSelect,
  onMarkRead,
  cardRef,
}: {
  n: NotificationItem
  isSelected: boolean
  onSelect: (id: string) => void
  onMarkRead: (id: string) => void
  cardRef?: React.RefObject<HTMLDivElement>
}) {
  return (
    <div
      ref={cardRef}
      onClick={() => {
        onSelect(n.id)
        if (!n.isRead) onMarkRead(n.id)
      }}
      className={`flex items-start gap-4 px-6 py-4 border-b border-gray-100 dark:border-gray-800 transition-all cursor-pointer
        hover:bg-gray-50 dark:hover:bg-gray-800/40
        ${isSelected ? "ring-2 ring-inset ring-brand-500 bg-brand-50/60 dark:bg-brand-900/10" : !n.isRead ? "bg-blue-50/50 dark:bg-blue-950/10" : ""}
      `}
    >
      {/* Selected accent bar */}
      {isSelected && (
        <div className="absolute left-0 top-0 bottom-0 w-1 bg-brand-500 rounded-r" />
      )}

      {/* Avatar */}
      <div className={`shrink-0 mt-0.5 flex items-center justify-center w-10 h-10 rounded-full transition-colors ${
        isSelected ? "bg-brand-100 dark:bg-brand-900/40" : "bg-green-100 dark:bg-green-900/30"
      }`}>
        <CheckCircle2 size={20} className={isSelected ? "text-brand-500" : "text-green-600 dark:text-green-400"} />
      </div>

      {/* Body */}
      <div className="flex-1 min-w-0">
        <p className={`text-sm leading-snug ${
          isSelected
            ? "font-bold text-brand-700 dark:text-brand-300"
            : !n.isRead
            ? "font-semibold text-gray-900 dark:text-white"
            : "font-medium text-gray-700 dark:text-gray-300"
        }`}>
          {n.title}
        </p>
        <p className="text-sm text-gray-500 dark:text-gray-400 mt-1 leading-relaxed">
          {n.message}
        </p>
        <p className="text-xs text-gray-400 dark:text-gray-500 mt-1.5">
          {relativeTime(n.createdAt)}
        </p>
      </div>

      {/* Status indicator */}
      <div className="shrink-0 mt-2">
        {isSelected
          ? <span className="inline-block w-2.5 h-2.5 rounded-full bg-brand-500 ring-2 ring-brand-200 dark:ring-brand-800" />
          : !n.isRead
          ? <span className="inline-block w-2.5 h-2.5 rounded-full bg-brand-500" />
          : <span className="inline-block w-2.5 h-2.5" />
        }
      </div>
    </div>
  )
}

function NotificationsContent() {
  const { notifications, unreadCount, loading, markRead, markAllRead } = useNotificationsContext()
  const searchParams = useSearchParams()
  const [selectedId, setSelectedId] = useState<string | null>(searchParams.get("selected"))
  const [filter, setFilter] = useState<"all" | "unread">("all")
  const selectedRef = useRef<HTMLDivElement>(null)

  // Auto-switch to "all" tab if the selected item is read (so it's always visible)
  useEffect(() => {
    if (selectedId) setFilter("all")
  }, [selectedId])

  // Scroll to selected notification after render
  useEffect(() => {
    if (selectedId && selectedRef.current) {
      setTimeout(() => {
        selectedRef.current?.scrollIntoView({ behavior: "smooth", block: "center" })
      }, 100)
    }
  }, [selectedId, notifications.length])

  const displayed = filter === "unread"
    ? notifications.filter(n => !n.isRead)
    : notifications

  return (
    <div className="max-w-3xl">
      {/* Page header */}
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-3">
          <h1 className="text-xl font-semibold text-gray-900 dark:text-white">Inbox</h1>
          {unreadCount > 0 && (
            <span className="inline-flex items-center justify-center min-w-[24px] h-6 px-2 text-xs font-bold bg-brand-500 text-white rounded-full">
              {unreadCount}
            </span>
          )}
        </div>
        {unreadCount > 0 && (
          <button
            onClick={markAllRead}
            className="flex items-center gap-1.5 text-sm text-brand-500 hover:text-brand-600 font-medium transition-colors"
          >
            <CheckCheck size={15} />
            Mark all as read
          </button>
        )}
      </div>

      {/* Filter tabs */}
      <div className="flex gap-1 mb-4 bg-gray-100 dark:bg-gray-800 p-1 rounded-lg w-fit">
        {(["all", "unread"] as const).map((tab) => (
          <button
            key={tab}
            onClick={() => setFilter(tab)}
            className={`px-4 py-1.5 text-sm font-medium rounded-md transition-colors capitalize ${
              filter === tab
                ? "bg-white dark:bg-gray-700 text-gray-900 dark:text-white shadow-sm"
                : "text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200"
            }`}
          >
            {tab}
            {tab === "unread" && unreadCount > 0 && (
              <span className="ml-1.5 text-[10px] font-bold text-brand-500">{unreadCount}</span>
            )}
          </button>
        ))}
      </div>

      {/* List */}
      <div className="relative bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 overflow-hidden">
        {loading && displayed.length === 0 && (
          <div className="py-16 flex items-center justify-center gap-2 text-sm text-gray-400">
            <Loader size={16} className="animate-spin" /> Loading…
          </div>
        )}

        {!loading && displayed.length === 0 && (
          <div className="py-20 flex flex-col items-center gap-3 text-gray-400">
            <Bell size={32} className="opacity-30" />
            <p className="text-sm font-medium">
              {filter === "unread" ? "No unread notifications" : "No notifications yet"}
            </p>
            <p className="text-xs">
              {filter === "unread"
                ? "You're all caught up."
                : "Notifications will appear here when subscription events occur."}
            </p>
          </div>
        )}

        {displayed.map((n) => {
          const isSelected = n.id === selectedId
          return (
            <div key={n.id} className="relative">
              <NotificationCard
                n={n}
                isSelected={isSelected}
                onSelect={setSelectedId}
                onMarkRead={markRead}
                cardRef={isSelected ? selectedRef : undefined}
              />
            </div>
          )
        })}
      </div>

      {notifications.length > 0 && (
        <p className="text-xs text-center text-gray-400 mt-4">
          Showing {displayed.length} of {notifications.length} notification{notifications.length !== 1 ? "s" : ""}
        </p>
      )}
    </div>
  )
}

export default function NotificationsPage() {
  return (
    <Suspense fallback={
      <div className="flex items-center justify-center py-16">
        <Loader size={20} className="animate-spin text-gray-400" />
      </div>
    }>
      <NotificationsContent />
    </Suspense>
  )
}
