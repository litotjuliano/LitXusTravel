"use client"

import { useState, useEffect } from "react"
import { Plus, Loader } from "lucide-react"
import { tenantApi } from "@/lib/api"
import ActionMenu from "@/components/common/ActionMenu"
import StatusBadge from "@/components/common/StatusBadge"
import { formatDate, formatCurrency } from "@/lib/utils"
import { toast } from "sonner"

type Booking = {
  id: string; tenantId: string; tourId: string; customerName: string
  customerEmail: string; bookingDate: string; tourDate: string
  totalAmount: number; status: string; referralCode?: string
}

export default function BookingsPage() {
  const [bookings, setBookings] = useState<Booking[]>([])
  const [loading, setLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)
  const [submitting, setSubmitting] = useState(false)
  const [form, setForm] = useState({ tourId: "", customerName: "", customerEmail: "", tourDate: "", referralCode: "" })
  const tenantId = typeof window !== "undefined" ? localStorage.getItem("litxus_tenant_id") ?? "" : ""

  const load = async () => {
    if (!tenantId) return
    try {
      setLoading(true)
      const res = await tenantApi.getBookings(tenantId)
      setBookings(res.data ?? [])
    } catch { /* silent */ } finally { setLoading(false) }
  }

  useEffect(() => { load() }, [tenantId])

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!tenantId) return
    setSubmitting(true)
    try {
      await tenantApi.createBooking(tenantId, form)
      toast.success("Booking created")
      setShowForm(false)
      setForm({ tourId: "", customerName: "", customerEmail: "", tourDate: "", referralCode: "" })
      load()
    } catch (err) { toast.error(err instanceof Error ? err.message : "Failed") }
    finally { setSubmitting(false) }
  }

  const handleCancel = async (bookingId: string) => {
    if (!tenantId) return
    try {
      await tenantApi.cancelBooking(tenantId, bookingId, "Cancelled by admin")
      toast.success("Booking cancelled — commissions reversed")
      load()
    } catch (err) { toast.error(err instanceof Error ? err.message : "Failed") }
  }

  const inputCls = "w-full px-3 py-2 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-950 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-(--color-brand-blue)"

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-semibold text-gray-900 dark:text-white">Bookings</h2>
          <p className="text-sm text-gray-500 dark:text-gray-400">{bookings.length} total</p>
        </div>
        <button onClick={() => setShowForm(!showForm)} className="flex items-center gap-2 px-4 py-2 bg-(--color-brand-blue) hover:bg-blue-700 text-white text-sm font-semibold rounded-lg transition-colors">
          <Plus size={15} /> New Booking
        </button>
      </div>

      {showForm && (
        <form onSubmit={handleCreate} className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-5 space-y-4">
          <h3 className="text-sm font-semibold text-gray-900 dark:text-white">Create Booking</h3>
          <div className="grid grid-cols-2 gap-4">
            <div><label className="block text-xs text-gray-500 mb-1">Tour ID</label><input className={inputCls} value={form.tourId} onChange={e => setForm(f => ({...f, tourId: e.target.value}))} placeholder="Tour UUID" required /></div>
            <div><label className="block text-xs text-gray-500 mb-1">Tour Date</label><input type="date" className={inputCls} value={form.tourDate} onChange={e => setForm(f => ({...f, tourDate: e.target.value}))} required /></div>
            <div><label className="block text-xs text-gray-500 mb-1">Customer Name</label><input className={inputCls} value={form.customerName} onChange={e => setForm(f => ({...f, customerName: e.target.value}))} required /></div>
            <div><label className="block text-xs text-gray-500 mb-1">Customer Email</label><input type="email" className={inputCls} value={form.customerEmail} onChange={e => setForm(f => ({...f, customerEmail: e.target.value}))} required /></div>
            <div><label className="block text-xs text-gray-500 mb-1">Referral Code (optional)</label><input className={inputCls} value={form.referralCode} onChange={e => setForm(f => ({...f, referralCode: e.target.value}))} /></div>
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
                {["Customer", "Tour Date", "Amount", "Ref. Code", "Status", ""].map(h => <th key={h} className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">{h}</th>)}
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200 dark:divide-gray-800">
              {bookings.length === 0 ? (
                <tr><td colSpan={6} className="px-4 py-8 text-center text-sm text-gray-500 dark:text-gray-400">No bookings yet</td></tr>
              ) : bookings.map(b => (
                <tr key={b.id} className="hover:bg-gray-50 dark:hover:bg-gray-800/30 transition-colors">
                  <td className="px-4 py-3"><p className="font-medium text-gray-900 dark:text-white">{b.customerName}</p><p className="text-xs text-gray-500">{b.customerEmail}</p></td>
                  <td className="px-4 py-3 text-gray-700 dark:text-gray-300">{formatDate(b.tourDate)}</td>
                  <td className="px-4 py-3 font-semibold text-gray-900 dark:text-white">{formatCurrency(b.totalAmount)}</td>
                  <td className="px-4 py-3 font-mono text-xs text-gray-500 dark:text-gray-400">{b.referralCode ?? "—"}</td>
                  <td className="px-4 py-3"><StatusBadge status={b.status} /></td>
                  <td className="px-4 py-3 text-right">
                    {b.status === "Confirmed" && (
                      <ActionMenu items={[{ label: "Cancel", onClick: () => handleCancel(b.id), danger: true }]} />
                    )}
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
