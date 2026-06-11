"use client"

import { MoreVertical } from "lucide-react"
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu"
import { cn } from "@/lib/utils"

interface MenuItem { label: string; action: () => void; danger?: boolean }
interface Props { items: MenuItem[] }

export default function ActionMenu({ items }: Props) {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger className="p-1.5 rounded-lg hover:bg-muted transition-colors text-muted-foreground hover:text-foreground">
        <MoreVertical size={16} />
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-44">
        {items.map((item) => (
          <DropdownMenuItem
            key={item.label}
            onClick={item.action}
            className={cn("cursor-pointer text-sm", item.danger && "text-destructive focus:text-destructive")}
          >
            {item.label}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  )
}
