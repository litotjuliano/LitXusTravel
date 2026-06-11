import Navbar from "@/components/layout/Navbar"
import Footer from "@/components/layout/Footer"
import HeroSection from "@/components/sections/HeroSection"
import FeaturedPackages from "@/components/sections/FeaturedPackages"
import PopularDestinations from "@/components/sections/PopularDestinations"
import WhatsAppCTA from "@/components/sections/WhatsAppCTA"
import TestimonialsSection from "@/components/sections/TestimonialsSection"
import { MOCK_PACKAGES } from "@/lib/mock-data"
import { encryptSlug } from "@/lib/slug"

export default function HomePage() {
  const featured = MOCK_PACKAGES
    .filter((p) => p.isFeatured || p.isPopular)
    .map((p) => ({ ...p, slug: encryptSlug(p.id) }))

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
