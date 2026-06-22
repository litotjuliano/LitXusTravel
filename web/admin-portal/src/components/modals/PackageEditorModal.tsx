"use client"

import { useState, useEffect, useRef } from "react"
import { Modal } from "@/components/ui/Modal"
import { Button } from "@/components/ui/button"
import { adminApi } from "@/lib/api"
import { toast } from "sonner"

interface PackageEditorModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onSuccess?: () => void
  tenantId?: string
  defaultCurrency?: string
  editPackageId?: string
  initialData?: {
    title: string; destination: string; basePrice: number; durationDays: number
    category: string; region: string; description: string; shortDescription: string
    featuredImageUrl: string; contactPhone: string; contactWhatsapp: string
    isOwnedPackage: boolean
  }
}

const inputCls = (hasError?: boolean) =>
  `w-full px-3 py-2 text-sm border rounded-lg bg-white dark:bg-gray-900 text-gray-900 dark:text-white focus:outline-none focus:ring-2 transition-colors ${
    hasError
      ? "border-red-400 focus:ring-red-400/40"
      : "border-gray-200 dark:border-gray-800 focus:ring-blue-500/40 hover:border-blue-400/60"
  }`

const selectCls =
  "w-full px-3 py-2 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-900 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-blue-500/40 hover:border-blue-400/60 transition-colors"

function Card({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className="bg-gray-100 dark:bg-gray-800 border border-gray-200 dark:border-gray-800 rounded-xl p-4 space-y-3">
      <p className="text-[11px] font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-widest">{title}</p>
      {children}
    </div>
  )
}

function Fld({ children }: { children: React.ReactNode }) {
  return <div className="space-y-1">{children}</div>
}

function Lbl({ children, req }: { children: React.ReactNode; req?: boolean }) {
  return (
    <label className="block text-sm font-medium text-gray-900 dark:text-white">
      {children}{req && <span className="text-red-500 ml-0.5">*</span>}
    </label>
  )
}

function Err({ msg }: { msg?: string }) {
  return msg ? <p className="text-xs text-red-500 mt-0.5">{msg}</p> : null
}

const ACCEPTED_TYPES = ["image/jpeg", "image/png", "image/webp"]
const MAX_FILE_SIZE_MB = 5
const MAX_DIM = 1200

function optimizeImage(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader()
    reader.onload = (e) => {
      const img = new window.Image()
      img.onload = () => {
        let { width, height } = img
        if (width > MAX_DIM || height > MAX_DIM) {
          if (width >= height) { height = Math.round(height * MAX_DIM / width); width = MAX_DIM }
          else { width = Math.round(width * MAX_DIM / height); height = MAX_DIM }
        }
        const canvas = document.createElement("canvas")
        canvas.width = width
        canvas.height = height
        canvas.getContext("2d")!.drawImage(img, 0, 0, width, height)
        resolve(canvas.toDataURL("image/jpeg", 0.82))
      }
      img.onerror = reject
      img.src = e.target?.result as string
    }
    reader.onerror = reject
    reader.readAsDataURL(file)
  })
}

