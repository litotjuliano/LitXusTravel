"use client"

import { useState } from "react"
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { adminApi } from "@/lib/api"
import { toast } from "sonner"

interface PackageEditorModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onSuccess?: () => void
}

export function PackageEditorModal({ open, onOpenChange, onSuccess }: PackageEditorModalProps) {
  const [loading, setLoading] = useState(false)
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [formData, setFormData] = useState({
    title: "",
    destination: "",
    basePrice: "",
    durationDays: "",
    category: "",
    description: "",
    shortDescription: "",
    currency: "USD",
    region: "",
    featuredImageUrl: "",
    maxGroupSize: "",
  })

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {}

    if (!formData.title.trim()) newErrors.title = "Title is required"
    if (!formData.destination.trim()) newErrors.destination = "Destination is required"
    if (!formData.basePrice || parseFloat(formData.basePrice) <= 0) newErrors.basePrice = "Valid price is required"
    if (!formData.durationDays || parseInt(formData.durationDays) <= 0) newErrors.durationDays = "Duration must be greater than 0"

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!validateForm()) {
      toast.error("Please fill in all required fields correctly")
      return
    }

    try {
      setLoading(true)
      await adminApi.createPackage({
        title: formData.title,
        destination: formData.destination,
        basePrice: parseFloat(formData.basePrice),
        durationDays: parseInt(formData.durationDays),
        category: formData.category || undefined,
        description: formData.description || undefined,
        shortDescription: formData.shortDescription || undefined,
        currency: formData.currency || "USD",
        region: formData.region || undefined,
        featuredImageUrl: formData.featuredImageUrl || undefined,
        maxGroupSize: formData.maxGroupSize ? parseInt(formData.maxGroupSize) : undefined,
      })

      toast.success("Package created successfully")
      onOpenChange(false)
      setFormData({
        title: "",
        destination: "",
        basePrice: "",
        durationDays: "",
        category: "",
        description: "",
        shortDescription: "",
        currency: "USD",
        region: "",
        featuredImageUrl: "",
        maxGroupSize: "",
      })
      onSuccess?.()
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Failed to create package")
    } finally {
      setLoading(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Create New Package</DialogTitle>
          <DialogDescription>Add a new master package to the catalog</DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-6">
          {/* Basic Info */}
          <div className="space-y-4">
            <h3 className="font-semibold text-sm text-foreground">Basic Information</h3>
            <Separator />

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-foreground mb-1.5">Title *</label>
                <input
                  type="text"
                  name="title"
                  value={formData.title}
                  onChange={handleChange}
                  placeholder="e.g., Japan Sakura Experience"
                  className={`w-full px-3 py-2 text-sm border rounded-lg bg-background focus:outline-none focus:ring-2 ${
                    errors.title ? "border-red-500 focus:ring-red-500" : "border-border focus:ring-blue-500"
                  }`}
                />
                {errors.title && <p className="text-xs text-red-600 mt-1">{errors.title}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-foreground mb-1.5">Destination *</label>
                <input
                  type="text"
                  name="destination"
                  value={formData.destination}
                  onChange={handleChange}
                  placeholder="e.g., Japan"
                  className={`w-full px-3 py-2 text-sm border rounded-lg bg-background focus:outline-none focus:ring-2 ${
                    errors.destination ? "border-red-500 focus:ring-red-500" : "border-border focus:ring-blue-500"
                  }`}
                />
                {errors.destination && <p className="text-xs text-red-600 mt-1">{errors.destination}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-foreground mb-1.5">Region</label>
                <select
                  name="region"
                  value={formData.region}
                  onChange={handleChange}
                  className="w-full px-3 py-2 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="">Select Region</option>
                  <option value="Asia-Pacific">Asia-Pacific</option>
                  <option value="Europe">Europe</option>
                  <option value="Americas">Americas</option>
                  <option value="Africa">Africa</option>
                  <option value="Middle East">Middle East</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-foreground mb-1.5">Category</label>
                <select
                  name="category"
                  value={formData.category}
                  onChange={handleChange}
                  className="w-full px-3 py-2 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="">Select Category</option>
                  <option value="Beach">Beach & Resort</option>
                  <option value="Cultural">Cultural</option>
                  <option value="Adventure">Adventure</option>
                  <option value="Family">Family</option>
                  <option value="Luxury">Luxury</option>
                  <option value="Urban">Urban & City</option>
                  <option value="Nature">Nature & Hiking</option>
                </select>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Short Description</label>
              <input
                type="text"
                name="shortDescription"
                value={formData.shortDescription}
                onChange={handleChange}
                placeholder="Brief overview of the package"
                className="w-full px-3 py-2 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Description</label>
              <textarea
                name="description"
                value={formData.description}
                onChange={handleChange}
                placeholder="Detailed description of the package"
                rows={3}
                className="w-full px-3 py-2 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          {/* Pricing & Duration */}
          <div className="space-y-4">
            <h3 className="font-semibold text-sm text-foreground">Pricing & Duration</h3>
            <Separator />

            <div className="grid grid-cols-3 gap-4">
              <div>
                <label className="block text-sm font-medium text-foreground mb-1.5">Base Price *</label>
                <input
                  type="number"
                  name="basePrice"
                  value={formData.basePrice}
                  onChange={handleChange}
                  placeholder="0.00"
                  step="0.01"
                  className={`w-full px-3 py-2 text-sm border rounded-lg bg-background focus:outline-none focus:ring-2 ${
                    errors.basePrice ? "border-red-500 focus:ring-red-500" : "border-border focus:ring-blue-500"
                  }`}
                />
                {errors.basePrice && <p className="text-xs text-red-600 mt-1">{errors.basePrice}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-foreground mb-1.5">Currency</label>
                <select
                  name="currency"
                  value={formData.currency}
                  onChange={handleChange}
                  className="w-full px-3 py-2 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option>USD</option>
                  <option>EUR</option>
                  <option>GBP</option>
                  <option>MYR</option>
                  <option>SGD</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-foreground mb-1.5">Duration (Days) *</label>
                <input
                  type="number"
                  name="durationDays"
                  value={formData.durationDays}
                  onChange={handleChange}
                  placeholder="5"
                  className={`w-full px-3 py-2 text-sm border rounded-lg bg-background focus:outline-none focus:ring-2 ${
                    errors.durationDays ? "border-red-500 focus:ring-red-500" : "border-border focus:ring-blue-500"
                  }`}
                />
                {errors.durationDays && <p className="text-xs text-red-600 mt-1">{errors.durationDays}</p>}
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Max Group Size</label>
              <input
                type="number"
                name="maxGroupSize"
                value={formData.maxGroupSize}
                onChange={handleChange}
                placeholder="20"
                className="w-full px-3 py-2 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          {/* Media */}
          <div className="space-y-4">
            <h3 className="font-semibold text-sm text-foreground">Media</h3>
            <Separator />

            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Featured Image URL</label>
              <input
                type="url"
                name="featuredImageUrl"
                value={formData.featuredImageUrl}
                onChange={handleChange}
                placeholder="https://example.com/image.jpg"
                className="w-full px-3 py-2 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          {/* Actions */}
          <div className="flex gap-3 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
              disabled={loading}
            >
              Cancel
            </Button>
            <Button
              type="submit"
              className="bg-blue-600 hover:bg-blue-700 text-white"
              disabled={loading}
            >
              {loading ? "Creating..." : "Create Package"}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  )
}
