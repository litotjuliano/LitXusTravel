"use client"

import { useState } from "react"
import StatusBadge from "@/components/common/StatusBadge"
import { Pagination } from "@/components/common/Pagination"
import { SortableHeader } from "@/components/common/SortableHeader"
import { Modal } from "@/components/ui/Modal"
import { formatDate, getTokenClaims } from "@/lib/utils"
import { useTenants } from "@/lib/hooks/useTenants"
import { adminApi } from "@/lib/api"
import { Package, Users, CalendarDays, Loader } from "lucide-react"
import { toast } from "sonner"

const PLANS = [
  { name: "Starter",    price: 99,  tenants: 12, packages: 10,  members: 2 },
  { name: "Pro",        price: 299, tenants: 8,  packages: 50,  members: 10 },
  { name: "Enterprise", price: 999, tenants: 3,  packages: 999, members: 50 },
]

function PlatformView() {
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(20)
  const [sortBy, setSortBy] = useState<string | undefined>(undefined)
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("desc")
  const [assigning, setAssigning] = useState<string | null>(null)
  const [pendingPlan, setPendingPlan] = useState<{ tenantId: string; tenantName: string; planName: string } | null>(null)
  const [resetNonce, setResetNonce] = useState(0)

  const { tenants, loading, pagination, refetch } = useTenants(page, pageSize, { sortBy, sortOrder })

  async function confirmAssignPlan() {
    if (!pendingPlan) return
    const { tenantId, tenantName, planName } = pendingPlan
    setAssigning(tenantId)
    try {
      await adminApi.assignPlan(tenantId, planName)
      toast.success(`${tenantName} → ${planName}`)
      refetch()
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Failed")
      setResetNonce((n) => n + 1)
    } finally {
      setAssigning(null)
      setPendingPlan(null)
    }
  }

  function cancelAssignPlan() {
    setPendingPlan(null)
    setResetNonce((n) => n + 1)
  }

  function handleSort(key: string) {
    if (sortBy === key) {
      setSortOrder((prev) => (prev === "asc" ? "desc" : "asc"))
    } else {
      setSortBy(key)
      setSortOrder("asc")
    }
    setPage(1)
  }

  function handlePageSizeChange(size: number) {
    setPageSize(size)
    setPage(1)
  }

  return (
    <div className="space-y-6">
      {/* Plan summary cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {PLANS.map((plan) => (
          <div key={plan.name} className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-5">
            <h3 className="font-bold text-gray-900 dark:text-white mb-1">{plan.name}</h3>
            <p className="text-2xl font-bold text-[--color-brand-blue] mb-4">
              RM {plan.price}
              <span className="text-sm font-normal text-gray-500 dark:text-gray-400">/mo</span>
            </p>
            <div className="space-y-1.5 text-sm text-gray-500 dark:text-gray-400 mb-4">
              <p>Up to {plan.packages === 999 ? "Unlimited" : plan.packages} packages</p>
              <p>Up to {plan.members} team members</p>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-xs text-gray-500 dark:text-gray-400">{plan.tenants} active tenants</span>
              <span className="text-xs font-semibold text-[--color-brand-blue]">
                RM {(plan.price * plan.tenants).toLocaleString()}/mo
              </span>
            </div>
          </div>
        ))}
      </div>

      {/* Tenant subscriptions table */}
      <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl overflow-hidden">
        <div className="px-5 py-4 border-b border-gray-200 dark:border-gray-800">
          <h2 className="text-base font-semibold text-gray-900 dark:text-white">Tenant Subscriptions</h2>
        </div>

        {loading ? (
          <div className="flex items-center justify-center py-12">
            <Loader className="animate-spin text-gray-500 dark:text-gray-400 mr-2" size={20} />
            <p className="text-gray-500 dark:text-gray-400">Loading subscriptions...</p>
          </div>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead className="bg-gray-100 dark:bg-gray-800/50 border-b border-gray-200 dark:border-gray-800">
                  <tr>
                    <SortableHeader
                      label="Tenant"
                      sortKey="name"
                      currentSortBy={sortBy}
                      sortOrder={sortOrder}
                      onSort={handleSort}
                    />
                    <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">
                      Plan
                    </th>
                    <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">
                      Status
                    </th>
                    <SortableHeader
                      label="Joined"
                      sortKey="createdat"
                      currentSortBy={sortBy}
                      sortOrder={sortOrder}
                      onSort={handleSort}
                    />
                    <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">
                      Packages Synced
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-border">
                  {tenants.length === 0 ? (
                    <tr>
                      <td
                        colSpan={5}
                        className="px-4 py-8 text-center text-gray-500 dark:text-gray-400"
                      >
                        No tenants found
                      </td>
                    </tr>
                  ) : (
                    tenants.map((t) => (
                      <tr key={t.id} className="hover:bg-gray-100 dark:bg-gray-800/30 transition-colors">
                        <td className="px-4 py-3">
                          <p className="font-medium text-gray-900 dark:text-white">{t.name}</p>
                          <p className="text-xs text-gray-500 dark:text-gray-400">{t.contactEmail}</p>
                        </td>
                        <td className="px-4 py-3">
                          <select
                            key={`${t.plan ?? "none"}-${resetNonce}`}
                            defaultValue={t.plan ?? ""}
                            disabled={assigning === t.id}
                            onChange={(e) => {
                              const plan = e.target.value
                              if (!plan) return
                              setPendingPlan({ tenantId: t.id, tenantName: t.name, planName: plan })
                            }}
                            className="text-sm border border-gray-200 dark:border-gray-700 rounded-lg px-2 py-1 bg-white dark:bg-gray-900 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue] disabled:opacity-50"
                          >
                            <option value="">— Assign plan —</option>
                            {PLANS.map(p => (
                              <option key={p.name} value={p.name}>{p.name}</option>
                            ))}
                          </select>
                        </td>
                        <td className="px-4 py-3">
                          <StatusBadge
                            status={
                              t.plan === "Trial"
                                ? "Trial"
                                : t.isActive
                                ? "Active"
                                : "Suspended"
                            }
                          />
                        </td>
                        <td className="px-4 py-3 text-gray-500 dark:text-gray-400">
                          {formatDate(t.createdAt)}
                        </td>
                        <td className="px-4 py-3 font-semibold text-gray-900 dark:text-white">
                          {t.syncedPackagesCount ?? 0}
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>

            <Pagination
              page={pagination.page}
              totalPages={pagination.totalPages}
              totalCount={pagination.totalCount}
              pageSize={pagination.pageSize}
              onPageChange={setPage}
              onPageSizeChange={handlePageSizeChange}
            />
          </>
        )}
      </div>

      <Modal isOpen={!!pendingPlan} onClose={cancelAssignPlan} className="max-w-sm rounded-2xl p-6">
        {pendingPlan && (
          <div className="space-y-4">
            <div>
              <h3 className="text-base font-semibold text-gray-900 dark:text-white">Assign plan</h3>
              <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                Assign <span className="font-semibold text-gray-900 dark:text-white">{pendingPlan.planName}</span> to{" "}
                <span className="font-semibold text-gray-900 dark:text-white">{pendingPlan.tenantName}</span>?
                This expires their current active subscription.
              </p>
            </div>
            <div className="flex justify-end gap-2">
              <button
                onClick={cancelAssignPlan}
                disabled={assigning === pendingPlan.tenantId}
                className="px-4 py-2 text-sm font-medium text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors disabled:opacity-50"
              >
                Cancel
              </button>
              <button
                onClick={confirmAssignPlan}
                disabled={assigning === pendingPlan.tenantId}
                className="px-4 py-2 text-sm font-semibold bg-[--color-brand-blue] hover:bg-blue-700 text-white rounded-lg transition-colors disabled:opacity-50 flex items-center gap-2"
              >
                {assigning === pendingPlan.tenantId && <Loader size={14} className="animate-spin" />}
                Confirm
              </button>
            </div>
          </div>
        )}
      </Modal>
    </div>
  )
}

function TenantView() {
  const { tenants, loading } = useTenants()
  const { tenantId } = getTokenClaims()
  const myTenant = tenants.find((t) => t.id === tenantId)

  const plan = myTenant?.plan ?? "Pro"
  const planDetails = PLANS.find((p) => p.name === plan) ?? PLANS[1]

  if (loading) {
    return (
      <div className="text-sm text-gray-500 dark:text-gray-400 py-8 text-center">
        Loading subscription...
      </div>
    )
  }

  return (
    <div className="space-y-6 max-w-2xl">
      <div>
        <h2 className="text-lg font-semibold text-gray-900 dark:text-white">My Subscription</h2>
        <p className="text-sm text-gray-500 dark:text-gray-400">Your current plan and usage</p>
      </div>

      {/* Current plan card */}
      <div className="bg-white dark:bg-gray-900 border border-[--color-brand-blue]/40 rounded-xl p-6 space-y-5">
        <div className="flex items-start justify-between">
          <div>
            <p className="text-xs text-gray-500 dark:text-gray-400 uppercase tracking-wide mb-1">
              Current Plan
            </p>
            <p className="text-2xl font-bold text-gray-900 dark:text-white">{plan}</p>
            <p className="text-xl font-semibold text-[--color-brand-blue] mt-0.5">
              RM {planDetails.price}
              <span className="text-sm font-normal text-gray-500 dark:text-gray-400">/month</span>
            </p>
          </div>
          <StatusBadge status={myTenant?.isActive ? "Active" : "Suspended"} />
        </div>

        <div className="grid grid-cols-3 gap-4 pt-2 border-t border-gray-200 dark:border-gray-800">
          <div className="flex flex-col gap-1">
            <div className="flex items-center gap-1.5 text-gray-500 dark:text-gray-400">
              <Package size={14} />
              <span className="text-xs uppercase tracking-wide">Packages</span>
            </div>
            <p className="text-sm font-semibold text-gray-900 dark:text-white">
              {myTenant?.syncedPackagesCount ?? 0} /{" "}
              {planDetails.packages === 999 ? "∞" : planDetails.packages}
            </p>
          </div>
          <div className="flex flex-col gap-1">
            <div className="flex items-center gap-1.5 text-gray-500 dark:text-gray-400">
              <Users size={14} />
              <span className="text-xs uppercase tracking-wide">Members</span>
            </div>
            <p className="text-sm font-semibold text-gray-900 dark:text-white">— / {planDetails.members}</p>
          </div>
          <div className="flex flex-col gap-1">
            <div className="flex items-center gap-1.5 text-gray-500 dark:text-gray-400">
              <CalendarDays size={14} />
              <span className="text-xs uppercase tracking-wide">Since</span>
            </div>
            <p className="text-sm font-semibold text-gray-900 dark:text-white">
              {myTenant?.createdAt ? formatDate(myTenant.createdAt) : "—"}
            </p>
          </div>
        </div>
      </div>

      {/* Available plans */}
      <div>
        <p className="text-sm font-medium text-gray-900 dark:text-white mb-3">Available Plans</p>
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
          {PLANS.map((p) => (
            <div
              key={p.name}
              className={`rounded-xl border p-4 ${
                p.name === plan
                  ? "border-[--color-brand-blue] bg-[--color-brand-blue]/5"
                  : "border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900"
              }`}
            >
              <p className="font-semibold text-gray-900 dark:text-white text-sm">{p.name}</p>
              <p className="text-[--color-brand-blue] font-bold mt-0.5">
                RM {p.price}
                <span className="text-xs font-normal text-gray-500 dark:text-gray-400">/mo</span>
              </p>
              <div className="mt-2 space-y-0.5 text-xs text-gray-500 dark:text-gray-400">
                <p>{p.packages === 999 ? "Unlimited" : `${p.packages}`} packages</p>
                <p>{p.members} members</p>
              </div>
              {p.name === plan ? (
                <p className="mt-3 text-xs font-semibold text-[--color-brand-blue]">
                  Current plan
                </p>
              ) : (
                <button className="mt-3 text-xs font-semibold text-gray-500 dark:text-gray-400 hover:text-foreground transition-colors">
                  {p.price > planDetails.price ? "Upgrade →" : "Downgrade →"}
                </button>
              )}
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}

export default function SubscriptionsPage() {
  const { tenantId } = getTokenClaims()
  return tenantId ? <TenantView /> : <PlatformView />
}