export function PackageEditorModal({ open, onOpenChange, onSuccess, tenantId, defaultCurrency, editPackageId, initialData }: PackageEditorModalProps) {
  const resolvedCurrency = defaultCurrency || "MYR"
  const isTenantMode = !!tenantId
  const isEditMode = !!editPackageId
  const [loading, setLoading] = useState(false)
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [extendToMaster, setExtendToMaster] = useState(false)
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [imgProcessing, setImgProcessing] = useState(false)
  const [imgInfo, setImgInfo] = useState<{ name: string; kb: number } | null>(null)
  const [imgError, setImgError] = useState<string | null>(null)
  const [isGenerating, setIsGenerating] = useState(false)
  const [form, setForm] = useState({
    title: "", destination: "", basePrice: "", durationDays: "",
    category: "", description: "", shortDescription: "",
    region: "", featuredImageUrl: "", maxGroupSize: "",
    contactPhone: "", contactWhatsapp: "",
  })

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
      setImgInfo(null)
      setImgError(null)
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
    setImgInfo(null)
    setImgError(null)
  }

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return
    setImgError(null)
    if (!ACCEPTED_TYPES.includes(file.type)) {
      setImgError("Only JPEG, PNG, or WEBP images are accepted.")
      return
    }
    if (file.size > MAX_FILE_SIZE_MB * 1024 * 1024) {
      setImgError(`File exceeds ${MAX_FILE_SIZE_MB}MB limit.`)
      return
    }
    setImgProcessing(true)
    try {
      const dataUrl = await optimizeImage(file)
      const kb = Math.round(dataUrl.length * 0.75 / 1024)
      setForm((prev) => ({ ...prev, featuredImageUrl: dataUrl }))
      setImgInfo({ name: file.name, kb })
    } catch {
      setImgError("Failed to process image. Try another file.")
    } finally {
      setImgProcessing(false)
      e.target.value = ""
    }
  }

  const handleGeneratePhoto = async () => {
    if (!editPackageId) return
    setIsGenerating(true)
    try {
      const res = tenantId
        ? await adminApi.generatePackagePhoto(tenantId, editPackageId)
        : await adminApi.generateAdminPackagePhoto(editPackageId)
      const url = res.data.featuredImageUrl
      setForm((prev) => ({ ...prev, featuredImageUrl: url }))
      setImgInfo(null)
      toast.success("Photo generated from Unsplash")
    } catch {
      toast.error("Photo generation failed. Check if Unsplash API key is configured.")
    } finally {
      setIsGenerating(false)
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!validate()) { toast.error("Please fix the errors before submitting"); return }
    try {
      setLoading(true)

      if (isEditMode && !tenantId && editPackageId) {
        await adminApi.updatePackage(editPackageId, {
          title: form.title,
          destination: form.destination,
          basePrice: parseFloat(form.basePrice),
          durationDays: parseInt(form.durationDays),
          category: form.category || undefined,
          description: form.description || undefined,
          shortDescription: form.shortDescription || undefined,
          region: form.region || undefined,
          featuredImageUrl: form.featuredImageUrl || undefined,
          contactPhone: form.contactPhone || undefined,
          contactWhatsapp: form.contactWhatsapp || undefined,
        })
        toast.success("Package updated successfully")
      } else if (isEditMode && tenantId && editPackageId) {
        await adminApi.updatePackageOverride(tenantId, editPackageId, {
          title: form.title || undefined,
          price: form.basePrice ? parseFloat(form.basePrice) : undefined,
          currency: resolvedCurrency,
          description: form.description || undefined,
          shortDescription: form.shortDescription || undefined,
          featuredImageUrl: form.featuredImageUrl || undefined,
          contactPhone: form.contactPhone || undefined,
          contactWhatsapp: form.contactWhatsapp || undefined,
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
    <Modal
      isOpen={open}
      onClose={() => { reset(); onOpenChange(false) }}
      showCloseButton={false}
      className="rounded-2xl max-w-[920px] flex flex-col max-h-[90vh] overflow-hidden"
    >
      {/* Fixed header */}
      <div className="px-6 py-5 border-b border-gray-200 dark:border-gray-800 shrink-0">
        <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
          {isEditMode ? "Edit Package" : isTenantMode ? "Create Portal Package" : "Create Master Package"}
        </h2>
        <p className="text-sm text-gray-500 dark:text-gray-400 mt-0.5">
          {isEditMode
            ? "Update the package details. Changes are saved to your portal."
            : isTenantMode
              ? "Create a package exclusive to your portal, or extend it to the master catalog."
              : "Add a new master package to the shared catalog."}
        </p>
      </div>

      {/* Scrollable body + fixed footer inside form */}
      <form onSubmit={handleSubmit} className="flex flex-col flex-1 overflow-hidden">
        <div className="flex-1 overflow-y-auto px-6 py-5">
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
                    : "bg-gray-100 dark:bg-gray-800 border-gray-200 dark:border-gray-800"
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
                      extendToMaster ? "text-blue-700 dark:text-blue-400" : "text-gray-900 dark:text-white"
                    }`}>
                      Extend to Master Catalog
                    </p>
                    <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5 leading-relaxed">
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
                  <Lbl>Featured Image</Lbl>
                  <input
                    ref={fileInputRef}
                    type="file"
                    accept="image/jpeg,image/png,image/webp"
                    onChange={handleFileChange}
                    className="hidden"
                  />

                  {/* Preview */}
                  {form.featuredImageUrl && (
                    <div className="relative rounded-lg overflow-hidden border border-gray-200 dark:border-gray-800 aspect-video bg-gray-100 dark:bg-gray-800 mb-2">
                      {/* eslint-disable-next-line @next/next/no-img-element */}
                      <img
                        src={form.featuredImageUrl}
                        alt="Preview"
                        className="w-full h-full object-cover"
                        onError={(e) => { (e.target as HTMLImageElement).style.display = "none" }}
                      />
                      <button
                        type="button"
                        onClick={() => { setForm((p) => ({ ...p, featuredImageUrl: "" })); setImgInfo(null) }}
                        className="absolute top-2 right-2 bg-black/60 hover:bg-black/80 text-white rounded-full w-6 h-6 text-xs flex items-center justify-center"
                        title="Remove image"
                      >✕</button>
                    </div>
                  )}

                  {/* Upload zone */}
                  <button
                    type="button"
                    onClick={() => fileInputRef.current?.click()}
                    disabled={imgProcessing}
                    className={`w-full flex flex-col items-center justify-center gap-1.5 rounded-lg border-2 border-dashed px-4 py-5 text-sm transition-colors ${
                      imgProcessing
                        ? "border-gray-200 dark:border-gray-800 text-gray-500 dark:text-gray-400 cursor-wait"
                        : "border-gray-200 dark:border-gray-800 hover:border-blue-400 hover:bg-blue-50/10 text-gray-500 dark:text-gray-400 cursor-pointer"
                    }`}
                  >
                    {imgProcessing ? (
                      <>
                        <svg className="animate-spin h-5 w-5" viewBox="0 0 24 24" fill="none">
                          <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"/>
                          <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"/>
                        </svg>
                        <span>Optimizing image…</span>
                      </>
                    ) : (
                      <>
                        <svg className="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"/>
                        </svg>
                        <span>{form.featuredImageUrl ? "Replace image" : "Click to upload"}</span>
                        <span className="text-xs">JPEG · PNG · WEBP · max {MAX_FILE_SIZE_MB}MB</span>
                      </>
                    )}
                  </button>

                  {imgInfo && (
                    <p className="text-xs text-gray-500 dark:text-gray-400 mt-1">
                      {imgInfo.name} · optimized to ~{imgInfo.kb}KB
                    </p>
                  )}
                  {imgError && <p className="text-xs text-red-500 mt-1">{imgError}</p>}

                  {/* Generate Photo — available in edit mode for both tenant and admin */}
                  {isEditMode && (
                    <button
                      type="button"
                      onClick={handleGeneratePhoto}
                      disabled={isGenerating || imgProcessing}
                      className="mt-2 w-full flex items-center justify-center gap-2 rounded-lg border border-gray-200 dark:border-gray-800 px-3 py-2 text-sm text-gray-500 dark:text-gray-400 hover:border-purple-400 hover:text-purple-600 hover:bg-purple-50/10 transition-colors disabled:opacity-50 disabled:cursor-wait"
                    >
                      {isGenerating ? (
                        <>
                          <svg className="animate-spin h-4 w-4" viewBox="0 0 24 24" fill="none">
                            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"/>
                            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"/>
                          </svg>
                          Searching Unsplash…
                        </>
                      ) : (
                        <>
                          <svg className="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M5 3l14 9-14 9V3z"/>
                          </svg>
                          Generate Photo
                        </>
                      )}
                    </button>
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
        <div className="shrink-0 px-6 py-4 border-t border-gray-200 dark:border-gray-800 flex items-center justify-between">
          <p className="text-xs text-gray-500 dark:text-gray-400">
            Fields marked <span className="text-red-500">*</span> are required
          </p>
          <div className="flex gap-2">
            <Button type="button" variant="outline" onClick={() => { reset(); onOpenChange(false) }} disabled={loading}>
              Cancel
            </Button>
            <Button
              type="submit"
              variant="primary"
              className="min-w-[130px]"
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
    </Modal>
  )
}
