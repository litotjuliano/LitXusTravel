"use client"

import Link from "next/link"
import Image from "next/image"
import { motion } from "framer-motion"
import { Star, Clock, MapPin } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { hoverLift, staggerItem } from "@/lib/animations"
import type { Package } from "@/types"

interface Props {
  pkg: Package
}

export default function PackageCard({ pkg }: Props) {
  const image = pkg.featuredImageUrl ?? "https://images.unsplash.com/photo-1500835556837-99ac94a94552?w=600&q=75"

  return (
    <motion.div variants={staggerItem} {...hoverLift}>
      <Link href={`/packages/${pkg.slug ?? pkg.id}`} className="block group">
        <div className="bg-card rounded-2xl overflow-hidden shadow-md hover:shadow-xl transition-shadow duration-300 border border-border h-full">
          {/* Image */}
          <div className="relative w-full h-56 overflow-hidden bg-muted">
            <Image
              src={image}
              alt={pkg.title}
              fill
              className="object-cover group-hover:scale-105 transition-transform duration-500"
              sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
            />
            {pkg.isFeatured && (
              <Badge className="absolute top-3 right-3 bg-[--color-brand-orange] hover:bg-orange-600 text-white text-xs font-semibold">
                Featured
              </Badge>
            )}
            {pkg.isPopular && !pkg.isFeatured && (
              <Badge className="absolute top-3 right-3 bg-[--color-brand-teal] hover:bg-teal-700 text-white text-xs font-semibold">
                Popular
              </Badge>
            )}
          </div>

          {/* Content */}
          <div className="p-5">
            {/* Destination & Rating */}
            <div className="flex items-center justify-between mb-2">
              <span className="flex items-center gap-1 text-xs text-muted-foreground uppercase tracking-wide font-medium">
                <MapPin size={12} />
                {pkg.destination}
              </span>
              {pkg.rating && (
                <span className="flex items-center gap-1 text-xs font-semibold">
                  <Star size={12} className="fill-yellow-400 text-yellow-400" />
                  {pkg.rating.toFixed(1)}
                  <span className="text-muted-foreground font-normal">({pkg.reviewCount})</span>
                </span>
              )}
            </div>

            {/* Title */}
            <h3 className="font-bold text-foreground text-base leading-snug mb-2 line-clamp-2 group-hover:text-[--color-brand-blue] transition-colors">
              {pkg.title}
            </h3>

            {/* Description */}
            {pkg.shortDescription && (
              <p className="text-sm text-muted-foreground mb-4 line-clamp-2 leading-relaxed">
                {pkg.shortDescription}
              </p>
            )}

            {/* Duration & Price */}
            <div className="flex items-center justify-between pt-3 border-t border-border">
              <span className="flex items-center gap-1.5 text-sm text-muted-foreground">
                <Clock size={14} />
                {pkg.durationDays} Days
              </span>
              <div className="text-right">
                <p className="text-xs text-muted-foreground">From</p>
                <p className="text-xl font-bold text-[--color-brand-blue]">
                  {pkg.currency} {pkg.price.toLocaleString()}
                </p>
              </div>
            </div>
          </div>
        </div>
      </Link>
    </motion.div>
  )
}
