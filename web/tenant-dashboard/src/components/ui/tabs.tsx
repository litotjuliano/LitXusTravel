"use client";

import { createContext, useContext, useState } from "react";
import { cn } from "@/lib/utils";

const TabsContext = createContext<{ value: string; onValueChange: (v: string) => void }>({
  value: "",
  onValueChange: () => {},
});

function Tabs({
  className,
  defaultValue = "",
  value: controlledValue,
  onValueChange,
  children,
}: {
  className?: string;
  defaultValue?: string;
  value?: string;
  onValueChange?: (value: string) => void;
  children: React.ReactNode;
}) {
  const [internalValue, setInternalValue] = useState(defaultValue);
  const value = controlledValue ?? internalValue;
  const handleChange = (v: string) => {
    setInternalValue(v);
    onValueChange?.(v);
  };
  return (
    <TabsContext.Provider value={{ value, onValueChange: handleChange }}>
      <div className={cn("flex flex-col gap-2", className)}>{children}</div>
    </TabsContext.Provider>
  );
}

function TabsList({ className, children }: { className?: string; children: React.ReactNode }) {
  return (
    <div className={cn("inline-flex items-center gap-1 rounded-lg bg-gray-100 dark:bg-gray-800 p-1", className)}>
      {children}
    </div>
  );
}

function TabsTrigger({ className, value, children }: { className?: string; value: string; children: React.ReactNode }) {
  const ctx = useContext(TabsContext);
  const active = ctx.value === value;
  return (
    <button
      type="button"
      onClick={() => ctx.onValueChange(value)}
      className={cn(
        "px-3 py-1.5 text-sm font-medium rounded-md transition-colors",
        active
          ? "bg-white dark:bg-gray-900 text-gray-900 dark:text-white shadow-sm"
          : "text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300",
        className
      )}
    >
      {children}
    </button>
  );
}

function TabsContent({ className, value, children }: { className?: string; value: string; children: React.ReactNode }) {
  const ctx = useContext(TabsContext);
  if (ctx.value !== value) return null;
  return <div className={cn("flex-1 text-sm outline-none", className)}>{children}</div>;
}

export { Tabs, TabsList, TabsTrigger, TabsContent };
