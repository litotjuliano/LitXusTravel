import type { Package, Tenant, Inquiry } from "@/types"

export const MOCK_PACKAGES: Package[] = [
  { id: "1", title: "5D4N Japan Sakura Tour", destination: "Japan", category: "Cultural", basePrice: 4999, currency: "RM", durationDays: 5, visibility: "Published", isFeatured: true, isPopular: true, syncedTenantsCount: 12, createdAt: "2026-01-10", updatedAt: "2026-05-01" },
  { id: "2", title: "7D6N Bali Family Escape", destination: "Bali", category: "Beach", basePrice: 3299, currency: "RM", durationDays: 7, visibility: "Published", isFeatured: true, isPopular: false, syncedTenantsCount: 8, createdAt: "2026-01-15", updatedAt: "2026-04-20" },
  { id: "3", title: "10D9N Europe Grand Tour", destination: "Europe", category: "City", basePrice: 8999, currency: "RM", durationDays: 10, visibility: "Published", isFeatured: false, isPopular: true, syncedTenantsCount: 15, createdAt: "2026-02-01", updatedAt: "2026-05-10" },
  { id: "4", title: "4D3N Maldives Overwater Retreat", destination: "Maldives", category: "Beach", basePrice: 5599, currency: "RM", durationDays: 4, visibility: "Published", isFeatured: true, isPopular: true, syncedTenantsCount: 5, createdAt: "2026-02-15", updatedAt: "2026-04-30" },
  { id: "5", title: "6D5N South Korea Discovery", destination: "South Korea", category: "Cultural", basePrice: 3799, currency: "RM", durationDays: 6, visibility: "Draft", isFeatured: false, isPopular: false, syncedTenantsCount: 0, createdAt: "2026-03-01", updatedAt: "2026-05-15" },
  { id: "6", title: "3D2N Cameron Highlands Retreat", destination: "Cameron Highlands", category: "Nature", basePrice: 699, currency: "RM", durationDays: 3, visibility: "Archived", isFeatured: false, isPopular: false, syncedTenantsCount: 3, createdAt: "2026-01-05", updatedAt: "2026-03-10" },
]

export const MOCK_TENANTS: Tenant[] = [
  { id: "t1", name: "TravelPro Agency", slug: "travelpro", subdomain: "travelpro", contactEmail: "hello@travelpro.com", contactPhone: "+60 11-1234 5678", isActive: true, provisioningStatus: "Completed", websiteUrl: "https://travelpro.litxustravel.com", plan: "Pro", createdAt: "2026-01-20", syncedPackagesCount: 8, totalInquiries: 45 },
  { id: "t2", name: "Wanderlust Travels", slug: "wanderlust", subdomain: "wanderlust", contactEmail: "info@wanderlust.com", contactPhone: "+60 12-8765 4321", isActive: true, provisioningStatus: "Completed", websiteUrl: "https://wanderlust.litxustravel.com", plan: "Starter", createdAt: "2026-02-05", syncedPackagesCount: 4, totalInquiries: 23 },
  { id: "t3", name: "Globe Hoppers", slug: "globehoppers", subdomain: "globehoppers", contactEmail: "admin@globehoppers.com", isActive: false, provisioningStatus: "Completed", plan: "Pro", createdAt: "2026-02-18", syncedPackagesCount: 10, totalInquiries: 67 },
  { id: "t4", name: "Sunrise Tours", slug: "sunrise", subdomain: "sunrise", contactEmail: "tours@sunrise.com", isActive: true, provisioningStatus: "Pending", plan: "Trial", createdAt: "2026-05-20", syncedPackagesCount: 0, totalInquiries: 0 },
]

export const MOCK_INQUIRIES: Inquiry[] = [
  { id: "i1", tenantId: "t1", customerName: "Ahmad Razif", customerEmail: "ahmad@email.com", customerPhone: "+60 11-2222 3333", message: "Interested in Japan tour for family of 4", status: "New", packageTitle: "5D4N Japan Sakura Tour", createdAt: "2026-05-28", updatedAt: "2026-05-28" },
  { id: "i2", tenantId: "t1", customerName: "Sarah Lim", customerEmail: "sarah@email.com", customerPhone: "+60 12-3333 4444", message: "Looking for Maldives honeymoon package", status: "Quoted", packageTitle: "4D3N Maldives Overwater Retreat", createdAt: "2026-05-25", updatedAt: "2026-05-27" },
  { id: "i3", tenantId: "t2", customerName: "Jennifer Tan", customerEmail: "jennifer@email.com", customerPhone: "+60 13-4444 5555", message: "Need Europe tour for 2 pax", status: "Contacted", packageTitle: "10D9N Europe Grand Tour", createdAt: "2026-05-22", updatedAt: "2026-05-23" },
]

export const STATS = {
  activeTenants: { value: "1,234", change: "+12.5%", positive: true },
  totalPackages: { value: "5,678", change: "+8.2%",  positive: true },
  monthlyRevenue:{ value: "RM 45,230", change: "+23.1%", positive: true },
  activeSubscriptions: { value: "891", change: "+4.3%", positive: true },
}

export const REVENUE_CHART = [
  { month: "Jan", revenue: 32000 },
  { month: "Feb", revenue: 28000 },
  { month: "Mar", revenue: 35000 },
  { month: "Apr", revenue: 38000 },
  { month: "May", revenue: 45230 },
]
