"use client"

import { useEffect, useState } from "react"
import { tenantApi } from "@/lib/api"
import type { InquiryStats } from "@/types"

export function useStats() {
  const [stats, setStats] = useState<InquiryStats | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState("")

  useEffect(() => {
    const fetchStats = async () => {
      try {
        setLoading(true)
        const tenantId = localStorage.getItem("litxus_tenant_id")
        if (!tenantId) {
          setError("Not authenticated")
          return
        }

        const response = await tenantApi.getInquiryStats(tenantId)
        setStats(response.data)
        setError("")
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to load stats")
      } finally {
        setLoading(false)
      }
    }

    fetchStats()
  }, [])

  return { stats, loading, error }
}
