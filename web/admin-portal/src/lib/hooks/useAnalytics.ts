"use client"

import { useState, useEffect } from "react"
import { adminApi } from "@/lib/api"

interface AnalyticsData {
  statusBreakdown: Record<string, number>
  totalInquiries: number
  conversionRate: number
}

export function useAnalytics() {
  const [data, setData] = useState<AnalyticsData | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const fetchAnalytics = async () => {
      try {
        setLoading(true)
        const response = await adminApi.getInquiryStats()
        setData(response.data)
        setError(null)
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to fetch analytics")
        setData(null)
      } finally {
        setLoading(false)
      }
    }

    fetchAnalytics()
  }, [])

  return { data, loading, error }
}
