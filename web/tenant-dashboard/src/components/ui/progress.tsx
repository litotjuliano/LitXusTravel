"use client";

import { cn } from "@/lib/utils";

function Progress({
  className,
  value,
}: {
  className?: string;
  value?: number | null;
}) {
  return (
    <div
      className={cn("relative h-1 w-full overflow-hidden rounded-full bg-gray-200 dark:bg-gray-700", className)}
    >
      <div
        className="h-full bg-brand-500 transition-all"
        style={{ width: `${Math.min(Math.max(value ?? 0, 0), 100)}%` }}
      />
    </div>
  );
}

export { Progress };
