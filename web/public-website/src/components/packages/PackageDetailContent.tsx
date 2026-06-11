"use client"

import { useState } from "react"
import Image from "next/image"
import Link from "next/link"
import { motion } from "framer-motion"
import {
  Calendar, MapPin, Users, Star, Check, X,
  MessageCircle, ArrowLeft, ChevronDown, ChevronUp,
} from "lucide-react"
import InquiryForm from "@/components/forms/InquiryForm"
import { Button, buttonVariants } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { Badge } from "@/components/ui/badge"
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import type { Package, ItineraryDay } from "@/types"

interface Props { pkg: Package }

export default function PackageDetailContent({ pkg }: Props) {
  const [selectedImage, setSelectedImage] = useState(
    pkg.featuredImageUrl ?? "https://images.unsplash.com/photo-1500835556837-99ac94a94552?w=1200&q=80"
  )
  const [openDay, setOpenDay] = useState<number | null>(null)
  const [showInquiry, setShowInquiry] = useState(false)

  const itinerary: ItineraryDay[] = pkg.itineraryJson ? JSON.parse(pkg.itineraryJson) : []
  const inclusions: string[] = pkg.inclusionsJson ? JSON.parse(pkg.inclusionsJson) : []
  const exclusions: string[] = pkg.exclusionsJson ? JSON.parse(pkg.exclusionsJson) : []
  const highlights: string[] = pkg.highlightsJson ? JSON.parse(pkg.highlightsJson) : []

  const whatsappUrl = `https://wa.me/${process.env.NEXT_PUBLIC_WHATSAPP_NUMBER ?? "601234567890"}?text=${encodeURIComponent(
    `Hi, I'm interested in the "${pkg.title}" package (${pkg.currency} ${pkg.price}). Can you share more details?`
  )}`

  return (
    <main className="flex-1 bg-white">
      {/* Hero */}
      <div className="relative w-full h-[60vh] min-h-80 bg-gray-900 overflow-hidden">
        <Image src={selectedImage} alt={pkg.title} fill className="object-cover" priority sizes="100vw" />
        <div className="absolute inset-0 bg-black/20" />
        <div className="absolute top-4 left-4">
          <Link
            href="/packages"
            className={cn(buttonVariants({ variant: "secondary", size: "sm" }), "gap-2 bg-white/90 backdrop-blur-sm")}
          >
            <ArrowLeft size={14} /> Back
          </Link>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 py-10">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-10">
          {/* Details */}
          <div className="lg:col-span-2 space-y-10">
            <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }}>
              <div className="flex flex-wrap gap-2 mb-3">
                {pkg.category && <Badge className="bg-blue-100 text-[--color-brand-blue]">{pkg.category}</Badge>}
                {pkg.isFeatured && <Badge className="bg-orange-100 text-[--color-brand-orange]">Featured</Badge>}
              </div>
              <h1 className="text-3xl sm:text-4xl font-bold text-foreground mb-4">{pkg.title}</h1>
              <div className="flex flex-wrap items-center gap-5 text-sm text-muted-foreground">
                <span className="flex items-center gap-1.5"><MapPin size={15} className="text-[--color-brand-blue]" />{pkg.destination}</span>
                <span className="flex items-center gap-1.5"><Calendar size={15} className="text-[--color-brand-blue]" />{pkg.durationDays} Days</span>
                {pkg.rating && (
                  <span className="flex items-center gap-1.5">
                    <Star size={15} className="fill-yellow-400 text-yellow-400" />
                    {pkg.rating} ({pkg.reviewCount} reviews)
                  </span>
                )}
              </div>
              {pkg.description && <p className="mt-5 text-foreground/80 leading-relaxed">{pkg.description}</p>}
            </motion.div>

            {highlights.length > 0 && (
              <div>
                <h2 className="text-xl font-bold mb-4">Highlights</h2>
                <div className="flex flex-wrap gap-2">
                  {highlights.map((h) => (
                    <span key={h} className="bg-blue-50 text-[--color-brand-blue] px-3 py-1.5 rounded-full text-sm font-medium">✦ {h}</span>
                  ))}
                </div>
              </div>
            )}

            {itinerary.length > 0 && (
              <div>
                <h2 className="text-xl font-bold mb-5">Itinerary</h2>
                <div className="space-y-3">
                  {itinerary.map((day) => (
                    <div key={day.dayNumber} className="border border-border rounded-xl overflow-hidden">
                      <button
                        onClick={() => setOpenDay(openDay === day.dayNumber ? null : day.dayNumber)}
                        className="w-full flex items-center justify-between px-5 py-4 text-left hover:bg-gray-50 transition-colors"
                      >
                        <div className="flex items-center gap-4">
                          <span className="w-9 h-9 rounded-full bg-[--color-brand-blue] text-white flex items-center justify-center text-sm font-bold shrink-0">{day.dayNumber}</span>
                          <span className="font-semibold">{day.title}</span>
                        </div>
                        {openDay === day.dayNumber ? <ChevronUp size={18} className="text-muted-foreground" /> : <ChevronDown size={18} className="text-muted-foreground" />}
                      </button>
                      {openDay === day.dayNumber && (
                        <div className="px-5 pb-5 text-sm text-muted-foreground leading-relaxed">{day.description}</div>
                      )}
                    </div>
                  ))}
                </div>
              </div>
            )}

            {(inclusions.length > 0 || exclusions.length > 0) && (
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-8">
                {inclusions.length > 0 && (
                  <div>
                    <h3 className="font-bold text-lg mb-4 flex items-center gap-2"><Check size={20} className="text-[--color-brand-success]" />What&apos;s Included</h3>
                    <ul className="space-y-2">
                      {inclusions.map((item) => <li key={item} className="flex items-start gap-2 text-sm text-foreground/80"><Check size={15} className="text-[--color-brand-success] mt-0.5 shrink-0" />{item}</li>)}
                    </ul>
                  </div>
                )}
                {exclusions.length > 0 && (
                  <div>
                    <h3 className="font-bold text-lg mb-4 flex items-center gap-2"><X size={20} className="text-[--color-brand-warning]" />Not Included</h3>
                    <ul className="space-y-2">
                      {exclusions.map((item) => <li key={item} className="flex items-start gap-2 text-sm text-foreground/80"><X size={15} className="text-[--color-brand-warning] mt-0.5 shrink-0" />{item}</li>)}
                    </ul>
                  </div>
                )}
              </div>
            )}
          </div>

          {/* Booking card */}
          <div>
            <motion.div
              initial={{ opacity: 0, x: 20 }} animate={{ opacity: 1, x: 0 }} transition={{ delay: 0.2 }}
              className="sticky top-24 bg-white border-2 border-[--color-brand-blue] rounded-2xl p-6 shadow-xl"
            >
              <p className="text-sm text-muted-foreground mb-1">Starting from</p>
              <p className="text-4xl font-bold text-[--color-brand-blue] mb-6">{pkg.currency} {pkg.price.toLocaleString()}</p>
              <div className="space-y-3 mb-6 text-sm">
                <div className="flex items-center gap-3 text-foreground/80"><Calendar size={16} className="text-[--color-brand-blue]" />{pkg.durationDays} Days / {pkg.durationDays - 1} Nights</div>
                <div className="flex items-center gap-3 text-foreground/80"><MapPin size={16} className="text-[--color-brand-blue]" />{pkg.destination}</div>
                <div className="flex items-center gap-3 text-foreground/80"><Users size={16} className="text-[--color-brand-blue]" />Min. 2 Pax</div>
              </div>
              <Button onClick={() => setShowInquiry(true)} className="w-full bg-[--color-brand-blue] hover:bg-blue-700 font-bold py-3 rounded-xl mb-3" size="lg">Get Quotation</Button>
              <a
                href={whatsappUrl}
                target="_blank"
                rel="noopener noreferrer"
                className={cn(buttonVariants({ variant: "outline", size: "lg" }), "w-full justify-center border-2 border-[--color-brand-blue] text-[--color-brand-blue] hover:bg-blue-50 font-bold py-3 rounded-xl gap-2")}
              >
                <MessageCircle size={18} />Chat on WhatsApp
              </a>
            </motion.div>
          </div>
        </div>
      </div>

      <Dialog open={showInquiry} onOpenChange={setShowInquiry}>
        <DialogContent className="max-w-lg max-h-[90vh] overflow-y-auto">
          <DialogHeader><DialogTitle>Get a Quotation</DialogTitle></DialogHeader>
          <InquiryForm packageId={pkg.id} packageTitle={pkg.title} onSuccess={() => setShowInquiry(false)} />
        </DialogContent>
      </Dialog>
    </main>
  )
}
