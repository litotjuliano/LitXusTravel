import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function formatCurrency(amount: number, currency = "RM") {
  return `${currency} ${amount.toLocaleString()}`
}

export function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString("en-MY", {
    day: "numeric", month: "short", year: "numeric",
  })
}

export function getStatusColor(status: string): string {
  const map: Record<string, string> = {
    Published: "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400",
    Active:    "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400",
    Completed: "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400",
    Booked:    "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400",
    Draft:     "bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400",
    Trial:     "bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400",
    New:       "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400",
    Contacted: "bg-purple-100 text-purple-700 dark:bg-purple-900/30 dark:text-purple-400",
    Quoted:    "bg-orange-100 text-orange-700 dark:bg-orange-900/30 dark:text-orange-400",
    Archived:  "bg-gray-100 text-gray-600 dark:bg-gray-800 dark:text-gray-400",
    Suspended: "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400",
    Lost:      "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400",
    Pending:   "bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400",
  }
  return map[status] ?? "bg-gray-100 text-gray-600"
}
