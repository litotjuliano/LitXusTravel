"use client"

import { useState, useEffect } from "react"
import Link from "next/link"
import { usePathname } from "next/navigation"
import { Menu, X, MessageCircle } from "lucide-react"
import { buttonVariants } from "@/components/ui/button"
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet"
import { cn } from "@/lib/utils"

const WHATSAPP_NUMBER = process.env.NEXT_PUBLIC_WHATSAPP_NUMBER ?? "601234567890"
const WHATSAPP_URL = `https://wa.me/${WHATSAPP_NUMBER}?text=Hi%2C%20I%27m%20interested%20in%20your%20travel%20packages!`

const NAV_LINKS = [
  { href: "/packages", label: "Packages" },
  { href: "/about",    label: "About" },
  { href: "/contact",  label: "Contact" },
]

export default function Navbar() {
  const pathname = usePathname()
  const [scrolled, setScrolled] = useState(false)
  const [open, setOpen] = useState(false)

  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 20)
    window.addEventListener("scroll", onScroll, { passive: true })
    return () => window.removeEventListener("scroll", onScroll)
  }, [])

  return (
    <header
      className={cn(
        "sticky top-0 z-50 w-full transition-all duration-300",
        scrolled
          ? "bg-white/95 backdrop-blur-md shadow-sm border-b border-border"
          : "bg-white"
      )}
    >
      <div className="max-w-7xl mx-auto px-4 sm:px-6 h-16 flex items-center justify-between">
        {/* Logo */}
        <Link href="/" className="flex items-center gap-2 font-bold text-xl text-[--color-brand-blue]">
          ✈ LitXusTravel
        </Link>

        {/* Desktop nav */}
        <nav className="hidden md:flex items-center gap-8">
          {NAV_LINKS.map(({ href, label }) => (
            <Link
              key={href}
              href={href}
              className={cn(
                "text-sm font-medium transition-colors hover:text-[--color-brand-blue]",
                pathname === href ? "text-[--color-brand-blue]" : "text-muted-foreground"
              )}
            >
              {label}
            </Link>
          ))}
        </nav>

        {/* Desktop CTA */}
        <div className="hidden md:flex items-center gap-3">
          <a
            href={WHATSAPP_URL}
            target="_blank"
            rel="noopener noreferrer"
            className={cn(buttonVariants({ size: "sm" }), "bg-[--color-brand-blue] hover:bg-blue-700 text-white gap-2")}
          >
            <MessageCircle size={16} />
            WhatsApp Us
          </a>
        </div>

        {/* Mobile menu */}
        <Sheet open={open} onOpenChange={setOpen}>
          <SheetTrigger className="md:hidden p-2 rounded-lg hover:bg-muted transition-colors">
            {open ? <X size={22} /> : <Menu size={22} />}
          </SheetTrigger>
          <SheetContent side="right" className="w-72">
            <div className="flex flex-col gap-1 mt-8">
              {NAV_LINKS.map(({ href, label }) => (
                <Link
                  key={href}
                  href={href}
                  onClick={() => setOpen(false)}
                  className={cn(
                    "px-4 py-3 rounded-lg text-sm font-medium transition-colors",
                    pathname === href
                      ? "bg-blue-50 text-[--color-brand-blue]"
                      : "hover:bg-muted text-foreground"
                  )}
                >
                  {label}
                </Link>
              ))}
              <div className="mt-4 px-4">
                <a
                  href={WHATSAPP_URL}
                  target="_blank"
                  rel="noopener noreferrer"
                  className={cn(buttonVariants(), "w-full justify-center bg-[--color-brand-blue] hover:bg-blue-700 text-white gap-2")}
                >
                  <MessageCircle size={16} />
                  WhatsApp Us
                </a>
              </div>
            </div>
          </SheetContent>
        </Sheet>
      </div>
    </header>
  )
}
