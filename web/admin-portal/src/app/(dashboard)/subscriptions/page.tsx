"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import StatusBadge from "@/components/common/StatusBadge"
import { Pagination } from "@/components/common/Pagination"
import { SortableHeader } from "@/components/common/SortableHeader"
import { Modal } from "@/components/ui/Modal"
import { formatDate, getTokenClaims } from "@/lib/utils"
import { useTenants } from "@/lib/hooks/useTenants"
import { useSubscriptionPlans, type SubscriptionPlan } from "@/lib/hooks/useSubscriptionPlans"
import { adminApi } from "@/lib/api"
import { Package, Users, CalendarDays, Loader, Plus, Pencil, Trash2, CheckCircle2, XCircle, Clock, Lock, AlertTriangle } from "lucide-react"
import { toast } from "sonner"

type PlanFormState = { name: string; price: string; maxPackages: string; maxTeamMembers: string }
const EMPTY_FORM: PlanFormState = { name: "", price: "", maxPackages: "", maxTeamMembers: "" }

function PlanFormModal({
  isOpen,
  onClose,
  editingPlan,
  onSaved,
}: {
  isOpen: boolean
  onClose: () => void
  editingPlan: SubscriptionPlan | null
  onSaved: () => void
}) {
  const [form, setForm] = useState<PlanFormState>(EMPTY_FORM)
  const [submitting, setSubmitting] = useState(false)

  // Sync form fields whenever the modal opens for a (possibly new) plan
  const planId = editingPlan?.id ?? "new"
  const [lastPlanId, setLastPlanId] = useState(planId)
  if (planId !== lastPlanId) {
    setLastPlanId(planId)
    setForm(
      editingPlan
        ? {
            name: editingPlan.name,
            price: String(editingPlan.price),
            maxPackages: String(editingPlan.maxPackages),
            maxTeamMembers: String(editingPlan.maxTeamMembers),
          }
        : EMPTY_FORM
    )
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    const data = {
      name: form.name.trim(),
      price: Number(form.price),
      maxPackages: Number(form.maxPackages),
      maxTeamMembers: Number(form.maxTeamMembers),
    }
    setSubmitting(true)
    try {
      if (editingPlan) {
        await adminApi.updateSubscriptionPlan(editingPlan.id, data)
        toast.success(`${data.name} updated`)
      } else {
        await adminApi.createSubscriptionPlan(data)
        toast.success(`${data.name} created`)
      }
      onSaved()
      onClose()
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Failed")
    } finally {
      setSubmitting(false)
    }
  }

  const inputCls =
    "w-full px-3 py-2 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-950 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-(--color-brand-blue)"

  return (
    <Modal isOpen={isOpen} onClose={onClose} className="max-w-md rounded-2xl p-6">
      <form onSubmit={handleSubmit} className="space-y-4">
        <div className="pr-10 sm:pr-12">
          <h3 className="text-base font-semibold text-gray-900 dark:text-white">
            {editingPlan ? "Edit plan" : "New plan"}
          </h3>
        </div>
        <div className="space-y-3">
          <div>
            <label className="block text-xs text-gray-500 dark:text-gray-400 mb-1">Plan name</label>
            <input
              className={inputCls}
              value={form.name}
              onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
              placeholder="e.g. Growth"
              required
            />
          </div>
          <div>
            <label className="block text-xs text-gray-500 dark:text-gray-400 mb-1">Price (RM/mo)</label>
            <input
              type="number"
              min={0}
              step="0.01"
              className={inputCls}
              value={form.price}
              onChange={(e) => setForm((f) => ({ ...f, price: e.target.value }))}
              required
            />
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-xs text-gray-500 dark:text-gray-400 mb-1">Max packages</label>
              <input
                type="number"
                min={1}
                className={inputCls}
                value={form.maxPackages}
                onChange={(e) => setForm((f) => ({ ...f, maxPackages: e.target.value }))}
                required
              />
            </div>
            <div>
              <label className="block text-xs text-gray-500 dark:text-gray-400 mb-1">Max team members</label>
              <input
                type="number"
                min={1}
                className={inputCls}
                value={form.maxTeamMembers}
                onChange={(e) => setForm((f) => ({ ...f, maxTeamMembers: e.target.value }))}
                required
              />
            </div>
          </div>
        </div>
        <div className="flex justify-end gap-2">
          <button
            type="button"
            onClick={onClose}
            disabled={submitting}
            className="px-4 py-2 text-sm font-medium text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors disabled:opacity-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={submitting}
            className="px-4 py-2 text-sm font-semibold bg-(--color-brand-blue) hover:bg-blue-700 text-white rounded-lg transition-colors disabled:opacity-50 flex items-center gap-2"
          >
            {submitting && <Loader size={14} className="animate-spin" />}
            {editingPlan ? "Save changes" : "Create plan"}
          </button>
        </div>
      </form>
    </Modal>
  )
}

