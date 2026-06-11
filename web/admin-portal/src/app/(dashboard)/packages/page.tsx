"use client"

import { useState } from "react"
import { Plus, Search, Loader } from "lucide-react"
import { motion } from "framer-motion"
import { Button } from "@/components/ui/button"
import StatusBadge from "@/components/common/StatusBadge"
import ActionMenu from "@/components/common/ActionMenu"
import { PackageEditorModal } from "@/components/modals/PackageEditorModal"
import { usePackages } from "@/lib/hooks/usePackages"
import { formatCurrency } from "@/lib/utils"
import { toast } from "sonner"

export default function PackagesPage() {
  const [search, setSearch] = useState("")
  const [filter, setFilter] = useState("All")
  const [editorOpen, setEditorOpen] = useState(false)
  const { packages, loading, error, refetch } = usePackages()

  const filtered = packages.filter((p) => {
    const matchSearch = p.title.toLowerCase().includes(search.toLowerCase()) ||
                        p.destination.toLowerCase().includes(search.toLowerCase())
    const matchFilter = filter === "All" || p.visibility === filter
    return matchSearch && matchFilter
  })

  return (
    <div className="space-y-5">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-semibold text-foreground">Master Packages</h2>
          <p className="text-sm text-muted-foreground">{packages.length} total packages</p>
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
          <Search size={15} className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
          <input
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Search packages..."
            className="w-full pl-9 pr-4 py-2 text-sm border border-border rounded-lg bg-card focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
          />
        </div>
        {["All", "Published", "Draft", "Archived"].map((s) => (
          <button
            key={s}
            onClick={() => setFilter(s)}
            className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors ${
              filter === s
                ? "bg-[--color-brand-blue] text-white"
                : "bg-muted text-muted-foreground hover:bg-muted/80"
            }`}
          >
            {s}
          </button>
        ))}
      </div>

      {/* Table */}
      <div className="bg-card border border-border rounded-xl overflow-hidden">
        {loading ? (
          <div className="flex items-center justify-center py-12">
            <Loader className="animate-spin text-muted-foreground mr-2" size={20} />
            <p className="text-muted-foreground">Loading packages...</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-muted/50 border-b border-border">
                <tr>
                  {["Package", "Destination", "Price", "Duration", "Synced", "Status", ""].map((h) => (
                    <th key={h} className="text-left px-4 py-3 text-xs font-semibold text-muted-foreground uppercase tracking-wide">
                      {h}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {filtered.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="px-4 py-8 text-center text-muted-foreground">
                      No packages found
                    </td>
                  </tr>
                ) : (
                  filtered.map((pkg, i) => (
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
                  <td className="px-4 py-3 font-semibold text-foreground">{formatCurrency(pkg.basePrice, pkg.currency)}</td>
                  <td className="px-4 py-3 text-foreground">{pkg.durationDays}D</td>
                  <td className="px-4 py-3">
                    <span className="font-semibold text-foreground">{pkg.syncedTenantsCount}</span>
                    <span className="text-muted-foreground"> tenants</span>
                  </td>
                  <td className="px-4 py-3">
                    <StatusBadge status={pkg.visibility} />
                  </td>
                  <td className="px-4 py-3">
                    <ActionMenu items={[
                      { label: "Edit",      action: () => toast.info("Edit coming soon") },
                      { label: "Publish",   action: () => toast.success(`${pkg.title} published`) },
                      { label: "Duplicate", action: () => toast.info("Duplicate coming soon") },
                      { label: "Delete",    action: () => toast.error("Delete coming soon"), danger: true },
                    ]} />
                  </td>
                </motion.tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <PackageEditorModal open={editorOpen} onOpenChange={setEditorOpen} onSuccess={refetch} />
    </div>
  )
}
