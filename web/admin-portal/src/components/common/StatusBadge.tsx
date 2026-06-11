import { cn, getStatusColor } from "@/lib/utils"

interface Props { status: string; className?: string }

export default function StatusBadge({ status, className }: Props) {
  return (
    <span className={cn("inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-semibold", getStatusColor(status), className)}>
      {status}
    </span>
  )
}
