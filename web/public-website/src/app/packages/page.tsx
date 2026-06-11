import type { Metadata } from "next"
import Navbar from "@/components/layout/Navbar"
import Footer from "@/components/layout/Footer"
import PackagesClient from "@/components/packages/PackagesClient"
import { MOCK_PACKAGES } from "@/lib/mock-data"
import { encryptSlug } from "@/lib/slug"

export const metadata: Metadata = {
  title: "Packages — LitXusTravel",
  description: "Browse our full collection of curated travel packages.",
}

export default function PackagesPage() {
  // Slugs computed server-side — raw numeric IDs never sent to the client
  const packages = MOCK_PACKAGES.map((p) => ({ ...p, slug: encryptSlug(p.id) }))

  return (
    <>
      <Navbar />
      <main className="flex-1 bg-gray-50 min-h-screen">
        <section className="bg-[--color-brand-blue] py-14 px-4 sm:px-6">
          <div className="max-w-7xl mx-auto">
            <h1 className="text-3xl sm:text-4xl font-bold text-white mb-2">All Packages</h1>
            <p className="text-white/80">{packages.length} packages available</p>
          </div>
        </section>
        <PackagesClient packages={packages} />
      </main>
      <Footer />
    </>
  )
}
