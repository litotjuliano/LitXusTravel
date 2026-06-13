import { useState, useEffect, useRef } from "react"
import { adminApi } from "@/lib/api"
import { getTokenClaims } from "@/lib/utils"

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
  tenants: string[]
  isOwnedPackage?: boolean
  syncSource?: string
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

  // Read token only after mount — localStorage is unavailable during SSR
  const tenantIdRef = useRef<string | undefined>(undefined)
  useEffect(() => {
    tenantIdRef.current = getTokenClaims().tenantId
  }, [])

  const fetchPackages = async () => {
    const tenantId = tenantIdRef.current
    try {
      setLoading(true)

      if (tenantId) {
        // Tenant Admin: fetch own synced packages from /tenants/{id}/packages
        const response = await adminApi.getTenantPackages(tenantId, { page, pageSize })
        const result = response.data
        const items: Package[] = (result.data || []).map((r: {
          id: string; title: string; category?: string; destination: string
          price: number; currency: string; durationDays: number
          isCustomized: boolean; isOwnedPackage: boolean
        }) => ({
          id: r.id,
          title: r.title,
          category: r.category ?? "",
          destination: r.destination,
          basePrice: r.price,
          currency: r.currency,
          durationDays: r.durationDays,
          visibility: r.isOwnedPackage ? "Owned" : r.isCustomized ? "Customized" : "Synced",
          syncedTenantsCount: 0,
          isOwnedPackage: r.isOwnedPackage,
          syncSource: r.syncSource ?? undefined,
        }))
        setPackages(items)
        if (result.pagination) setPagination(result.pagination)
      } else {
        // SuperAdmin / Platform Admin: fetch all master packages
        const response = await adminApi.getPackages({
          page,
          pageSize,
          status: filters?.status,
          sortBy: filters?.sortBy,
          sortOrder: filters?.sortOrder,
        })
        const result = response.data
        const mapped = (result.data || []).map((p: {
          id: string; title: string; category?: string; destination: string
          basePrice: number; currency: string; durationDays: number
          visibility: string; syncedTenantsCount: number; tenants?: string[]
        }) => ({
          ...p,
          category: p.category ?? "",
          tenants: p.tenants ?? [],
        }))
        setPackages(mapped)
        if (result.pagination) setPagination(result.pagination)
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
