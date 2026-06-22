import type { Metadata } from "next"
import { Inter } from "next/font/google"
import { Toaster } from "@/components/ui/sonner"
import { AppInitializer } from "@/components/AppInitializer"
import "./globals.css"

const inter = Inter({ subsets: ["latin"], variable: "--font-inter", display: "swap" })

export const metadata: Metadata = {
  title: "LitXusTravel Admin",
  description: "LitXusTravel platform administration portal.",
}

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en" className={`${inter.variable} dark`}>
      <body className="min-h-screen bg-white dark:bg-gray-900 text-gray-900 dark:text-white">
        <AppInitializer />
        {children}
        <Toaster richColors position="top-right" />
      </body>
    </html>
  )
}
