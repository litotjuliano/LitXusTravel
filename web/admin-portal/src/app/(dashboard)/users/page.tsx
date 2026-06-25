"use client"

import { useState, useEffect, useCallback } from "react"
import { Search, Plus, Loader } from "lucide-react"
import { motion } from "framer-motion"
import StatusBadge from "@/components/common/StatusBadge"
import ActionMenu from "@/components/common/ActionMenu"
import { CreateAdminUserModal } from "@/components/modals/CreateAdminUserModal"
import { formatDate, getTokenClaims } from "@/lib/utils"
import { activityStatus } from "@/lib/statuses"
import { adminApi, AdminUserListDto } from "@/lib/api"
import { useTenants } from "@/lib/hooks/useTenants"
import { toast } from "sonner"

const ROLE_COLOR: Record<string, string> = {
  SuperAdmin: "text-red-600 dark:text-red-400 bg-red-100 dark:bg-red-900/30 border-red-200 dark:border-red-800",
  Admin:      "text-blue-600 dark:text-blue-400 bg-blue-100 dark:bg-blue-900/30 border-blue-200 dark:border-blue-800",
}

export default function UsersPage() {
  const [search, setSearch] = useState("")
  const [users, setUsers] = useState<AdminUserListDto[]>([])
  const [loading, setLoading] = useState(true)
  const [createOpen, setCreateOpen] = useState(false)
  const [refreshKey, setRefreshKey] = useState(0)

  const { role, tenantId, email } = getTokenClaims()
  const isSuperAdmin = role === "SuperAdmin"
  const isPlatformAdmin = !tenantId

  const { tenants } = useTenants(1, 200)
  const tenantNameById = Object.fromEntries(tenants.map((t) => [t.id, t.name]))

  const fetchUsers = useCallback(async () => {
    setLoading(true)
    try {
      const res = await adminApi.getAdminUsers(tenantId ? { tenantId } : undefined)
      let data = res.data as unknown as AdminUserListDto[]
      // Tenant-scoped admin only sees themselves
      if (tenantId && email) {
        data = data.filter((u) => u.email === email)
      }
      setUsers(data)
    } catch {
      toast.error("Failed to load users")
    } finally {
      setLoading(false)
    }
  }, [tenantId, email, refreshKey]) // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => { fetchUsers() }, [fetchUsers])

  const displayed = users.filter(
    (u) =>
      !search ||
      u.name.toLowerCase().includes(search.toLowerCase()) ||
      u.email.toLowerCase().includes(search.toLowerCase())
  )

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-semibold text-gray-900 dark:text-white">User Management</h2>
          <p className="text-sm text-gray-500 dark:text-gray-400">{users.length} admin user{users.length !== 1 ? "s" : ""}</p>
        </div>
        {isSuperAdmin && (
          <button
            onClick={() => setCreateOpen(true)}
            className="flex items-center gap-2 px-4 py-2.5 bg-(--color-brand-blue) hover:bg-blue-700 text-white text-sm font-semibold rounded-lg transition-colors"
          >
            <Plus size={16} />
            Invite Admin
          </button>
        )}
      </div>

      <div className="relative max-w-xs">
        <Search size={15} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500 dark:text-gray-400" />
        <input
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Search users..."
          className="w-full pl-9 pr-4 py-2 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-900 focus:outline-none focus:ring-2 focus:ring-(--color-brand-blue)"
        />
      </div>

      <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl overflow-hidden">
        {loading ? (
          <div className="flex items-center justify-center py-12">
            <Loader className="animate-spin text-gray-400 mr-2" size={18} />
            <p className="text-sm text-gray-400">Loading users…</p>
          </div>
        ) : displayed.length === 0 ? (
          <div className="py-12 text-center text-sm text-gray-400">
            {search ? "No users match your search." : "No users found."}
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-gray-100 dark:bg-gray-800/50 border-b border-gray-200 dark:border-gray-800">
                <tr>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">User</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">Role</th>
                  {isPlatformAdmin && <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">Tenant</th>}
                  <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">Status</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">Created</th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {displayed.map((user, i) => (
                  <motion.tr
                    key={user.id}
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ delay: i * 0.04 }}
                    className="hover:bg-gray-50 dark:hover:bg-gray-800/30 transition-colors"
                  >
                    <td className="px-4 py-3">
                      <p className="font-medium text-gray-900 dark:text-white">{user.name}</p>
                      <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5">{user.email}</p>
                    </td>
                    <td className="px-4 py-3">
                      <span className={`inline-block px-2 py-0.5 rounded text-[11px] font-semibold border ${ROLE_COLOR[user.role] ?? ROLE_COLOR.Admin}`}>
                        {user.role}{user.scope && user.scope !== "None" ? ` · ${user.scope}` : ""}
                      </span>
                    </td>
                    {isPlatformAdmin && (
                      <td className="px-4 py-3 text-gray-500 dark:text-gray-400">
                        {user.assignedTenantId ? (tenantNameById[user.assignedTenantId] ?? "—") : "—"}
                      </td>
                    )}
                    <td className="px-4 py-3">
                      <StatusBadge status={activityStatus(user.isActive)} />
                    </td>
                    <td className="px-4 py-3 text-gray-500 dark:text-gray-400">
                      {formatDate(user.createdAt)}
                    </td>
                    <td className="px-4 py-3">
                      <ActionMenu
                        items={[
                          { label: "Edit", action: () => toast.info("Edit user — coming soon") },
                        ]}
                      />
                    </td>
                  </motion.tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <CreateAdminUserModal
        open={createOpen}
        onClose={() => setCreateOpen(false)}
        onSuccess={() => { setCreateOpen(false); setRefreshKey((k) => k + 1) }}
      />
    </div>
  )
}
