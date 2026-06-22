"use client"

import { useState } from "react"
import { Download, CreditCard, Loader } from "lucide-react"
import StatusBadge from "@/components/common/StatusBadge"
import { formatDate, getTokenClaims } from "@/lib/utils"

type Invoice = {
  id: string
  tenant: string
  plan: string
  amount: number
  period: string
  status: "Paid" | "Pending" | "Failed"
  date: string
}

const MOCK_INVOICES: Invoice[] = [
  { id: "INV-2026-042", tenant: "TravelPro Agency",      plan: "Pro",        amount: 299, period: "Jun 2026", status: "Paid",    date: "2026-06-01" },
  { id: "INV-2026-041", tenant: "Wanderlust Tours",      plan: "Starter",    amount: 99,  period: "Jun 2026", status: "Paid",    date: "2026-06-01" },
  { id: "INV-2026-040", tenant: "AdventureSeek",         plan: "Enterprise", amount: 999, period: "Jun 2026", status: "Pending", date: "2026-06-01" },
  { id: "INV-2026-039", tenant: "TravelPro Agency",      plan: "Pro",        amount: 299, period: "May 2026", status: "Paid",    date: "2026-05-01" },
  { id: "INV-2026-038", tenant: "Wanderlust Tours",      plan: "Starter",    amount: 99,  period: "May 2026", status: "Paid",    date: "2026-05-01" },
  { id: "INV-2026-037", tenant: "AdventureSeek",         plan: "Enterprise", amount: 999, period: "May 2026", status: "Paid",    date: "2026-05-01" },
  { id: "INV-2026-036", tenant: "TravelPro Agency",      plan: "Pro",        amount: 299, period: "Apr 2026", status: "Paid",    date: "2026-04-01" },
  { id: "INV-2026-035", tenant: "Wanderlust Tours",      plan: "Starter",    amount: 99,  period: "Apr 2026", status: "Failed",  date: "2026-04-01" },
]

const STATUS_MAP = { Paid: "Active", Pending: "Trial", Failed: "Suspended" } as const

export default function BillingPage() {
  const { tenantId } = getTokenClaims()
  const [filter, setFilter] = useState<"All" | "Paid" | "Pending" | "Failed">("All")

  // Tenant view: show only their invoices
  const invoices = MOCK_INVOICES.filter((inv) => {
    if (tenantId) return true // tenant sees their own (mock: all for now)
    return filter === "All" || inv.status === filter
  })

  const mrr = MOCK_INVOICES.filter((i) => i.status === "Paid" && i.period === "Jun 2026")
    .reduce((sum, i) => sum + i.amount, 0)

  return (
    <div className="space-y-5">
      {/* Header */}
      <div>
        <h2 className="text-lg font-semibold text-gray-900 dark:text-white">Billing & Invoices</h2>
        <p className="text-sm text-gray-500 dark:text-gray-400">Platform subscription revenue and invoice history</p>
      </div>

      {/* Summary cards — SuperAdmin only */}
      {!tenantId && (
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
          {[
            { label: "MRR (Jun 2026)", value: `RM ${mrr.toLocaleString()}`, icon: <CreditCard size={18} /> },
            { label: "Paid Invoices",  value: MOCK_INVOICES.filter((i) => i.status === "Paid").length.toString(),    icon: <CreditCard size={18} /> },
            { label: "Pending / Failed", value: MOCK_INVOICES.filter((i) => i.status !== "Paid").length.toString(), icon: <CreditCard size={18} /> },
          ].map((card) => (
            <div key={card.label} className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-5 flex items-center gap-4">
              <div className="w-10 h-10 rounded-lg bg-[--color-brand-blue]/10 flex items-center justify-center text-[--color-brand-blue]">
                {card.icon}
              </div>
              <div>
                <p className="text-xs text-gray-500 dark:text-gray-400">{card.label}</p>
                <p className="text-xl font-bold text-gray-900 dark:text-white">{card.value}</p>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Invoices table */}
      <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl overflow-hidden">
        {/* Table toolbar */}
        <div className="flex items-center gap-3 px-5 py-4 border-b border-gray-200 dark:border-gray-800">
          <h3 className="text-sm font-semibold text-gray-900 dark:text-white flex-1">Invoices</h3>
          {!tenantId && (
            <div className="flex gap-2">
              {(["All", "Paid", "Pending", "Failed"] as const).map((s) => (
                <button
                  key={s}
                  onClick={() => setFilter(s)}
                  className={`px-3 py-1 rounded-lg text-xs font-medium transition-colors ${
                    filter === s
                      ? "bg-[--color-brand-blue] text-white"
                      : "bg-gray-100 dark:bg-gray-800 text-gray-500 dark:text-gray-400"
                  }`}
                >
                  {s}
                </button>
              ))}
            </div>
          )}
        </div>

        {invoices.length === 0 ? (
          <div className="flex items-center justify-center py-12">
            <Loader className="animate-spin text-gray-400 mr-2" size={18} />
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-gray-100 dark:bg-gray-800/50 border-b border-gray-200 dark:border-gray-800">
                <tr>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">Invoice</th>
                  {!tenantId && <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">Tenant</th>}
                  <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">Plan</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">Period</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">Amount</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">Status</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">Date</th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {invoices.map((inv) => (
                  <tr key={inv.id} className="hover:bg-gray-50 dark:hover:bg-gray-800/30 transition-colors">
                    <td className="px-4 py-3 font-mono text-xs text-gray-700 dark:text-gray-300">{inv.id}</td>
                    {!tenantId && (
                      <td className="px-4 py-3 font-medium text-gray-900 dark:text-white">{inv.tenant}</td>
                    )}
                    <td className="px-4 py-3 text-gray-700 dark:text-gray-300">{inv.plan}</td>
                    <td className="px-4 py-3 text-gray-500 dark:text-gray-400">{inv.period}</td>
                    <td className="px-4 py-3 font-semibold text-gray-900 dark:text-white">RM {inv.amount}</td>
                    <td className="px-4 py-3">
                      <StatusBadge status={STATUS_MAP[inv.status]} label={inv.status} />
                    </td>
                    <td className="px-4 py-3 text-gray-500 dark:text-gray-400">{formatDate(inv.date)}</td>
                    <td className="px-4 py-3">
                      <button
                        title="Download PDF"
                        onClick={() => {}}
                        className="p-1.5 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors text-gray-400 hover:text-gray-700 dark:hover:text-gray-200"
                      >
                        <Download size={14} />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  )
}
