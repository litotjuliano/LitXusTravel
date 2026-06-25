import axios from "axios"

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
    if (err.response?.status === 401 && typeof window !== "undefined"
        && !err.config?.url?.includes("/auth/login")) {
      localStorage.removeItem("litxus_token")
      window.location.href = "/auth/login"
    }
    if (!err.response) {
      return Promise.reject(new Error("NETWORK_ERROR"))
    }
    const message = err.response.data?.message ?? err.response.data?.errors?.[0] ?? "Something went wrong."
    return Promise.reject(new Error(message))
  }
)

export const adminApi = {
  // Auth
  login: (email: string, password: string) =>
    api.post("/auth/login", { email, password }),
  updateProfile: (firstName: string, lastName: string) =>
    api.put("/auth/profile", { firstName, lastName }),

  // Packages
  getPackages: (params?: object) =>
    api.get("/admin/packages", { params }),
  getPackageById: (id: string) =>
    api.get(`/admin/packages/${id}`),
  uploadPackageImage: (id: string, file: File) => {
    const form = new FormData()
    form.append("file", file)
    return api.post(`/admin/packages/${id}/image`, form, {
      headers: { "Content-Type": "multipart/form-data" },
    })
  },
  createPackage: (data: object) =>
    api.post("/admin/packages", data),
  updatePackage: (id: string, data: object) =>
    api.put(`/admin/packages/${id}`, data),
  deletePackage: (id: string) =>
    api.delete(`/admin/packages/${id}`),
  publishPackage: (id: string) =>
    api.post(`/admin/packages/${id}/publish`),

  // Tenants
  getTenants: (params?: object) =>
    api.get("/admin/tenants", { params }),
  getTenant: (id: string) =>
    api.get(`/admin/tenants/${id}`),
  getTenantSettings: (id: string) =>
    api.get(`/admin/tenants/${id}/settings`),
  updateTenantSettings: (id: string, data: { defaultCurrency: string }) =>
    api.put(`/admin/tenants/${id}/settings`, data),
  createTenant: (data: object) =>
    api.post("/admin/tenants", data),
  assignPlan: (tenantId: string, planName: string) =>
    api.post(`/admin/tenants/${tenantId}/assign-plan`, { planName }),
  processMockPayment: (tenantId: string, data: {
    planName: string; cardholderName: string;
    cardNumber: string; expiryDate: string; cvv: string;
  }) =>
    api.post(`/admin/tenants/${tenantId}/process-mock-payment`, data),

  // Subscription Plans
  getSubscriptionPlans: () =>
    api.get("/admin/subscription-plans"),
  createSubscriptionPlan: (data: { name: string; price: number; maxPackages: number; maxTeamMembers: number }) =>
    api.post("/admin/subscription-plans", data),
  updateSubscriptionPlan: (id: string, data: { name: string; price: number; maxPackages: number; maxTeamMembers: number }) =>
    api.put(`/admin/subscription-plans/${id}`, data),
  deleteSubscriptionPlan: (id: string) =>
    api.delete(`/admin/subscription-plans/${id}`),

  // Inquiries (tenant-scoped)
  getInquiries: (tenantId: string, params?: object) =>
    api.get(`/tenants/${tenantId}/inquiries`, { params }),
  getInquiryDetail: (tenantId: string, inquiryId: string) =>
    api.get(`/tenants/${tenantId}/inquiries/${inquiryId}`),
  updateInquiryStatus: (tenantId: string, inquiryId: string, status: string) =>
    api.put(`/tenants/${tenantId}/inquiries/${inquiryId}`, { status }),
  getInquiryStats: () =>
    api.get("/admin/inquiries/stats"),

  // Tenant package creation
  createTenantPackage: (tenantId: string, data: object) =>
    api.post(`/tenants/${tenantId}/packages`, data),

  // Package Sync (tenant-scoped)
  syncPackagesToTenant: (tenantId: string, masterPackageIds: string[]) =>
    api.post(`/admin/tenants/${tenantId}/packages/sync`, { masterPackageIds }),
  getTenantPackages: (tenantId: string, params?: object) =>
    api.get(`/tenants/${tenantId}/packages`, { params }),
  updatePackageOverride: (tenantId: string, tenantPackageId: string, data: object) =>
    api.put(`/tenants/${tenantId}/packages/${tenantPackageId}/override`, data),
  unsyncPackage: (tenantId: string, tenantPackageId: string) =>
    api.delete(`/admin/tenants/${tenantId}/packages/${tenantPackageId}`),
  getMarketplacePackages: (tenantId: string) =>
    api.get(`/tenants/${tenantId}/marketplace`),
  addFromMarketplace: (tenantId: string, packageId: string) =>
    api.post(`/tenants/${tenantId}/marketplace/${packageId}/add`),
  generatePackagePhoto: (tenantId: string, pkgId: string) =>
    api.post<{ featuredImageUrl: string }>(`/tenants/${tenantId}/packages/${pkgId}/generate-photo`),
  generateAdminPackagePhoto: (pkgId: string) =>
    api.post<{ featuredImageUrl: string }>(`/admin/packages/${pkgId}/generate-photo`),
  publishTenantPackage: (tenantId: string, packageId: string) =>
    api.post(`/tenants/${tenantId}/packages/${packageId}/publish`),

  // Notifications
  getNotifications: () =>
    api.get<{ data: NotificationItem[]; unreadCount: number }>("/admin/notifications"),
  markNotificationRead: (id: string) =>
    api.patch(`/admin/notifications/${id}/read`),
  markAllNotificationsRead: () =>
    api.patch("/admin/notifications/read-all"),

  // Billing
  getInvoices: (tenantId?: string) =>
    api.get<InvoicesResponse>(`/admin/billing/invoices${tenantId ? `?tenantId=${tenantId}` : ""}`),

  // Subscription notifications (manual trigger)
  sendSubscriptionNotification: (tenantId: string, type: "expiring_soon" | "grace_period" | "fully_expired") =>
    api.post(`/admin/tenants/${tenantId}/send-subscription-notification`, { type }),
}

export interface NotificationItem {
  id: string
  title: string
  message: string
  isRead: boolean
  createdAt: string
}

export interface InvoiceDto {
  id: string
  invoiceNumber: string
  tenantName: string
  tenantId: string
  planName: string
  amount: number
  period: string
  status: "Paid" | "Pending" | "Failed"
  date: string
}

export interface InvoicesResponse {
  data: InvoiceDto[]
  mrrCurrentMonth: number
  totalPaid: number
  totalPending: number
  totalFailed: number
}
