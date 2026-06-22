"use client"

import { Users, Package, TrendingUp, CreditCard, Loader } from "lucide-react"
import StatCard from "@/components/dashboard/StatCard"
import RevenueChart from "@/components/dashboard/RevenueChart"
import StatusBadge from "@/components/common/StatusBadge"
import { formatDate, getTokenClaims } from "@/lib/utils"
import { useDashboard } from "@/lib/hooks/useDashboard"
import type { StatCard as StatCardType } from "@/types"

export default function DashboardPage() {
  const { stats, recentTenants, recentInquiries, loading, error } = useDashboard()
  const { tenantId } = getTokenClaims()
  const isTenantAdmin = !!tenantId

  if (loading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="text-center">
          <Loader className="animate-spin mx-auto mb-4" size={32} />
          <p className="text-gray-500 dark:text-gray-400">Loading dashboard...</p>
        </div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-6">
        <h3 className="text-red-800 dark:text-red-200 font-semibold mb-2">Error Loading Dashboard</h3>
        <p className="text-red-700 dark:text-red-300">{error}</p>
      </div>
    )
  }

  const STAT_CARDS: StatCardType[] = stats ? [
    {
      label: isTenantAdmin ? "My Tenant" : "Active Tenants",
      value: stats.activeTenants.value,
      change: stats.activeTenants.change,
      positive: stats.activeTenants.positive,
      icon: Users,
    },
    { label: "Total Packages",       value: stats.totalPackages.value,       change: stats.totalPackages.change,       positive: stats.totalPackages.positive,       icon: Package },
    { label: "Monthly Revenue",      value: stats.monthlyRevenue.value,      change: stats.monthlyRevenue.change,      positive: stats.monthlyRevenue.positive,      icon: TrendingUp },
    { label: "Active Subscriptions", value: stats.activeSubscriptions.value, change: stats.activeSubscriptions.change, positive: stats.activeSubscriptions.positive, icon: CreditCard },
  ] : []

  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4">
        {STAT_CARDS.map((stat, i) => (
          <StatCard key={stat.label} {...stat} delay={i * 0.07} />
        ))}
      </div>

      <RevenueChart />

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-5">
          <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-4">
            {isTenantAdmin ? "My Tenant" : "Recent Tenants"}
          </h2>
          <div className="space-y-3">
            {recentTenants.length > 0 ? (
              recentTenants.map((t) => (
                <div key={t.id} className="flex items-center justify-between py-2 border-b border-gray-200 dark:border-gray-800 last:border-0">
                  <div>
                    <p className="text-sm font-medium text-gray-900 dark:text-white">{t.name}</p>
                    <p className="text-xs text-gray-500 dark:text-gray-400">{t.contactEmail}</p>
                  </div>
                  <div className="text-right">
                    <StatusBadge status={t.isActive ? "Active" : "Suspended"} />
                    <p className="text-xs text-gray-500 dark:text-gray-400 mt-1">{formatDate(t.createdAt)}</p>
                  </div>
                </div>
              ))
            ) : (
              <p className="text-xs text-gray-500 dark:text-gray-400 py-4">No tenants yet</p>
            )}
          </div>
        </div>

        <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-5">
          <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-4">Recent Inquiries</h2>
          <div className="space-y-3">
            {recentInquiries.length > 0 ? (
              recentInquiries.map((inq) => (
                <div key={inq.id} className="flex items-center justify-between py-2 border-b border-gray-200 dark:border-gray-800 last:border-0">
                  <div>
                    <p className="text-sm font-medium text-gray-900 dark:text-white">{inq.customerName}</p>
                    <p className="text-xs text-gray-500 dark:text-gray-400 truncate max-w-48">{inq.packageTitle || "N/A"}</p>
                  </div>
                  <StatusBadge status={inq.status} />
                </div>
              ))
            ) : (
              <p className="text-xs text-gray-500 dark:text-gray-400 py-4">No inquiries yet</p>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
