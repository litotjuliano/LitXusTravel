"use client"

import { useState, useEffect } from "react"
import { Button } from "@/components/ui/button"
import { toast } from "sonner"
import { useTenants } from "@/lib/hooks/useTenants"
import { useSettings } from "@/lib/hooks/useSettings"
import { adminApi } from "@/lib/api"
import { getTokenClaims } from "@/lib/utils"

const CURRENCIES = ["MYR", "USD", "EUR", "GBP", "SGD", "AUD", "JPY", "CNY"]

const selectCls = "w-full px-3 py-2 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-900 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-brand-500/30 transition-colors"
const readonlyCls = "px-3 py-2 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-gray-100 dark:bg-gray-800 text-gray-900 dark:text-white"
const labelCls = "block text-sm font-medium text-gray-900 dark:text-white mb-1.5"

export default function SettingsPage() {
  const { tenantId: myTenantId } = getTokenClaims()
  const isTenantAdmin = !!myTenantId

  const [selectedTenantId, setSelectedTenantId] = useState<string>(myTenantId ?? "")
  const { tenants: tenantsList, loading: tenantsLoading } = useTenants()
  const { settings, loading: settingsLoading, error, refetch } = useSettings(selectedTenantId)

  const [currency, setCurrency] = useState("MYR")
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    if (!isTenantAdmin && !selectedTenantId && tenantsList && tenantsList.length > 0) {
      setSelectedTenantId(tenantsList[0].id)
    }
  }, [tenantsList, selectedTenantId, isTenantAdmin])

  useEffect(() => {
    if (settings?.defaultCurrency) {
      setCurrency(settings.defaultCurrency)
    }
  }, [settings?.defaultCurrency])

  const handleSave = async () => {
    if (!selectedTenantId) return
    setSaving(true)
    try {
      await adminApi.updateTenantSettings(selectedTenantId, { defaultCurrency: currency })
      toast.success("Settings saved.")
      await refetch()
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Failed to save settings.")
    } finally {
      setSaving(false)
    }
  }

  if (!isTenantAdmin && tenantsLoading) {
    return (
      <div className="space-y-4">
        <div className="h-10 w-full animate-pulse rounded-md bg-gray-200 dark:bg-gray-700" />
      </div>
    )
  }

  const tenants = tenantsList || []

  return (
    <div className="max-w-2xl space-y-8">
      {/* Tenant Selector — platform admins only */}
      {!isTenantAdmin && (
        <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-6 space-y-4">
          <h2 className="text-base font-semibold text-gray-900 dark:text-white">Select Tenant</h2>
          <div className="h-px w-full bg-gray-200 dark:bg-gray-800" />
          <select
            value={selectedTenantId}
            onChange={(e) => setSelectedTenantId(e.target.value || "")}
            className={selectCls}
          >
            <option value="">Choose a tenant</option>
            {tenants.map((tenant) => (
              <option key={tenant.id} value={tenant.id}>
                {tenant.name}
              </option>
            ))}
          </select>
        </div>
      )}

      {/* Settings */}
      {error ? (
        <div className="text-red-600 p-4 bg-red-50 dark:bg-red-900/20 rounded-lg text-sm">{error}</div>
      ) : settingsLoading ? (
        <div className="space-y-4">
          <div className="h-10 w-full animate-pulse rounded-md bg-gray-200 dark:bg-gray-700" />
          <div className="h-10 w-full animate-pulse rounded-md bg-gray-200 dark:bg-gray-700" />
        </div>
      ) : settings ? (
        <>
          {/* Tenant Information — read-only */}
          <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-6 space-y-5">
            <h2 className="text-base font-semibold text-gray-900 dark:text-white">Tenant Information</h2>
            <div className="h-px w-full bg-gray-200 dark:bg-gray-800" />

            <div className="space-y-4">
              <div>
                <label className={labelCls}>Tenant Name</label>
                <p className={readonlyCls}>{settings.name}</p>
              </div>
              <div>
                <label className={labelCls}>Contact Email</label>
                <p className={readonlyCls}>{settings.contactEmail || "Not set"}</p>
              </div>
              <div>
                <label className={labelCls}>Contact Phone</label>
                <p className={readonlyCls}>{settings.contactPhone || "Not set"}</p>
              </div>
              <div>
                <label className={labelCls}>Country</label>
                <p className={readonlyCls}>{settings.country || "Not set"}</p>
              </div>
              <div>
                <label className={labelCls}>Status</label>
                <p className={readonlyCls}>{settings.isActive ? "Active" : "Inactive"}</p>
              </div>
              <div>
                <label className={labelCls}>Provisioning Status</label>
                <p className={readonlyCls + " capitalize"}>{settings.provisioningStatus}</p>
              </div>
              <div>
                <label className={labelCls}>Created At</label>
                <p className={readonlyCls}>{new Date(settings.createdAt).toLocaleDateString()}</p>
              </div>
            </div>
          </div>

          {/* Global Settings — editable */}
          <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-6 space-y-5">
            <h2 className="text-base font-semibold text-gray-900 dark:text-white">Global Settings</h2>
            <div className="h-px w-full bg-gray-200 dark:bg-gray-800" />

            <div className="space-y-4">
              <div>
                <label className={labelCls}>Default Currency</label>
                <p className="text-xs text-gray-500 dark:text-gray-400 mb-2">Applied to all packages. No need to select currency per package.</p>
                <select
                  value={currency}
                  onChange={(e) => { if (e.target.value) setCurrency(e.target.value) }}
                  className={selectCls + " w-48"}
                >
                  {CURRENCIES.map((c) => (
                    <option key={c} value={c}>{c}</option>
                  ))}
                </select>
              </div>
            </div>

            <Button variant="primary" onClick={handleSave} disabled={saving}>
              {saving ? "Saving…" : "Save Settings"}
            </Button>
          </div>
        </>
      ) : null}
    </div>
  )
}
