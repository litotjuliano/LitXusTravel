"use client"

import { AlertTriangle, XCircle, Clock } from "lucide-react"
import type { SubscriptionStatus } from "@/lib/hooks/useSubscription"

interface Props {
  subscription: SubscriptionStatus
}

export function SubscriptionBanner({ subscription }: Props) {
  const { subscriptionHealth, daysRemaining, planName, gracePeriodDaysRemaining } = subscription

  if (subscriptionHealth === "GracePeriod") {
    const days = gracePeriodDaysRemaining ?? 0
    return (
      <div className="flex items-start gap-3 bg-orange-50 dark:bg-orange-900/20 border border-orange-300 dark:border-orange-700 rounded-xl px-4 py-3">
        <Clock size={18} className="text-orange-600 dark:text-orange-400 mt-0.5 shrink-0" />
        <div>
          <p className="text-sm font-semibold text-orange-800 dark:text-orange-200">
            Grace period — {days} day{days !== 1 ? "s" : ""} left before read-only lockout
          </p>
          <p className="text-xs text-orange-700 dark:text-orange-300 mt-0.5">
            Your {planName} subscription has expired but all features remain available.
            Contact your administrator to renew now.
          </p>
        </div>
      </div>
    )
  }

  if (subscriptionHealth === "Expired") {
    return (
      <div className="flex items-start gap-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-xl px-4 py-3">
        <XCircle size={18} className="text-red-600 dark:text-red-400 mt-0.5 shrink-0" />
        <div>
          <p className="text-sm font-semibold text-red-800 dark:text-red-200">
            Your {planName} subscription has expired — read-only mode active
          </p>
          <p className="text-xs text-red-700 dark:text-red-300 mt-0.5">
            You can view existing data but creating, editing, and deleting records is disabled.
            Contact your administrator to renew.
          </p>
        </div>
      </div>
    )
  }

  if (subscriptionHealth === "ExpiringSoon" && daysRemaining !== null) {
    return (
      <div className="flex items-start gap-3 bg-amber-50 dark:bg-amber-900/20 border border-amber-200 dark:border-amber-800 rounded-xl px-4 py-3">
        <AlertTriangle size={18} className="text-amber-600 dark:text-amber-400 mt-0.5 shrink-0" />
        <div>
          <p className="text-sm font-semibold text-amber-800 dark:text-amber-200">
            Your {planName} plan expires in {daysRemaining} day{daysRemaining !== 1 ? "s" : ""}
          </p>
          <p className="text-xs text-amber-700 dark:text-amber-300 mt-0.5">
            Contact your administrator to renew before access is interrupted.
          </p>
        </div>
      </div>
    )
  }

  return null
}
