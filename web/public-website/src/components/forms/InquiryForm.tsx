"use client"

import { useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { motion } from "framer-motion"
import { Loader2, Send, CheckCircle } from "lucide-react"
import { Button } from "@/components/ui/button"
import { toast } from "sonner"
import { publicApi } from "@/lib/api"
import type { InquiryFormData } from "@/types"

const schema = z.object({
  customerName:  z.string().min(2, "Name is required"),
  customerEmail: z.string().email("Enter a valid email"),
  customerPhone: z.string().min(8, "Enter a valid phone number"),
  message:       z.string().min(10, "Message must be at least 10 characters"),
  numberOfPax:   z.number().min(1).optional(),
  preferredTravelDates: z.string().optional(),
})

type FormValues = z.infer<typeof schema>

interface Props {
  packageId?: string
  packageTitle?: string
  onSuccess?: () => void
}

export default function InquiryForm({ packageId, packageTitle, onSuccess }: Props) {
  const [submitted, setSubmitted] = useState(false)

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  const onSubmit = async (data: FormValues) => {
    try {
      await publicApi.submitInquiry({
        ...data,
        masterPackageId: packageId,
      })
      setSubmitted(true)
      reset()
      toast.success("Inquiry sent! Our agent will contact you shortly.")
      onSuccess?.()
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : "Failed to send inquiry."
      toast.error(message)
    }
  }

  if (submitted) {
    return (
      <motion.div
        initial={{ opacity: 0, scale: 0.95 }}
        animate={{ opacity: 1, scale: 1 }}
        className="flex flex-col items-center gap-4 py-8 text-center"
      >
        <div className="w-16 h-16 rounded-full bg-green-100 flex items-center justify-center">
          <CheckCircle size={32} className="text-[--color-brand-success]" />
        </div>
        <h3 className="text-xl font-bold">Inquiry Sent!</h3>
        <p className="text-muted-foreground text-sm">
          Thank you! Our travel agent will reach out to you within 24 hours.
        </p>
        <Button variant="outline" size="sm" onClick={() => setSubmitted(false)}>
          Send Another
        </Button>
      </motion.div>
    )
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      {packageTitle && (
        <div className="bg-blue-50 text-[--color-brand-blue] text-sm px-4 py-3 rounded-lg font-medium">
          ✈ Enquiring about: {packageTitle}
        </div>
      )}

      {/* Name */}
      <div>
        <label className="block text-sm font-semibold mb-1.5">
          Full Name <span className="text-red-500">*</span>
        </label>
        <input
          {...register("customerName")}
          placeholder="Your full name"
          className="w-full px-4 py-2.5 rounded-lg border border-border focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue] text-sm bg-background"
        />
        {errors.customerName && <p className="text-xs text-red-500 mt-1">{errors.customerName.message}</p>}
      </div>

      {/* Email */}
      <div>
        <label className="block text-sm font-semibold mb-1.5">
          Email Address <span className="text-red-500">*</span>
        </label>
        <input
          {...register("customerEmail")}
          type="email"
          placeholder="your@email.com"
          className="w-full px-4 py-2.5 rounded-lg border border-border focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue] text-sm bg-background"
        />
        {errors.customerEmail && <p className="text-xs text-red-500 mt-1">{errors.customerEmail.message}</p>}
      </div>

      {/* Phone */}
      <div>
        <label className="block text-sm font-semibold mb-1.5">
          Phone Number <span className="text-red-500">*</span>
        </label>
        <input
          {...register("customerPhone")}
          type="tel"
          placeholder="+60 12-345 6789"
          className="w-full px-4 py-2.5 rounded-lg border border-border focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue] text-sm bg-background"
        />
        {errors.customerPhone && <p className="text-xs text-red-500 mt-1">{errors.customerPhone.message}</p>}
      </div>

      {/* Pax + Dates */}
      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-semibold mb-1.5">No. of Pax</label>
          <input
            {...register("numberOfPax", { setValueAs: (v: string) => v === "" ? undefined : parseInt(v, 10) })}
            type="number"
            min={1}
            placeholder="2"
            className="w-full px-4 py-2.5 rounded-lg border border-border focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue] text-sm bg-background"
          />
        </div>
        <div>
          <label className="block text-sm font-semibold mb-1.5">Preferred Dates</label>
          <input
            {...register("preferredTravelDates")}
            placeholder="e.g. March 2026"
            className="w-full px-4 py-2.5 rounded-lg border border-border focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue] text-sm bg-background"
          />
        </div>
      </div>

      {/* Message */}
      <div>
        <label className="block text-sm font-semibold mb-1.5">
          Message <span className="text-red-500">*</span>
        </label>
        <textarea
          {...register("message")}
          rows={4}
          placeholder="Tell us what you're looking for..."
          className="w-full px-4 py-2.5 rounded-lg border border-border focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue] text-sm bg-background resize-none"
        />
        {errors.message && <p className="text-xs text-red-500 mt-1">{errors.message.message}</p>}
      </div>

      <Button
        type="submit"
        disabled={isSubmitting}
        className="w-full bg-[--color-brand-blue] hover:bg-blue-700 text-white font-bold py-3 rounded-xl gap-2"
        size="lg"
      >
        {isSubmitting ? (
          <><Loader2 size={18} className="animate-spin" /> Sending...</>
        ) : (
          <><Send size={18} /> Send Inquiry</>
        )}
      </Button>

      <p className="text-xs text-muted-foreground text-center">
        Or reach us directly on{" "}
        <a
          href={`https://wa.me/${process.env.NEXT_PUBLIC_WHATSAPP_NUMBER ?? "601234567890"}`}
          className="text-[--color-brand-blue] font-semibold hover:underline"
          target="_blank"
          rel="noopener noreferrer"
        >
          WhatsApp
        </a>
      </p>
    </form>
  )
}
