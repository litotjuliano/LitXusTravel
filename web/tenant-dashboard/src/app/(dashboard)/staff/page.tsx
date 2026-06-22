"use client"

import { useState, useEffect } from "react"
import { Plus, Loader, RotateCcw } from "lucide-react"
import { tenantApi } from "@/lib/api"
import StatusBadge from "@/components/common/StatusBadge"
import { toast } from "sonner"

type StaffAgent = {
  id: string; tenantId: string; name: string; email: string
  referralCode: string; isActive: boolean; codeExpiresAt: string
}

export default function StaffPage() {
  const [agents, setAgents] = useState<StaffAgent[]>([])
  const [loading, setLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)
  const [submitting, setSubmitting] = useState(false)
  const [form, setForm] = useState({ name: "", email: "" })
  const tenantId = typeof window !== "undefined" ? localStorage.getItem("litxus_tenant_id") ?? "" : ""

  const load = async () => {
    if (!tenantId) return
    try {
      setLoading(true)
      const res = await tenantApi.getStaffAgents(tenantId)
      setAgents(res.data ?? [])
    } catch { /* silent */ } finally { setLoading(false) }
  }

  useEffect(() => { load() }, [tenantId])

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!tenantId) return
    setSubmitting(true)
    try {
      await tenantApi.createStaffAgent(tenantId, form)
      toast.success("Staff agent created")
      setShowForm(false)
      setForm({ name: "", email: "" })
      load()
    } catch (err) { toast.error(err instanceof Error ? err.message : "Failed") }
    finally { setSubmitting(false) }
  }

  const handleRotate = async (agentId: string) => {
    if (!tenantId) return
    try {
      const res = await tenantApi.rotateStaffAgentCode(tenantId, agentId)
      toast.success(`New code: ${res.data?.newCode ?? "—"}`)
      load()
    } catch (err) { toast.error(err instanceof Error ? err.message : "Failed") }
  }

  const inputCls = "w-full px-3 py-2 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-950 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-semibold text-gray-900 dark:text-white">Staff Agents</h2>
          <p className="text-sm text-gray-500 dark:text-gray-400">{agents.length} agents</p>
        </div>
        <button onClick={() => setShowForm(!showForm)} className="flex items-center gap-2 px-4 py-2 bg-[--color-brand-blue] hover:bg-blue-700 text-white text-sm font-semibold rounded-lg transition-colors">
          <Plus size={15} /> Add Agent
        </button>
      </div>

      {showForm && (
        <form onSubmit={handleCreate} className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-5 space-y-4">
          <h3 className="text-sm font-semibold text-gray-900 dark:text-white">New Staff Agent</h3>
          <div className="grid grid-cols-2 gap-4">
            <div><label className="block text-xs text-gray-500 mb-1">Name</label><input className={inputCls} value={form.name} onChange={e => setForm(f => ({...f, name: e.target.value}))} required /></div>
            <div><label className="block text-xs text-gray-500 mb-1">Email</label><input type="email" className={inputCls} value={form.email} onChange={e => setForm(f => ({...f, email: e.target.value}))} required /></div>
          </div>
          <div className="flex gap-2 justify-end">
            <button type="button" onClick={() => setShowForm(false)} className="px-4 py-2 text-sm text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg">Cancel</button>
            <button type="submit" disabled={submitting} className="px-4 py-2 text-sm font-semibold bg-[--color-brand-blue] text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">{submitting ? "Creating..." : "Create"}</button>
          </div>
        </form>
      )}

      <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl overflow-hidden">
        {loading ? (
          <div className="flex items-center justify-center py-12"><Loader className="animate-spin text-gray-400 mr-2" size={18} /><span className="text-sm text-gray-500">Loading...</span></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-gray-50 dark:bg-gray-800/50 border-b border-gray-200 dark:border-gray-800">
              <tr>
                {["Agent", "Referral Code", "Status", ""].map(h => <th key={h} className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">{h}</th>)}
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200 dark:divide-gray-800">
              {agents.length === 0 ? (
                <tr><td colSpan={4} className="px-4 py-8 text-center text-sm text-gray-500 dark:text-gray-400">No staff agents yet</td></tr>
              ) : agents.map(a => (
                <tr key={a.id} className="hover:bg-gray-50 dark:hover:bg-gray-800/30 transition-colors">
                  <td className="px-4 py-3"><p className="font-medium text-gray-900 dark:text-white">{a.name}</p><p className="text-xs text-gray-500">{a.email}</p></td>
                  <td className="px-4 py-3 font-mono text-sm text-[--color-brand-blue]">{a.referralCode}</td>
                  <td className="px-4 py-3"><StatusBadge status={a.isActive ? "Active" : "Suspended"} /></td>
                  <td className="px-4 py-3 text-right">
                    <button onClick={() => handleRotate(a.id)} title="Rotate code" className="p-1.5 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg text-gray-400 hover:text-gray-700 dark:hover:text-gray-200 transition-colors">
                      <RotateCcw size={14} />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  )
}
