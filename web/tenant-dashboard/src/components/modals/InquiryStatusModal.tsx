"use client"

import { useState } from "react"
import { Loader2 } from "lucide-react"
import { Modal } from "@/components/ui/Modal"
import { Button } from "@/components/ui/button"
import { tenantApi } from "@/lib/api"
import { toast } from "sonner"

const STATUSES = ["New", "Contacted", "Quoted", "Booked", "Lost"]

interface Props {
  isOpen: boolean
  onClose: () => void
  inquiry: any
  onSuccess: () => void
}

export default function InquiryStatusModal({ isOpen, onClose, inquiry, onSuccess }: Props) {
  const [loading, setLoading] = useState(false)
  const [status, setStatus] = useState(inquiry.status)

  const handleSubmit = async () => {
    try {
      setLoading(true)
      const tenantId = localStorage.getItem("litxus_tenant_id")
      if (!tenantId) {
        toast.error("Not authenticated")
        return
      }

      await tenantApi.updateInquiryStatus(tenantId, inquiry.id, status)
      toast.success("Inquiry status updated")
      onClose()
      onSuccess()
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Failed to update status")
    } finally {
      setLoading(false)
    }
  }

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      className="max-w-md mx-4 rounded-2xl shadow-theme-xl"
    >
      <div className="px-6 pt-6 pb-4 border-b border-gray-200 dark:border-gray-800">
        <h3 className="text-lg font-semibold text-gray-900 dark:text-white">Update Inquiry Status</h3>
      </div>

      <div className="p-6 space-y-4">
        <div>
          <p className="text-sm text-gray-500 dark:text-gray-400 mb-2">
            Customer: <span className="text-gray-900 dark:text-white font-medium">{inquiry.customerName}</span>
          </p>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-900 dark:text-white mb-3">Select New Status</label>
          <div className="space-y-2">
            {STATUSES.map((s) => (
              <button
                key={s}
                onClick={() => setStatus(s)}
                className={`w-full px-4 py-2.5 rounded-lg text-left text-sm font-medium transition-colors ${
                  status === s
                    ? "bg-brand-500 text-white"
                    : "bg-gray-100 dark:bg-gray-800 text-gray-900 dark:text-white hover:bg-gray-200 dark:hover:bg-gray-700"
                }`}
              >
                {s}
              </button>
            ))}
          </div>
        </div>

        <div className="flex gap-3 pt-2">
          <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
          <Button
            onClick={handleSubmit}
            disabled={loading || status === inquiry.status}
            className="flex-1"
          >
            {loading ? <><Loader2 size={16} className="animate-spin mr-2" />Updating...</> : "Update Status"}
          </Button>
        </div>
      </div>
    </Modal>
  )
}
