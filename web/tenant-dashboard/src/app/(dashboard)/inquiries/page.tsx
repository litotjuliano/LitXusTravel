"use client"

import { useState } from "react"
import { Loader } from "lucide-react"
import { useInquiries } from "@/lib/hooks/useInquiries"
import { Button } from "@/components/ui/button"
import StatusBadge from "@/components/common/StatusBadge"
import InquiryStatusModal from "@/components/modals/InquiryStatusModal"
import { formatDate } from "@/lib/utils"

const STATUS_FILTERS = ["All", "New", "Contacted", "Quoted", "Booked", "Lost"]

export default function InquiriesPage() {
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(20)
  const [activeFilter, setActiveFilter] = useState("All")
  const [selectedInquiry, setSelectedInquiry] = useState<any>(null)
  const [showStatusModal, setShowStatusModal] = useState(false)

  const statusFilter = activeFilter === "All" ? undefined : activeFilter
  const { inquiries, loading, pagination, refetch } = useInquiries(statusFilter, page, pageSize)

  const handleStatusChange = (inquiry: any) => {
    setSelectedInquiry(inquiry)
    setShowStatusModal(true)
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="text-center">
          <Loader className="animate-spin mx-auto mb-4" size={32} />
          <p className="text-muted-foreground">Loading inquiries...</p>
        </div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Status filters */}
      <div className="flex gap-2 overflow-x-auto pb-2">
        {STATUS_FILTERS.map((status) => (
          <button
            key={status}
            onClick={() => {
              setActiveFilter(status)
              setPage(1)
            }}
            className={`px-4 py-2 rounded-lg text-sm font-medium whitespace-nowrap transition-colors ${
              activeFilter === status
                ? "bg-[--color-brand-blue] text-white"
                : "bg-card border border-border text-foreground hover:bg-muted"
            }`}
          >
            {status}
          </button>
        ))}
      </div>

      {/* Table */}
      <div className="bg-card border border-border rounded-xl overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-muted/50 border-b border-border">
            <tr>
              <th className="px-4 py-3 text-left font-medium text-foreground">Customer</th>
              <th className="px-4 py-3 text-left font-medium text-foreground">Email</th>
              <th className="px-4 py-3 text-left font-medium text-foreground">Phone</th>
              <th className="px-4 py-3 text-left font-medium text-foreground">Status</th>
              <th className="px-4 py-3 text-left font-medium text-foreground">Received</th>
              <th className="px-4 py-3 text-right font-medium text-foreground">Action</th>
            </tr>
          </thead>
          <tbody>
            {inquiries.length > 0 ? (
              inquiries.map((inq) => (
                <tr key={inq.id} className="border-b border-border hover:bg-muted/30 transition-colors last:border-0">
                  <td className="px-4 py-3 text-foreground font-medium">{inq.customerName}</td>
                  <td className="px-4 py-3 text-foreground text-sm">{inq.customerEmail}</td>
                  <td className="px-4 py-3 text-foreground text-sm">{inq.customerPhone || "-"}</td>
                  <td className="px-4 py-3">
                    <StatusBadge status={inq.status} />
                  </td>
                  <td className="px-4 py-3 text-muted-foreground text-sm">{formatDate(inq.createdAt)}</td>
                  <td className="px-4 py-3 text-right">
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => handleStatusChange(inq)}
                      className="text-[--color-brand-blue] hover:bg-muted"
                    >
                      Change Status
                    </Button>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan={6} className="px-4 py-8 text-center text-muted-foreground">
                  No inquiries yet
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
            {inquiries.length > 0 ? `${(page - 1) * pageSize + 1}-${Math.min(page * pageSize, pagination.totalCount)}` : "0"} of {pagination.totalCount}
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

      {/* Status change modal */}
      {selectedInquiry && (
        <InquiryStatusModal
          isOpen={showStatusModal}
          onClose={() => setShowStatusModal(false)}
          inquiry={selectedInquiry}
          onSuccess={() => {
            setShowStatusModal(false)
            refetch()
          }}
        />
      )}
    </div>
  )
}
