"use client"

import { ChevronUp, ChevronDown, ChevronsUpDown } from "lucide-react"

interface SortableHeaderProps {
  label: string
  sortKey: string
  currentSortBy?: string
  sortOrder: "asc" | "desc"
  onSort: (key: string) => void
  className?: string
}

export function SortableHeader({
  label,
  sortKey,
  currentSortBy,
  sortOrder,
  onSort,
  className,
}: SortableHeaderProps) {
  const isActive = currentSortBy === sortKey

  return (
    <th
      onClick={() => onSort(sortKey)}
      className={`text-left px-4 py-3 text-xs font-semibold uppercase tracking-wide cursor-pointer select-none group transition-colors ${
        isActive ? "text-foreground" : "text-muted-foreground hover:text-foreground"
      } ${className ?? ""}`}
    >
      <span className="flex items-center gap-1">
        {label}
        <span className="inline-flex shrink-0">
          {isActive ? (
            sortOrder === "asc" ? (
              <ChevronUp size={12} />
            ) : (
              <ChevronDown size={12} />
            )
          ) : (
            <ChevronsUpDown
              size={12}
              className="opacity-40 group-hover:opacity-70 transition-opacity"
            />
          )}
        </span>
      </span>
    </th>
  )
}
