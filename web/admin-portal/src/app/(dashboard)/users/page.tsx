"use client"

import { useState } from "react"
import { Search, Plus, Loader } from "lucide-react"
import { motion } from "framer-motion"
import StatusBadge from "@/components/common/StatusBadge"
import ActionMenu from "@/components/common/ActionMenu"
import { formatDate, getTokenClaims } from "@/lib/utils"
import { toast } from "sonner"

type AdminUser = {
  id: string
  name: string
  email: string
  role: "SuperAdmin" | "Admin"
  scope?: "Platform" | "Tenant"
  tenant?: string
  createdAt: string
  isActive: boolean
}

const MOCK_USERS: AdminUser[] = [
  { id: "1", name: "Super Admin",         email: "superadmin@litxustravel.com", role: "SuperAdmin", createdAt: "2026-05-28", isActive: true },
  { id: "2", name: "Platform Admin",      email: "admin@litxustravel.com",      role: "Admin", scope: "Platform",  createdAt: "2026-05-28", isActive: true },
  { id: "3", name: "TravelPro Admin",     email: "admin@travelpro.com",         role: "Admin", scope: "Tenant", tenant: "TravelPro",     createdAt: "2026-05-29", isActive: true },
  { id: "4", name: "Wanderlust Admin",    email: "admin@wanderlust.com",        role: "Admin", scope: "Tenant", tenant: "Wanderlust",    createdAt: "2026-05-30", isActive: true },
  { id: "5", name: "AdventureSeek Admin", email: "admin@adventureseek.com",     role: "Admin", scope: "Tenant", tenant: "AdventureSeek", createdAt: "2026-06-01", isActive: true },
]

const ROLE_COLOR: Record<string, string> = {
  SuperAdmin: "text-red-600 dark:text-red-400 bg-red-100 dark:bg-red-900/30 border-red-200 dark:border-red-800",
  Admin:      "text-blue-600 dark:text-blue-400 bg-blue-100 dark:bg-blue-900/30 border-blue-200 dark:border-blue-800",
}

export default function UsersPage() {
  const [search, setSearch] = useState("")

  const { role, tenantId, email } = getTokenClaims()

  // Tenant Admins only see users within their own tenant (themselves)
  // SuperAdmin and Platform Admin see everyone
  const visibleUsers = tenantId
    ? MOCK_USERS.filter((u) => u.email === email)
    : MOCK_USERS

  const isSuperAdmin = role === "SuperAdmin"

  const displayed = visibleUsers.filter(
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
          <p className="text-sm text-gray-500 dark:text-gray-400">{displayed.length} admin users</p>
        </div>
        {isSuperAdmin && (
          <button
            onClick={() => toast.info("Invite admin — coming soon")}
            className="flex items-center gap-2 px-4 py-2.5 bg-[--color-brand-blue] hover:bg-blue-700 text-white text-sm font-semibold rounded-lg transition-colors"
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
          className="w-full pl-9 pr-4 py-2 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-900 focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
        />
      </div>

      <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl overflow-hidden">
        {displayed.length === 0 ? (
          <div className="flex items-center justify-center py-12">
            <Loader className="animate-spin text-gray-400 mr-2" size={18} />
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-gray-100 dark:bg-gray-800/50 border-b border-gray-200 dark:border-gray-800">
                <tr>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">User</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">Role</th>
                  {!tenantId && <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">Tenant</th>}
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
                      <span className={`inline-block px-2 py-0.5 rounded text-[11px] font-semibold border ${ROLE_COLOR[user.role]}`}>
                        {user.role}{user.scope ? ` · ${user.scope}` : ""}
                      </span>
                    </td>
                    {!tenantId && (
                      <td className="px-4 py-3 text-gray-500 dark:text-gray-400">
                        {user.tenant ?? "—"}
                      </td>
                    )}
                    <td className="px-4 py-3">
                      <StatusBadge status={user.isActive ? "Active" : "Suspended"} />
                    </td>
                    <td className="px-4 py-3 text-gray-500 dark:text-gray-400">
                      {formatDate(user.createdAt)}
                    </td>
                    <td className="px-4 py-3">
                      <ActionMenu
                        items={[
                          { label: "Edit", action: () => toast.info("Edit user — coming soon") },
                          {
                            label: user.isActive ? "Suspend" : "Reactivate",
                            action: () => toast.info("Update status — coming soon"),
                            danger: user.isActive,
                          },
                          ...(user.role !== "SuperAdmin" ? [{ label: "Delete", action: () => toast.error("Delete user — coming soon"), danger: true }] : []),
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
    </div>
  )
}

