"use client"

import { useState } from "react"
import { Loader2 } from "lucide-react"
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog"
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
      const tenantId = localStorage.getItem("nexus_tenant_id")
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
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Update Inquiry Status</DialogTitle>
        </DialogHeader>

        <div className="space-y-4">
          <div>
            <p className="text-sm text-muted-foreground mb-2">Customer: <span className="text-foreground font-medium">{inquiry.customerName}</span></p>
          </div>

          <div>
            <label className="block text-sm font-medium text-foreground mb-3">Select New Status</label>
            <div className="space-y-2">
              {STATUSES.map((s) => (
                <button
                  key={s}
                  onClick={() => setStatus(s)}
                  className={`w-full px-4 py-2.5 rounded-lg text-left text-sm font-medium transition-colors ${
                    status === s
                      ? "bg-[--color-brand-blue] text-white"
                      : "bg-muted text-foreground hover:bg-muted/80"
                  }`}
                >
                  {s}
                </button>
              ))}
            </div>
          </div>

          <div className="flex gap-3 pt-6">
            <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
            <Button
              onClick={handleSubmit}
              disabled={loading || status === inquiry.status}
              className="flex-1 bg-[--color-brand-blue] hover:bg-blue-700"
            >
              {loading ? <><Loader2 size={16} className="animate-spin mr-2" />Updating...</> : "Update Status"}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  )
}
