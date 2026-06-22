"use client"

import Link from "next/link"
import { motion } from "framer-motion"
import { ArrowRight } from "lucide-react"
import { buttonVariants } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import PackageCard from "@/components/packages/PackageCard"
import { staggerContainer } from "@/lib/animations"
import type { Package } from "@/types"

interface Props {
  packages: Package[]
}

export default function FeaturedPackages({ packages }: Props) {
  if (!packages.length) return null

  return (
    <section className="py-20 px-4 sm:px-6 bg-white">
      <div className="max-w-7xl mx-auto">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
          className="flex flex-col sm:flex-row sm:items-end justify-between gap-4 mb-12"
        >
          <div>
            <p className="text-sm font-semibold text-(--color-brand-teal) uppercase tracking-widest mb-2">
              Handpicked For You
            </p>
            <h2 className="text-3xl sm:text-4xl font-bold text-foreground">
              Featured Packages
            </h2>
          </div>
          <Link
            href="/packages"
            className={cn(
              buttonVariants({ variant: "outline" }),
              "gap-2 border-(--color-brand-blue) text-(--color-brand-blue) hover:bg-blue-50 self-start sm:self-auto"
            )}
          >
            View All
            <ArrowRight size={16} />
          </Link>
        </motion.div>

        <motion.div
          className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6"
          variants={staggerContainer}
          initial="hidden"
          animate="visible"
        >
          {packages.slice(0, 6).map((pkg) => (
            <PackageCard key={pkg.id} pkg={pkg} />
          ))}
        </motion.div>
      </div>
    </section>
  )
}
