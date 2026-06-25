export const STATUS = {
  // Activity
  ACTIVE:        "Active",
  INACTIVE:      "Inactive",
  SUSPENDED:     "Suspended",
  // Subscription health (from backend TenantSubscription.SubscriptionHealth)
  TRIAL:         "Trial",
  EXPIRING_SOON: "ExpiringSoon",
  GRACE_PERIOD:  "GracePeriod",
  EXPIRED:       "Expired",
  // Provisioning
  PENDING:       "Pending",
  COMPLETED:     "Completed",
  FAILED:        "Failed",
  // Invoice
  PAID:          "Paid",
  // Packages
  PUBLISHED:     "Published",
  DRAFT:         "Draft",
  ARCHIVED:      "Archived",
  SYNCED:        "Synced",
  OWNED:         "Owned",
  CUSTOMIZED:    "Customized",
  // Inquiry
  NEW:           "New",
  CONTACTED:     "Contacted",
  QUOTED:        "Quoted",
  BOOKED:        "Booked",
  LOST:          "Lost",
} as const

export type StatusValue = typeof STATUS[keyof typeof STATUS]

export function getStatusColor(status: string): string {
  const map: Record<string, string> = {
    [STATUS.PUBLISHED]:     "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400",
    [STATUS.ACTIVE]:        "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400",
    [STATUS.COMPLETED]:     "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400",
    [STATUS.BOOKED]:        "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400",
    [STATUS.PAID]:          "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400",
    [STATUS.OWNED]:         "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400",
    [STATUS.DRAFT]:         "bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400",
    [STATUS.TRIAL]:         "bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400",
    [STATUS.PENDING]:       "bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400",
    [STATUS.NEW]:           "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400",
    [STATUS.SYNCED]:        "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400",
    [STATUS.CONTACTED]:     "bg-purple-100 text-purple-700 dark:bg-purple-900/30 dark:text-purple-400",
    [STATUS.CUSTOMIZED]:    "bg-purple-100 text-purple-700 dark:bg-purple-900/30 dark:text-purple-400",
    [STATUS.QUOTED]:        "bg-orange-100 text-orange-700 dark:bg-orange-900/30 dark:text-orange-400",
    [STATUS.GRACE_PERIOD]:  "bg-orange-100 text-orange-700 dark:bg-orange-900/30 dark:text-orange-400",
    [STATUS.EXPIRING_SOON]: "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-400",
    [STATUS.ARCHIVED]:      "bg-gray-100 text-gray-600 dark:bg-gray-800 dark:text-gray-400",
    [STATUS.INACTIVE]:      "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400",
    [STATUS.SUSPENDED]:     "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400",
    [STATUS.FAILED]:        "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400",
    [STATUS.LOST]:          "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400",
    [STATUS.EXPIRED]:       "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400",
  }
  return map[status] ?? "bg-gray-100 text-gray-600"
}

export function activityStatus(isActive: boolean): "Active" | "Suspended" {
  return isActive ? STATUS.ACTIVE : STATUS.SUSPENDED
}

export function healthLabel(health: string, daysRemaining: number | null | undefined): string | undefined {
  if (health === STATUS.EXPIRING_SOON)
    return daysRemaining ? `Expiring in ${daysRemaining}d` : "Expiring Soon"
  if (health === STATUS.GRACE_PERIOD) return "Grace Period"
  return undefined
}
