"use client"

import { useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { Loader2 } from "lucide-react"
import { Modal } from "@/components/ui/Modal"
import { Button } from "@/components/ui/button"
import { tenantApi } from "@/lib/api"
import { toast } from "sonner"

const schema = z.object({
  Title: z.string().optional(),
  Price: z.string().optional(),
  Currency: z.string().optional(),
  FeaturedImageUrl: z.string().optional(),
  Description: z.string().optional(),
  ShortDescription: z.string().optional(),
  ContactPhone: z.string().optional(),
  ContactWhatsapp: z.string().optional(),
})

type FormValues = z.infer<typeof schema>

interface Props {
  isOpen: boolean
  onClose: () => void
  package: any
  onSuccess: () => void
}

export default function PackageOverrideModal({ isOpen, onClose, package: pkg, onSuccess }: Props) {
  const [loading, setLoading] = useState(false)

  const { register, handleSubmit, formState: { errors }, reset } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      Title: pkg.title,
      Price: pkg.price.toString(),
      Currency: pkg.currency,
      FeaturedImageUrl: pkg.featuredImageUrl,
      Description: pkg.description,
      ShortDescription: pkg.shortDescription,
      ContactPhone: pkg.contactPhone,
      ContactWhatsapp: pkg.contactWhatsapp,
    },
  })

  const onSubmit = async (data: FormValues) => {
    try {
      setLoading(true)
      const tenantId = localStorage.getItem("litxus_tenant_id")
      if (!tenantId) {
        toast.error("Not authenticated")
        return
      }

      // Strip empty strings from the payload
      const payload = Object.fromEntries(
        Object.entries(data).filter(([, v]) => v !== "")
      )

      await tenantApi.overridePackage(tenantId, pkg.id, payload)
      toast.success("Package customization saved")
      reset()
      onClose()
      onSuccess()
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Failed to save customization")
    } finally {
      setLoading(false)
    }
  }

  const inputClass = "w-full px-3 py-2.5 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-900 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-brand-500/30"
  const labelClass = "block text-sm font-medium text-gray-900 dark:text-white mb-1"
  const errorClass = "text-xs text-error-600 mt-1"

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      className="max-w-xl mx-4 rounded-2xl shadow-theme-xl"
    >
      <div className="px-6 pt-6 pb-4 border-b border-gray-200 dark:border-gray-800">
        <h3 className="text-lg font-semibold text-gray-900 dark:text-white">Customize Package</h3>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="p-6 space-y-4">
        <div>
          <label className={labelClass}>Title</label>
          <input
            {...register("Title")}
            type="text"
            placeholder="Package title"
            className={inputClass}
          />
          {errors.Title && <p className={errorClass}>{errors.Title.message}</p>}
        </div>

        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass}>Price</label>
            <input
              {...register("Price")}
              type="number"
              step="0.01"
              placeholder="Price"
              className={inputClass}
            />
            {errors.Price && <p className={errorClass}>{errors.Price.message}</p>}
          </div>
          <div>
            <label className={labelClass}>Currency</label>
            <input
              {...register("Currency")}
              type="text"
              placeholder="USD"
              className={inputClass}
            />
            {errors.Currency && <p className={errorClass}>{errors.Currency.message}</p>}
          </div>
        </div>

        <div>
          <label className={labelClass}>Short Description</label>
          <textarea
            {...register("ShortDescription")}
            placeholder="Brief description for listings"
            rows={2}
            className={inputClass}
          />
          {errors.ShortDescription && <p className={errorClass}>{errors.ShortDescription.message}</p>}
        </div>

        <div>
          <label className={labelClass}>Description</label>
          <textarea
            {...register("Description")}
            placeholder="Full description"
            rows={3}
            className={inputClass}
          />
          {errors.Description && <p className={errorClass}>{errors.Description.message}</p>}
        </div>

        <div>
          <label className={labelClass}>Featured Image URL</label>
          <input
            {...register("FeaturedImageUrl")}
            type="url"
            placeholder="https://example.com/image.jpg"
            className={inputClass}
          />
          {errors.FeaturedImageUrl && <p className={errorClass}>{errors.FeaturedImageUrl.message}</p>}
        </div>

        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={labelClass}>Contact Phone</label>
            <input
              {...register("ContactPhone")}
              type="tel"
              placeholder="+1234567890"
              className={inputClass}
            />
            {errors.ContactPhone && <p className={errorClass}>{errors.ContactPhone.message}</p>}
          </div>
          <div>
            <label className={labelClass}>Contact WhatsApp</label>
            <input
              {...register("ContactWhatsapp")}
              type="tel"
              placeholder="+1234567890"
              className={inputClass}
            />
            {errors.ContactWhatsapp && <p className={errorClass}>{errors.ContactWhatsapp.message}</p>}
          </div>
        </div>

        <div className="flex gap-3 pt-2">
          <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
          <Button
            type="submit"
            disabled={loading}
            className="flex-1"
          >
            {loading ? <><Loader2 size={16} className="animate-spin mr-2" />Saving...</> : "Save Changes"}
          </Button>
        </div>
      </form>
    </Modal>
  )
}
