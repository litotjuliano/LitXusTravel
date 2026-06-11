"use client"

import { useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { Loader2 } from "lucide-react"
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog"
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
      const tenantId = localStorage.getItem("nexus_tenant_id")
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

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-xl">
        <DialogHeader>
          <DialogTitle>Customize Package</DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-foreground mb-1">Title</label>
            <input
              {...register("Title")}
              type="text"
              placeholder="Package title"
              className="w-full px-3 py-2.5 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
            />
            {errors.Title && <p className="text-xs text-destructive mt-1">{errors.Title.message}</p>}
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-foreground mb-1">Price</label>
              <input
                {...register("Price")}
                type="number"
                step="0.01"
                placeholder="Price"
                className="w-full px-3 py-2.5 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
              />
              {errors.Price && <p className="text-xs text-destructive mt-1">{errors.Price.message}</p>}
            </div>
            <div>
              <label className="block text-sm font-medium text-foreground mb-1">Currency</label>
              <input
                {...register("Currency")}
                type="text"
                placeholder="USD"
                className="w-full px-3 py-2.5 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
              />
              {errors.Currency && <p className="text-xs text-destructive mt-1">{errors.Currency.message}</p>}
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-foreground mb-1">Short Description</label>
            <textarea
              {...register("ShortDescription")}
              placeholder="Brief description for listings"
              rows={2}
              className="w-full px-3 py-2.5 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
            />
            {errors.ShortDescription && <p className="text-xs text-destructive mt-1">{errors.ShortDescription.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-foreground mb-1">Description</label>
            <textarea
              {...register("Description")}
              placeholder="Full description"
              rows={3}
              className="w-full px-3 py-2.5 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
            />
            {errors.Description && <p className="text-xs text-destructive mt-1">{errors.Description.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-foreground mb-1">Featured Image URL</label>
            <input
              {...register("FeaturedImageUrl")}
              type="url"
              placeholder="https://example.com/image.jpg"
              className="w-full px-3 py-2.5 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
            />
            {errors.FeaturedImageUrl && <p className="text-xs text-destructive mt-1">{errors.FeaturedImageUrl.message}</p>}
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-foreground mb-1">Contact Phone</label>
              <input
                {...register("ContactPhone")}
                type="tel"
                placeholder="+1234567890"
                className="w-full px-3 py-2.5 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
              />
              {errors.ContactPhone && <p className="text-xs text-destructive mt-1">{errors.ContactPhone.message}</p>}
            </div>
            <div>
              <label className="block text-sm font-medium text-foreground mb-1">Contact WhatsApp</label>
              <input
                {...register("ContactWhatsapp")}
                type="tel"
                placeholder="+1234567890"
                className="w-full px-3 py-2.5 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
              />
              {errors.ContactWhatsapp && <p className="text-xs text-destructive mt-1">{errors.ContactWhatsapp.message}</p>}
            </div>
          </div>

          <div className="flex gap-3 pt-6">
            <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
            <Button
              type="submit"
              disabled={loading}
              className="flex-1 bg-[--color-brand-blue] hover:bg-blue-700"
            >
              {loading ? <><Loader2 size={16} className="animate-spin mr-2" />Saving...</> : "Save Changes"}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  )
}
