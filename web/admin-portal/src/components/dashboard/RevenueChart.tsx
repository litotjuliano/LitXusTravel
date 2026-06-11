"use client"

import { useState } from "react"
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from "recharts"
import { REVENUE_CHART } from "@/lib/mock-data"

const PERIODS = ["7d", "30d", "90d", "1y"]

export default function RevenueChart() {
  const [period, setPeriod] = useState("30d")

  return (
    <div className="bg-card border border-border rounded-xl p-6">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h2 className="text-base font-semibold text-foreground">Revenue Overview</h2>
          <p className="text-sm text-muted-foreground">Monthly subscription revenue</p>
        </div>
        <div className="flex gap-1">
          {PERIODS.map((p) => (
            <button
              key={p}
              onClick={() => setPeriod(p)}
              className={`px-2.5 py-1 rounded text-xs font-semibold transition-colors ${
                period === p
                  ? "bg-[--color-brand-blue] text-white"
                  : "bg-muted text-muted-foreground hover:bg-muted/80"
              }`}
            >
              {p}
            </button>
          ))}
        </div>
      </div>

      <ResponsiveContainer width="100%" height={240}>
        <LineChart data={REVENUE_CHART} margin={{ top: 5, right: 5, left: 5, bottom: 5 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" />
          <XAxis dataKey="month" tick={{ fontSize: 12, fill: "var(--muted-foreground)" }} axisLine={false} tickLine={false} />
          <YAxis tick={{ fontSize: 12, fill: "var(--muted-foreground)" }} axisLine={false} tickLine={false} tickFormatter={(v) => `RM ${(v/1000).toFixed(0)}k`} />
          <Tooltip
            contentStyle={{ backgroundColor: "var(--card)", border: "1px solid var(--border)", borderRadius: "8px", fontSize: "12px" }}
            formatter={(v) => [`RM ${Number(v ?? 0).toLocaleString()}`, "Revenue"]}
          />
          <Line type="monotone" dataKey="revenue" stroke="#0066CC" strokeWidth={2.5} dot={false} />
        </LineChart>
      </ResponsiveContainer>
    </div>
  )
}
