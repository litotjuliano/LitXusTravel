"use client"

import { useState, useEffect } from "react"
import { Plus, Search, Loader } from "lucide-react"
import { motion } from "framer-motion"
import { Button } from "@/components/ui/button"
import StatusBadge from "@/components/common/StatusBadge"
import ActionMenu from "@/components/common/ActionMenu"
import { Pagination } from "@/components/common/Pagination"
import { SortableHeader } from "@/components/common/SortableHeader"
import { PackageEditorModal } from "@/components/modals/PackageEditorModal"
import { PackageViewModal } from "@/components/modals/PackageViewModal"
import { usePackages } from "@/lib/hooks/usePackages"
import { formatCurrency, getTokenClaims } from "@/lib/utils"
import { toast } from "sonner"

export default function PackagesPage() {
  const [search, setSearch] = useState("")
  const [filter, setFilter] = useState("All")
  const [editorOpen, setEditorOpen] = useState(false)
  const [viewOpen, setViewOpen] = useState(false)
  const [viewPackageId, setViewPackageId] = useState<string | null>(null)
  const [tenantId, setTenantId] = useState<string | undefined>(undefined)

  const [tenantFilter, setTenantFilter] = useState("All")

  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(20)
  const [sortBy, setSortBy] = useState<string | undefined>(undefined)
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("desc")

  useEffect(() => {
    setTenantId(getTokenClaims().tenantId)
  }, [])

  const isTenantAdmin = !!tenantId

  // Admin: status filter is server-side. Tenant: filtered client-side after fetch.
  const serverStatus = !isTenantAdmin && filter !== "All" ? filter : undefined

  const { packages, loading, error, pagination, refetch } = usePackages(
    page,
    pageSize,
    { sortBy, sortOrder, status: serverStatus }
  )

  const tenantOptions = !isTenantAdmin
    ? ["All", ...Array.from(new Set(packages.flatMap((p) => p.tenants))).sort()]
    : []

  const filterTabs = isTenantAdmin
    ? ["All", "Owned", "Customized", "Synced"]
    : ["All", "Published", "Draft", "Archived"]

  // For tenant: apply tab filter client-side (Owned/Customized/Synced are computed).
  // For admin: server already filtered; only apply search client-side.
  const displayed = packages.filter((p) => {
    const matchSearch =
      !search ||
      p.title.toLowerCase().includes(search.toLowerCase()) ||
      p.destination.toLowerCase().includes(search.toLowerCase())
    const matchFilter =
      !isTenantAdmin || filter === "All" || p.visibility === filter
    const matchTenant =
      isTenantAdmin || tenantFilter === "All" || p.tenants.includes(tenantFilter)
    return matchSearch && matchFilter && matchTenant
  })

  function handleSort(key: string) {
    if (sortBy === key) {
      setSortOrder((prev) => (prev === "asc" ? "desc" : "asc"))
    } else {
      setSortBy(key)
      setSortOrder("asc")
    }
    setPage(1)
  }

  function handleFilterChange(f: string) {
    setFilter(f)
    setTenantFilter("All")
    setPage(1)
  }

  function handlePageSizeChange(size: number) {
    setPageSize(size)
    setPage(1)
  }

  const colCount = isTenantAdmin ? 6 : 7

  return (
    <div className="space-y-5">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-semibold text-foreground">
            {isTenantAdmin ? "My Packages" : "Master Packages"}
          </h2>
          <p className="text-sm text-muted-foreground">
            {pagination.totalCount} total packages
          </p>
        </div>
        <Button
          className="bg-[--color-brand-blue] hover:bg-blue-700 text-white gap-2"
          onClick={() => setEditorOpen(true)}
        >
          <Plus size={16} />
          New Package
        </Button>
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-red-700 text-sm">
          {error}
        </div>
      )}

      {/* Filters */}
      <div className="flex flex-wrap gap-3 items-center">
        <div className="relative flex-1 max-w-xs">
          <Search
            size={15}
            className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
          />
          <input
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Search packages..."
            className="w-full pl-9 pr-4 py-2 text-sm border border-border rounded-lg bg-card focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
          />
        </div>
        {filterTabs.map((s) => (
          <button
            key={s}
            onClick={() => handleFilterChange(s)}
            className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors ${
              filter === s
                ? "bg-[--color-brand-blue] text-white"
                : "bg-muted text-muted-foreground hover:bg-muted/80"
            }`}
          >
            {s}
          </button>
        ))}
        {!isTenantAdmin && tenantOptions.length > 1 && (
          <select
            value={tenantFilter}
            onChange={(e) => { setTenantFilter(e.target.value); setPage(1) }}
            className="px-3 py-1.5 rounded-lg text-xs font-medium bg-muted text-muted-foreground border border-border focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
          >
            {tenantOptions.map((t) => (
              <option key={t} value={t}>{t === "All" ? "All Tenants" : t}</option>
            ))}
          </select>
        )}
      </div>

      {/* Table */}
      <div className="bg-card border border-border rounded-xl overflow-hidden">
        {loading ? (
          <div className="flex items-center justify-center py-12">
            <Loader className="animate-spin text-muted-foreground mr-2" size={20} />
            <p className="text-muted-foreground">Loading packages...</p>
          </div>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead className="bg-muted/50 border-b border-border">
                  <tr>
                    <SortableHeader
                      label="Package"
                      sortKey="title"
                      currentSortBy={sortBy}
                      sortOrder={sortOrder}
                      onSort={handleSort}
                    />
                    <th className="text-left px-4 py-3 text-xs font-semibold text-muted-foreground uppercase tracking-wide">
                      Destination
                    </th>
                    <SortableHeader
                      label="Price"
                      sortKey="price"
                      currentSortBy={sortBy}
                      sortOrder={sortOrder}
                      onSort={handleSort}
                    />
                    <th className="text-left px-4 py-3 text-xs font-semibold text-muted-foreground uppercase tracking-wide">
                      Duration
                    </th>
                    {!isTenantAdmin && (
                      <th className="text-left px-4 py-3 text-xs font-semibold text-muted-foreground uppercase tracking-wide">
                        Tenants
                      </th>
                    )}
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
                        colSpan={colCount}
                        className="px-4 py-8 text-center text-muted-foreground"
                      >
                        No packages found
                      </td>
                    </tr>
                  ) : (
                    displayed.map((pkg, i) => (
                      <motion.tr
                        key={pkg.id}
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        transition={{ delay: i * 0.04 }}
                        className="hover:bg-muted/30 transition-colors"
                      >
                        <td className="px-4 py-3">
                          <p className="font-medium text-foreground">{pkg.title}</p>
                          <p className="text-xs text-muted-foreground mt-0.5">{pkg.category}</p>
                        </td>
                        <td className="px-4 py-3 text-foreground">{pkg.destination}</td>
                        <td className="px-4 py-3 font-semibold text-foreground">
                          {formatCurrency(pkg.basePrice, pkg.currency)}
                        </td>
                        <td className="px-4 py-3 text-foreground">{pkg.durationDays}D</td>
                        {!isTenantAdmin && (
                          <td className="px-4 py-3">
                            <div className="flex flex-wrap gap-1">
                              {pkg.tenants.map((name) =>
                                name === "System Adm" ? (
                                  <span
                                    key="system-adm"
                                    className="inline-block px-2 py-0.5 text-[11px] font-medium rounded-full bg-gray-100 text-gray-600 dark:bg-gray-800 dark:text-gray-400 whitespace-nowrap"
                                  >
                                    System Adm
                                  </span>
                                ) : (
                                  <span
                                    key={name}
                                    className="inline-block px-2 py-0.5 text-[11px] font-medium rounded-full bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400 whitespace-nowrap"
                                  >
                                    {name}
                                  </span>
                                )
                              )}
                            </div>
                          </td>
                        )}
                        <td className="px-4 py-3">
                          <StatusBadge
                            status={pkg.visibility}
                            label={pkg.visibility === "Synced" && pkg.syncSource
                              ? `Synced (${pkg.syncSource})`
                              : pkg.visibility}
                          />
                        </td>
                        <td className="px-4 py-3">
                          <ActionMenu
                            items={[
                              {
                                label: "View",
                                action: () => { setViewPackageId(pkg.id); setViewOpen(true) },
                              },
                              ...(pkg.visibility !== "Synced" ? [{
                                label: "Edit",
                                action: () => toast.info("Edit coming soon"),
                              }] : []),
                              {
                                label: "Publish",
                                action: () => toast.success(`${pkg.title} published`),
                              },
                              {
                                label: "Duplicate",
                                action: () => toast.info("Duplicate coming soon"),
                              },
                              ...(pkg.visibility !== "Synced" ? [{
                                label: "Delete",
                                action: () => toast.error("Delete coming soon"),
                                danger: true,
                              }] : []),
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

      <PackageViewModal
        packageId={viewPackageId}
        open={viewOpen}
        onOpenChange={setViewOpen}
      />

      <PackageEditorModal
        open={editorOpen}
        onOpenChange={setEditorOpen}
        onSuccess={refetch}
        tenantId={isTenantAdmin ? tenantId : undefined}
      />
    </div>
  )
}
