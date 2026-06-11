import { notFound } from "next/navigation"
import type { Metadata } from "next"
import Navbar from "@/components/layout/Navbar"
import Footer from "@/components/layout/Footer"
import PackageDetailContent from "@/components/packages/PackageDetailContent"
import { MOCK_PACKAGES } from "@/lib/mock-data"
import { decryptSlug, encryptSlug } from "@/lib/slug"

interface Props {
  params: Promise<{ slug: string }>
}

function findPackage(slug: string) {
  const id = decryptSlug(slug)
  if (!id) return null
  return MOCK_PACKAGES.find((p) => p.id === id) ?? null
}

export async function generateMetadata({ params }: Props): Promise<Metadata> {
  const { slug } = await params
  const pkg = findPackage(slug)
  if (!pkg) return {}
  return {
    title: `${pkg.title} — LitXusTravel`,
    description: pkg.shortDescription ?? pkg.description,
  }
}

export async function generateStaticParams() {
  return MOCK_PACKAGES.map((p) => ({ slug: encryptSlug(p.id) }))
}

export default async function PackageDetailPage({ params }: Props) {
  const { slug } = await params
  const pkg = findPackage(slug)
  if (!pkg) notFound()

  return (
    <>
      <Navbar />
      <PackageDetailContent pkg={{ ...pkg, slug }} />
      <Footer />
    </>
  )
}
