"use client"

import { useState } from "react"
import { Loader } from "lucide-react"
import { Modal } from "@/components/ui/Modal"
import { adminApi } from "@/lib/api"
import { toast } from "sonner"

const inputClass =
  "w-full px-3 py-2 text-sm border border-gray-200 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-(--color-brand-blue)"
const labelClass = "block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"

interface Props {
  open: boolean
  onClose: () => void
  onSuccess: () => void
}

export function CreateTenantModal({ open, onClose, onSuccess }: Props) {
  const [name, setName] = useState("")
  const [email, setEmail] = useState("")
  const [phone, setPhone] = useState("")
  const [country, setCountry] = useState("")
  const [loading, setLoading] = useState(false)
  const [errors, setErrors] = useState<Record<string, string>>({})

  function validate() {
    const e: Record<string, string> = {}
    if (!name.trim()) e.name = "Name is required"
    if (!email.trim()) e.email = "Email is required"
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) e.email = "Invalid email address"
    return e
  }

  async function handleSubmit() {
    const e = validate()
    setErrors(e)
    if (Object.keys(e).length > 0) return

    setLoading(true)
    try {
      await adminApi.createTenant({
        name: name.trim(),
        email: email.trim(),
        phone: phone.trim() || undefined,
        country: country.trim() || undefined,
      })
      toast.success(`Tenant "${name}" created successfully`)
      handleClose()
      onSuccess()
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : "Failed to create tenant")
    } finally {
      setLoading(false)
    }
  }

  function handleClose() {
    setName(""); setEmail(""); setPhone(""); setCountry("")
    setErrors({})
    onClose()
  }

  return (
    <Modal isOpen={open} onClose={handleClose} className="rounded-2xl max-w-md w-full p-6 shadow-xl">
      <h2 className="text-lg font-semibold text-gray-900 dark:text-white mb-1">New Tenant</h2>
      <p className="text-sm text-gray-500 dark:text-gray-400 mb-6">
        Create a new tenant agency on the platform.
      </p>

      <div className="space-y-4">
        <div>
          <label className={labelClass}>Agency Name <span className="text-red-500">*</span></label>
          <input
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="e.g. Horizon Travel"
            className={inputClass}
          />
          {errors.name && <p className="text-xs text-red-500 mt-1">{errors.name}</p>}
        </div>

        <div>
          <label className={labelClass}>Contact Email <span className="text-red-500">*</span></label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="admin@horizontravel.com"
            className={inputClass}
          />
          {errors.email && <p className="text-xs text-red-500 mt-1">{errors.email}</p>}
        </div>

        <div>
          <label className={labelClass}>Phone <span className="text-gray-400 font-normal">(optional)</span></label>
          <input
            type="tel"
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
            placeholder="+60 12-345 6789"
            className={inputClass}
          />
        </div>

        <div>
          <label className={labelClass}>Country <span className="text-gray-400 font-normal">(optional)</span></label>
          <input
            value={country}
            onChange={(e) => setCountry(e.target.value)}
            placeholder="e.g. Malaysia"
            className={inputClass}
          />
        </div>
      </div>

      <div className="flex gap-3 mt-6">
        <button
          onClick={handleClose}
          className="flex-1 px-4 py-2.5 text-sm font-medium border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors text-gray-700 dark:text-gray-300"
        >
          Cancel
        </button>
        <button
          onClick={handleSubmit}
          disabled={loading}
          className="flex-1 flex items-center justify-center gap-2 px-4 py-2.5 text-sm font-semibold bg-(--color-brand-blue) hover:bg-blue-700 disabled:opacity-60 text-white rounded-lg transition-colors"
        >
          {loading ? <><Loader size={14} className="animate-spin" /> Creating…</> : "Create Tenant"}
        </button>
      </div>
    </Modal>
  )
}
