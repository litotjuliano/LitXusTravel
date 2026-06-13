"use client"

import { useState, useEffect } from "react"
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { adminApi } from "@/lib/api"
import { toast } from "sonner"

interface PackageEditorModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onSuccess?: () => void
  tenantId?: string
  defaultCurrency?: string
  // Edit mode — when provided, the modal saves changes instead of creating
  editPackageId?: string
  initialData?: {
    title: string; destination: string; basePrice: number; durationDays: number
    category: string; region: string; description: string; shortDescription: string
    featuredImageUrl: string; contactPhone: string; contactWhatsapp: string
    isOwnedPackage: boolean
  }
}

const inputCls = (hasError?: boolean) =>
  `w-full px-3 py-2 text-sm border rounded-lg bg-background focus:outline-none focus:ring-2 transition-colors ${
    hasError
      ? "border-red-400 focus:ring-red-400/40"
      : "border-border focus:ring-blue-500/40 hover:border-blue-400/60"
  }`

const selectCls =
  "w-full px-3 py-2 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-blue-500/40 hover:border-blue-400/60 transition-colors"

function Card({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className="bg-muted border border-border rounded-xl p-4 space-y-3">
      <p className="text-[11px] font-semibold text-muted-foreground uppercase tracking-widest">{title}</p>
      {children}
    </div>
  )
}

function Fld({ children }: { children: React.ReactNode }) {
  return <div className="space-y-1">{children}</div>
}

function Lbl({ children, req }: { children: React.ReactNode; req?: boolean }) {
  return (
    <label className="block text-sm font-medium text-foreground">
      {children}{req && <span className="text-red-500 ml-0.5">*</span>}
    </label>
  )
}

function Err({ msg }: { msg?: string }) {
  return msg ? <p className="text-xs text-red-500 mt-0.5">{msg}</p> : null
}

