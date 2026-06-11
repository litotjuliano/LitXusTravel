"use client"

import { useState } from "react"
import Sidebar from "@/components/layout/Sidebar"
import TopBar from "@/components/layout/TopBar"
import { Sheet, SheetContent } from "@/components/ui/sheet"
import { usePathname } from "next/navigation"

const PAGE_TITLES: Record<string, string> = {
  "/":              "Dashboard",
  "/packages":      "Packages",
  "/tenants":       "Tenants",
  "/subscriptions": "Subscriptions",
  "/analytics":     "Analytics",
  "/settings":      "Settings",
}

export default function DashboardLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname()
  const [mobileOpen, setMobileOpen] = useState(false)

  const title = Object.entries(PAGE_TITLES)
    .find(([key]) => key === "/" ? pathname === "/" : pathname.startsWith(key))?.[1] ?? "Admin"

  return (
    <div className="flex h-screen overflow-hidden bg-background">
      {/* Desktop sidebar */}
      <Sidebar />

      {/* Mobile sidebar */}
      <Sheet open={mobileOpen} onOpenChange={setMobileOpen}>
        <SheetContent side="left" className="p-0 w-64 bg-[--color-sidebar]" showCloseButton={false}>
          <Sidebar />
        </SheetContent>
      </Sheet>

      {/* Main content */}
      <div className="flex-1 flex flex-col min-w-0 md:ml-64 transition-all duration-300">
        <TopBar title={title} onMenuClick={() => setMobileOpen(true)} />
        <main className="flex-1 overflow-y-auto p-4 sm:p-6">
          {children}
        </main>
      </div>
    </div>
  )
}
