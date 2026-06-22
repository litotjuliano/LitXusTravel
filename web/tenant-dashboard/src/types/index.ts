import type { LucideIcon } from "lucide-react"

export type InquiryStatus = "New" | "Contacted" | "Quoted" | "Booked" | "Lost"

export interface StatCard {
  label: string
  value: string
  change: string
  positive: boolean
  icon: LucideIcon
}

export interface ResolvedPackage {
  id: string
  masterPackageId: string
  title: string
  destination: string
  description?: string
  shortDescription?: string
  price: number
  currency: string
  durationDays: number
  featuredImageUrl?: string
  imagesJson?: string
  itineraryJson?: string
  highlightsJson?: string
  inclusionsJson?: string
  exclusionsJson?: string
  contactPhone?: string
  contactWhatsapp?: string
  isCustomized: boolean
  syncedAt: string
}

export interface TenantPackageOverrideInput {
  Title?: string
  Price?: number
  Currency?: string
  FeaturedImageUrl?: string
  Description?: string
  ShortDescription?: string
  ContactPhone?: string
  ContactWhatsapp?: string
}

export interface Inquiry {
  id: string
  customerName: string
  customerEmail: string
  customerPhone: string
  message: string
  status: InquiryStatus
  tenantPackageId?: string
  createdAt: string
  updatedAt: string
}

export interface InquiryStats {
  statusBreakdown: Record<string, number>
  totalInquiries: number
  conversionRate: number
}

export interface Pagination {
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}
