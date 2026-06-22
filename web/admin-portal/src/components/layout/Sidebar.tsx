"use client"

import Link from "next/link"
import { usePathname } from "next/navigation"
import {
  LayoutDashboard, Package, Users, CreditCard,
  BarChart3, Settings, LogOut, ChevronLeft, ChevronRight, UserCircle,
} from "lucide-react"
import { cn, getTokenClaims } from "@/lib/utils"
import { useState, useEffect } from "react"

const NAV = [
  { href: "/",             icon: LayoutDashboard, label: "Dashboard" },
  { href: "/packages",     icon: Package,         label: "Packages" },
  { href: "/tenants",      icon: Users,           label: "Tenants",       superOnly: true },
  { href: "/subscriptions",icon: CreditCard,      label: "Subscriptions" },
  { href: "/analytics",    icon: BarChart3,       label: "Analytics" },
  { href: "/profile",      icon: UserCircle,      label: "Profile" },
  { href: "/settings",     icon: Settings,        label: "Settings" },
]

export default function Sidebar() {
  const pathname = usePathname()
  const [collapsed, setCollapsed] = useState(false)
  // Initialize to undefined so server and client render the same nav on first paint,
  // then update after mount to apply role-based filtering (avoids hydration mismatch).
  const [tenantId, setTenantId] = useState<string | undefined>(undefined)

  useEffect(() => {
    const { tenantId: tid } = getTokenClaims()
    setTenantId(tid)
  }, [])

  const nav = NAV.filter((item) => !item.superOnly || !tenantId)

  return (
    <aside
      className={cn(
        "hidden md:flex flex-col fixed left-0 top-0 h-screen z-40 transition-all duration-300",
        "bg-[--color-sidebar] border-r border-white/10",
        collapsed ? "w-16" : "w-64"
      )}
    >
      {/* Logo */}
      <div className={cn("flex items-center gap-3 px-4 h-16 border-b border-white/10 shrink-0", collapsed && "justify-center px-2")}>
        <span className="text-xl">✈</span>
        {!collapsed && (
          <div>
            <p className="text-white font-bold text-sm leading-tight">LitXusTravel</p>
            <p className="text-white/50 text-xs">Admin Portal</p>
          </div>
        )}
      </div>

      {/* Nav */}
      <nav className="flex-1 py-4 px-2 space-y-1 overflow-y-auto">
        {nav.map(({ href, icon: Icon, label }) => {
          const active = href === "/" ? pathname === "/" : pathname.startsWith(href)
          return (
            <Link
              key={href}
              href={href}
              title={collapsed ? label : undefined}
              className={cn(
                "flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors",
                active
                  ? "bg-[--color-brand-blue] text-white"
                  : "text-white/60 hover:text-white hover:bg-white/10",
                collapsed && "justify-center px-2"
              )}
            >
              <Icon size={18} className="shrink-0" />
              {!collapsed && label}
            </Link>
          )
        })}
      </nav>

      {/* Footer */}
      <div className="px-2 pb-4 space-y-1 shrink-0 border-t border-white/10 pt-3">
        <button
          onClick={() => {
            if (typeof window !== "undefined") localStorage.removeItem("litxus_token")
            window.location.href = "/auth/login"
          }}
          className={cn(
            "w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium text-white/60 hover:text-white hover:bg-white/10 transition-colors",
            collapsed && "justify-center px-2"
          )}
        >
          <LogOut size={18} className="shrink-0" />
          {!collapsed && "Logout"}
        </button>

        <button
          onClick={() => setCollapsed(!collapsed)}
          className="w-full flex items-center justify-center py-2 text-white/40 hover:text-white/70 transition-colors"
        >
          {collapsed ? <ChevronRight size={16} /> : <ChevronLeft size={16} />}
        </button>
      </div>
    </aside>
  )
}
