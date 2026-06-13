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
    const token = localStorage.getItem("nexus_token")
    if (token) config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401 && typeof window !== "undefined") {
      localStorage.removeItem("nexus_token")
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
}
