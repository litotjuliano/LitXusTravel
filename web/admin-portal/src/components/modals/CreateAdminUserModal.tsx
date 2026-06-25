"use client"

import { useState } from "react"
import { Loader } from "lucide-react"
import { Modal } from "@/components/ui/Modal"
import { adminApi } from "@/lib/api"
import { useTenants } from "@/lib/hooks/useTenants"
import { toast } from "sonner"

const inputClass =
  "w-full px-3 py-2 text-sm border border-gray-200 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-(--color-brand-blue)"
const labelClass = "block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"

interface Props {
  open: boolean
  onClose: () => void
  onSuccess: () => void
}

export function CreateAdminUserModal({ open, onClose, onSuccess }: Props) {
  const [name, setName] = useState("")
  const [email, setEmail] = useState("")
  const [scope, setScope] = useState<"Platform" | "Tenant">("Platform")
  const [assignedTenantId, setAssignedTenantId] = useState("")
  const [loading, setLoading] = useState(false)
  const [errors, setErrors] = useState<Record<string, string>>({})

  const { tenants } = useTenants(1, 200)

  function validate() {
    const e: Record<string, string> = {}
    if (!name.trim()) e.name = "Name is required"
    if (!email.trim()) e.email = "Email is required"
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) e.email = "Invalid email address"
    if (scope === "Tenant" && !assignedTenantId) e.tenant = "Please select a tenant"
    return e
  }

  async function handleSubmit() {
    const e = validate()
    setErrors(e)
    if (Object.keys(e).length > 0) return

    setLoading(true)
    try {
      await adminApi.createAdminUser({
        name: name.trim(),
        email: email.trim(),
        role: "Admin",
        scope,
        assignedTenantId: scope === "Tenant" ? assignedTenantId : undefined,
      })
      toast.success(`Admin user "${name}" created successfully`)
      handleClose()
      onSuccess()
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : "Failed to create admin user")
    } finally {
      setLoading(false)
    }
  }

  function handleClose() {
    setName(""); setEmail(""); setScope("Platform"); setAssignedTenantId("")
    setErrors({})
    onClose()
  }

  return (
    <Modal isOpen={open} onClose={handleClose} className="rounded-2xl max-w-md w-full p-6 shadow-xl">
      <h2 className="text-lg font-semibold text-gray-900 dark:text-white mb-1">Invite Admin</h2>
      <p className="text-sm text-gray-500 dark:text-gray-400 mb-6">
        Create a new admin user with platform or tenant-level access.
      </p>

      <div className="space-y-4">
        <div>
          <label className={labelClass}>Full Name <span className="text-red-500">*</span></label>
          <input
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="e.g. Jane Smith"
            className={inputClass}
          />
          {errors.name && <p className="text-xs text-red-500 mt-1">{errors.name}</p>}
        </div>

        <div>
          <label className={labelClass}>Email Address <span className="text-red-500">*</span></label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="jane@example.com"
            className={inputClass}
          />
          {errors.email && <p className="text-xs text-red-500 mt-1">{errors.email}</p>}
        </div>

        <div>
          <label className={labelClass}>Access Scope <span className="text-red-500">*</span></label>
          <select
            value={scope}
            onChange={(e) => { setScope(e.target.value as "Platform" | "Tenant"); setAssignedTenantId("") }}
            className={inputClass}
          >
            <option value="Platform">Platform Admin — full platform access</option>
            <option value="Tenant">Tenant Admin — scoped to one tenant</option>
          </select>
        </div>

        {scope === "Tenant" && (
          <div>
            <label className={labelClass}>Assign to Tenant <span className="text-red-500">*</span></label>
            <select
              value={assignedTenantId}
              onChange={(e) => setAssignedTenantId(e.target.value)}
              className={inputClass}
            >
              <option value="">Select a tenant…</option>
              {tenants.map((t) => (
                <option key={t.id} value={t.id}>{t.name}</option>
              ))}
            </select>
            {errors.tenant && <p className="text-xs text-red-500 mt-1">{errors.tenant}</p>}
          </div>
        )}

        <div className="rounded-lg bg-blue-50 dark:bg-blue-950/30 border border-blue-100 dark:border-blue-900 px-4 py-3 text-xs text-blue-700 dark:text-blue-300">
          Role is set to <strong>Admin</strong>. SuperAdmin accounts can only be created by system configuration.
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
          {loading ? <><Loader size={14} className="animate-spin" /> Creating…</> : "Create Admin"}
        </button>
      </div>
    </Modal>
  )
}