export function PackageEditorModal({ open, onOpenChange, onSuccess, tenantId, defaultCurrency, editPackageId, initialData }: PackageEditorModalProps) {
  const resolvedCurrency = defaultCurrency ?? "MYR"
  const isTenantMode = !!tenantId
  const isEditMode = !!editPackageId
  const [loading, setLoading] = useState(false)
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [extendToMaster, setExtendToMaster] = useState(false)
  const [form, setForm] = useState({
    title: "", destination: "", basePrice: "", durationDays: "",
    category: "", description: "", shortDescription: "",
    region: "", featuredImageUrl: "", maxGroupSize: "",
    contactPhone: "", contactWhatsapp: "",
  })

  // Pre-populate form when opening in edit mode
  useEffect(() => {
    if (open && isEditMode && initialData) {
      setForm({
        title: initialData.title,
        destination: initialData.destination,
        basePrice: String(initialData.basePrice),
        durationDays: String(initialData.durationDays),
        category: initialData.category,
        description: initialData.description,
        shortDescription: initialData.shortDescription,
        region: initialData.region,
        featuredImageUrl: initialData.featuredImageUrl,
        maxGroupSize: "",
        contactPhone: initialData.contactPhone,
        contactWhatsapp: initialData.contactWhatsapp,
      })
      setErrors({})
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, editPackageId])

  const set = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target
    setForm((prev) => ({ ...prev, [name]: value }))
  }

  const validate = (): boolean => {
    const e: Record<string, string> = {}
    if (!form.title.trim()) e.title = "Title is required"
    if (!form.destination.trim()) e.destination = "Destination is required"
    if (!form.basePrice || parseFloat(form.basePrice) <= 0) e.basePrice = "Enter a valid price"
    if (!form.durationDays || parseInt(form.durationDays) <= 0) e.durationDays = "Must be at least 1 day"
    setErrors(e)
    return Object.keys(e).length === 0
  }

  const reset = () => {
    setForm({
      title: "", destination: "", basePrice: "", durationDays: "",
      category: "", description: "", shortDescription: "",
      region: "", featuredImageUrl: "", maxGroupSize: "",
      contactPhone: "", contactWhatsapp: "",
    })
    setExtendToMaster(false)
    setErrors({})
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!validate()) { toast.error("Please fix the errors before submitting"); return }
    try {
      setLoading(true)

      if (isEditMode && tenantId && editPackageId) {
        await adminApi.updatePackageOverride(tenantId, editPackageId, {
          title: form.title || undefined,
          price: form.basePrice ? parseFloat(form.basePrice) : undefined,
          currency: resolvedCurrency,
          description: form.description || undefined,
          shortDescription: form.shortDescription || undefined,
          featuredImageUrl: form.featuredImageUrl || undefined,
          contactPhone: form.contactPhone || undefined,
          contactWhatsapp: form.contactWhatsapp || undefined,
          // Owned-package fields (only submitted when relevant, ignored by backend for synced)
          destination: form.destination || undefined,
          durationDays: form.durationDays ? parseInt(form.durationDays) : undefined,
          category: form.category || undefined,
          region: form.region || undefined,
        })
        toast.success("Package updated successfully")
      } else if (isTenantMode) {
        await adminApi.createTenantPackage(tenantId!, {
          title: form.title,
          destination: form.destination,
          durationDays: parseInt(form.durationDays),
          price: parseFloat(form.basePrice),
          currency: resolvedCurrency,
          category: form.category || undefined,
          region: form.region || undefined,
          description: form.description || undefined,
          shortDescription: form.shortDescription || undefined,
          featuredImageUrl: form.featuredImageUrl || undefined,
          contactPhone: form.contactPhone || undefined,
          contactWhatsapp: form.contactWhatsapp || undefined,
          extendToMaster,
        })
        toast.success("Package created successfully")
      } else {
        await adminApi.createPackage({
          title: form.title,
          destination: form.destination,
          basePrice: parseFloat(form.basePrice),
          durationDays: parseInt(form.durationDays),
          category: form.category || undefined,
          description: form.description || undefined,
          shortDescription: form.shortDescription || undefined,
          currency: resolvedCurrency,
          region: form.region || undefined,
          featuredImageUrl: form.featuredImageUrl || undefined,
          maxGroupSize: form.maxGroupSize ? parseInt(form.maxGroupSize) : undefined,
        })
        toast.success("Package created successfully")
      }

      onOpenChange(false)
      reset()
      onSuccess?.()
    } catch (err) {
      toast.error(err instanceof Error ? err.message : isEditMode ? "Failed to update package" : "Failed to create package")
    } finally {
      setLoading(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={(v) => { if (!v) reset(); onOpenChange(v) }}>
      <DialogContent className="sm:max-w-[920px] w-full flex flex-col max-h-[92vh] p-0 gap-0 overflow-hidden bg-background text-foreground backdrop-blur-none">

        {/* Fixed header */}
        <DialogHeader className="px-6 py-5 border-b border-border shrink-0 bg-background">
          <DialogTitle className="text-lg font-semibold">
            {isEditMode ? "Edit Package" : isTenantMode ? "Create Portal Package" : "Create Master Package"}
          </DialogTitle>
          <DialogDescription className="text-sm text-muted-foreground mt-0.5">
            {isEditMode
              ? "Update the package details. Changes are saved to your portal."
              : isTenantMode
                ? "Create a package exclusive to your portal, or extend it to the master catalog."
                : "Add a new master package to the shared catalog."}
          </DialogDescription>
        </DialogHeader>

        {/* Scrollable body */}
        <form onSubmit={handleSubmit} className="flex flex-col flex-1 overflow-hidden">
          <div className="flex-1 overflow-y-auto px-6 py-5 bg-background">
            <div className="grid grid-cols-[1fr_300px] gap-4 items-start">

              {/* ── Left column ── */}
              <div className="space-y-4">
                <Card title="Basic Information">
                  <Fld>
                    <Lbl req>Package Title</Lbl>
                    <input name="title" value={form.title} onChange={set}
                      placeholder="e.g., Japan Sakura Experience"
                      className={inputCls(!!errors.title)} />
                    <Err msg={errors.title} />
                  </Fld>

                  <div className="grid grid-cols-2 gap-3">
                    <Fld>
                      <Lbl req>Destination</Lbl>
                      <input name="destination" value={form.destination} onChange={set}
                        placeholder="e.g., Japan"
                        className={inputCls(!!errors.destination)} />
                      <Err msg={errors.destination} />
                    </Fld>
                    <Fld>
                      <Lbl>Region</Lbl>
                      <select name="region" value={form.region} onChange={set} className={selectCls}>
                        <option value="">Select region</option>
                        <option>Asia-Pacific</option>
                        <option>Europe</option>
                        <option>Americas</option>
                        <option>Africa</option>
                        <option>Middle East</option>
                      </select>
                    </Fld>
                  </div>

                  <Fld>
                    <Lbl>Category</Lbl>
                    <select name="category" value={form.category} onChange={set} className={selectCls}>
                      <option value="">Select category</option>
                      <option value="Beach">Beach &amp; Resort</option>
                      <option value="Cultural">Cultural</option>
                      <option value="Adventure">Adventure</option>
                      <option value="Family">Family</option>
                      <option value="Luxury">Luxury</option>
                      <option value="Urban">Urban &amp; City</option>
                      <option value="Nature">Nature &amp; Hiking</option>
                    </select>
                  </Fld>
                </Card>

                <Card title="Description">
                  <Fld>
                    <Lbl>Short Description</Lbl>
                    <input name="shortDescription" value={form.shortDescription} onChange={set}
                      placeholder="Brief overview shown in listings"
                      className={inputCls()} />
                  </Fld>
                  <Fld>
                    <Lbl>Full Description</Lbl>
                    <textarea name="description" value={form.description} onChange={set}
                      placeholder="Detailed itinerary, highlights, and inclusions"
                      rows={6}
                      className={`${inputCls()} resize-none`} />
                  </Fld>
                </Card>

                {/* Extend to Master toggle — create mode only */}
                {isTenantMode && !isEditMode && (
                  <div className={`flex items-start gap-4 rounded-xl border p-4 transition-all ${
                    extendToMaster
                      ? "bg-blue-50 border-blue-200 dark:bg-blue-950/30 dark:border-blue-800"
                      : "bg-muted border-border"
                  }`}>
                    <button
                      type="button"
                      role="switch"
                      aria-checked={extendToMaster}
                      onClick={() => setExtendToMaster((v) => !v)}
                      className={`relative mt-0.5 w-11 h-6 rounded-full transition-colors shrink-0 focus:outline-none focus-visible:ring-2 focus-visible:ring-blue-500 ${
                        extendToMaster ? "bg-blue-600" : "bg-gray-300 dark:bg-gray-600"
                      }`}
                    >
                      <span className={`absolute top-1 left-1 w-4 h-4 bg-white rounded-full shadow-sm transition-transform ${
                        extendToMaster ? "translate-x-5" : "translate-x-0"
                      }`} />
                    </button>
                    <div>
                      <p className={`text-sm font-semibold ${
                        extendToMaster ? "text-blue-700 dark:text-blue-400" : "text-foreground"
                      }`}>
                        Extend to Master Catalog
                      </p>
                      <p className="text-xs text-muted-foreground mt-0.5 leading-relaxed">
                        {extendToMaster
                          ? "This package will be added to the master catalog and can be synced to other tenants."
                          : "This package stays private to your portal. Other tenants cannot see or sync it."}
                      </p>
                    </div>
                  </div>
                )}
              </div>

              {/* ── Right column ── */}
              <div className="space-y-4">
                <Card title="Pricing &amp; Duration">
                  <Fld>
                    <Lbl req>Base Price ({resolvedCurrency})</Lbl>
                    <input type="number" name="basePrice" value={form.basePrice} onChange={set}
                      placeholder="0.00" step="0.01" min="0"
                      className={inputCls(!!errors.basePrice)} />
                    <Err msg={errors.basePrice} />
                  </Fld>

                  <Fld>
                    <Lbl req>Duration (Days)</Lbl>
                    <input type="number" name="durationDays" value={form.durationDays} onChange={set}
                      placeholder="5" min="1"
                      className={inputCls(!!errors.durationDays)} />
                    <Err msg={errors.durationDays} />
                  </Fld>

                  <Fld>
                    <Lbl>Max Group Size</Lbl>
                    <input type="number" name="maxGroupSize" value={form.maxGroupSize} onChange={set}
                      placeholder="20" min="1"
                      className={inputCls()} />
                  </Fld>
                </Card>

                <Card title="Media">
                  <Fld>
                    <Lbl>Featured Image URL</Lbl>
                    <input type="url" name="featuredImageUrl" value={form.featuredImageUrl} onChange={set}
                      placeholder="https://example.com/image.jpg"
                      className={inputCls()} />
                    {form.featuredImageUrl && (
                      <div className="mt-2 rounded-lg overflow-hidden border border-border aspect-video bg-muted">
                        {/* eslint-disable-next-line @next/next/no-img-element */}
                        <img
                          src={form.featuredImageUrl}
                          alt="Preview"
                          className="w-full h-full object-cover"
                          onError={(e) => { (e.target as HTMLImageElement).style.display = "none" }}
                        />
                      </div>
                    )}
                  </Fld>
                </Card>

                {(isTenantMode || isEditMode) && (
                  <Card title="Contact Details">
                    <Fld>
                      <Lbl>Phone</Lbl>
                      <input type="text" name="contactPhone" value={form.contactPhone} onChange={set}
                        placeholder="+60 12-345 6789"
                        className={inputCls()} />
                    </Fld>
                    <Fld>
                      <Lbl>WhatsApp</Lbl>
                      <input type="text" name="contactWhatsapp" value={form.contactWhatsapp} onChange={set}
                        placeholder="+60 12-345 6789"
                        className={inputCls()} />
                    </Fld>
                  </Card>
                )}
              </div>

            </div>
          </div>

          {/* Fixed footer */}
          <div className="shrink-0 px-6 py-4 border-t border-border bg-background flex items-center justify-between">
            <p className="text-xs text-muted-foreground">
              Fields marked <span className="text-red-500">*</span> are required
            </p>
            <div className="flex gap-2">
              <Button type="button" variant="outline" onClick={() => onOpenChange(false)} disabled={loading}>
                Cancel
              </Button>
              <Button
                type="submit"
                className="bg-blue-600 hover:bg-blue-700 text-white min-w-[130px]"
                disabled={loading}
              >
                {loading ? (
                  <span className="flex items-center gap-2">
                    <svg className="animate-spin h-4 w-4" viewBox="0 0 24 24" fill="none">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z" />
                    </svg>
                    {isEditMode ? "Saving…" : "Creating…"}
                  </span>
                ) : isEditMode ? "Save Changes" : "Create Package"}
              </Button>
            </div>
          </div>
        </form>

      </DialogContent>
    </Dialog>
  )
}
