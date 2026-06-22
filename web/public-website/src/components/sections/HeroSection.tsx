"use client"

import Link from "next/link"
import { motion } from "framer-motion"
import { MessageCircle, Search } from "lucide-react"
import { buttonVariants } from "@/components/ui/button"
import { cn } from "@/lib/utils"

const WHATSAPP_URL = `https://wa.me/${process.env.NEXT_PUBLIC_WHATSAPP_NUMBER ?? "601234567890"}?text=Hi%2C%20I%27d%20love%20to%20learn%20more%20about%20your%20packages!`

interface Props {
  title?: string
  subtitle?: string
  backgroundImage?: string
}

export default function HeroSection({
  title = "Discover Your Next Adventure",
  subtitle = "Curated travel packages for unforgettable experiences",
  backgroundImage = "https://images.unsplash.com/photo-1488085061387-422e29b40080?w=1600&q=80",
}: Props) {
  return (
    <section className="relative w-full h-[92vh] min-h-[520px] overflow-hidden">
      <div
        className="absolute inset-0 bg-cover bg-center scale-105"
        style={{ backgroundImage: `url(${backgroundImage})` }}
      />
      <div className="absolute inset-0 bg-gradient-to-b from-black/50 via-black/30 to-black/60" />

      <div className="relative h-full flex flex-col justify-center px-4 sm:px-6 max-w-7xl mx-auto">
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.7, ease: [0.25, 0.46, 0.45, 0.94] }}
          className="max-w-2xl"
        >
          <motion.span
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ delay: 0.2 }}
            className="inline-block text-sm font-semibold text-white/80 uppercase tracking-widest mb-4"
          >
            ✈ Travel Packages
          </motion.span>

          <h1 className="text-4xl sm:text-5xl md:text-6xl font-bold text-white leading-tight mb-6">
            {title}
          </h1>

          <p className="text-lg sm:text-xl text-white/90 mb-10 max-w-xl">{subtitle}</p>

          <div className="flex flex-wrap gap-4">
            <motion.div whileHover={{ scale: 1.04 }} whileTap={{ scale: 0.97 }}>
              <Link
                href="/packages"
                className={cn(
                  buttonVariants({ size: "lg" }),
                  "bg-(--color-brand-blue) hover:bg-blue-700 text-white px-8 rounded-xl font-semibold gap-2 shadow-lg"
                )}
              >
                <Search size={18} />
                Explore Packages
              </Link>
            </motion.div>

            <motion.div whileHover={{ scale: 1.04 }} whileTap={{ scale: 0.97 }}>
              <a
                href={WHATSAPP_URL}
                target="_blank"
                rel="noopener noreferrer"
                className={cn(
                  buttonVariants({ variant: "outline", size: "lg" }),
                  "border-2 border-white text-white bg-white/10 hover:bg-white/20 px-8 rounded-xl font-semibold gap-2 backdrop-blur-sm"
                )}
              >
                <MessageCircle size={18} />
                Chat With Us
              </a>
            </motion.div>
          </div>
        </motion.div>

        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 1.2 }}
          className="absolute bottom-8 left-1/2 -translate-x-1/2"
        >
          <div className="w-6 h-10 rounded-full border-2 border-white/50 flex justify-center pt-2">
            <motion.div
              animate={{ y: [0, 10, 0] }}
              transition={{ duration: 1.4, repeat: Infinity }}
              className="w-1 h-2 rounded-full bg-white/70"
            />
          </div>
        </motion.div>
      </div>
    </section>
  )
}
