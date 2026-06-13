"use client"

import { useState, useEffect } from "react"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { Skeleton } from "@/components/ui/skeleton"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { toast } from "sonner"
import { useTenants } from "@/lib/hooks/useTenants"
import { useSettings } from "@/lib/hooks/useSettings"
import { getTokenClaims } from "@/lib/utils"

export default function SettingsPage() {
  const { tenantId: myTenantId } = getTokenClaims()
  const isTenantAdmin = !!myTenantId

  const [selectedTenantId, setSelectedTenantId] = useState<string>(myTenantId ?? "")
  const { tenants: tenantsList, loading: tenantsLoading } = useTenants()
  const { settings, loading: settingsLoading, error } = useSettings(selectedTenantId)

  // Platform admins: default to first tenant
  useEffect(() => {
    if (!isTenantAdmin && !selectedTenantId && tenantsList && tenantsList.length > 0) {
      setSelectedTenantId(tenantsList[0].id)
    }
  }, [tenantsList, selectedTenantId, isTenantAdmin])

  if (!isTenantAdmin && tenantsLoading) {
    return <div className="space-y-4"><Skeleton className="h-10 w-full" /></div>
  }

  const tenants = tenantsList || []

  return (
    <div className="max-w-2xl space-y-8">
      {/* Tenant Selector — platform admins only */}
      {!isTenantAdmin && (
        <div className="bg-card border border-border rounded-xl p-6 space-y-4">
          <h2 className="text-base font-semibold text-foreground">Select Tenant</h2>
          <Separator />
          <Select value={selectedTenantId} onValueChange={(value) => setSelectedTenantId(value || "")}>
            <SelectTrigger className="w-full">
              <SelectValue placeholder="Choose a tenant" />
            </SelectTrigger>
            <SelectContent>
              {tenants.map((tenant) => (
                <SelectItem key={tenant.id} value={tenant.id}>
                  {tenant.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      )}

      {/* Settings */}
      {error ? (
        <div className="text-red-600 p-4 bg-red-50 rounded-lg">{error}</div>
      ) : settingsLoading ? (
        <div className="space-y-4">
          <Skeleton className="h-10 w-full" />
          <Skeleton className="h-10 w-full" />
        </div>
      ) : settings ? (
        <div className="bg-card border border-border rounded-xl p-6 space-y-5">
          <h2 className="text-base font-semibold text-foreground">{isTenantAdmin ? "My Settings" : "Tenant Settings"}</h2>
          <Separator />

          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Tenant Name</label>
              <p className="px-3 py-2 text-sm border border-border rounded-lg bg-muted text-foreground">{settings.name}</p>
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Contact Email</label>
              <p className="px-3 py-2 text-sm border border-border rounded-lg bg-muted text-foreground">{settings.contactEmail || "Not set"}</p>
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Contact Phone</label>
              <p className="px-3 py-2 text-sm border border-border rounded-lg bg-muted text-foreground">{settings.contactPhone || "Not set"}</p>
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Country</label>
              <p className="px-3 py-2 text-sm border border-border rounded-lg bg-muted text-foreground">{settings.country || "Not set"}</p>
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Status</label>
              <p className="px-3 py-2 text-sm border border-border rounded-lg bg-muted text-foreground">{settings.isActive ? "Active" : "Inactive"}</p>
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Provisioning Status</label>
              <p className="px-3 py-2 text-sm border border-border rounded-lg bg-muted text-foreground capitalize">{settings.provisioningStatus}</p>
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Created At</label>
              <p className="px-3 py-2 text-sm border border-border rounded-lg bg-muted text-foreground">
                {new Date(settings.createdAt).toLocaleDateString()}
              </p>
            </div>
          </div>

          <Button className="bg-[--color-brand-blue] hover:bg-blue-700 text-white" disabled>
            Settings are read-only
          </Button>
        </div>
      ) : null}
    </div>
  )
}
