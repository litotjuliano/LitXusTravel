import { headers } from "next/headers"
import Navbar from "@/components/layout/Navbar"
import Footer from "@/components/layout/Footer"
import HeroSection from "@/components/sections/HeroSection"
import FeaturedPackages from "@/components/sections/FeaturedPackages"
import PopularDestinations from "@/components/sections/PopularDestinations"
import WhatsAppCTA from "@/components/sections/WhatsAppCTA"
import TestimonialsSection from "@/components/sections/TestimonialsSection"
import { MOCK_PACKAGES } from "@/lib/mock-data"
import { encryptSlug } from "@/lib/slug"
import type { Package } from "@/types"

export default async function HomePage() {
  const host = (await headers()).get("host") ?? ""
  const subdomain = host.includes(".") ? host.split(".")[0] : null

  let featured: Package[] = []

  if (subdomain) {
    try {
      const apiBase = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5085/api/v1"
      const res = await fetch(`${apiBase}/public/websites/${subdomain}/packages?pageSize=6`, {
        cache: "no-store",
      })
      if (res.ok) {
        const json = await res.json()
        const items: any[] = json.items ?? json.data?.items ?? []
        featured = items.map((p: any) => ({
          id: p.id,
          slug: encryptSlug(p.id),
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
      // API unavailable — fall through to mock
    }
  }

  // Fall back to mock data only on plain localhost (no subdomain context)
  if (featured.length === 0 && !subdomain) {
    featured = MOCK_PACKAGES
      .filter((p) => p.isFeatured || p.isPopular)
      .map((p) => ({ ...p, slug: encryptSlug(p.id) }))
  }

  return (
    <>
      <Navbar />
      <main className="flex-1">
        <HeroSection />
        <FeaturedPackages packages={featured} />
        <PopularDestinations />
        <WhatsAppCTA />
        <TestimonialsSection />
      </main>
      <Footer />
    </>
  )
}
