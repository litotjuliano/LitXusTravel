"use client"

import { useState } from "react"
import { Plus, Search, Loader } from "lucide-react"
import { motion } from "framer-motion"
import { Button } from "@/components/ui/button"
import StatusBadge from "@/components/common/StatusBadge"
import ActionMenu from "@/components/common/ActionMenu"
import { Pagination } from "@/components/common/Pagination"
import { SortableHeader } from "@/components/common/SortableHeader"
import { TenantDetailsModal } from "@/components/modals/TenantDetailsModal"
import { useTenants } from "@/lib/hooks/useTenants"
import { formatDate, getTokenClaims } from "@/lib/utils"
import { toast } from "sonner"

export default function TenantsPage() {
  const [search, setSearch] = useState("")
  const [selectedTenantId, setSelectedTenantId] = useState<string | null>(null)
  const [detailsOpen, setDetailsOpen] = useState(false)

  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(20)
  const [sortBy, setSortBy] = useState<string | undefined>(undefined)
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("desc")

  const { tenants, loading, error, pagination } = useTenants(page, pageSize, {
    sortBy,
    sortOrder,
  })

  const { tenantId: myTenantId } = getTokenClaims()

  // Tenant Admins (role=Admin with a tenantId claim) only see their own tenant
  const scoped = myTenantId ? tenants.filter((t) => t.id === myTenantId) : tenants

  const displayed = scoped.filter(
    (t) =>
      !search ||
      t.name.toLowerCase().includes(search.toLowerCase()) ||
      t.contactEmail.toLowerCase().includes(search.toLowerCase())
  )

  function handleSort(key: string) {
    if (sortBy === key) {
      setSortOrder((prev) => (prev === "asc" ? "desc" : "asc"))
    } else {
      setSortBy(key)
      setSortOrder("asc")
    }
    setPage(1)
  }

  function handlePageSizeChange(size: number) {
    setPageSize(size)
    setPage(1)
  }

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-semibold text-foreground">Tenants</h2>
          <p className="text-sm text-muted-foreground">
            {pagination.totalCount} registered agents
          </p>
        </div>
        <Button
          className="bg-[--color-brand-blue] hover:bg-blue-700 text-white gap-2"
          onClick={() => toast.info("Invite agent coming soon")}
        >
          <Plus size={16} />
          Invite Agent
        </Button>
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-red-700 text-sm">
          {error}
        </div>
      )}

      <div className="relative max-w-xs">
        <Search
          size={15}
          className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
        />
        <input
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Search tenants..."
          className="w-full pl-9 pr-4 py-2 text-sm border border-border rounded-lg bg-card focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
        />
      </div>

      <div className="bg-card border border-border rounded-xl overflow-hidden">
        {loading ? (
          <div className="flex items-center justify-center py-12">
            <Loader className="animate-spin text-muted-foreground mr-2" size={20} />
            <p className="text-muted-foreground">Loading tenants...</p>
          </div>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead className="bg-muted/50 border-b border-border">
                  <tr>
                    <SortableHeader
                      label="Tenant"
                      sortKey="name"
                      currentSortBy={sortBy}
                      sortOrder={sortOrder}
                      onSort={handleSort}
                    />
                    <th className="text-left px-4 py-3 text-xs font-semibold text-muted-foreground uppercase tracking-wide">
                      Subdomain
                    </th>
                    <th className="text-left px-4 py-3 text-xs font-semibold text-muted-foreground uppercase tracking-wide">
                      Plan
                    </th>
                    <th className="text-left px-4 py-3 text-xs font-semibold text-muted-foreground uppercase tracking-wide">
                      Packages
                    </th>
                    <SortableHeader
                      label="Joined"
                      sortKey="createdat"
                      currentSortBy={sortBy}
                      sortOrder={sortOrder}
                      onSort={handleSort}
                    />
                    <th className="text-left px-4 py-3 text-xs font-semibold text-muted-foreground uppercase tracking-wide">
                      Status
                    </th>
                    <th className="px-4 py-3" />
                  </tr>
                </thead>
                <tbody className="divide-y divide-border">
                  {displayed.length === 0 ? (
                    <tr>
                      <td
                        colSpan={7}
                        className="px-4 py-8 text-center text-muted-foreground"
                      >
                        No tenants found
                      </td>
                    </tr>
                  ) : (
                    displayed.map((t, i) => (
                      <motion.tr
                        key={t.id}
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        transition={{ delay: i * 0.05 }}
                        className="hover:bg-muted/30 transition-colors"
                      >
                        <td className="px-4 py-3">
                          <p className="font-medium text-foreground">{t.name}</p>
                          <p className="text-xs text-muted-foreground">{t.contactEmail}</p>
                        </td>
                        <td className="px-4 py-3">
                          {t.subdomain ? (
                            <code className="text-xs bg-muted px-2 py-0.5 rounded text-foreground">
                              {t.subdomain}.nexustravel.com
                            </code>
                          ) : (
                            <span className="text-muted-foreground text-xs">—</span>
                          )}
                        </td>
                        <td className="px-4 py-3 text-foreground">{t.plan ?? "—"}</td>
                        <td className="px-4 py-3 font-semibold text-foreground">
                          {t.syncedPackagesCount ?? 0}
                        </td>
                        <td className="px-4 py-3 text-muted-foreground">
                          {formatDate(t.createdAt)}
                        </td>
                        <td className="px-4 py-3">
                          <StatusBadge status={t.isActive ? "Active" : "Suspended"} />
                        </td>
                        <td className="px-4 py-3">
                          <ActionMenu
                            items={[
                              {
                                label: "View Details",
                                action: () => {
                                  setSelectedTenantId(t.id)
                                  setDetailsOpen(true)
                                },
                              },
                              {
                                label: "Edit",
                                action: () => toast.info("Edit coming soon"),
                              },
                              {
                                label: "Suspend",
                                action: () => toast.warning(`${t.name} suspended`),
                                danger: true,
                              },
                            ]}
                          />
                        </td>
                      </motion.tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>

            <Pagination
              page={pagination.page}
              totalPages={pagination.totalPages}
              totalCount={pagination.totalCount}
              pageSize={pagination.pageSize}
              onPageChange={setPage}
              onPageSizeChange={handlePageSizeChange}
            />
          </>
        )}
      </div>

      <TenantDetailsModal
        tenantId={selectedTenantId}
        open={detailsOpen}
        onOpenChange={setDetailsOpen}
      />
    </div>
  )
}
