"use client"

import { useState, useMemo } from "react"
import { motion } from "framer-motion"
import { Search, SlidersHorizontal, X } from "lucide-react"
import PackageCard from "@/components/packages/PackageCard"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { staggerContainer } from "@/lib/animations"
import { CATEGORIES } from "@/lib/mock-data"
import type { Package } from "@/types"

interface Props {
  packages: Package[]   // slugs pre-computed server-side
}

export default function PackagesClient({ packages }: Props) {
  const [search, setSearch] = useState("")
  const [category, setCategory] = useState("All")
  const [sortBy, setSortBy] = useState("popular")
  const [showFilters, setShowFilters] = useState(false)
  const [priceMax, setPriceMax] = useState(15000)

  const filtered = useMemo(() => {
    let list = [...packages]

    if (search) {
      const q = search.toLowerCase()
      list = list.filter(
        (p) =>
          p.title.toLowerCase().includes(q) ||
          p.destination.toLowerCase().includes(q) ||
          p.category?.toLowerCase().includes(q)
      )
    }

    if (category !== "All") list = list.filter((p) => p.category === category)
    list = list.filter((p) => p.price <= priceMax)

    switch (sortBy) {
      case "price_asc":  list.sort((a, b) => a.price - b.price); break
      case "price_desc": list.sort((a, b) => b.price - a.price); break
      case "rating":     list.sort((a, b) => (b.rating ?? 0) - (a.rating ?? 0)); break
      default:           list.sort((a, b) => (b.isPopular ? 1 : 0) - (a.isPopular ? 1 : 0))
    }

    return list
  }, [packages, search, category, sortBy, priceMax])

  return (
    <>
      {/* Search + Sort bar */}
      <div className="bg-white border-b border-border sticky top-16 z-30 shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 py-3 flex gap-3 items-center">
          <div className="relative flex-1 max-w-md">
            <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
            <input
              type="text"
              placeholder="Search packages, destinations..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-full pl-9 pr-4 py-2 rounded-lg border border-border text-sm focus:outline-none focus:ring-2 focus:ring-(--color-brand-blue) bg-background"
            />
            {search && (
              <button onClick={() => setSearch("")} className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground">
                <X size={14} />
              </button>
            )}
          </div>

          <select
            value={sortBy}
            onChange={(e) => setSortBy(e.target.value)}
            className="hidden sm:block px-3 py-2 rounded-lg border border-border text-sm focus:outline-none focus:ring-2 focus:ring-(--color-brand-blue) bg-background"
          >
            <option value="popular">Most Popular</option>
            <option value="price_asc">Price: Low to High</option>
            <option value="price_desc">Price: High to Low</option>
            <option value="rating">Top Rated</option>
          </select>

          <Button
            variant="outline"
            size="sm"
            onClick={() => setShowFilters(!showFilters)}
            className="gap-2 shrink-0"
          >
            <SlidersHorizontal size={15} />
            Filters
          </Button>
        </div>

        {showFilters && (
          <div className="border-t border-border px-4 sm:px-6 py-4 bg-gray-50">
            <div className="max-w-7xl mx-auto flex flex-wrap gap-6 items-start">
              <div>
                <p className="text-xs font-semibold text-muted-foreground uppercase mb-2">Category</p>
                <div className="flex flex-wrap gap-2">
                  {CATEGORIES.map((cat) => (
                    <Badge
                      key={cat}
                      onClick={() => setCategory(cat)}
                      className={`cursor-pointer transition-colors ${
                        category === cat
                          ? "bg-(--color-brand-blue) hover:bg-blue-700 text-white"
                          : "bg-white text-foreground border border-border hover:bg-gray-100"
                      }`}
                    >
                      {cat}
                    </Badge>
                  ))}
                </div>
              </div>

              <div className="min-w-48">
                <p className="text-xs font-semibold text-muted-foreground uppercase mb-2">
                  Max Price: RM {priceMax.toLocaleString()}
                </p>
                <input
                  type="range" min={500} max={15000} step={500} value={priceMax}
                  onChange={(e) => setPriceMax(Number(e.target.value))}
                  className="w-full accent-(--color-brand-blue)"
                />
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Grid */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 py-10">
        <p className="text-sm text-muted-foreground mb-6">{filtered.length} packages found</p>

        {filtered.length === 0 ? (
          <div className="text-center py-20">
            <p className="text-muted-foreground text-lg">No packages match your filters.</p>
            <Button
              variant="outline"
              className="mt-4"
              onClick={() => { setSearch(""); setCategory("All"); setPriceMax(15000) }}
            >
              Clear Filters
            </Button>
          </div>
        ) : (
          <motion.div
            className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6"
            variants={staggerContainer}
            initial="hidden"
            animate="visible"
          >
            {filtered.map((pkg) => (
              <PackageCard key={pkg.id} pkg={pkg} />
            ))}
          </motion.div>
        )}
      </div>
    </>
  )
}
