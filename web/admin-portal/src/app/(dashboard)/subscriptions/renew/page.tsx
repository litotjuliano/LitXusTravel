"use client"

import { useState, Suspense } from "react"
import { useRouter, useSearchParams } from "next/navigation"
import { CreditCard, ArrowLeft, Lock, Loader, ShieldCheck } from "lucide-react"
import { useSubscriptionPlans } from "@/lib/hooks/useSubscriptionPlans"
import { adminApi } from "@/lib/api"
import { getTokenClaims } from "@/lib/utils"
import { toast } from "sonner"

type CardForm = {
  cardholderName: string
  cardNumber: string
  expiryDate: string
  cvv: string
}

function RenewContent() {
  const router = useRouter()
  const searchParams = useSearchParams()
  const planName = searchParams.get("plan") ?? ""
  const { tenantId } = getTokenClaims()

  const { plans, loading: plansLoading } = useSubscriptionPlans()
  const planDetails = plans.find((p) => p.name === planName) ?? plans[0]

  const [form, setForm] = useState<CardForm>({
    cardholderName: "Demo User",
    cardNumber: "4111 1111 1111 1111",
    expiryDate: "12/28",
    cvv: "123",
  })
  const [submitting, setSubmitting] = useState(false)

  function set(field: keyof CardForm, value: string) {
    setForm((f) => ({ ...f, [field]: value }))
  }

  async function handlePay(e: React.FormEvent) {
    e.preventDefault()
    if (!tenantId || !planDetails) return
    setSubmitting(true)
    try {
      await adminApi.processMockPayment(tenantId, {
        planName: planDetails.name,
        cardholderName: form.cardholderName,
        cardNumber: form.cardNumber,
        expiryDate: form.expiryDate,
        cvv: form.cvv,
      })
      toast.success("Payment successful! Your subscription has been renewed and full access is restored.")
      router.push("/subscriptions")
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Payment failed. Please try again.")
    } finally {
      setSubmitting(false)
    }
  }

  const inputCls =
    "w-full px-3 py-2.5 text-sm border border-gray-200 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-950 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-(--color-brand-blue) transition-colors"

  return (
    <div className="max-w-md mx-auto space-y-6">
      {/* Back link */}
      <button
        onClick={() => router.back()}
        className="flex items-center gap-1.5 text-sm text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white transition-colors"
      >
        <ArrowLeft size={15} />
        Back to Subscription
      </button>

      {/* Page header */}
      <div className="flex items-center gap-3">
        <div className="flex items-center justify-center w-10 h-10 rounded-xl bg-(--color-brand-blue)/10">
          <CreditCard size={20} className="text-(--color-brand-blue)" />
        </div>
        <div>
          <h1 className="text-lg font-bold text-gray-900 dark:text-white">Complete Your Payment</h1>
          <p className="text-xs text-gray-500 dark:text-gray-400">Subscription renewal — 1-year term</p>
        </div>
      </div>

      {/* Plan summary */}
      {plansLoading ? (
        <div className="h-16 bg-gray-100 dark:bg-gray-800 rounded-xl animate-pulse" />
      ) : planDetails ? (
        <div className="bg-(--color-brand-blue)/5 border border-(--color-brand-blue)/30 rounded-xl px-4 py-3 flex items-center justify-between">
          <div>
            <p className="text-sm font-semibold text-gray-900 dark:text-white">{planDetails.name} Plan</p>
            <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
              {planDetails.maxPackages >= 999 ? "Unlimited" : planDetails.maxPackages} packages · {planDetails.maxTeamMembers} members
            </p>
          </div>
          <div className="text-right">
            <p className="text-lg font-bold text-(--color-brand-blue)">RM {planDetails.price}</p>
            <p className="text-xs text-gray-500 dark:text-gray-400">per year</p>
          </div>
        </div>
      ) : null}

      {/* Payment form */}
      <form onSubmit={handlePay} className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-6 space-y-4">
        <div>
          <label className="block text-xs font-medium text-gray-500 dark:text-gray-400 mb-1.5">Cardholder Name</label>
          <input
            className={inputCls}
            value={form.cardholderName}
            onChange={(e) => set("cardholderName", e.target.value)}
            placeholder="Full name on card"
            required
          />
        </div>

        <div>
          <label className="block text-xs font-medium text-gray-500 dark:text-gray-400 mb-1.5">Card Number</label>
          <input
            className={inputCls}
            value={form.cardNumber}
            onChange={(e) => set("cardNumber", e.target.value)}
            placeholder="1234 5678 9012 3456"
            maxLength={19}
            required
          />
        </div>

        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className="block text-xs font-medium text-gray-500 dark:text-gray-400 mb-1.5">Expiry Date</label>
            <input
              className={inputCls}
              value={form.expiryDate}
              onChange={(e) => set("expiryDate", e.target.value)}
              placeholder="MM/YY"
              maxLength={5}
              required
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-gray-500 dark:text-gray-400 mb-1.5">CVV</label>
            <input
              className={inputCls}
              value={form.cvv}
              onChange={(e) => set("cvv", e.target.value)}
              placeholder="123"
              maxLength={4}
              required
            />
          </div>
        </div>

        {/* Demo notice */}
        <div className="flex items-start gap-2 px-3 py-2.5 rounded-lg bg-amber-50 dark:bg-amber-900/20 border border-amber-200 dark:border-amber-700">
          <ShieldCheck size={15} className="text-amber-600 dark:text-amber-400 mt-0.5 shrink-0" />
          <p className="text-xs text-amber-700 dark:text-amber-300">
            <span className="font-semibold">Demo mode</span> — This is a simulated payment. No real charges are made. Use any card details above.
          </p>
        </div>

        {/* Actions */}
        <div className="flex gap-3 pt-1">
          <button
            type="button"
            onClick={() => router.back()}
            disabled={submitting}
            className="flex-1 px-4 py-2.5 text-sm font-medium text-gray-600 dark:text-gray-400 border border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-800 rounded-lg transition-colors disabled:opacity-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={submitting || !planDetails}
            className="flex-1 flex items-center justify-center gap-2 px-4 py-2.5 text-sm font-semibold bg-(--color-brand-blue) hover:bg-blue-700 text-white rounded-lg transition-colors disabled:opacity-50"
          >
            {submitting ? (
              <><Loader size={14} className="animate-spin" />Processing…</>
            ) : (
              <><Lock size={14} />Pay RM {planDetails?.price ?? "—"} Now</>
            )}
          </button>
        </div>
      </form>
    </div>
  )
}

export default function RenewSubscriptionPage() {
  return (
    <Suspense fallback={
      <div className="flex items-center justify-center py-12">
        <Loader className="animate-spin text-gray-400" size={20} />
      </div>
    }>
      <RenewContent />
    </Suspense>
  )
}