function DeletePlanModal({
  plan,
  onClose,
  onDeleted,
}: {
  plan: SubscriptionPlan | null
  onClose: () => void
  onDeleted: () => void
}) {
  const [deleting, setDeleting] = useState(false)

  async function handleDelete() {
    if (!plan) return
    setDeleting(true)
    try {
      await adminApi.deleteSubscriptionPlan(plan.id)
      toast.success(`${plan.name} deleted`)
      onDeleted()
      onClose()
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Failed")
    } finally {
      setDeleting(false)
    }
  }

  return (
    <Modal isOpen={!!plan} onClose={onClose} className="max-w-sm rounded-2xl p-6">
      {plan && (
        <div className="space-y-4">
          <div className="pr-10 sm:pr-12">
            <h3 className="text-base font-semibold text-gray-900 dark:text-white">Delete plan</h3>
            <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
              Delete <span className="font-semibold text-gray-900 dark:text-white">{plan.name}</span>?
              {plan.activeTenantCount > 0 && (
                <>
                  {" "}It has <span className="font-semibold text-gray-900 dark:text-white">{plan.activeTenantCount}</span> active
                  {plan.activeTenantCount === 1 ? " subscriber" : " subscribers"} — they keep their current subscription, but you won't be able to assign this plan to new tenants.
                </>
              )}
            </p>
          </div>
          <div className="flex justify-end gap-2">
            <button
              onClick={onClose}
              disabled={deleting}
              className="px-4 py-2 text-sm font-medium text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors disabled:opacity-50"
            >
              Cancel
            </button>
            <button
              onClick={handleDelete}
              disabled={deleting}
              className="px-4 py-2 text-sm font-semibold bg-red-600 hover:bg-red-700 text-white rounded-lg transition-colors disabled:opacity-50 flex items-center gap-2"
            >
              {deleting && <Loader size={14} className="animate-spin" />}
              Delete
            </button>
          </div>
        </div>
      )}
    </Modal>
  )
}

