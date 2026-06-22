"use client"

import { motion } from "framer-motion"
import { Star } from "lucide-react"
import { staggerContainer, staggerItem } from "@/lib/animations"

const TESTIMONIALS = [
  {
    id: "1",
    author: "Sarah Lim",
    destination: "Japan Sakura Tour",
    text: "Absolutely amazing experience! The itinerary was perfectly planned and our guide was incredibly knowledgeable. Would definitely book again.",
    rating: 5,
  },
  {
    id: "2",
    author: "Ahmad Razif",
    destination: "Bali Family Package",
    text: "Best family vacation we've ever had. Everything from accommodation to activities was top-notch. The kids loved every moment!",
    rating: 5,
  },
  {
    id: "3",
    author: "Jennifer Tan",
    destination: "Europe Explorer",
    text: "Seamless booking process and exceptional service throughout. The team went above and beyond to make our honeymoon special.",
    rating: 5,
  },
]

export default function TestimonialsSection() {
  return (
    <section className="py-20 px-4 sm:px-6 bg-white">
      <div className="max-w-7xl mx-auto">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="text-center mb-12"
        >
          <p className="text-sm font-semibold text-(--color-brand-teal) uppercase tracking-widest mb-2">
            Happy Travellers
          </p>
          <h2 className="text-3xl sm:text-4xl font-bold text-foreground">
            What Our Customers Say
          </h2>
        </motion.div>

        <motion.div
          className="grid grid-cols-1 md:grid-cols-3 gap-6"
          variants={staggerContainer}
          initial="hidden"
          animate="visible"
        >
          {TESTIMONIALS.map((t) => (
            <motion.div
              key={t.id}
              variants={staggerItem}
              className="bg-gray-50 rounded-2xl p-7 border border-border hover:shadow-lg transition-shadow"
            >
              {/* Stars */}
              <div className="flex gap-1 mb-4">
                {Array.from({ length: t.rating }).map((_, i) => (
                  <Star key={i} size={16} className="fill-yellow-400 text-yellow-400" />
                ))}
              </div>

              {/* Quote */}
              <p className="text-foreground/80 text-sm leading-relaxed mb-6 italic">
                &ldquo;{t.text}&rdquo;
              </p>

              {/* Author */}
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-full bg-(--color-brand-blue) flex items-center justify-center text-white font-bold text-sm">
                  {t.author[0]}
                </div>
                <div>
                  <p className="font-semibold text-sm text-foreground">{t.author}</p>
                  <p className="text-xs text-muted-foreground">{t.destination}</p>
                </div>
              </div>
            </motion.div>
          ))}
        </motion.div>
      </div>
    </section>
  )
}
