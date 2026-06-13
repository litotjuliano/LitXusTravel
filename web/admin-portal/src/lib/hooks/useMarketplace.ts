import { useState, useEffect } from "react"
import { adminApi } from "@/lib/api"

export interface MarketplacePackage {
  id: string
  title: string
  category: string | null
  destination: string
  region: string | null
  basePrice: number
  currency: string
  durationDays: number
  featuredImageUrl: string | null
  sourceTenantName: string
}

export const useMarketplace = (tenantId: string | undefined) => {
  const [packages, setPackages] = useState<MarketplacePackage[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const fetchMarketplace = async () => {
    if (!tenantId) return
    try {
      setLoading(true)
      const response = await adminApi.getMarketplacePackages(tenantId)
      setPackages(response.data ?? [])
      setError(null)
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load marketplace")
      setPackages([])
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    if (tenantId) fetchMarketplace()
  }, [tenantId])

  const addToCatalog = async (packageId: string): Promise<boolean> => {
    if (!tenantId) return false
    try {
      await adminApi.addFromMarketplace(tenantId, packageId)
      setPackages(prev => prev.filter(p => p.id !== packageId))
      return true
    } catch {
      return false
    }
  }

  return { packages, loading, error, refetch: fetchMarketplace, addToCatalog }
}
