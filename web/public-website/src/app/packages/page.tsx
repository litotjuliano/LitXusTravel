import type { Metadata } from "next"
import { headers } from "next/headers"
import Navbar from "@/components/layout/Navbar"
import Footer from "@/components/layout/Footer"
import PackagesClient from "@/components/packages/PackagesClient"
import { MOCK_PACKAGES } from "@/lib/mock-data"
import { encryptSlug } from "@/lib/slug"
import type { Package } from "@/types"

export const metadata: Metadata = {
  title: "Packages — LitXusTravel",
  description: "Browse our full collection of curated travel packages.",
}

export default async function PackagesPage() {
  const host = (await headers()).get("host") ?? ""
  const subdomain = host.includes(".") ? host.split(".")[0] : null

  let packages: Package[] = []

  if (subdomain) {
    try {
      const apiBase = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5085/api/v1"
      const res = await fetch(`${apiBase}/public/websites/${subdomain}/packages?pageSize=50`, {
        cache: "no-store",
      })
      if (res.ok) {
        const json = await res.json()
        const items: any[] = json.items ?? json.data?.items ?? []
        packages = items.map((p: any) => ({
          id: p.id,
          title: p.title,
          shortDescription: p.shortDescription ?? "",
          category: p.category ?? "",
          price: p.price,
          currency: p.currency ?? "MYR",
          durationDays: p.durationDays,
          destination: p.destination,
          region: p.region ?? "",
          featuredImageUrl: p.featuredImageUrl ?? "",
          isFeatured: p.isFeatured ?? false,
          isPopular: p.isPopular ?? false,
          isCustomized: false,
          lastSyncedAt: new Date().toISOString(),
          masterPackageId: p.id,
          reviewCount: 0,
        }))
      }
    } catch {
      // API unavailable — show empty
    }
  }

  // Fall back to mock data only on plain localhost (no subdomain context)
  if (packages.length === 0 && !subdomain) {
    packages = MOCK_PACKAGES
  }

  const withSlugs = packages.map((p) => ({ ...p, slug: encryptSlug(p.id) }))

  return (
    <>
      <Navbar />
      <main className="flex-1 bg-gray-50 min-h-screen">
        <section className="bg-(--color-brand-blue) py-14 px-4 sm:px-6">
          <div className="max-w-7xl mx-auto">
            <h1 className="text-3xl sm:text-4xl font-bold text-white mb-2">All Packages</h1>
            <p className="text-white/80">{withSlugs.length} packages available</p>
          </div>
        </section>
        <PackagesClient packages={withSlugs} />
      </main>
      <Footer />
    </>
  )
}
