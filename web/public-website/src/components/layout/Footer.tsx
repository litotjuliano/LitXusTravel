"use client"

import Link from "next/link"
import { Globe, Share2, MessageCircle, Phone, Mail, MapPin } from "lucide-react"
import { useTenantName } from "@/lib/hooks/useTenantName"

const QUICK_LINKS = [
  { href: "/packages", label: "Packages" },
  { href: "/about",    label: "About Us" },
  { href: "/contact",  label: "Contact" },
]

const SUPPORT_LINKS = [
  { href: "/faq",          label: "FAQ" },
  { href: "/privacy",      label: "Privacy Policy" },
  { href: "/terms",        label: "Terms of Service" },
  { href: "/cancellation", label: "Cancellation Policy" },
]

export default function Footer() {
  const tenantName = useTenantName()

  return (
    <footer className="bg-gray-900 text-white">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 py-16">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-10 mb-12">
          {/* Brand */}
          <div>
            <h3 className="text-xl font-bold mb-4">✈ {tenantName}</h3>
            <p className="text-gray-400 text-sm leading-relaxed mb-5">
              Discover extraordinary travel experiences curated just for you.
            </p>
            <div className="flex gap-3">
              {[Globe, Share2, MessageCircle].map((Icon, i) => (
                <a
                  key={i}
                  href="#"
                  className="w-9 h-9 rounded-lg bg-gray-800 flex items-center justify-center hover:bg-[--color-brand-blue] transition-colors"
                >
                  <Icon size={16} />
                </a>
              ))}
            </div>
          </div>

          {/* Quick links */}
          <div>
            <h4 className="font-semibold mb-4">Quick Links</h4>
            <ul className="space-y-2">
              {QUICK_LINKS.map(({ href, label }) => (
                <li key={href}>
                  <Link href={href} className="text-gray-400 text-sm hover:text-white transition-colors">
                    {label}
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Support */}
          <div>
            <h4 className="font-semibold mb-4">Support</h4>
            <ul className="space-y-2">
              {SUPPORT_LINKS.map(({ href, label }) => (
                <li key={href}>
                  <Link href={href} className="text-gray-400 text-sm hover:text-white transition-colors">
                    {label}
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Contact */}
          <div>
            <h4 className="font-semibold mb-4">Get In Touch</h4>
            <ul className="space-y-3 text-gray-400 text-sm">
              <li className="flex items-start gap-2">
                <Phone size={16} className="mt-0.5 shrink-0" />
                <span>+60 12-345 6789</span>
              </li>
              <li className="flex items-start gap-2">
                <Mail size={16} className="mt-0.5 shrink-0" />
                <span>hello@nexustravel.com</span>
              </li>
              <li className="flex items-start gap-2">
                <MapPin size={16} className="mt-0.5 shrink-0" />
                <span>Kuala Lumpur, Malaysia</span>
              </li>
            </ul>
          </div>
        </div>

        <div className="border-t border-gray-800 pt-8 flex flex-col sm:flex-row items-center justify-between gap-3 text-sm text-gray-500">
          <p>© {new Date().getFullYear()} {tenantName}. All rights reserved.</p>
          <p>
            Powered by{" "}
            <span className="text-[--color-brand-blue] font-semibold">LitXusTravel Platform</span>
          </p>
        </div>
      </div>
    </footer>
  )
}
