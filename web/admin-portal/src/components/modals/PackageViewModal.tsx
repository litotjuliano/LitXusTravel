"use client"

import { useEffect, useState } from "react"
import { Modal } from "@/components/ui/Modal"
import { adminApi } from "@/lib/api"
import { formatCurrency } from "@/lib/utils"
import {
  Loader2, MapPin, Clock, Star, CheckCircle2, XCircle,
  ChevronDown, ChevronUp, ImageOff, Calendar, Users,
} from "lucide-react"

interface PackageDetail {
  id: string
  title: string
  description?: string
  shortDescription?: string
  category?: string
  basePrice: number
  currency: string
  durationDays: number
  destination: string
  region?: string
  featuredImageUrl?: string
  imagesJson?: string
  itineraryJson?: string
  highlightsJson?: string
  inclusionsJson?: string
  exclusionsJson?: string
  rating?: number
  reviewCount?: number
  visibility: string
  isPopular?: boolean
  isFeatured?: boolean
  syncedTenantsCount?: number
  createdAt?: string
  updatedAt?: string
}

interface PreloadedPackage {
  id: string
  title: string
  description?: string
  shortDescription?: string
  category?: string
  basePrice: number
  currency: string
  durationDays: number
  destination: string
  region?: string
  featuredImageUrl?: string
  imagesJson?: string
  itineraryJson?: string
  highlightsJson?: string
  inclusionsJson?: string
  exclusionsJson?: string
  visibility?: string
}

interface Props {
  packageId: string | null
  packageData?: PreloadedPackage
  open: boolean
  onOpenChange: (open: boolean) => void
}

function parseJson<T>(json: string | null | undefined, fallback: T): T {
  if (!json) return fallback
  try { return JSON.parse(json) } catch { return fallback }
}

interface ItineraryDay { day: number; title?: string; description?: string }

