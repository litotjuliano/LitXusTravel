"use client"

import { useState } from "react"
import { Loader, Search } from "lucide-react"
import { usePackages } from "@/lib/hooks/usePackages"
import { Button } from "@/components/ui/button"
import ActionMenu from "@/components/common/ActionMenu"
import PackageOverrideModal from "@/components/modals/PackageOverrideModal"
import { formatCurrency } from "@/lib/utils"
import { tenantApi } from "@/lib/api"
import { toast } from "sonner"

export default function PackagesPage() {
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const [search, setSearch] = useState("")
  const [selectedPackage, setSelectedPackage] = useState<any>(null)
  const [showOverrideModal, setShowOverrideModal] = useState(false)
  const [unsyncing, setUnsyncing] = useState<string | null>(null)

  const { packages, loading, pagination, refetch } = usePackages(page, pageSize)

  const handleUnsync = async (packageId: string) => {
    try {
      setUnsyncing(packageId)
      const tenantId = localStorage.getItem("nexus_tenant_id")
      if (!tenantId) {
        toast.error("Not authenticated")
        return
      }
      await tenantApi.unsyncPackage(tenantId, packageId)
      toast.success("Package unsynced successfully")
      refetch()
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Failed to unsync package")
    } finally {
      setUnsyncing(null)
    }
  }

  const handleOverride = (pkg: any) => {
    setSelectedPackage(pkg)
    setShowOverrideModal(true)
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="text-center">
          <Loader className="animate-spin mx-auto mb-4" size={32} />
          <p className="text-muted-foreground">Loading packages...</p>
        </div>
      </div>
    )
  }

  const filteredPackages = packages.filter(
    pkg => pkg.title.toLowerCase().includes(search.toLowerCase()) ||
           pkg.destination.toLowerCase().includes(search.toLowerCase())
  )

  return (
    <div className="space-y-6">
      {/* Search */}
      <div className="flex items-center gap-2 px-4 py-2.5 bg-card rounded-lg border border-border">
        <Search size={16} className="text-muted-foreground" />
        <input
          type="text"
          placeholder="Search packages..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="flex-1 bg-transparent outline-none text-sm placeholder-muted-foreground text-foreground"
        />
      </div>

      {/* Table */}
      <div className="bg-card border border-border rounded-xl overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-muted/50 border-b border-border">
            <tr>
              <th className="px-4 py-3 text-left font-medium text-foreground">Package</th>
              <th className="px-4 py-3 text-left font-medium text-foreground">Destination</th>
              <th className="px-4 py-3 text-left font-medium text-foreground">Price</th>
              <th className="px-4 py-3 text-left font-medium text-foreground">Duration</th>
              <th className="px-4 py-3 text-right font-medium text-foreground">Actions</th>
            </tr>
          </thead>
          <tbody>
            {filteredPackages.length > 0 ? (
              filteredPackages.map((pkg) => (
                <tr key={pkg.id} className="border-b border-border hover:bg-muted/30 transition-colors last:border-0">
                  <td className="px-4 py-3">
                    <p className="font-medium text-foreground">{pkg.title}</p>
                    <p className="text-xs text-muted-foreground">{pkg.isCustomized ? "Customized" : "Master"}</p>
                  </td>
                  <td className="px-4 py-3 text-foreground">{pkg.destination}</td>
                  <td className="px-4 py-3 text-foreground">{formatCurrency(pkg.price, pkg.currency)}</td>
                  <td className="px-4 py-3 text-foreground">{pkg.durationDays} days</td>
                  <td className="px-4 py-3 text-right">
                    <ActionMenu
                      items={[
                        { label: "Customize", onClick: () => handleOverride(pkg) },
                        { label: "Unsync", onClick: () => handleUnsync(pkg.id), destructive: true, disabled: unsyncing === pkg.id },
                      ]}
                    />
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan={5} className="px-4 py-8 text-center text-muted-foreground">
                  {search ? "No packages match your search" : "No packages synced yet"}
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Pagination */}
      {pagination && (
        <div className="flex items-center justify-between">
          <span className="text-sm text-muted-foreground">
            {filteredPackages.length > 0 ? `${(page - 1) * pageSize + 1}-${Math.min(page * pageSize, pagination.totalCount)}` : "0"} of {pagination.totalCount}
          </span>
          <div className="flex gap-2">
            <Button
              variant="outline"
              size="sm"
              disabled={!pagination.hasPreviousPage}
              onClick={() => setPage(page - 1)}
            >
              Previous
            </Button>
            <Button
              variant="outline"
              size="sm"
              disabled={!pagination.hasNextPage}
              onClick={() => setPage(page + 1)}
            >
              Next
            </Button>
          </div>
        </div>
      )}

      {/* Override modal */}
      {selectedPackage && (
        <PackageOverrideModal
          isOpen={showOverrideModal}
          onClose={() => setShowOverrideModal(false)}
          package={selectedPackage}
          onSuccess={() => {
            setShowOverrideModal(false)
            refetch()
          }}
        />
      )}
    </div>
  )
}