function PlatformView() {
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(20)
  const [sortBy, setSortBy] = useState<string | undefined>(undefined)
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("desc")
  const [assigning, setAssigning] = useState<string | null>(null)
  const [pendingPlan, setPendingPlan] = useState<{ tenantId: string; tenantName: string; planName: string } | null>(null)
  const [resetNonce, setResetNonce] = useState(0)
  const [planModalOpen, setPlanModalOpen] = useState(false)
  const [editingPlan, setEditingPlan] = useState<SubscriptionPlan | null>(null)
  const [deletingPlan, setDeletingPlan] = useState<SubscriptionPlan | null>(null)

  const { tenants, loading, pagination, refetch } = useTenants(page, pageSize, { sortBy, sortOrder })
  const { plans, loading: plansLoading, refetch: refetchPlans } = useSubscriptionPlans()

  async function confirmAssignPlan() {
    if (!pendingPlan) return
    const { tenantId, tenantName, planName } = pendingPlan
    setAssigning(tenantId)
    try {
      await adminApi.assignPlan(tenantId, planName)
      toast.success(`${tenantName} → ${planName}`)
      refetch()
      refetchPlans()
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

  function openCreatePlan() {
    setEditingPlan(null)
    setPlanModalOpen(true)
  }

  function openEditPlan(plan: SubscriptionPlan) {
    setEditingPlan(plan)
    setPlanModalOpen(true)
  }

  return (
    <div className="space-y-6">
      {/* Plan summary cards */}
      <div className="flex items-center justify-between">
        <h2 className="text-base font-semibold text-gray-900 dark:text-white">Plans</h2>
        <button
          onClick={openCreatePlan}
          className="flex items-center gap-2 px-3 py-1.5 bg-(--color-brand-blue) hover:bg-blue-700 text-white text-xs font-semibold rounded-lg transition-colors"
        >
          <Plus size={14} /> New Plan
        </button>
      </div>

      {plansLoading ? (
        <div className="flex items-center justify-center py-8">
          <Loader className="animate-spin text-gray-400 mr-2" size={18} />
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {plans.length === 0 ? (
            <p className="text-sm text-gray-500 dark:text-gray-400 col-span-3 py-4">No plans configured yet.</p>
          ) : (
            plans.map((plan) => (
              <div
                key={plan.id}
                className="group relative bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-5"
              >
                <div className="absolute top-4 right-4 flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                  <button
                    onClick={() => openEditPlan(plan)}
                    title="Edit plan"
                    className="p-1.5 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg text-gray-400 hover:text-gray-700 dark:hover:text-gray-200 transition-colors"
                  >
                    <Pencil size={14} />
                  </button>
                  <button
                    onClick={() => setDeletingPlan(plan)}
                    title="Delete plan"
                    className="p-1.5 hover:bg-red-50 dark:hover:bg-red-900/20 rounded-lg text-gray-400 hover:text-red-600 dark:hover:text-red-400 transition-colors"
                  >
                    <Trash2 size={14} />
                  </button>
                </div>
                <h3 className="font-bold text-gray-900 dark:text-white mb-1">{plan.name}</h3>
                <p className="text-2xl font-bold text-(--color-brand-blue) mb-4">
                  RM {plan.price}
                  <span className="text-sm font-normal text-gray-500 dark:text-gray-400">/mo</span>
                </p>
                <div className="space-y-1.5 text-sm text-gray-500 dark:text-gray-400 mb-4">
                  <p>Up to {plan.maxPackages >= 999 ? "Unlimited" : plan.maxPackages} packages</p>
                  <p>Up to {plan.maxTeamMembers} team members</p>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-xs text-gray-500 dark:text-gray-400">{plan.activeTenantCount} active tenants</span>
                  <span className="text-xs font-semibold text-(--color-brand-blue)">
                    RM {(plan.price * plan.activeTenantCount).toLocaleString()}/mo
                  </span>
                </div>
              </div>
            ))
          )}
        </div>
      )}

      {/* Tenant subscriptions table */}
      <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl overflow-hidden">
        <div className="px-5 py-4 border-b border-gray-200 dark:border-gray-800">
          <h2 className="text-base font-semibold text-gray-900 dark:text-white">Tenant Subscriptions</h2>
        </div>

        {loading || plansLoading ? (
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
                    <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">
                      Expires
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
                        colSpan={6}
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
                            className="text-sm border border-gray-200 dark:border-gray-700 rounded-lg px-2 py-1 bg-white dark:bg-gray-900 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-(--color-brand-blue) disabled:opacity-50"
                          >
                            <option value="">— Assign plan —</option>
                            {plans.filter((p) => p.isActive).map((p) => (
                              <option key={p.id} value={p.name}>{p.name}</option>
                            ))}
                          </select>
                        </td>
                        <td className="px-4 py-3">
                          <StatusBadge
                            status={t.subscriptionHealth ?? (t.isActive ? "Active" : "Suspended")}
                            label={
                              t.subscriptionHealth === "ExpiringSoon" && t.daysRemaining !== null
                                ? `Expiring in ${t.daysRemaining}d`
                                : undefined
                            }
                          />
                        </td>
                        <td className="px-4 py-3 text-sm text-gray-500 dark:text-gray-400">
                          {t.subscriptionEndDate ? formatDate(t.subscriptionEndDate) : "—"}
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
            <div className="pr-10 sm:pr-12">
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
                className="px-4 py-2 text-sm font-semibold bg-(--color-brand-blue) hover:bg-blue-700 text-white rounded-lg transition-colors disabled:opacity-50 flex items-center gap-2"
              >
                {assigning === pendingPlan.tenantId && <Loader size={14} className="animate-spin" />}
                Confirm
              </button>
            </div>
          </div>
        )}
      </Modal>

      <PlanFormModal
        isOpen={planModalOpen}
        onClose={() => setPlanModalOpen(false)}
        editingPlan={editingPlan}
        onSaved={refetchPlans}
      />

      <DeletePlanModal
        plan={deletingPlan}
        onClose={() => setDeletingPlan(null)}
        onDeleted={refetchPlans}
      />
    </div>
  )
}

function TenantView() {
  const { tenants, loading } = useTenants()
  const { plans, loading: plansLoading } = useSubscriptionPlans()
  const { tenantId } = getTokenClaims()
  const myTenant = tenants.find((t) => t.id === tenantId)
  const router = useRouter()

  if (loading || plansLoading) {
    return (
      <div className="text-sm text-gray-500 dark:text-gray-400 py-8 text-center">
        Loading subscription...
      </div>
    )
  }

  const plan = myTenant?.plan ?? plans[0]?.name ?? ""
  const planDetails = plans.find((p) => p.name === plan) ?? plans[0]

  if (!planDetails) {
    return (
      <div className="text-sm text-gray-500 dark:text-gray-400 py-8 text-center">
        No subscription plans are configured yet.
      </div>
    )
  }

  const health = myTenant?.subscriptionHealth ?? (myTenant?.isActive ? "Active" : "Suspended")
  const isExpiredOrGrace = health === "Expired" || health === "GracePeriod"

  const gracePeriodEndsAt = myTenant?.subscriptionEndDate
    ? new Date(new Date(myTenant.subscriptionEndDate).getTime() + 7 * 24 * 3600 * 1000)
    : null
  const gracePeriodEnded = gracePeriodEndsAt ? gracePeriodEndsAt < new Date() : true

  return (
    <div className="space-y-6 max-w-2xl">
      <div>
        <h2 className="text-lg font-semibold text-gray-900 dark:text-white">My Subscription</h2>
        <p className="text-sm text-gray-500 dark:text-gray-400">Your current plan and usage</p>
      </div>

      {/* Current plan card */}
      <div className={`bg-white dark:bg-gray-900 border rounded-xl p-6 space-y-5 ${
        health === "Expired" ? "border-red-300 dark:border-red-800"
        : health === "GracePeriod" ? "border-orange-300 dark:border-orange-800"
        : "border-(--color-brand-blue)/40"
      }`}>
        <div className="flex items-start justify-between">
          <div>
            <p className="text-xs text-gray-500 dark:text-gray-400 uppercase tracking-wide mb-1">
              Current Plan
            </p>
            <p className="text-2xl font-bold text-gray-900 dark:text-white">{plan}</p>
            <p className="text-xl font-semibold text-(--color-brand-blue) mt-0.5">
              RM {planDetails.price}
              <span className="text-sm font-normal text-gray-500 dark:text-gray-400">/month</span>
            </p>
          </div>
          <StatusBadge
            status={health}
            label={
              health === "ExpiringSoon" && myTenant?.daysRemaining != null
                ? `Expiring in ${myTenant.daysRemaining}d`
                : health === "GracePeriod"
                  ? "Grace period"
                  : undefined
            }
          />
        </div>

        <div className="grid grid-cols-3 gap-4 pt-2 border-t border-gray-200 dark:border-gray-800">
          <div className="flex flex-col gap-1">
            <div className="flex items-center gap-1.5 text-gray-500 dark:text-gray-400">
              <Package size={14} />
              <span className="text-xs uppercase tracking-wide">Packages</span>
            </div>
            <p className="text-sm font-semibold text-gray-900 dark:text-white">
              {myTenant?.syncedPackagesCount ?? 0} /{" "}
              {planDetails.maxPackages >= 999 ? "∞" : planDetails.maxPackages}
            </p>
          </div>
          <div className="flex flex-col gap-1">
            <div className="flex items-center gap-1.5 text-gray-500 dark:text-gray-400">
              <CalendarDays size={14} />
              <span className="text-xs uppercase tracking-wide">Started</span>
            </div>
            <p className="text-sm font-semibold text-gray-900 dark:text-white">
              {myTenant?.subscriptionStartDate
                ? formatDate(myTenant.subscriptionStartDate)
                : myTenant?.createdAt
                  ? formatDate(myTenant.createdAt)
                  : "—"}
            </p>
          </div>
          <div className="flex flex-col gap-1">
            <div className="flex items-center gap-1.5 text-gray-500 dark:text-gray-400">
              <CalendarDays size={14} />
              <span className="text-xs uppercase tracking-wide">Expires</span>
            </div>
            <p className={`text-sm font-semibold ${
              isExpiredOrGrace ? "text-red-600 dark:text-red-400" : "text-gray-900 dark:text-white"
            }`}>
              {myTenant?.subscriptionEndDate ? formatDate(myTenant.subscriptionEndDate) : "Never"}
            </p>
          </div>
        </div>
      </div>

      {/* Subscription lifecycle timeline — shown when expired or in grace period */}
      {isExpiredOrGrace && myTenant && (
        <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-5">
          <p className="text-sm font-semibold text-gray-900 dark:text-white mb-4">Subscription Timeline</p>
          <div className="space-y-0">
            {/* Row 1: Started */}
            <div className="flex items-start gap-3">
              <div className="flex flex-col items-center">
                <CheckCircle2 size={18} className="text-green-500 shrink-0" />
                <div className="w-px h-6 bg-gray-200 dark:bg-gray-700 mt-1" />
              </div>
              <div className="pb-4">
                <p className="text-sm font-medium text-gray-900 dark:text-white">Subscription started</p>
                <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
                  {myTenant.subscriptionStartDate
                    ? formatDate(myTenant.subscriptionStartDate)
                    : formatDate(myTenant.createdAt)}
                </p>
              </div>
            </div>

            {/* Row 2: Expired */}
            <div className="flex items-start gap-3">
              <div className="flex flex-col items-center">
                <XCircle size={18} className="text-red-500 shrink-0" />
                <div className="w-px h-6 bg-gray-200 dark:bg-gray-700 mt-1" />
              </div>
              <div className="pb-4">
                <p className="text-sm font-medium text-gray-900 dark:text-white">Subscription expired</p>
                <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
                  {myTenant.subscriptionEndDate ? formatDate(myTenant.subscriptionEndDate) : "—"}
                </p>
              </div>
            </div>

            {/* Row 3: Grace period ends */}
            <div className="flex items-start gap-3">
              <div className="flex flex-col items-center">
                {gracePeriodEnded
                  ? <XCircle size={18} className="text-red-400 shrink-0" />
                  : <Clock size={18} className="text-orange-500 shrink-0" />
                }
                <div className="w-px h-6 bg-gray-200 dark:bg-gray-700 mt-1" />
              </div>
              <div className="pb-4">
                <p className="text-sm font-medium text-gray-900 dark:text-white">
                  7-day grace period {gracePeriodEnded ? "ended" : "ends"}
                </p>
                <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
                  {gracePeriodEndsAt ? formatDate(gracePeriodEndsAt.toISOString()) : "—"}
                  {!gracePeriodEnded && (
                    <span className="ml-2 text-orange-600 dark:text-orange-400 font-medium">
                      Full access still available
                    </span>
                  )}
                </p>
              </div>
            </div>

            {/* Row 4: Current status */}
            <div className="flex items-start gap-3">
              <div className="flex flex-col items-center">
                {health === "Expired"
                  ? <Lock size={18} className="text-red-600 shrink-0" />
                  : <Clock size={18} className="text-orange-500 shrink-0" />
                }
              </div>
              <div>
                <p className={`text-sm font-semibold ${
                  health === "Expired"
                    ? "text-red-600 dark:text-red-400"
                    : "text-orange-600 dark:text-orange-400"
                }`}>
                  {health === "Expired" ? "Read-only mode active" : "Grace period — renew soon"}
                </p>
                <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
                  {health === "Expired"
                    ? "Creating, editing, and deleting records is disabled until you renew."
                    : "All features are available. Renew now to avoid losing access."}
                </p>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Renewal / billing panel — shown when expired or in grace period */}
      {isExpiredOrGrace ? (
        <div className={`border rounded-xl p-5 space-y-4 ${
          health === "Expired"
            ? "bg-red-50 dark:bg-red-900/10 border-red-200 dark:border-red-800"
            : "bg-orange-50 dark:bg-orange-900/10 border-orange-200 dark:border-orange-800"
        }`}>
          <div className="flex items-start gap-3">
            <AlertTriangle size={18} className={`shrink-0 mt-0.5 ${
              health === "Expired" ? "text-red-600" : "text-orange-500"
            }`} />
            <div>
              <p className={`text-sm font-semibold ${
                health === "Expired"
                  ? "text-red-800 dark:text-red-200"
                  : "text-orange-800 dark:text-orange-200"
              }`}>
                {health === "Expired" ? "Restore Full Access" : "Renew Your Subscription Early"}
              </p>
              <p className={`text-xs mt-0.5 ${
                health === "Expired"
                  ? "text-red-700 dark:text-red-300"
                  : "text-orange-700 dark:text-orange-300"
              }`}>
                {health === "Expired"
                  ? `Your ${plan} plan expired on ${myTenant?.subscriptionEndDate ? formatDate(myTenant.subscriptionEndDate) : "—"}. Select a plan below to restore access.`
                  : `Your ${plan} plan has expired but you're still in the grace period. Renew now to avoid interruption.`}
              </p>
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
            {plans.map((p) => (
              <div
                key={p.id}
                className="rounded-xl border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 p-4"
              >
                <p className="font-semibold text-gray-900 dark:text-white text-sm">{p.name}</p>
                <p className="text-(--color-brand-blue) font-bold mt-0.5">
                  RM {p.price}
                  <span className="text-xs font-normal text-gray-500 dark:text-gray-400">/mo</span>
                </p>
                <div className="mt-2 space-y-0.5 text-xs text-gray-500 dark:text-gray-400">
                  <p>{p.maxPackages >= 999 ? "Unlimited" : p.maxPackages} packages</p>
                  <p>{p.maxTeamMembers} members</p>
                </div>
                <button
                  onClick={() => router.push(`/subscriptions/renew?plan=${encodeURIComponent(p.name)}`)}
                  className="mt-3 w-full flex items-center justify-center gap-1.5 px-3 py-1.5 text-xs font-semibold bg-(--color-brand-blue) hover:bg-blue-700 text-white rounded-lg transition-colors"
                >
                  {`Renew with ${p.name} →`}
                </button>
              </div>
            ))}
          </div>
        </div>
      ) : (
        /* Available plans — shown for active / expiring soon tenants */
        <div>
          <p className="text-sm font-medium text-gray-900 dark:text-white mb-3">Available Plans</p>
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
            {plans.map((p) => (
              <div
                key={p.id}
                className={`rounded-xl border p-4 ${
                  p.name === plan
                    ? "border-(--color-brand-blue) bg-(--color-brand-blue)/5"
                    : "border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900"
                }`}
              >
                <p className="font-semibold text-gray-900 dark:text-white text-sm">{p.name}</p>
                <p className="text-(--color-brand-blue) font-bold mt-0.5">
                  RM {p.price}
                  <span className="text-xs font-normal text-gray-500 dark:text-gray-400">/mo</span>
                </p>
                <div className="mt-2 space-y-0.5 text-xs text-gray-500 dark:text-gray-400">
                  <p>{p.maxPackages >= 999 ? "Unlimited" : `${p.maxPackages}`} packages</p>
                  <p>{p.maxTeamMembers} members</p>
                </div>
                {p.name === plan ? (
                  <p className="mt-3 text-xs font-semibold text-(--color-brand-blue)">
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
      )}
    </div>
  )
}

export default function SubscriptionsPage() {
  const [tenantId, setTenantId] = useState<string | null | undefined>(undefined)

  useEffect(() => {
    const { tenantId: id } = getTokenClaims()
    setTenantId(id ?? null)
  }, [])

  if (tenantId === undefined) {
    return (
      <div className="flex items-center justify-center py-12">
        <Loader className="animate-spin text-gray-400" size={20} />
      </div>
    )
  }

  return tenantId ? <TenantView /> : <PlatformView />
}
