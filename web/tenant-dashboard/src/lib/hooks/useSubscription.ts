"use client"

import { useEffect, useState } from "react"
import { tenantApi } from "@/lib/api"

export interface SubscriptionStatus {
  planName: string
  monthlyPrice: number
  status: string
  subscriptionHealth: string
  startDate: string
  endDate: string | null
  daysRemaining: number | null
  maxPackages: number
  maxTeamMembers: number
  autoRenew: boolean
  isInGracePeriod: boolean
  isReadOnly: boolean
  gracePeriodEndsAt: string | null
  gracePeriodDaysRemaining: number | null
}

export function useSubscription() {
  const [subscription, setSubscription] = useState<SubscriptionStatus | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const fetchSubscription = async () => {
      try {
        const tenantId = localStorage.getItem("litxus_tenant_id")
        if (!tenantId) { setLoading(false); return }
        const response = await tenantApi.getSubscriptionStatus(tenantId)
        setSubscription(response.data)
      } catch {
        // Silent fail — banner simply won't render if subscription can't be loaded
      } finally {
        setLoading(false)
      }
    }

    fetchSubscription()
  }, [])

  return { subscription, loading }
}
