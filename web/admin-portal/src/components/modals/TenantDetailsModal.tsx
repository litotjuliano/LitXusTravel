"use client"

import { useState, useEffect } from "react"
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { Skeleton } from "@/components/ui/skeleton"
import { adminApi } from "@/lib/api"
import { formatDate } from "@/lib/utils"
import { toast } from "sonner"

interface TenantDetailsModalProps {
  tenantId: string | null
  open: boolean
  onOpenChange: (open: boolean) => void
}

interface TenantDetails {
  tenantId: string
  name: string
  contactEmail: string | null
  contactPhone: string | null
  isActive: boolean
  provisioningStatus: string
  country: string | null
  createdAt: string
}

export function TenantDetailsModal({ tenantId, open, onOpenChange }: TenantDetailsModalProps) {
  const [details, setDetails] = useState<TenantDetails | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (open && tenantId) {
      fetchDetails()
    }
  }, [open, tenantId])

  const fetchDetails = async () => {
    if (!tenantId) return

    try {
      setLoading(true)
      const response = await adminApi.getTenantSettings(tenantId)
      setDetails(response.data)
      setError(null)
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to fetch tenant details")
      setDetails(null)
    } finally {
      setLoading(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>Tenant Details</DialogTitle>
          <DialogDescription>View and manage tenant information</DialogDescription>
        </DialogHeader>

        {error ? (
          <div className="text-red-600 p-4 bg-red-50 rounded-lg">{error}</div>
        ) : loading ? (
          <div className="space-y-4">
            <Skeleton className="h-10 w-full" />
            <Skeleton className="h-10 w-full" />
            <Skeleton className="h-10 w-full" />
          </div>
        ) : details ? (
          <div className="space-y-6">
            {/* Basic Info */}
            <div className="space-y-4">
              <h3 className="font-semibold text-sm text-foreground">Basic Information</h3>
              <Separator />

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-xs font-medium text-muted-foreground mb-1">Tenant Name</label>
                  <p className="text-sm font-medium text-foreground">{details.name}</p>
                </div>

                <div>
                  <label className="block text-xs font-medium text-muted-foreground mb-1">Status</label>
                  <span className={`inline-block px-2.5 py-0.5 text-xs font-semibold rounded-full ${
                    details.isActive
                      ? "bg-green-100 text-green-700"
                      : "bg-red-100 text-red-700"
                  }`}>
                    {details.isActive ? "Active" : "Inactive"}
                  </span>
                </div>

                <div>
                  <label className="block text-xs font-medium text-muted-foreground mb-1">Contact Email</label>
                  <p className="text-sm text-foreground">{details.contactEmail || "Not set"}</p>
                </div>

                <div>
                  <label className="block text-xs font-medium text-muted-foreground mb-1">Contact Phone</label>
                  <p className="text-sm text-foreground">{details.contactPhone || "Not set"}</p>
                </div>

                <div>
                  <label className="block text-xs font-medium text-muted-foreground mb-1">Country</label>
                  <p className="text-sm text-foreground">{details.country || "Not set"}</p>
                </div>

                <div>
                  <label className="block text-xs font-medium text-muted-foreground mb-1">Provisioning Status</label>
                  <span className={`inline-block px-2.5 py-0.5 text-xs font-semibold rounded-full ${
                    details.provisioningStatus === "Completed"
                      ? "bg-blue-100 text-blue-700"
                      : details.provisioningStatus === "Pending"
                      ? "bg-yellow-100 text-yellow-700"
                      : "bg-red-100 text-red-700"
                  }`}>
                    {details.provisioningStatus}
                  </span>
                </div>
              </div>
            </div>

            {/* Metadata */}
            <div className="space-y-4">
              <h3 className="font-semibold text-sm text-foreground">Metadata</h3>
              <Separator />

              <div>
                <label className="block text-xs font-medium text-muted-foreground mb-1">Created At</label>
                <p className="text-sm text-foreground">{formatDate(details.createdAt)}</p>
              </div>

              <div>
                <label className="block text-xs font-medium text-muted-foreground mb-1">Tenant ID</label>
                <p className="text-xs font-mono text-muted-foreground break-all">{details.tenantId}</p>
              </div>
            </div>

            {/* Actions */}
            <div className="flex gap-3 pt-4">
              <Button
                variant="outline"
                onClick={() => onOpenChange(false)}
              >
                Close
              </Button>
              <Button
                className="bg-blue-600 hover:bg-blue-700 text-white"
                disabled
              >
                Edit (Coming Soon)
              </Button>
            </div>
          </div>
        ) : null}
      </DialogContent>
    </Dialog>
  )
}
