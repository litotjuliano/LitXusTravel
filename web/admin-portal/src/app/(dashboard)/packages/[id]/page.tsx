"use client"

import { useEffect, useState } from "react"
import { useParams, useRouter } from "next/navigation"
import { ArrowLeft, Loader, ExternalLink } from "lucide-react"
import Link from "next/link"
import { Button } from "@/components/ui/button"
import { adminApi } from "@/lib/api"
import { toast } from "sonner"

type PackageData = {
  id: string
  title: string
  destination: string
  basePrice: number
  durationDays: number
  category: string
  region?: string
  description?: string
  shortDescription?: string
  featuredImageUrl?: string
  contactPhone?: string
  contactWhatsapp?: string
  visibility: string
}

const CATEGORIES = ["Adventure", "Cultural", "Family", "Honeymoon", "Beach", "Business", "Religious", "Eco-Tourism"]

function FormSection({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className="p-6 bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl space-y-5">
      <h3 className="text-base font-bold text-gray-900 dark:text-white">{title}</h3>
      {children}
    </div>
  )
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1.5">{label}</label>
      {children}
    </div>
  )
}

const inputCls = "w-full px-3 py-2.5 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-950 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-(--color-brand-blue) transition-colors"

export default function PackageEditorPage() {
  const { id } = useParams<{ id: string }>()
  const router = useRouter()
  const isNew = id === "new"

  const [loading, setLoading] = useState(!isNew)
  const [saving, setSaving] = useState(false)
  const [form, setForm] = useState<Partial<PackageData>>({ visibility: "Draft" })

  useEffect(() => {
    if (isNew) return
    setLoading(true)
    adminApi.getPackageById(id)
      .then((res) => setForm(res.data))
      .catch(() => toast.error("Failed to load package"))
      .finally(() => setLoading(false))
  }, [id, isNew])

  const set = (field: keyof PackageData, value: string | number) =>
    setForm((prev) => ({ ...prev, [field]: value }))

  const handleSave = async () => {
    setSaving(true)
    try {
      if (isNew) {
        await adminApi.createPackage(form)
        toast.success("Package created")
        router.push("/packages")
      } else {
        await adminApi.updatePackage(id, form)
        toast.success("Changes saved")
      }
    } catch (e) {
      toast.error(e instanceof Error ? e.message : "Save failed")
    } finally {
      setSaving(false)
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <Loader className="animate-spin text-gray-400 mr-2" size={20} />
        <span className="text-gray-500 dark:text-gray-400 text-sm">Loading package...</span>
      </div>
    )
  }

  return (
    <div className="space-y-5">
      {/* Breadcrumb */}
      <div className="flex items-center gap-3">
        <Link href="/packages" className="p-2 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors">
          <ArrowLeft size={18} className="text-gray-500 dark:text-gray-400" />
        </Link>
        <div>
          <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
            {isNew ? "New Package" : (form.title || "Edit Package")}
          </h2>
          <p className="text-sm text-gray-500 dark:text-gray-400">
            {isNew ? "Create a new master package" : "Master Package"}
          </p>
        </div>
      </div>

      {/* 2-column layout */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-5">
        {/* Main content — 2/3 */}
        <div className="lg:col-span-2 space-y-5">
          <FormSection title="Basic Information">
            <Field label="Title">
              <input className={inputCls} value={form.title ?? ""} onChange={(e) => set("title", e.target.value)} placeholder="e.g. Japan Cherry Blossom Tour" />
            </Field>
            <div className="grid grid-cols-2 gap-4">
              <Field label="Destination">
                <input className={inputCls} value={form.destination ?? ""} onChange={(e) => set("destination", e.target.value)} placeholder="e.g. Tokyo, Japan" />
              </Field>
              <Field label="Region">
                <input className={inputCls} value={form.region ?? ""} onChange={(e) => set("region", e.target.value)} placeholder="e.g. East Asia" />
              </Field>
            </div>
            <Field label="Category">
              <select className={inputCls} value={form.category ?? ""} onChange={(e) => set("category", e.target.value)}>
                <option value="">Select category</option>
                {CATEGORIES.map((c) => <option key={c} value={c}>{c}</option>)}
              </select>
            </Field>
            <Field label="Short Description">
              <input className={inputCls} value={form.shortDescription ?? ""} onChange={(e) => set("shortDescription", e.target.value)} placeholder="One-line teaser shown on cards" />
            </Field>
            <Field label="Description">
              <textarea
                className={`${inputCls} resize-none`}
                rows={5}
                value={form.description ?? ""}
                onChange={(e) => set("description", e.target.value)}
                placeholder="Full package description..."
              />
            </Field>
          </FormSection>

          <FormSection title="Pricing & Duration">
            <div className="grid grid-cols-2 gap-4">
              <Field label="Base Price (RM)">
                <input className={inputCls} type="number" min={0} value={form.basePrice ?? ""} onChange={(e) => set("basePrice", Number(e.target.value))} placeholder="0" />
              </Field>
              <Field label="Duration (days)">
                <input className={inputCls} type="number" min={1} value={form.durationDays ?? ""} onChange={(e) => set("durationDays", Number(e.target.value))} placeholder="1" />
              </Field>
            </div>
          </FormSection>

          <FormSection title="Media">
            <Field label="Featured Image URL">
              <input className={inputCls} value={form.featuredImageUrl ?? ""} onChange={(e) => set("featuredImageUrl", e.target.value)} placeholder="https://..." />
            </Field>
            {form.featuredImageUrl && (
              // eslint-disable-next-line @next/next/no-img-element
              <img src={form.featuredImageUrl} alt="preview" className="w-full h-48 object-cover rounded-lg border border-gray-200 dark:border-gray-800" />
            )}
          </FormSection>

          <FormSection title="Contact">
            <div className="grid grid-cols-2 gap-4">
              <Field label="Phone">
                <input className={inputCls} value={form.contactPhone ?? ""} onChange={(e) => set("contactPhone", e.target.value)} placeholder="+60..." />
              </Field>
              <Field label="WhatsApp">
                <input className={inputCls} value={form.contactWhatsapp ?? ""} onChange={(e) => set("contactWhatsapp", e.target.value)} placeholder="+60..." />
              </Field>
            </div>
          </FormSection>
        </div>

        {/* Sidebar — 1/3 */}
        <div className="space-y-4">
          {/* Status panel */}
          <div className="p-5 bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl space-y-4">
            <h3 className="text-sm font-semibold text-gray-900 dark:text-white">Status</h3>
            <select
              className={inputCls}
              value={form.visibility ?? "Draft"}
              onChange={(e) => set("visibility", e.target.value)}
            >
              <option value="Draft">Draft</option>
              <option value="Published">Published</option>
              <option value="Archived">Archived</option>
            </select>
            <Button
              onClick={handleSave}
              disabled={saving}
              className="w-full bg-(--color-brand-blue) hover:bg-blue-700 text-white font-semibold"
            >
              {saving ? <><Loader size={14} className="animate-spin mr-2" />Saving...</> : "Save Changes"}
            </Button>
            {!isNew && (
              <button className="w-full flex items-center justify-center gap-2 px-4 py-2.5 text-sm font-medium border border-gray-200 dark:border-gray-800 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors">
                <ExternalLink size={14} />
                Preview
              </button>
            )}
          </div>

          {/* SEO panel */}
          <div className="p-5 bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl space-y-3">
            <h3 className="text-sm font-semibold text-gray-900 dark:text-white">SEO</h3>
            <p className="text-xs text-gray-500 dark:text-gray-400">
              Title and description are auto-generated from package fields. Override via tenant settings if needed.
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}
