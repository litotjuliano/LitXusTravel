import { useState, useEffect, useCallback } from "react"
import { adminApi } from "@/lib/api"

export interface SubscriptionPlan {
  id: string
  name: string
  price: number
  maxPackages: number
  maxTeamMembers: number
  isActive: boolean
  createdAt: string
  activeTenantCount: number
}

export function useSubscriptionPlans() {
  const [plans, setPlans] = useState<SubscriptionPlan[]>([])
  const [loading, setLoading] = useState(true)

  const refetch = useCallback(() => {
    setLoading(true)
    adminApi.getSubscriptionPlans()
      .then((res) => setPlans(res.data.data ?? []))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => {
    refetch()
  }, [refetch])

  return { plans, loading, refetch }
}
