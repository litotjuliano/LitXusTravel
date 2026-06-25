import { useState, useEffect, useCallback } from "react"
import { adminApi } from "@/lib/api"

export interface Tenant {
  id: string
  name: string
  contactEmail: string
  subdomain: string | null
  plan: string | null
  syncedPackagesCount: number
  isActive: boolean
  createdAt: string
  subscriptionHealth: string | null
  daysRemaining: number | null
  subscriptionEndDate: string | null
  subscriptionStartDate: string | null
}

export interface UseTenantsResult {
  tenants: Tenant[]
  loading: boolean
  error: string | null
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
    hasNextPage: boolean
    hasPreviousPage: boolean
  }
  refetch: () => void
}

export const useTenants = (
  page = 1,
  pageSize = 20,
  filters?: { status?: string; sortBy?: string; sortOrder?: string }
): UseTenantsResult => {
  const [tenants, setTenants] = useState<Tenant[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [refreshKey, setRefreshKey] = useState(0)
  const [pagination, setPagination] = useState({
    page,
    pageSize,
    totalCount: 0,
    totalPages: 0,
    hasNextPage: false,
    hasPreviousPage: false,
  })

  useEffect(() => {
    const fetchTenants = async () => {
      try {
        setLoading(true)
        const response = await adminApi.getTenants({
          page,
          pageSize,
          status: filters?.status,
          sortBy: filters?.sortBy,
          sortOrder: filters?.sortOrder,
        })

        const result = response.data
        setTenants(result.data || [])
        if (result.pagination) {
          setPagination(result.pagination)
        }
        setError(null)
      } catch (err) {
        const message = err instanceof Error ? err.message : "Failed to fetch tenants"
        setError(message)
        setTenants([])
      } finally {
        setLoading(false)
      }
    }

    fetchTenants()
  }, [page, pageSize, filters?.status, filters?.sortBy, filters?.sortOrder, refreshKey])

  const refetch = useCallback(() => setRefreshKey((k) => k + 1), [])

  return { tenants, loading, error, pagination, refetch }
}
