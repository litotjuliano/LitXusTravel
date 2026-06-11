"use client"

import { Users, Package, TrendingUp, CreditCard, Loader } from "lucide-react"
import StatCard from "@/components/dashboard/StatCard"
import RevenueChart from "@/components/dashboard/RevenueChart"
import StatusBadge from "@/components/common/StatusBadge"
import { formatDate } from "@/lib/utils"
import { useDashboard } from "@/lib/hooks/useDashboard"
import type { StatCard as StatCardType } from "@/types"

export default function DashboardPage() {
  const { stats, recentTenants, recentInquiries, loading, error } = useDashboard()

  if (loading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="text-center">
          <Loader className="animate-spin mx-auto mb-4" size={32} />
          <p className="text-muted-foreground">Loading dashboard...</p>
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
    { label: "Active Tenants",        value: stats.activeTenants.value,       change: stats.activeTenants.change,       positive: stats.activeTenants.positive, icon: Users },
    { label: "Total Packages",        value: stats.totalPackages.value,        change: stats.totalPackages.change,        positive: stats.totalPackages.positive, icon: Package },
    { label: "Monthly Revenue",       value: stats.monthlyRevenue.value,       change: stats.monthlyRevenue.change,       positive: stats.monthlyRevenue.positive, icon: TrendingUp },
    { label: "Active Subscriptions",  value: stats.activeSubscriptions.value,  change: stats.activeSubscriptions.change,  positive: stats.activeSubscriptions.positive, icon: CreditCard },
  ] : []

  return (
    <div className="space-y-6">
      {/* Stat cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4">
        {STAT_CARDS.map((stat, i) => (
          <StatCard key={stat.label} {...stat} delay={i * 0.07} />
        ))}
      </div>

      {/* Chart */}
      <RevenueChart />

      {/* Recent rows */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Recent tenants */}
        <div className="bg-card border border-border rounded-xl p-5">
          <h2 className="text-base font-semibold text-foreground mb-4">Recent Tenants</h2>
          <div className="space-y-3">
            {recentTenants.length > 0 ? (
              recentTenants.map((t) => (
                <div key={t.id} className="flex items-center justify-between py-2 border-b border-border last:border-0">
                  <div>
                    <p className="text-sm font-medium text-foreground">{t.name}</p>
                    <p className="text-xs text-muted-foreground">{t.contactEmail}</p>
                  </div>
                  <div className="text-right">
                    <StatusBadge status={t.isActive ? "Active" : "Suspended"} />
                    <p className="text-xs text-muted-foreground mt-1">{formatDate(t.createdAt)}</p>
                  </div>
                </div>
              ))
            ) : (
              <p className="text-xs text-muted-foreground py-4">No tenants yet</p>
            )}
          </div>
        </div>

        {/* Recent inquiries */}
        <div className="bg-card border border-border rounded-xl p-5">
          <h2 className="text-base font-semibold text-foreground mb-4">Recent Inquiries</h2>
          <div className="space-y-3">
            {recentInquiries.length > 0 ? (
              recentInquiries.map((inq) => (
                <div key={inq.id} className="flex items-center justify-between py-2 border-b border-border last:border-0">
                  <div>
                    <p className="text-sm font-medium text-foreground">{inq.customerName}</p>
                    <p className="text-xs text-muted-foreground truncate max-w-48">{inq.packageTitle || "N/A"}</p>
                  </div>
                  <StatusBadge status={inq.status} />
                </div>
              ))
            ) : (
              <p className="text-xs text-muted-foreground py-4">No inquiries yet</p>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
