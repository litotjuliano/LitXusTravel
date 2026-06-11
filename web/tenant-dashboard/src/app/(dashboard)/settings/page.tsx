"use client"

import { useEffect, useState } from "react"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { toast } from "sonner"

export default function SettingsPage() {
  const router = useRouter()
  const [email, setEmail] = useState("")
  const [tenantId, setTenantId] = useState("")

  useEffect(() => {
    const storedEmail = localStorage.getItem("nexus_user_email") || ""
    const storedTenantId = localStorage.getItem("nexus_tenant_id") || ""
    setEmail(storedEmail)
    setTenantId(storedTenantId)
  }, [])

  const handleLogout = () => {
    localStorage.removeItem("nexus_token")
    localStorage.removeItem("nexus_tenant_id")
    localStorage.removeItem("nexus_user_email")
    toast.success("Logged out successfully")
    router.push("/auth/login")
  }

  return (
    <div className="max-w-2xl">
      {/* Account Info */}
      <div className="bg-card border border-border rounded-xl p-6 mb-6">
        <h2 className="text-lg font-semibold text-foreground mb-6">Account Information</h2>

        <div className="space-y-6">
          <div>
            <label className="block text-sm font-medium text-foreground mb-2">Email</label>
            <input
              type="email"
              value={email}
              disabled
              className="w-full px-4 py-2.5 bg-muted border border-border rounded-lg text-foreground text-sm"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-foreground mb-2">Tenant ID</label>
            <input
              type="text"
              value={tenantId}
              disabled
              className="w-full px-4 py-2.5 bg-muted border border-border rounded-lg text-foreground text-sm font-mono text-xs"
            />
          </div>
        </div>
      </div>

      {/* Danger Zone */}
      <div className="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-xl p-6">
        <h3 className="text-lg font-semibold text-red-800 dark:text-red-200 mb-4">Danger Zone</h3>
        <p className="text-sm text-red-700 dark:text-red-300 mb-4">
          Logging out will clear your session and you'll need to sign in again.
        </p>
        <Button
          onClick={handleLogout}
          className="bg-red-600 hover:bg-red-700 text-white"
        >
          Logout
        </Button>
      </div>
    </div>
  )
}
