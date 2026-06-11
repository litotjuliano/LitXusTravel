"use client"

import { useState, useEffect } from "react"
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
}

export function useSettings(tenantId: string | undefined) {
  const [settings, setSettings] = useState<TenantSettings | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!tenantId) {
      setLoading(false)
      return
    }

    const fetchSettings = async () => {
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
    }

    fetchSettings()
  }, [tenantId])

  return { settings, loading, error }
}
