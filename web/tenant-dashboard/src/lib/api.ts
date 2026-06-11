import axios from "axios"

const API_BASE = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5084/api/v1"

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
      localStorage.removeItem("nexus_tenant_id")
      localStorage.removeItem("nexus_user_email")
      window.location.href = "/auth/login"
    }
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
}
