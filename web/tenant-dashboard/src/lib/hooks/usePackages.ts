"use client"

import { useEffect, useState } from "react"
import { tenantApi } from "@/lib/api"
import type { ResolvedPackage, Pagination } from "@/types"

interface Response {
  data: ResolvedPackage[]
  pagination: Pagination
}

export function usePackages(page = 1, pageSize = 20) {
  const [packages, setPackages] = useState<ResolvedPackage[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState("")
  const [pagination, setPagination] = useState<Pagination | null>(null)

  const fetchPackages = async () => {
    try {
      setLoading(true)
      const tenantId = localStorage.getItem("litxus_tenant_id")
      if (!tenantId) {
        setError("Not authenticated")
        return
      }

      const response = await tenantApi.getPackages(tenantId, { page, pageSize })
      const data = response.data as Response
      setPackages(data.data)
      setPagination(data.pagination)
      setError("")
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load packages")
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchPackages()
  }, [page, pageSize])

  return { packages, loading, error, pagination, refetch: fetchPackages }
}
