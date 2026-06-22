"use client";

import { useState, useRef } from "react";
import { MoreVertical } from "lucide-react";
import { Dropdown } from "@/components/ui/Dropdown";
import { cn } from "@/lib/utils";

interface MenuItem {
  label: string;
  onClick?: () => void;
  action?: () => void;
  danger?: boolean;
  destructive?: boolean;
  disabled?: boolean;
}

interface Props {
  items: MenuItem[];
}

export default function ActionMenu({ items }: Props) {
  const [isOpen, setIsOpen] = useState(false);
  const triggerRef = useRef<HTMLButtonElement>(null);

  return (
    <div className="relative inline-block">
      <button
        ref={triggerRef}
        onClick={() => setIsOpen((o) => !o)}
        className="dropdown-toggle p-1.5 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200"
      >
        <MoreVertical size={16} />
      </button>
      <Dropdown isOpen={isOpen} onClose={() => setIsOpen(false)} className="w-44 py-1">
        {items.map((item) => {
          const isDanger = item.danger || item.destructive;
          const handleClick = item.onClick || item.action;
          return (
            <button
              key={item.label}
              onClick={() => {
                if (!item.disabled) {
                  handleClick?.();
                  setIsOpen(false);
                }
              }}
              disabled={item.disabled}
              className={cn(
                "block w-full text-left px-4 py-2 text-sm transition-colors",
                isDanger
                  ? "text-error-600 hover:bg-error-50 dark:text-error-400 dark:hover:bg-error-500/10"
                  : "text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-white/5",
                item.disabled && "opacity-50 cursor-not-allowed"
              )}
            >
              {item.label}
            </button>
          );
        })}
      </Dropdown>
    </div>
  );
}
