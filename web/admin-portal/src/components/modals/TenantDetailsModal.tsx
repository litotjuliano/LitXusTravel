"use client"

import { useState, useEffect } from "react"
import { Modal } from "@/components/ui/Modal"
import { Button } from "@/components/ui/button"
import { adminApi } from "@/lib/api"
import { formatDate } from "@/lib/utils"

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
  // eslint-disable-next-line react-hooks/exhaustive-deps
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
    <Modal
      isOpen={open}
      onClose={() => onOpenChange(false)}
      showCloseButton={true}
      className="rounded-2xl max-w-2xl p-6"
    >
      <div className="mb-5 pr-8">
        <h2 className="text-lg font-semibold text-gray-900 dark:text-white">Tenant Details</h2>
        <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">View and manage tenant information</p>
      </div>

      {error ? (
        <div className="text-red-600 p-4 bg-red-50 dark:bg-red-900/20 rounded-lg text-sm">{error}</div>
      ) : loading ? (
        <div className="space-y-4">
          <div className="h-10 w-full animate-pulse rounded-md bg-gray-200 dark:bg-gray-700" />
          <div className="h-10 w-full animate-pulse rounded-md bg-gray-200 dark:bg-gray-700" />
          <div className="h-10 w-full animate-pulse rounded-md bg-gray-200 dark:bg-gray-700" />
        </div>
      ) : details ? (
        <div className="space-y-6">
          {/* Basic Info */}
          <div className="space-y-4">
            <h3 className="font-semibold text-sm text-gray-900 dark:text-white">Basic Information</h3>
            <div className="h-px w-full bg-gray-200 dark:bg-gray-800" />

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-xs font-medium text-gray-500 dark:text-gray-400 mb-1">Tenant Name</label>
                <p className="text-sm font-medium text-gray-900 dark:text-white">{details.name}</p>
              </div>

              <div>
                <label className="block text-xs font-medium text-gray-500 dark:text-gray-400 mb-1">Status</label>
                <span className={`inline-block px-2.5 py-0.5 text-xs font-semibold rounded-full ${
                  details.isActive
                    ? "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400"
                    : "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400"
                }`}>
                  {details.isActive ? "Active" : "Inactive"}
                </span>
              </div>

              <div>
                <label className="block text-xs font-medium text-gray-500 dark:text-gray-400 mb-1">Contact Email</label>
                <p className="text-sm text-gray-900 dark:text-white">{details.contactEmail || "Not set"}</p>
              </div>

              <div>
                <label className="block text-xs font-medium text-gray-500 dark:text-gray-400 mb-1">Contact Phone</label>
                <p className="text-sm text-gray-900 dark:text-white">{details.contactPhone || "Not set"}</p>
              </div>

              <div>
                <label className="block text-xs font-medium text-gray-500 dark:text-gray-400 mb-1">Country</label>
                <p className="text-sm text-gray-900 dark:text-white">{details.country || "Not set"}</p>
              </div>

              <div>
                <label className="block text-xs font-medium text-gray-500 dark:text-gray-400 mb-1">Provisioning Status</label>
                <span className={`inline-block px-2.5 py-0.5 text-xs font-semibold rounded-full ${
                  details.provisioningStatus === "Completed"
                    ? "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400"
                    : details.provisioningStatus === "Pending"
                    ? "bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400"
                    : "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400"
                }`}>
                  {details.provisioningStatus}
                </span>
              </div>
            </div>
          </div>

          {/* Metadata */}
          <div className="space-y-4">
            <h3 className="font-semibold text-sm text-gray-900 dark:text-white">Metadata</h3>
            <div className="h-px w-full bg-gray-200 dark:bg-gray-800" />

            <div>
              <label className="block text-xs font-medium text-gray-500 dark:text-gray-400 mb-1">Created At</label>
              <p className="text-sm text-gray-900 dark:text-white">{formatDate(details.createdAt)}</p>
            </div>

            <div>
              <label className="block text-xs font-medium text-gray-500 dark:text-gray-400 mb-1">Tenant ID</label>
              <p className="text-xs font-mono text-gray-500 dark:text-gray-400 break-all">{details.tenantId}</p>
            </div>
          </div>

          {/* Actions */}
          <div className="flex gap-3 pt-4">
            <Button variant="outline" onClick={() => onOpenChange(false)}>
              Close
            </Button>
            <Button variant="primary" disabled>
              Edit (Coming Soon)
            </Button>
          </div>
        </div>
      ) : null}
    </Modal>
  )
}
