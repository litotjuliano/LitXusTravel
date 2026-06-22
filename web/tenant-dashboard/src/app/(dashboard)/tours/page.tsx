"use client"

import { useState, useEffect } from "react"
import { Plus, Loader } from "lucide-react"
import { tenantApi } from "@/lib/api"
import ActionMenu from "@/components/common/ActionMenu"
import StatusBadge from "@/components/common/StatusBadge"
import { formatDate } from "@/lib/utils"
import { toast } from "sonner"

type Tour = {
  id: string; tenantId: string; packageId: string; packageTitle: string
  tourDate: string; capacity: number; bookedCount: number; basePrice: number
  status: string; guideNotes?: string
}

export default function ToursPage() {
  const [tours, setTours] = useState<Tour[]>([])
  const [loading, setLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)
  const [submitting, setSubmitting] = useState(false)
  const [form, setForm] = useState({ packageId: "", tourDate: "", capacity: 20, basePrice: 0, guideNotes: "" })
  const tenantId = typeof window !== "undefined" ? localStorage.getItem("litxus_tenant_id") ?? "" : ""

  const load = async () => {
    if (!tenantId) return
    try {
      setLoading(true)
      const res = await tenantApi.getTours(tenantId)
      setTours(res.data ?? [])
    } catch { /* silent */ } finally { setLoading(false) }
  }

  useEffect(() => { load() }, [tenantId])

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!tenantId) return
    setSubmitting(true)
    try {
      await tenantApi.createTour(tenantId, form)
      toast.success("Tour created")
      setShowForm(false)
      setForm({ packageId: "", tourDate: "", capacity: 20, basePrice: 0, guideNotes: "" })
      load()
    } catch (err) { toast.error(err instanceof Error ? err.message : "Failed") }
    finally { setSubmitting(false) }
  }

  const handleComplete = async (tourId: string) => {
    if (!tenantId) return
    try {
      await tenantApi.completeTour(tenantId, tourId)
      toast.success("Tour completed — commissions finalized")
      load()
    } catch (err) { toast.error(err instanceof Error ? err.message : "Failed") }
  }

  const inputCls = "w-full px-3 py-2 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-950 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-(--color-brand-blue)"

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-semibold text-gray-900 dark:text-white">Tours</h2>
          <p className="text-sm text-gray-500 dark:text-gray-400">{tours.length} total</p>
        </div>
        <button onClick={() => setShowForm(!showForm)} className="flex items-center gap-2 px-4 py-2 bg-(--color-brand-blue) hover:bg-blue-700 text-white text-sm font-semibold rounded-lg transition-colors">
          <Plus size={15} /> New Tour
        </button>
      </div>

      {showForm && (
        <form onSubmit={handleCreate} className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-5 space-y-4">
          <h3 className="text-sm font-semibold text-gray-900 dark:text-white">Create Tour</h3>
          <div className="grid grid-cols-2 gap-4">
            <div><label className="block text-xs text-gray-500 mb-1">Package ID</label><input className={inputCls} value={form.packageId} onChange={e => setForm(f => ({...f, packageId: e.target.value}))} placeholder="Package UUID" required /></div>
            <div><label className="block text-xs text-gray-500 mb-1">Tour Date</label><input type="date" className={inputCls} value={form.tourDate} onChange={e => setForm(f => ({...f, tourDate: e.target.value}))} required /></div>
            <div><label className="block text-xs text-gray-500 mb-1">Capacity</label><input type="number" className={inputCls} value={form.capacity} onChange={e => setForm(f => ({...f, capacity: Number(e.target.value)}))} min={1} /></div>
            <div><label className="block text-xs text-gray-500 mb-1">Base Price</label><input type="number" className={inputCls} value={form.basePrice} onChange={e => setForm(f => ({...f, basePrice: Number(e.target.value)}))} min={0} /></div>
          </div>
          <div className="flex gap-2 justify-end">
            <button type="button" onClick={() => setShowForm(false)} className="px-4 py-2 text-sm text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg">Cancel</button>
            <button type="submit" disabled={submitting} className="px-4 py-2 text-sm font-semibold bg-(--color-brand-blue) text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">{submitting ? "Creating..." : "Create"}</button>
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
                {["Package", "Date", "Capacity", "Status", ""].map(h => <th key={h} className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">{h}</th>)}
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200 dark:divide-gray-800">
              {tours.length === 0 ? (
                <tr><td colSpan={5} className="px-4 py-8 text-center text-sm text-gray-500 dark:text-gray-400">No tours yet</td></tr>
              ) : tours.map(t => (
                <tr key={t.id} className="hover:bg-gray-50 dark:hover:bg-gray-800/30 transition-colors">
                  <td className="px-4 py-3 font-medium text-gray-900 dark:text-white">{t.packageTitle}</td>
                  <td className="px-4 py-3 text-gray-700 dark:text-gray-300">{formatDate(t.tourDate)}</td>
                  <td className="px-4 py-3 text-gray-700 dark:text-gray-300">{t.bookedCount}/{t.capacity}</td>
                  <td className="px-4 py-3"><StatusBadge status={t.status} /></td>
                  <td className="px-4 py-3 text-right">
                    <ActionMenu items={[
                      ...(t.status === "Scheduled" ? [{ label: "Mark Complete", onClick: () => handleComplete(t.id) }] : []),
                    ]} />
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
