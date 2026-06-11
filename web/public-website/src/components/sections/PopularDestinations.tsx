"use client"

import Link from "next/link"
import { motion } from "framer-motion"
import { staggerContainer, staggerItem } from "@/lib/animations"

const DESTINATIONS = [
  { name: "Japan",       image: "https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?w=400&q=75" },
  { name: "Bali",        image: "https://images.unsplash.com/photo-1537996194471-e657df975ab4?w=400&q=75" },
  { name: "Europe",      image: "https://images.unsplash.com/photo-1499856871958-5b9627545d1a?w=400&q=75" },
  { name: "Maldives",    image: "https://images.unsplash.com/photo-1514282401047-d79a71a590e8?w=400&q=75" },
  { name: "South Korea", image: "https://images.unsplash.com/photo-1517154421773-0529f29ea451?w=400&q=75" },
]

export default function PopularDestinations() {
  return (
    <section className="py-20 px-4 sm:px-6 bg-gray-50">
      <div className="max-w-7xl mx-auto">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="mb-12"
        >
          <p className="text-sm font-semibold text-[--color-brand-teal] uppercase tracking-widest mb-2">
            Explore the World
          </p>
          <h2 className="text-3xl sm:text-4xl font-bold text-foreground">
            Popular Destinations
          </h2>
        </motion.div>

        <motion.div
          className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4"
          variants={staggerContainer}
          initial="hidden"
          animate="visible"
        >
          {DESTINATIONS.map((dest) => (
            <motion.div key={dest.name} variants={staggerItem}>
              <Link
                href={`/packages?destination=${encodeURIComponent(dest.name)}`}
                className="block relative h-44 rounded-2xl overflow-hidden group cursor-pointer"
              >
                <div
                  className="absolute inset-0 bg-cover bg-center group-hover:scale-110 transition-transform duration-500"
                  style={{ backgroundImage: `url(${dest.image})` }}
                />
                <div className="absolute inset-0 bg-black/30 group-hover:bg-black/40 transition-colors" />
                <div className="absolute inset-0 flex items-end p-4">
                  <h3 className="text-white font-bold text-base drop-shadow-md">
                    {dest.name}
                  </h3>
                </div>
              </Link>
            </motion.div>
          ))}
        </motion.div>
      </div>
    </section>
  )
}
