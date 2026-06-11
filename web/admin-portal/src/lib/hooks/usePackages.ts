import { useState, useEffect } from "react"
import { adminApi } from "@/lib/api"

export interface Package {
  id: string
  title: string
  category: string
  destination: string
  basePrice: number
  currency: string
  durationDays: number
  visibility: string
  syncedTenantsCount: number
}

export interface UsePackagesResult {
  packages: Package[]
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
  refetch: () => Promise<void>
}

export const usePackages = (
  page = 1,
  pageSize = 20,
  filters?: { status?: string; sortBy?: string; sortOrder?: string }
): UsePackagesResult => {
  const [packages, setPackages] = useState<Package[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [pagination, setPagination] = useState({
    page,
    pageSize,
    totalCount: 0,
    totalPages: 0,
    hasNextPage: false,
    hasPreviousPage: false,
  })

  const fetchPackages = async () => {
    try {
      setLoading(true)
      const response = await adminApi.getPackages({
        page,
        pageSize,
        status: filters?.status,
        sortBy: filters?.sortBy,
        sortOrder: filters?.sortOrder,
      })

      const result = response.data
      setPackages(result.data || [])
      if (result.pagination) {
        setPagination(result.pagination)
      }
      setError(null)
    } catch (err) {
      const message = err instanceof Error ? err.message : "Failed to fetch packages"
      setError(message)
      setPackages([])
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchPackages()
  }, [page, pageSize, filters?.status, filters?.sortBy, filters?.sortOrder])

  return { packages, loading, error, pagination, refetch: fetchPackages }
}
