"use client"

import { useState, useEffect, useCallback } from "react"
import { adminApi } from "@/lib/api"

interface TenantSettings {
  tenantId: string
  name: string
  contactEmail: string | null
  contactPhone: string | null
  isActive: boolean
  provisioningStatus: string
  country: string | null
  createdAt: string
  defaultCurrency: string
}

export function useSettings(tenantId: string | undefined) {
  const [settings, setSettings] = useState<TenantSettings | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const fetchSettings = useCallback(async () => {
    if (!tenantId) {
      setLoading(false)
      return
    }
    try {
      setLoading(true)
      const response = await adminApi.getTenantSettings(tenantId)
      setSettings(response.data)
      setError(null)
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to fetch settings")
      setSettings(null)
    } finally {
      setLoading(false)
    }
  }, [tenantId])

  useEffect(() => {
    fetchSettings()
  }, [fetchSettings])

  return { settings, loading, error, refetch: fetchSettings }
}
