"use client"

import { useState, useEffect } from "react"
import { Download, CreditCard, Loader } from "lucide-react"
import StatusBadge from "@/components/common/StatusBadge"
import { formatDate, getTokenClaims } from "@/lib/utils"
import { adminApi, InvoiceDto } from "@/lib/api"

type FilterTab = "All" | "Paid" | "Pending" | "Failed"

export default function BillingPage() {
  const [tenantId, setTenantId] = useState<string | undefined>(undefined)
  const [ready, setReady] = useState(false)
  const [filter, setFilter] = useState<FilterTab>("All")
  const [invoices, setInvoices] = useState<InvoiceDto[]>([])
  const [mrr, setMrr] = useState(0)
  const [totalPaid, setTotalPaid] = useState(0)
  const [totalPending, setTotalPending] = useState(0)
  const [totalFailed, setTotalFailed] = useState(0)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const { tenantId: tid } = getTokenClaims()
    setTenantId(tid ?? undefined)
    setReady(true)
  }, [])

  useEffect(() => {
    if (!ready) return
    setLoading(true)
    adminApi.getInvoices(tenantId)
      .then((res) => {
        setInvoices(res.data.data)
        setMrr(res.data.mrrCurrentMonth)
        setTotalPaid(res.data.totalPaid)
        setTotalPending(res.data.totalPending)
        setTotalFailed(res.data.totalFailed)
      })
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [ready, tenantId])

  const filtered = filter === "All" ? invoices : invoices.filter((i) => i.status === filter)

  const currentPeriod = new Date().toLocaleString("en-US", { month: "short", year: "numeric" })

  function handleDownload(inv: InvoiceDto) {
    const html = `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8" />
  <title>Invoice ${inv.invoiceNumber}</title>
  <style>
    body { font-family: Arial, sans-serif; color: #111; max-width: 640px; margin: 60px auto; padding: 0 24px; }
    .header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 40px; }
    .brand { font-size: 22px; font-weight: 700; color: #1d4ed8; }
    .badge { display: inline-block; padding: 4px 12px; border-radius: 20px; font-size: 13px; font-weight: 600;
      background: ${inv.status === "Paid" ? "#dcfce7" : inv.status === "Failed" ? "#fee2e2" : "#fef9c3"};
      color: ${inv.status === "Paid" ? "#166534" : inv.status === "Failed" ? "#991b1b" : "#854d0e"}; }
    h2 { font-size: 15px; font-weight: 600; color: #374151; margin: 0 0 4px; }
    .meta { color: #6b7280; font-size: 13px; margin-bottom: 32px; }
    table { width: 100%; border-collapse: collapse; margin-bottom: 32px; }
    th { text-align: left; font-size: 11px; text-transform: uppercase; letter-spacing: .05em;
      color: #6b7280; border-bottom: 2px solid #e5e7eb; padding: 8px 0; }
    td { padding: 12px 0; border-bottom: 1px solid #f3f4f6; font-size: 14px; }
    .total-row td { font-weight: 700; font-size: 16px; border-bottom: none; padding-top: 20px; }
    .footer { font-size: 12px; color: #9ca3af; margin-top: 40px; border-top: 1px solid #e5e7eb; padding-top: 20px; }
    @media print { body { margin: 20px auto; } }
  </style>
</head>
<body>
  <div class="header">
    <div>
      <div class="brand">LitXusTravel</div>
      <div style="font-size:12px;color:#6b7280;margin-top:4px">travel.litxus.com</div>
    </div>
    <span class="badge">${inv.status}</span>
  </div>

  <h2>Invoice ${inv.invoiceNumber}</h2>
  <div class="meta">
    Issued: ${formatDate(inv.date)} &nbsp;·&nbsp; Period: ${inv.period}
  </div>

  <table>
    <thead>
      <tr><th>Description</th><th>Plan</th><th style="text-align:right">Amount</th></tr>
    </thead>
    <tbody>
      <tr>
        <td>${inv.tenantName} — Subscription</td>
        <td>${inv.planName}</td>
        <td style="text-align:right">RM ${inv.amount}</td>
      </tr>
      <tr class="total-row">
        <td colspan="2">Total</td>
        <td style="text-align:right">RM ${inv.amount}</td>
      </tr>
    </tbody>
  </table>

  <div class="footer">
    Thank you for using LitXusTravel. For billing enquiries contact support@litxustravel.com
  </div>

  <script>window.onload = () => window.print()</script>
</body>
</html>`

    const blob = new Blob([html], { type: "text/html" })
    const url = URL.createObjectURL(blob)
    window.open(url, "_blank")
    setTimeout(() => URL.revokeObjectURL(url), 10000)
  }

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
            { label: `MRR (${currentPeriod})`, value: `RM ${mrr.toLocaleString()}`, icon: <CreditCard size={18} /> },
            { label: "Paid Invoices",           value: totalPaid.toString(),                              icon: <CreditCard size={18} /> },
            { label: "Pending / Failed",         value: (totalPending + totalFailed).toString(),           icon: <CreditCard size={18} /> },
          ].map((card) => (
            <div key={card.label} className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-5 flex items-center gap-4">
              <div className="w-10 h-10 rounded-lg bg-(--color-brand-blue)/10 flex items-center justify-center text-(--color-brand-blue)">
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
                      ? "bg-(--color-brand-blue) text-white"
                      : "bg-gray-100 dark:bg-gray-800 text-gray-500 dark:text-gray-400"
                  }`}
                >
                  {s}
                </button>
              ))}
            </div>
          )}
        </div>

        {loading ? (
          <div className="flex items-center justify-center py-12">
            <Loader className="animate-spin text-gray-400 mr-2" size={18} />
          </div>
        ) : filtered.length === 0 ? (
          <div className="py-12 text-center text-sm text-gray-400">No invoices found.</div>
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
                {filtered.map((inv) => (
                  <tr key={inv.id} className="hover:bg-gray-50 dark:hover:bg-gray-800/30 transition-colors">
                    <td className="px-4 py-3 font-mono text-xs text-gray-700 dark:text-gray-300">{inv.invoiceNumber}</td>
                    {!tenantId && (
                      <td className="px-4 py-3 font-medium text-gray-900 dark:text-white">{inv.tenantName}</td>
                    )}
                    <td className="px-4 py-3 text-gray-700 dark:text-gray-300">{inv.planName}</td>
                    <td className="px-4 py-3 text-gray-500 dark:text-gray-400">{inv.period}</td>
                    <td className="px-4 py-3 font-semibold text-gray-900 dark:text-white">RM {inv.amount}</td>
                    <td className="px-4 py-3">
                      <StatusBadge status={inv.status} />
                    </td>
                    <td className="px-4 py-3 text-gray-500 dark:text-gray-400">{formatDate(inv.date)}</td>
                    <td className="px-4 py-3">
                      <button
                        title="Download receipt"
                        onClick={() => handleDownload(inv)}
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
