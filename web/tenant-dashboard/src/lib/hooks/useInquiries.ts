"use client"

import { useEffect, useState } from "react"
import { tenantApi } from "@/lib/api"
import type { Inquiry, Pagination } from "@/types"

interface Response {
  data: Inquiry[]
  pagination: Pagination
}

export function useInquiries(statusFilter?: string, page = 1, pageSize = 20) {
  const [inquiries, setInquiries] = useState<Inquiry[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState("")
  const [pagination, setPagination] = useState<Pagination | null>(null)

  const fetchInquiries = async () => {
    try {
      setLoading(true)
      const tenantId = localStorage.getItem("litxus_tenant_id")
      if (!tenantId) {
        setError("Not authenticated")
        return
      }

      const params: Record<string, any> = { page, pageSize }
      if (statusFilter) params.status = statusFilter

      const response = await tenantApi.getInquiries(tenantId, params)
      const data = response.data as Response
      setInquiries(data.data)
      setPagination(data.pagination)
      setError("")
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load inquiries")
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchInquiries()
  }, [statusFilter, page, pageSize])

  return { inquiries, loading, error, pagination, refetch: fetchInquiries }
}
