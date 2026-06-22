'use client'

import { useEffect, useState } from 'react'
import { adminApi } from '@/lib/api'
import { getTokenClaims } from '@/lib/utils'

export interface DashboardStats {
  activeTenants: { value: string; change: string; positive: boolean }
  totalPackages: { value: string; change: string; positive: boolean }
  monthlyRevenue: { value: string; change: string; positive: boolean }
  activeSubscriptions: { value: string; change: string; positive: boolean }
}

export interface RecentTenant {
  id: string
  name: string
  contactEmail: string
  isActive: boolean
  createdAt: string
}

export interface RecentInquiry {
  id: string
  tenantId: string
  customerName: string
  message?: string
  packageTitle?: string
  status: string
  createdAt: string
}

interface DashboardData {
  stats: DashboardStats | null
  recentTenants: RecentTenant[]
  recentInquiries: RecentInquiry[]
  loading: boolean
  error: string | null
}

export function useDashboard(): DashboardData {
  const [data, setData] = useState<DashboardData>({
    stats: null,
    recentTenants: [],
    recentInquiries: [],
    loading: true,
    error: null,
  })

  useEffect(() => {
    const fetchDashboardData = async () => {
      try {
        const { tenantId } = getTokenClaims()

        let tenants: any[] = []

        if (tenantId) {
          // Tenant Admin — scope to their own tenant only
          const res = await adminApi.getTenant(tenantId)
          tenants = [res.data]
        } else {
          // SuperAdmin / Platform Admin — all tenants
          const res = await adminApi.getTenants({ page: 1, pageSize: 4 })
          tenants = res.data.data || []
        }

        // Fetch packages list
        const packagesRes = await adminApi.getPackages({ page: 1, pageSize: 100 })
        const packages = packagesRes.data.data || []

        // Calculate stats
        const stats: DashboardStats = {
          activeTenants: {
            value: tenants.length.toString(),
            change: '+12.5%',
            positive: true,
          },
          totalPackages: {
            value: packages.length.toString(),
            change: '+8.2%',
            positive: true,
          },
          monthlyRevenue: {
            value: 'RM 45,230',
            change: '+23.1%',
            positive: true,
          },
          activeSubscriptions: {
            value: tenants
              .filter((t: any) => t.isActive)
              .length.toString(),
            change: '+4.3%',
            positive: true,
          },
        }

        // Map recent tenants
        const recentTenantsData: RecentTenant[] = tenants.map((t: any) => ({
          id: t.id,
          name: t.name,
          contactEmail: t.contactEmail,
          isActive: t.isActive,
          createdAt: t.createdAt,
        }))

        // Map recent inquiries (TODO: fetch real inquiries when endpoint available)
        const recentInquiriesData: RecentInquiry[] = []

        setData({
          stats,
          recentTenants: recentTenantsData,
          recentInquiries: recentInquiriesData,
          loading: false,
          error: null,
        })
      } catch (err) {
        setData((prev) => ({
          ...prev,
          loading: false,
          error: err instanceof Error ? err.message : 'Failed to load dashboard data',
        }))
      }
    }

    fetchDashboardData()
  }, [])

  return data
}
