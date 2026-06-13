"use client"

import { useRouter } from "next/navigation"
import { Bell, Search, Menu } from "lucide-react"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu"
import { toast } from "sonner"
import { useEffect, useState } from "react"

interface Props {
  title: string
  onMenuClick?: () => void
}

interface User {
  email: string
  firstName: string
  lastName: string
}

export default function TopBar({ title, onMenuClick }: Props) {
  const router = useRouter()
  const [user, setUser] = useState<User | null>(null)

  const loadUser = () => {
    // Check for updated user info from profile page first
    const storedUserInfo = localStorage.getItem("user_info")
    if (storedUserInfo) {
      try {
        const userInfo = JSON.parse(storedUserInfo)
        setUser({
          email: userInfo.email,
          firstName: userInfo.firstName || "User",
          lastName: userInfo.lastName || "",
        })
        return
      } catch {
        console.error("Failed to parse stored user info")
      }
    }

    // Fall back to JWT token
    const token = localStorage.getItem("nexus_token")
    if (token) {
      try {
        const decoded = JSON.parse(atob(token.split(".")[1]))
        setUser({
          email: decoded.email,
          firstName: decoded.given_name || "User",
          lastName: decoded.family_name || "",
        })
      } catch {
        console.error("Failed to decode token")
      }
    }
  }

  useEffect(() => {
    loadUser()

    // Listen for storage changes (when profile is updated in other tabs)
    const handleStorageChange = () => {
      loadUser()
    }

    // Listen for custom user info update event (when profile is saved in current tab)
    const handleUserInfoUpdated = () => {
      loadUser()
    }

    window.addEventListener("storage", handleStorageChange)
    window.addEventListener("userInfoUpdated", handleUserInfoUpdated)
    return () => {
      window.removeEventListener("storage", handleStorageChange)
      window.removeEventListener("userInfoUpdated", handleUserInfoUpdated)
    }
  }, [])

  const handleLogout = () => {
    localStorage.removeItem("nexus_token")
    localStorage.removeItem("user_info")
    toast.success("Logged out successfully")
    router.push("/auth/login")
  }

  const handleProfile = () => {
    router.push("/profile")
  }

  return (
    <header className="sticky top-0 z-20 h-16 bg-card border-b border-border flex items-center justify-between px-4 sm:px-6 shrink-0">
      <div className="flex items-center gap-4">
        <button onClick={onMenuClick} className="md:hidden p-2 rounded-lg hover:bg-muted transition-colors">
          <Menu size={20} />
        </button>
        <h1 className="text-lg font-semibold text-foreground">{title}</h1>
      </div>

      <div className="flex items-center gap-3">
        {/* Search */}
        <div className="hidden lg:flex items-center gap-2 px-3 py-2 bg-muted rounded-lg border border-border text-sm text-muted-foreground w-56">
          <Search size={15} />
          <span>Search...</span>
        </div>

        {/* Notifications */}
        <button className="relative p-2 rounded-lg hover:bg-muted transition-colors">
          <Bell size={18} className="text-muted-foreground" />
          <span className="absolute top-1.5 right-1.5 w-2 h-2 bg-[--color-brand-error] rounded-full" />
        </button>

        {/* User */}
        <DropdownMenu>
          <DropdownMenuTrigger className="flex items-center gap-2 p-1 rounded-lg hover:bg-muted transition-colors">
              <Avatar className="w-8 h-8">
                <AvatarFallback className="bg-[--color-brand-blue] text-white text-xs font-bold">
                  {user?.firstName?.[0]?.toUpperCase() || "A"}
                </AvatarFallback>
              </Avatar>
              <div className="hidden sm:block text-left">
                <p className="text-sm font-medium text-foreground leading-tight">{user?.firstName || "User"}</p>
                <p className="text-xs text-muted-foreground">{user?.email || "—"}</p>
              </div>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-44">
            <DropdownMenuItem onClick={handleProfile}>Profile</DropdownMenuItem>
            <DropdownMenuItem>Settings</DropdownMenuItem>
            <DropdownMenuItem className="text-destructive" onClick={handleLogout}>
              Logout
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </header>
  )
}
