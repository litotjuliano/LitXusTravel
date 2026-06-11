import axios from "axios"

const API_BASE = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5084/api/v1"

export const api = axios.create({
  baseURL: API_BASE,
  headers: { "Content-Type": "application/json" },
  timeout: 15_000,
})

api.interceptors.response.use(
  (res) => res,
  (err) => {
    const message =
      err.response?.data?.message ??
      err.response?.data?.errors?.[0] ??
      "Something went wrong. Please try again."
    return Promise.reject(new Error(message))
  }
)

/* ── Public endpoints ──────────────────────────────────────────── */
const subdomain = () =>
  typeof window !== "undefined"
    ? window.location.hostname.split(".")[0]
    : "demo"

export const publicApi = {
  getWebsite: () =>
    api.get(`/public/websites/${subdomain()}`),

  getPackages: (params?: { category?: string; destination?: string }) =>
    api.get(`/public/websites/${subdomain()}/packages`, { params }),

  getPackage: (id: string) =>
    api.get(`/public/websites/${subdomain()}/packages/${id}`),

  submitInquiry: (data: {
    customerName: string
    customerEmail: string
    customerPhone: string
    message: string
    masterPackageId?: string
    numberOfPax?: number
    preferredTravelDates?: string
  }) => api.post(`/public/websites/${subdomain()}/inquiries`, data),
}
