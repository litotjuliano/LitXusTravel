"use client"

import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from "recharts"
import { useAnalytics } from "@/lib/hooks/useAnalytics"
import { REVENUE_CHART } from "@/lib/mock-data"
import { Skeleton } from "@/components/ui/skeleton"

const STATUS_COLORS: Record<string, string> = {
  New: "#3B82F6",
  Contacted: "#8B5CF6",
  Quoted: "#F59E0B",
  Booked: "#10B981",
  Lost: "#EF4444",
}

const PACKAGE_POPULARITY = [
  { name: "Japan Sakura",  synced: 12 },
  { name: "Europe Grand",  synced: 15 },
  { name: "Maldives",      synced: 5  },
  { name: "Bali Family",   synced: 8  },
  { name: "South Korea",   synced: 0  },
]

export default function AnalyticsPage() {
  const { data: analyticsData, loading, error } = useAnalytics()

  const inquiryByStatus = analyticsData
    ? Object.entries(analyticsData.statusBreakdown).map(([name, value]) => ({
        name,
        value,
        color: STATUS_COLORS[name as keyof typeof STATUS_COLORS] || "#6B7280",
      }))
    : []
  if (error) {
    return <div className="text-red-600 p-6">Failed to load analytics: {error}</div>
  }

  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Revenue bar chart */}
        <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-6">
          <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-1">Monthly Revenue</h2>
          <p className="text-sm text-gray-500 dark:text-gray-400 mb-5">RM performance over time</p>
          {loading ? (
            <Skeleton className="h-[220px]" />
          ) : (
            <ResponsiveContainer width="100%" height={220}>
              <BarChart data={REVENUE_CHART}>
                <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" />
                <XAxis dataKey="month" tick={{ fontSize: 12, fill: "var(--muted-foreground)" }} axisLine={false} tickLine={false} />
                <YAxis tick={{ fontSize: 12, fill: "var(--muted-foreground)" }} axisLine={false} tickLine={false} tickFormatter={(v) => `${(v/1000).toFixed(0)}k`} />
                <Tooltip
                  contentStyle={{ backgroundColor: "var(--card)", border: "1px solid var(--border)", borderRadius: "8px", fontSize: "12px" }}
                  formatter={(v) => [`RM ${Number(v ?? 0).toLocaleString()}`, "Revenue"]}
                />
                <Bar dataKey="revenue" fill="#0066CC" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          )}
        </div>

        {/* Inquiry status pie */}
        <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-6">
          <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-1">Inquiry Status</h2>
          <p className="text-sm text-gray-500 dark:text-gray-400 mb-5">Breakdown by conversion stage</p>
          {loading ? (
            <Skeleton className="h-[160px]" />
          ) : inquiryByStatus.length > 0 ? (
            <div className="flex items-center gap-6">
              <PieChart width={160} height={160}>
                <Pie data={inquiryByStatus} cx={75} cy={75} innerRadius={45} outerRadius={70} dataKey="value" strokeWidth={0}>
                  {inquiryByStatus.map((entry, i) => <Cell key={i} fill={entry.color} />)}
                </Pie>
              </PieChart>
              <div className="space-y-2">
                {inquiryByStatus.map((s) => (
                  <div key={s.name} className="flex items-center gap-2 text-sm">
                    <span className="w-3 h-3 rounded-full shrink-0" style={{ backgroundColor: s.color }} />
                    <span className="text-gray-500 dark:text-gray-400">{s.name}</span>
                    <span className="font-semibold text-gray-900 dark:text-white ml-auto">{s.value}</span>
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <p className="text-gray-500 dark:text-gray-400">No inquiries yet</p>
          )}
        </div>
      </div>

      {/* Key metrics */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-6">
          <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-1">Total Inquiries</h2>
          <p className="text-sm text-gray-500 dark:text-gray-400 mb-3">Lifetime inquiry count</p>
          {loading ? <Skeleton className="h-8 w-20" /> : <p className="text-3xl font-bold text-gray-900 dark:text-white">{analyticsData?.totalInquiries ?? 0}</p>}
        </div>
        <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-6">
          <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-1">Conversion Rate</h2>
          <p className="text-sm text-gray-500 dark:text-gray-400 mb-3">Inquiries converted to bookings</p>
          {loading ? <Skeleton className="h-8 w-20" /> : <p className="text-3xl font-bold text-green-600">{analyticsData?.conversionRate.toFixed(1) ?? 0}%</p>}
        </div>
      </div>

      {/* Package popularity */}
      <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-6">
        <h2 className="text-base font-semibold text-gray-900 dark:text-white mb-1">Package Sync Popularity</h2>
        <p className="text-sm text-gray-500 dark:text-gray-400 mb-5">Number of tenants who synced each package</p>
        {loading ? (
          <Skeleton className="h-[200px]" />
        ) : (
          <ResponsiveContainer width="100%" height={200}>
            <BarChart data={PACKAGE_POPULARITY} layout="vertical">
              <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" horizontal={false} />
              <XAxis type="number" tick={{ fontSize: 12, fill: "var(--muted-foreground)" }} axisLine={false} tickLine={false} />
              <YAxis dataKey="name" type="category" tick={{ fontSize: 11, fill: "var(--muted-foreground)" }} axisLine={false} tickLine={false} width={100} />
              <Tooltip contentStyle={{ backgroundColor: "var(--card)", border: "1px solid var(--border)", borderRadius: "8px", fontSize: "12px" }} />
              <Bar dataKey="synced" fill="#00A89A" radius={[0, 4, 4, 0]} />
            </BarChart>
          </ResponsiveContainer>
        )}
      </div>
    </div>
  )
}
