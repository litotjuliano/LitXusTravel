import type { Metadata } from "next"
import { Toaster } from "@/components/ui/sonner"
import "./globals.css"

export const metadata: Metadata = {
  title: "LitXusTravel Tenant Dashboard",
  description: "LitXusTravel travel agency management portal.",
}

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body className="min-h-screen bg-white dark:bg-gray-900 text-gray-900 dark:text-white">
        {children}
        <Toaster richColors position="top-right" />
      </body>
    </html>
  )
}
