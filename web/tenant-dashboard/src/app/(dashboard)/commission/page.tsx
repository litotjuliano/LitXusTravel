"use client"

import { useState, useEffect } from "react"
import { Plus, Loader } from "lucide-react"
import { tenantApi } from "@/lib/api"
import { formatCurrency } from "@/lib/utils"
import { toast } from "sonner"

type CommissionRule = {
  id: string; agentId?: string; ruleType: string; trigger: string
  amount: number; isPercentage: boolean; isActive: boolean; minimumThreshold: number
}
type StatementLine = {
  id: string; sourceId: string; commissionAmount: number; status: string; accruedAt: string; description: string
}
type Statement = {
  agentId: string; agentName: string; totalAccrued: number; totalFinalized: number
  totalPaid: number; totalReversed: number; lineItems: StatementLine[]
}

export default function CommissionPage() {
  const [rules, setRules] = useState<CommissionRule[]>([])
  const [statement, setStatement] = useState<Statement | null>(null)
  const [loading, setLoading] = useState(true)
  const [agentId, setAgentId] = useState("")
  const [showForm, setShowForm] = useState(false)
  const [submitting, setSubmitting] = useState(false)
  const [form, setForm] = useState({ trigger: "TourCompleted", amount: 10, isPercentage: true, minimumThreshold: 100 })
  const tenantId = typeof window !== "undefined" ? localStorage.getItem("litxus_tenant_id") ?? "" : ""

  useEffect(() => {
    if (!tenantId) return
    tenantApi.getCommissionRules(tenantId)
      .then(r => setRules(r.data ?? []))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [tenantId])

  const loadStatement = async () => {
    if (!tenantId || !agentId) return
    try {
      const r = await tenantApi.getCommissionStatement(tenantId, agentId)
      setStatement(r.data)
    } catch (err) { toast.error(err instanceof Error ? err.message : "Failed") }
  }

  const handleCreateRule = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!tenantId) return
    setSubmitting(true)
    try {
      await tenantApi.configureCommissionRule(tenantId, form)
      toast.success("Commission rule created")
      setShowForm(false)
      const r = await tenantApi.getCommissionRules(tenantId)
      setRules(r.data ?? [])
    } catch (err) { toast.error(err instanceof Error ? err.message : "Failed") }
    finally { setSubmitting(false) }
  }

  const inputCls = "w-full px-3 py-2 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-950 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-(--color-brand-blue)"

  return (
    <div className="space-y-6">
      <h2 className="text-lg font-semibold text-gray-900 dark:text-white">Commission</h2>

      {/* Rules section */}
      <div className="space-y-3">
        <div className="flex items-center justify-between">
          <p className="text-sm font-medium text-gray-700 dark:text-gray-300">Commission Rules</p>
          <button onClick={() => setShowForm(!showForm)} className="flex items-center gap-2 px-3 py-1.5 bg-(--color-brand-blue) hover:bg-blue-700 text-white text-xs font-semibold rounded-lg transition-colors">
            <Plus size={13} /> Add Rule
          </button>
        </div>

        {showForm && (
          <form onSubmit={handleCreateRule} className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-5 space-y-4">
            <h3 className="text-sm font-semibold text-gray-900 dark:text-white">New Commission Rule</h3>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-xs text-gray-500 mb-1">Trigger</label>
                <select className={inputCls} value={form.trigger} onChange={e => setForm(f => ({...f, trigger: e.target.value}))}>
                  <option value="TourBooked">Tour Booked</option>
                  <option value="TourCompleted">Tour Completed</option>
                  <option value="RevenueGenerated">Revenue Generated</option>
                </select>
              </div>
              <div>
                <label className="block text-xs text-gray-500 mb-1">Amount</label>
                <input type="number" min={0} className={inputCls} value={form.amount} onChange={e => setForm(f => ({...f, amount: Number(e.target.value)}))} required />
              </div>
              <div className="flex items-center gap-2">
                <input type="checkbox" id="isPerc" checked={form.isPercentage} onChange={e => setForm(f => ({...f, isPercentage: e.target.checked}))} className="rounded" />
                <label htmlFor="isPerc" className="text-sm text-gray-700 dark:text-gray-300">Is percentage</label>
              </div>
              <div>
                <label className="block text-xs text-gray-500 mb-1">Min. Booking Value</label>
                <input type="number" min={0} className={inputCls} value={form.minimumThreshold} onChange={e => setForm(f => ({...f, minimumThreshold: Number(e.target.value)}))} />
              </div>
            </div>
            <div className="flex gap-2 justify-end">
              <button type="button" onClick={() => setShowForm(false)} className="px-4 py-2 text-sm text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg">Cancel</button>
              <button type="submit" disabled={submitting} className="px-4 py-2 text-sm font-semibold bg-(--color-brand-blue) text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">{submitting ? "Creating..." : "Create"}</button>
            </div>
          </form>
        )}

        <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl overflow-hidden">
          {loading ? (
            <div className="flex items-center justify-center py-10"><Loader className="animate-spin text-gray-400 mr-2" size={16} /></div>
          ) : (
            <table className="w-full text-sm">
              <thead className="bg-gray-50 dark:bg-gray-800/50 border-b border-gray-200 dark:border-gray-800">
                <tr>{["Type", "Trigger", "Rate", "Min. Value", "Status"].map(h => <th key={h} className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">{h}</th>)}</tr>
              </thead>
              <tbody className="divide-y divide-gray-200 dark:divide-gray-800">
                {rules.length === 0 ? (
                  <tr><td colSpan={5} className="px-4 py-8 text-center text-sm text-gray-500">No commission rules configured</td></tr>
                ) : rules.map(r => (
                  <tr key={r.id} className="hover:bg-gray-50 dark:hover:bg-gray-800/30 transition-colors">
                    <td className="px-4 py-3 text-gray-900 dark:text-white">{r.ruleType}</td>
                    <td className="px-4 py-3 text-gray-700 dark:text-gray-300">{r.trigger}</td>
                    <td className="px-4 py-3 font-semibold text-gray-900 dark:text-white">{r.isPercentage ? `${r.amount}%` : formatCurrency(r.amount)}</td>
                    <td className="px-4 py-3 text-gray-500">{formatCurrency(r.minimumThreshold)}</td>
                    <td className="px-4 py-3">
                      <span className={`inline-flex px-2 py-0.5 rounded text-xs font-medium ${r.isActive ? "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400" : "bg-gray-100 text-gray-600"}`}>
                        {r.isActive ? "Active" : "Inactive"}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>

      {/* Statement lookup */}
      <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-5 space-y-4">
        <p className="text-sm font-medium text-gray-700 dark:text-gray-300">Agent Commission Statement</p>
        <div className="flex gap-3">
          <input className={`flex-1 ${inputCls}`} placeholder="Agent ID (UUID)" value={agentId} onChange={e => setAgentId(e.target.value)} />
          <button onClick={loadStatement} disabled={!agentId} className="px-4 py-2 text-sm font-semibold bg-(--color-brand-blue) text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 transition-colors">Load</button>
        </div>
        {statement && (
          <div className="space-y-4">
            <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
              {[
                { label: "Accrued", value: formatCurrency(statement.totalAccrued) },
                { label: "Finalized", value: formatCurrency(statement.totalFinalized) },
                { label: "Paid", value: formatCurrency(statement.totalPaid) },
                { label: "Reversed", value: formatCurrency(statement.totalReversed) },
              ].map(s => (
                <div key={s.label} className="bg-gray-50 dark:bg-gray-800 rounded-lg p-3">
                  <p className="text-xs text-gray-500">{s.label}</p>
                  <p className="text-base font-bold text-gray-900 dark:text-white">{s.value}</p>
                </div>
              ))}
            </div>
            <div className="overflow-x-auto">
              <table className="w-full text-xs">
                <thead><tr className="border-b border-gray-200 dark:border-gray-800">{["Description", "Amount", "Status"].map(h => <th key={h} className="text-left py-2 px-3 text-gray-500">{h}</th>)}</tr></thead>
                <tbody>
                  {statement.lineItems.map(l => (
                    <tr key={l.id} className="border-b border-gray-100 dark:border-gray-800/50">
                      <td className="py-2 px-3 text-gray-700 dark:text-gray-300">{l.description}</td>
                      <td className="py-2 px-3 font-medium text-gray-900 dark:text-white">{formatCurrency(l.commissionAmount)}</td>
                      <td className="py-2 px-3 text-gray-500">{l.status}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
    </div>
  )
}
