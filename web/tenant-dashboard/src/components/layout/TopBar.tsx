"use client"

import { useEffect, useState } from "react"
import { useRouter } from "next/navigation"
import { Bell, Search, Menu } from "lucide-react"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu"
import { toast } from "sonner"

interface Props {
  title: string
  onMenuClick?: () => void
}

export default function TopBar({ title, onMenuClick }: Props) {
  const router = useRouter()
  const [email, setEmail] = useState("")

  useEffect(() => {
    const storedEmail = localStorage.getItem("nexus_user_email") || ""
    setEmail(storedEmail)
  }, [])

  const handleLogout = () => {
    localStorage.removeItem("nexus_token")
    localStorage.removeItem("nexus_tenant_id")
    localStorage.removeItem("nexus_user_email")
    toast.success("Logged out successfully")
    router.push("/auth/login")
  }

  return (
    <header className="h-16 bg-card border-b border-border flex items-center justify-between px-4 sm:px-6 shrink-0">
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
                  {email.charAt(0).toUpperCase()}
                </AvatarFallback>
              </Avatar>
              <div className="hidden sm:block text-left">
                <p className="text-sm font-medium text-foreground leading-tight">Agent</p>
                <p className="text-xs text-muted-foreground">{email}</p>
              </div>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-44">
            <DropdownMenuItem>Profile</DropdownMenuItem>
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