export function PackageViewModal({ packageId, packageData, open, onOpenChange }: Props) {
  const [pkg, setPkg] = useState<PackageDetail | null>(null)
  const [loading, setLoading] = useState(false)
  const [showMore, setShowMore] = useState(false)

  useEffect(() => {
    if (!open) return
    setShowMore(false)

    if (packageData) {
      setPkg({
        id: packageData.id,
        title: packageData.title,
        description: packageData.description,
        shortDescription: packageData.shortDescription,
        category: packageData.category,
        basePrice: packageData.basePrice,
        currency: packageData.currency,
        durationDays: packageData.durationDays,
        destination: packageData.destination,
        region: packageData.region,
        featuredImageUrl: packageData.featuredImageUrl,
        imagesJson: packageData.imagesJson,
        itineraryJson: packageData.itineraryJson,
        highlightsJson: packageData.highlightsJson,
        inclusionsJson: packageData.inclusionsJson,
        exclusionsJson: packageData.exclusionsJson,
        visibility: packageData.visibility ?? "",
      })
      return
    }

    if (!packageId) return
    setLoading(true)
    setPkg(null)
    adminApi.getPackageById(packageId)
      .then((res) => setPkg(res.data))
      .catch(() => setPkg(null))
      .finally(() => setLoading(false))
  }, [open, packageId, packageData])

  const highlights = parseJson<string[]>(pkg?.highlightsJson, [])
  const inclusions = parseJson<string[]>(pkg?.inclusionsJson, [])
  const exclusions = parseJson<string[]>(pkg?.exclusionsJson, [])
  const itinerary  = parseJson<ItineraryDay[]>(pkg?.itineraryJson, [])
  const images     = parseJson<string[]>(pkg?.imagesJson, [])
  const heroImage  = pkg?.featuredImageUrl || images[0]

  const hasExtras = highlights.length > 0 || inclusions.length > 0 || exclusions.length > 0 || itinerary.length > 0

  return (
    <Modal
      isOpen={open}
      onClose={() => onOpenChange(false)}
      showCloseButton={true}
      className="rounded-3xl max-w-sm p-0 overflow-hidden shadow-2xl"
    >
      {/* ── Hero image ── */}
      <div className="relative h-52 bg-gray-100 dark:bg-gray-800 overflow-hidden">
        {loading ? (
          <div className="absolute inset-0 flex items-center justify-center">
            <Loader2 className="animate-spin text-gray-400 dark:text-gray-500" size={28} />
          </div>
        ) : heroImage ? (
          // eslint-disable-next-line @next/next/no-img-element
          <img src={heroImage} alt={pkg?.title} className="w-full h-full object-cover" />
        ) : (
          <div className="w-full h-full flex flex-col items-center justify-center gap-2">
            <ImageOff size={36} className="text-gray-300 dark:text-gray-600" />
            <p className="text-xs text-gray-400 dark:text-gray-500">No image</p>
          </div>
        )}

        {/* Gradient overlay */}
        {!loading && <div className="absolute inset-0 bg-gradient-to-t from-black/30 via-transparent to-transparent" />}

        {/* Badges */}
        {pkg?.isFeatured && (
          <span className="absolute top-3 right-3 px-2.5 py-1 text-[11px] font-semibold rounded-full bg-white/90 text-gray-800 shadow pointer-events-none">
            Featured
          </span>
        )}
        {pkg?.isPopular && !pkg?.isFeatured && (
          <span className="absolute top-3 right-3 px-2.5 py-1 text-[11px] font-semibold rounded-full bg-orange-400 text-white shadow pointer-events-none">
            Popular
          </span>
        )}
      </div>

      {/* ── Card body ── */}
      {!loading && pkg && (
        <div className="p-5 space-y-3">

          {/* Destination + Rating row */}
          <div className="flex items-center justify-between">
            <span className="flex items-center gap-1 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">
              <MapPin size={12} />
              {pkg.destination}
            </span>
            {pkg.rating ? (
              <span className="flex items-center gap-1 text-sm font-medium text-gray-900 dark:text-white">
                <Star size={13} className="text-yellow-400 fill-yellow-400" />
                {pkg.rating.toFixed(1)}
                {pkg.reviewCount ? (
                  <span className="text-xs text-gray-500 dark:text-gray-400">({pkg.reviewCount})</span>
                ) : null}
              </span>
            ) : null}
          </div>

          {/* Title */}
          <h2 className="text-xl font-bold text-gray-900 dark:text-white leading-snug">{pkg.title}</h2>

          {/* Description */}
          {(pkg.shortDescription || pkg.description) && (
            <p className="text-sm text-gray-500 dark:text-gray-400 leading-relaxed line-clamp-3">
              {pkg.shortDescription || pkg.description}
            </p>
          )}

          {/* Divider */}
          <div className="border-t border-gray-200 dark:border-gray-800" />

          {/* Duration + Price row */}
          <div className="flex items-end justify-between">
            <div className="flex flex-col gap-1.5">
              <span className="flex items-center gap-1.5 text-sm text-gray-500 dark:text-gray-400">
                <Clock size={14} />
                {pkg.durationDays} Days
              </span>
              {pkg.category && (
                <span className="flex items-center gap-1.5 text-sm text-gray-500 dark:text-gray-400">
                  <Users size={14} />
                  {pkg.category}
                </span>
              )}
            </div>
            <div className="text-right">
              <p className="text-xs text-gray-500 dark:text-gray-400">From</p>
              <p className="text-2xl font-bold text-gray-900 dark:text-white">
                {formatCurrency(pkg.basePrice, pkg.currency)}
              </p>
            </div>
          </div>

          {/* Metadata pills */}
          <div className="flex flex-wrap gap-2 pt-1">
            {pkg.region && (
              <span className="px-2.5 py-1 text-[11px] rounded-full bg-gray-100 dark:bg-gray-800 text-gray-500 dark:text-gray-400 border border-gray-200 dark:border-gray-700">
                {pkg.region}
              </span>
            )}
            {pkg.syncedTenantsCount !== undefined && pkg.syncedTenantsCount > 0 && (
              <span className="px-2.5 py-1 text-[11px] rounded-full bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400 border border-blue-200 dark:border-blue-800">
                {pkg.syncedTenantsCount} tenant{pkg.syncedTenantsCount !== 1 ? "s" : ""}
              </span>
            )}
            {pkg.createdAt && (
              <span className="flex items-center gap-1 px-2.5 py-1 text-[11px] rounded-full bg-gray-100 dark:bg-gray-800 text-gray-500 dark:text-gray-400 border border-gray-200 dark:border-gray-700">
                <Calendar size={10} />
                {new Date(pkg.createdAt).toLocaleDateString()}
              </span>
            )}
          </div>

          {/* Expandable extras */}
          {hasExtras && (
            <>
              <button
                onClick={() => setShowMore((p) => !p)}
                className="w-full flex items-center justify-center gap-1.5 py-2 text-xs font-medium text-brand-500 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-xl transition-colors"
              >
                {showMore ? <><ChevronUp size={13} />Hide details</> : <><ChevronDown size={13} />Show full details</>}
              </button>

              {showMore && (
                <div className="space-y-4 pt-1 border-t border-gray-200 dark:border-gray-800">
                  {highlights.length > 0 && (
                    <div className="space-y-2">
                      <p className="text-[11px] font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-widest">Highlights</p>
                      <ul className="space-y-1.5">
                        {highlights.map((h, i) => (
                          <li key={i} className="flex items-start gap-2 text-sm text-gray-900 dark:text-white">
                            <span className="mt-1 w-1.5 h-1.5 rounded-full bg-brand-500 shrink-0" />
                            {h}
                          </li>
                        ))}
                      </ul>
                    </div>
                  )}

                  {inclusions.length > 0 && (
                    <div className="space-y-2">
                      <p className="text-[11px] font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-widest">Inclusions</p>
                      <ul className="space-y-1.5">
                        {inclusions.map((item, i) => (
                          <li key={i} className="flex items-start gap-2 text-sm text-gray-900 dark:text-white">
                            <CheckCircle2 size={14} className="mt-0.5 shrink-0 text-green-500" />
                            {item}
                          </li>
                        ))}
                      </ul>
                    </div>
                  )}

                  {exclusions.length > 0 && (
                    <div className="space-y-2">
                      <p className="text-[11px] font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-widest">Exclusions</p>
                      <ul className="space-y-1.5">
                        {exclusions.map((item, i) => (
                          <li key={i} className="flex items-start gap-2 text-sm text-gray-500 dark:text-gray-400">
                            <XCircle size={14} className="mt-0.5 shrink-0 text-red-400" />
                            {item}
                          </li>
                        ))}
                      </ul>
                    </div>
                  )}

                  {itinerary.length > 0 && (
                    <div className="space-y-2">
                      <p className="text-[11px] font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-widest">Itinerary</p>
                      <div className="space-y-3">
                        {itinerary.map((day) => (
                          <div key={day.day} className="flex gap-3">
                            <span className="shrink-0 w-7 h-7 rounded-full bg-brand-500/10 text-brand-500 text-[11px] font-bold flex items-center justify-center">
                              D{day.day}
                            </span>
                            <div className="pt-0.5">
                              {day.title && <p className="text-sm font-medium text-gray-900 dark:text-white">{day.title}</p>}
                              {day.description && <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5">{day.description}</p>}
                            </div>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              )}
            </>
          )}
        </div>
      )}

      {/* Not found state */}
      {!loading && !pkg && (
        <div className="p-8 text-center text-sm text-gray-500 dark:text-gray-400">
          Package not found.
        </div>
      )}
    </Modal>
  )
}
