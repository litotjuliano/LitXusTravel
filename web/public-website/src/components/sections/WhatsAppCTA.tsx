"use client"

import { motion } from "framer-motion"
import { MessageCircle } from "lucide-react"
import { buttonVariants } from "@/components/ui/button"
import { cn } from "@/lib/utils"

const WHATSAPP_URL = `https://wa.me/${process.env.NEXT_PUBLIC_WHATSAPP_NUMBER ?? "601234567890"}?text=Hi%2C%20I%27d%20like%20custom%20tour%20recommendations!`

export default function WhatsAppCTA() {
  return (
    <section className="py-20 px-4 sm:px-6 bg-[--color-brand-blue]">
      <motion.div
        className="max-w-2xl mx-auto text-center"
        initial={{ opacity: 0, scale: 0.96 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ duration: 0.5 }}
      >
        <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-white/20 mb-6">
          <MessageCircle size={32} className="text-white" />
        </div>

        <h2 className="text-3xl sm:text-4xl font-bold text-white mb-4">
          Need Custom Recommendations?
        </h2>
        <p className="text-lg text-white/85 mb-10 leading-relaxed">
          Chat with our travel experts on WhatsApp for personalised package suggestions and exclusive deals.
        </p>

        <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.97 }}>
          <a
            href={WHATSAPP_URL}
            target="_blank"
            rel="noopener noreferrer"
            className={cn(buttonVariants({ size: "lg" }), "bg-white text-[--color-brand-blue] hover:bg-gray-100 font-bold px-10 py-4 rounded-xl gap-3 shadow-xl text-base")}
          >
            <MessageCircle size={20} />
            Chat on WhatsApp
          </a>
        </motion.div>
      </motion.div>
    </section>
  )
}
