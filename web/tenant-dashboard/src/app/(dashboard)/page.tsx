"use client"

import { MessageSquare, TrendingUp, Package, Loader } from "lucide-react"
import StatCard from "@/components/dashboard/StatCard"
import StatusBadge from "@/components/common/StatusBadge"
import { formatDate } from "@/lib/utils"
import { useStats } from "@/lib/hooks/useStats"
import { useInquiries } from "@/lib/hooks/useInquiries"
import type { StatCard as StatCardType } from "@/types"

export default function DashboardPage() {
  const { stats, loading: statsLoading, error: statsError } = useStats()
  const { inquiries, loading: inquiriesLoading } = useInquiries()

  const loading = statsLoading || inquiriesLoading

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

  if (statsError) {
    return (
      <div className="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-6">
        <h3 className="text-red-800 dark:text-red-200 font-semibold mb-2">Error Loading Dashboard</h3>
        <p className="text-red-700 dark:text-red-300">{statsError}</p>
      </div>
    )
  }

  const STAT_CARDS: StatCardType[] = stats ? [
    { label: "Total Inquiries",    value: stats.totalInquiries?.toString() || "0",  change: "0%", positive: true, icon: MessageSquare },
    { label: "Conversion Rate",    value: stats.conversionRate ? `${Math.round(stats.conversionRate * 100)}%` : "0%",  change: "0%", positive: true, icon: TrendingUp },
    { label: "Inquiries This Month", value: stats.statusBreakdown?.["New"]?.toString() || "0",  change: "0%", positive: true, icon: Package },
  ] : []

  const recentInquiries = inquiries.slice(0, 5)

  return (
    <div className="space-y-6">
      {/* Stat cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-4">
        {STAT_CARDS.map((stat, i) => (
          <StatCard key={stat.label} {...stat} delay={i * 0.07} />
        ))}
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
                  <p className="text-xs text-muted-foreground truncate max-w-48">{inq.customerEmail}</p>
                </div>
                <StatusBadge status={inq.status} />
              </div>
            ))
          ) : (
            <p className="text-xs text-muted-foreground py-4">No inquiries yet</p>
          )}
        </div>
      </div>

      {/* Quick links */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <a href="/packages" className="bg-card border border-border rounded-xl p-5 hover:border-[--color-brand-blue] transition-colors">
          <h3 className="text-base font-semibold text-foreground mb-2">Manage Packages</h3>
          <p className="text-xs text-muted-foreground">Sync, customize, and manage your packages</p>
        </a>
        <a href="/settings" className="bg-card border border-border rounded-xl p-5 hover:border-[--color-brand-blue] transition-colors">
          <h3 className="text-base font-semibold text-foreground mb-2">Account Settings</h3>
          <p className="text-xs text-muted-foreground">View and manage your account information</p>
        </a>
      </div>
    </div>
  )
}
