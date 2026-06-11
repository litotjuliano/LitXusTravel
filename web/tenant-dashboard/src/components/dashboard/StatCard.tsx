"use client"

import { TrendingUp, TrendingDown } from "lucide-react"
import { motion } from "framer-motion"
import { cn } from "@/lib/utils"
import type { StatCard as StatCardType } from "@/types"

interface Props extends StatCardType {
  delay?: number
}

export default function StatCard({ label, value, change, positive, icon: Icon, delay = 0 }: Props) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 16 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3, delay }}
      className="bg-card border border-border rounded-xl p-5 hover:shadow-md dark:hover:shadow-black/30 transition-shadow"
    >
      <div className="flex items-start justify-between mb-4">
        <div>
          <p className="text-xs font-semibold text-muted-foreground uppercase tracking-wide mb-1">{label}</p>
          <h3 className="text-2xl font-bold text-foreground">{value}</h3>
        </div>
        <div className="w-10 h-10 rounded-lg bg-[--color-brand-blue]/10 flex items-center justify-center">
          <Icon size={20} className="text-[--color-brand-blue]" />
        </div>
      </div>
      <div className="flex items-center gap-1.5">
        {positive
          ? <TrendingUp size={14} className="text-[--color-brand-success]" />
          : <TrendingDown size={14} className="text-[--color-brand-error]" />
        }
        <span className={cn("text-xs font-semibold", positive ? "text-[--color-brand-success]" : "text-[--color-brand-error]")}>
          {change}
        </span>
        <span className="text-xs text-muted-foreground">vs last month</span>
      </div>
    </motion.div>
  )
}
