"use client"

import { useEffect, useRef, useState } from "react"
import { Dialog, DialogContent } from "@/components/ui/dialog"
import { adminApi } from "@/lib/api"
import { formatCurrency } from "@/lib/utils"
import { toast } from "sonner"
import {
  Loader2, MapPin, Clock, Star, CheckCircle2, XCircle,
  ChevronDown, ChevronUp, ImageOff, Calendar, Users, Camera,
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

interface Props {
  packageId: string | null
  open: boolean
  onOpenChange: (open: boolean) => void
}

function parseJson<T>(json: string | null | undefined, fallback: T): T {
  if (!json) return fallback
  try { return JSON.parse(json) } catch { return fallback }
}

interface ItineraryDay { day: number; title?: string; description?: string }

export function PackageViewModal({ packageId, open, onOpenChange }: Props) {
  const [pkg, setPkg] = useState<PackageDetail | null>(null)
  const [loading, setLoading] = useState(false)
  const [showMore, setShowMore] = useState(false)
  const [uploading, setUploading] = useState(false)
  const fileInputRef = useRef<HTMLInputElement>(null)

  useEffect(() => {
    if (!open || !packageId) return
    setLoading(true)
    setPkg(null)
    setShowMore(false)
    adminApi.getPackageById(packageId)
      .then((res) => setPkg(res.data))
      .catch(() => setPkg(null))
      .finally(() => setLoading(false))
  }, [open, packageId])

  async function handleImageUpload(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0]
    if (!file || !pkg) return
    setUploading(true)
    try {
      const res = await adminApi.uploadPackageImage(pkg.id, file)
      setPkg((prev) => prev ? { ...prev, featuredImageUrl: res.data.imageUrl } : prev)
      toast.success("Image updated")
    } catch {
      toast.error("Upload failed")
    } finally {
      setUploading(false)
      if (fileInputRef.current) fileInputRef.current.value = ""
    }
  }

  const highlights = parseJson<string[]>(pkg?.highlightsJson, [])
  const inclusions = parseJson<string[]>(pkg?.inclusionsJson, [])
  const exclusions = parseJson<string[]>(pkg?.exclusionsJson, [])
  const itinerary  = parseJson<ItineraryDay[]>(pkg?.itineraryJson, [])
  const images     = parseJson<string[]>(pkg?.imagesJson, [])
  const heroImage  = pkg?.featuredImageUrl || images[0]

  const hasExtras = highlights.length > 0 || inclusions.length > 0 || exclusions.length > 0 || itinerary.length > 0

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="p-0 gap-0 border-0 bg-transparent shadow-none max-w-sm w-full overflow-visible">
        <div className="bg-card rounded-3xl overflow-hidden shadow-2xl border border-border">

          {/* ── Hero image ── */}
          <div className="group relative h-52 bg-muted overflow-hidden">
            {loading ? (
              <div className="absolute inset-0 flex items-center justify-center">
                <Loader2 className="animate-spin text-muted-foreground" size={28} />
              </div>
            ) : heroImage ? (
              <img src={heroImage} alt={pkg?.title} className="w-full h-full object-cover" />
            ) : (
              <div className="w-full h-full flex flex-col items-center justify-center gap-2">
                <ImageOff size={36} className="text-muted-foreground/30" />
                <p className="text-xs text-muted-foreground/50">No image</p>
              </div>
            )}

            {/* Gradient overlay */}
            {!loading && <div className="absolute inset-0 bg-gradient-to-t from-black/30 via-transparent to-transparent" />}

            {/* Upload overlay — visible on hover or when no image */}
            {!loading && pkg && (
              <button
                onClick={() => fileInputRef.current?.click()}
                disabled={uploading}
                className={`absolute inset-0 flex flex-col items-center justify-center gap-2 transition-opacity
                  ${heroImage ? "opacity-0 group-hover:opacity-100 bg-black/40" : "opacity-100 bg-black/20"}
                `}
              >
                {uploading ? (
                  <Loader2 size={22} className="text-white animate-spin" />
                ) : (
                  <>
                    <Camera size={22} className="text-white drop-shadow" />
                    <span className="text-xs font-medium text-white drop-shadow">
                      {heroImage ? "Change image" : "Upload image"}
                    </span>
                  </>
                )}
              </button>
            )}

            <input
              ref={fileInputRef}
              type="file"
              accept="image/jpeg,image/png,image/webp"
              className="hidden"
              onChange={handleImageUpload}
            />

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
                <span className="flex items-center gap-1 text-xs font-semibold text-muted-foreground uppercase tracking-wide">
                  <MapPin size={12} />
                  {pkg.destination}
                </span>
                {pkg.rating ? (
                  <span className="flex items-center gap-1 text-sm font-medium text-foreground">
                    <Star size={13} className="text-yellow-400 fill-yellow-400" />
                    {pkg.rating.toFixed(1)}
                    {pkg.reviewCount ? (
                      <span className="text-xs text-muted-foreground">({pkg.reviewCount})</span>
                    ) : null}
                  </span>
                ) : null}
              </div>

              {/* Title */}
              <h2 className="text-xl font-bold text-foreground leading-snug">{pkg.title}</h2>

              {/* Description */}
              {(pkg.shortDescription || pkg.description) && (
                <p className="text-sm text-muted-foreground leading-relaxed line-clamp-3">
                  {pkg.shortDescription || pkg.description}
                </p>
              )}

              {/* Divider */}
              <div className="border-t border-border" />

              {/* Duration + Price row */}
              <div className="flex items-end justify-between">
                <div className="flex flex-col gap-1.5">
                  <span className="flex items-center gap-1.5 text-sm text-muted-foreground">
                    <Clock size={14} />
                    {pkg.durationDays} Days
                  </span>
                  {pkg.category && (
                    <span className="flex items-center gap-1.5 text-sm text-muted-foreground">
                      <Users size={14} />
                      {pkg.category}
                    </span>
                  )}
                </div>
                <div className="text-right">
                  <p className="text-xs text-muted-foreground">From</p>
                  <p className="text-2xl font-bold text-foreground">
                    {formatCurrency(pkg.basePrice, pkg.currency)}
                  </p>
                </div>
              </div>

              {/* Metadata pills */}
              <div className="flex flex-wrap gap-2 pt-1">
                {pkg.region && (
                  <span className="px-2.5 py-1 text-[11px] rounded-full bg-muted text-muted-foreground border border-border">
                    {pkg.region}
                  </span>
                )}
                {pkg.syncedTenantsCount !== undefined && pkg.syncedTenantsCount > 0 && (
                  <span className="px-2.5 py-1 text-[11px] rounded-full bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400 border border-blue-200 dark:border-blue-800">
                    {pkg.syncedTenantsCount} tenant{pkg.syncedTenantsCount !== 1 ? "s" : ""}
                  </span>
                )}
                {pkg.createdAt && (
                  <span className="flex items-center gap-1 px-2.5 py-1 text-[11px] rounded-full bg-muted text-muted-foreground border border-border">
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
                    className="w-full flex items-center justify-center gap-1.5 py-2 text-xs font-medium text-[--color-brand-blue] hover:bg-muted/50 rounded-xl transition-colors"
                  >
                    {showMore ? <><ChevronUp size={13} />Hide details</> : <><ChevronDown size={13} />Show full details</>}
                  </button>

                  {showMore && (
                    <div className="space-y-4 pt-1 border-t border-border">
                      {highlights.length > 0 && (
                        <div className="space-y-2">
                          <p className="text-[11px] font-semibold text-muted-foreground uppercase tracking-widest">Highlights</p>
                          <ul className="space-y-1.5">
                            {highlights.map((h, i) => (
                              <li key={i} className="flex items-start gap-2 text-sm text-foreground">
                                <span className="mt-1 w-1.5 h-1.5 rounded-full bg-[--color-brand-blue] shrink-0" />
                                {h}
                              </li>
                            ))}
                          </ul>
                        </div>
                      )}

                      {inclusions.length > 0 && (
                        <div className="space-y-2">
                          <p className="text-[11px] font-semibold text-muted-foreground uppercase tracking-widest">Inclusions</p>
                          <ul className="space-y-1.5">
                            {inclusions.map((item, i) => (
                              <li key={i} className="flex items-start gap-2 text-sm text-foreground">
                                <CheckCircle2 size={14} className="mt-0.5 shrink-0 text-green-500" />
                                {item}
                              </li>
                            ))}
                          </ul>
                        </div>
                      )}

                      {exclusions.length > 0 && (
                        <div className="space-y-2">
                          <p className="text-[11px] font-semibold text-muted-foreground uppercase tracking-widest">Exclusions</p>
                          <ul className="space-y-1.5">
                            {exclusions.map((item, i) => (
                              <li key={i} className="flex items-start gap-2 text-sm text-muted-foreground">
                                <XCircle size={14} className="mt-0.5 shrink-0 text-red-400" />
                                {item}
                              </li>
                            ))}
                          </ul>
                        </div>
                      )}

                      {itinerary.length > 0 && (
                        <div className="space-y-2">
                          <p className="text-[11px] font-semibold text-muted-foreground uppercase tracking-widest">Itinerary</p>
                          <div className="space-y-3">
                            {itinerary.map((day) => (
                              <div key={day.day} className="flex gap-3">
                                <span className="shrink-0 w-7 h-7 rounded-full bg-[--color-brand-blue]/10 text-[--color-brand-blue] text-[11px] font-bold flex items-center justify-center">
                                  D{day.day}
                                </span>
                                <div className="pt-0.5">
                                  {day.title && <p className="text-sm font-medium text-foreground">{day.title}</p>}
                                  {day.description && <p className="text-xs text-muted-foreground mt-0.5">{day.description}</p>}
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
            <div className="p-8 text-center text-sm text-muted-foreground">
              Package not found.
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  )
}
