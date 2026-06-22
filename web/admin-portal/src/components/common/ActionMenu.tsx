"use client"

import { useState } from "react"
import { MoreVertical } from "lucide-react"
import { Dropdown } from "@/components/ui/Dropdown"
import { DropdownItem } from "@/components/ui/DropdownItem"

interface MenuItem { label: string; action: () => void; danger?: boolean }
interface Props { items: MenuItem[] }

export default function ActionMenu({ items }: Props) {
  const [open, setOpen] = useState(false)

  return (
    <div className="relative">
      <button
        onClick={() => setOpen((v) => !v)}
        className="dropdown-toggle p-1.5 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white"
      >
        <MoreVertical size={16} />
      </button>
      <Dropdown isOpen={open} onClose={() => setOpen(false)} className="w-44">
        {items.map((item) => (
          <DropdownItem
            key={item.label}
            onClick={item.action}
            onItemClick={() => setOpen(false)}
            className={item.danger ? "text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20" : ""}
          >
            {item.label}
          </DropdownItem>
        ))}
      </Dropdown>
    </div>
  )
}
