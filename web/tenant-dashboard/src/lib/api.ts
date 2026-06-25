import axios from "axios"
import { toast } from "sonner"

const API_BASE = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5085/api/v1"

export const api = axios.create({
  baseURL: API_BASE,
  headers: { "Content-Type": "application/json" },
  timeout: 15_000,
})

// Attach JWT from localStorage on every request
api.interceptors.request.use((config) => {
  if (typeof window !== "undefined") {
    const token = localStorage.getItem("litxus_token")
    if (token) config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401 && typeof window !== "undefined") {
      localStorage.removeItem("litxus_token")
      localStorage.removeItem("litxus_tenant_id")
      localStorage.removeItem("litxus_user_email")
      window.location.href = "/auth/login"
    }
    if (err.response?.status === 402) {
      const description = err.response.data?.message
        ?? "Your subscription has expired. This action is disabled in read-only mode."
      toast.error("Access Restricted", { description })
      return Promise.reject(new Error(description))
    }
    if (!err.response) return Promise.reject(new Error("NETWORK_ERROR"))
    const message = err.response?.data?.message ?? err.response?.data?.errors?.[0] ?? "Something went wrong."
    return Promise.reject(new Error(message))
  }
)

export const tenantApi = {
  // Auth
  login: (email: string, password: string) =>
    api.post("/auth/login", { email, password }),

  // Packages (tenant-scoped)
  getPackages: (tenantId: string, params?: object) =>
    api.get(`/tenants/${tenantId}/packages`, { params }),
  syncPackages: (tenantId: string, masterPackageIds: string[]) =>
    api.post(`/tenants/${tenantId}/packages/sync`, { masterPackageIds }),
  overridePackage: (tenantId: string, packageId: string, data: object) =>
    api.put(`/tenants/${tenantId}/packages/${packageId}/override`, data),
  unsyncPackage: (tenantId: string, packageId: string) =>
    api.delete(`/tenants/${tenantId}/packages/${packageId}`),

  // Inquiries (tenant-scoped)
  getInquiries: (tenantId: string, params?: object) =>
    api.get(`/tenants/${tenantId}/inquiries`, { params }),
  updateInquiryStatus: (tenantId: string, inquiryId: string, status: string) =>
    api.put(`/tenants/${tenantId}/inquiries/${inquiryId}`, { status }),
  getInquiryStats: (tenantId: string) =>
    api.get(`/tenants/${tenantId}/inquiries/stats`),

  // Commission
  getCommissionStatement: (tenantId: string, agentId: string, from?: string, to?: string) =>
    api.get(`/tenants/${tenantId}/commission-payouts/statement/${agentId}`, { params: { from, to } }),
  getCommissionRules: (tenantId: string, agentId?: string) =>
    api.get(`/tenants/${tenantId}/commission-rules`, { params: { agentId } }),
  configureCommissionRule: (tenantId: string, data: object) =>
    api.post(`/tenants/${tenantId}/commission-rules`, data),

  // Staff Agents
  getStaffAgents: (tenantId: string) =>
    api.get(`/tenants/${tenantId}/staff-agents`),
  createStaffAgent: (tenantId: string, data: object) =>
    api.post(`/tenants/${tenantId}/staff-agents`, data),
  rotateStaffAgentCode: (tenantId: string, agentId: string) =>
    api.post(`/tenants/${tenantId}/staff-agents/${agentId}/rotate-code`),

  // Tours
  getTours: (tenantId: string) =>
    api.get(`/tenants/${tenantId}/tours`),
  createTour: (tenantId: string, data: object) =>
    api.post(`/tenants/${tenantId}/tours`, data),
  completeTour: (tenantId: string, tourId: string) =>
    api.post(`/tenants/${tenantId}/tours/${tourId}/complete`),

  // Bookings
  getBookings: (tenantId: string, params?: object) =>
    api.get(`/tenants/${tenantId}/bookings`, { params }),
  createBooking: (tenantId: string, data: object) =>
    api.post(`/tenants/${tenantId}/bookings`, data),
  cancelBooking: (tenantId: string, bookingId: string, reason?: string) =>
    api.post(`/tenants/${tenantId}/bookings/${bookingId}/cancel`, { reason }),

  // Subscription
  getSubscriptionStatus: (tenantId: string) =>
    api.get(`/tenants/${tenantId}/subscription`),
}
