import StatusBadge from "@/components/common/StatusBadge"
import { formatCurrency, formatDate } from "@/lib/utils"
import { MOCK_TENANTS } from "@/lib/mock-data"

const PLANS = [
  { name: "Starter", price: 99,  tenants: 12, packages: 10, members: 2 },
  { name: "Pro",     price: 299, tenants: 8,  packages: 50, members: 10 },
  { name: "Enterprise", price: 999, tenants: 3, packages: 999, members: 50 },
]

export default function SubscriptionsPage() {
  return (
    <div className="space-y-6">
      {/* Plan cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {PLANS.map((plan) => (
          <div key={plan.name} className="bg-card border border-border rounded-xl p-5">
            <h3 className="font-bold text-foreground mb-1">{plan.name}</h3>
            <p className="text-2xl font-bold text-[--color-brand-blue] mb-4">
              RM {plan.price}<span className="text-sm font-normal text-muted-foreground">/mo</span>
            </p>
            <div className="space-y-1.5 text-sm text-muted-foreground mb-4">
              <p>Up to {plan.packages === 999 ? "Unlimited" : plan.packages} packages</p>
              <p>Up to {plan.members} team members</p>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-xs text-muted-foreground">{plan.tenants} active tenants</span>
              <span className="text-xs font-semibold text-[--color-brand-blue]">
                RM {(plan.price * plan.tenants).toLocaleString()}/mo
              </span>
            </div>
          </div>
        ))}
      </div>

      {/* Tenant subscriptions */}
      <div className="bg-card border border-border rounded-xl overflow-hidden">
        <div className="px-5 py-4 border-b border-border">
          <h2 className="text-base font-semibold text-foreground">Tenant Subscriptions</h2>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b border-border">
              <tr>
                {["Tenant", "Plan", "Status", "Joined", "Packages Synced"].map((h) => (
                  <th key={h} className="text-left px-4 py-3 text-xs font-semibold text-muted-foreground uppercase tracking-wide">{h}</th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-border">
              {MOCK_TENANTS.map((t) => (
                <tr key={t.id} className="hover:bg-muted/30 transition-colors">
                  <td className="px-4 py-3">
                    <p className="font-medium text-foreground">{t.name}</p>
                    <p className="text-xs text-muted-foreground">{t.contactEmail}</p>
                  </td>
                  <td className="px-4 py-3 text-foreground">{t.plan ?? "—"}</td>
                  <td className="px-4 py-3"><StatusBadge status={t.plan === "Trial" ? "Trial" : t.isActive ? "Active" : "Suspended"} /></td>
                  <td className="px-4 py-3 text-muted-foreground">{formatDate(t.createdAt)}</td>
                  <td className="px-4 py-3 font-semibold text-foreground">{t.syncedPackagesCount ?? 0}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}
