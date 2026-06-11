export type PackageStatus = "Draft" | "Published" | "Archived"
export type TenantStatus  = "Active" | "Suspended" | "Trial"
export type InquiryStatus = "New" | "Contacted" | "Quoted" | "Booked" | "Lost"

export interface Package {
  id: string
  title: string
  destination: string
  category?: string
  basePrice: number
  currency: string
  durationDays: number
  visibility: PackageStatus
  isFeatured: boolean
  isPopular: boolean
  syncedTenantsCount: number
  createdAt: string
  updatedAt: string
}

export interface Tenant {
  id: string
  name: string
  slug: string
  subdomain?: string
  contactEmail: string
  contactPhone?: string
  logoUrl?: string
  isActive: boolean
  provisioningStatus: string
  websiteUrl?: string
  plan?: string
  createdAt: string
  syncedPackagesCount?: number
  totalInquiries?: number
}

export interface Inquiry {
  id: string
  tenantId: string
  customerName: string
  customerEmail: string
  customerPhone: string
  message: string
  status: InquiryStatus
  packageTitle?: string
  createdAt: string
  updatedAt: string
}

export interface StatCard {
  label: string
  value: string
  change: string
  positive: boolean
  icon: React.ComponentType<{ size?: number; className?: string }>
}

export interface AuthUser {
  id: string
  email: string
  role: string
  tenantId: string | null
}
