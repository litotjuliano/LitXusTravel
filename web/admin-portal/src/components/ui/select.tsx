"use client";

// Lightweight native-select wrapper — replaces the @base-ui/react implementation.
import type React from "react";
import { cn } from "@/lib/utils";

interface SelectProps {
  value?: string;
  onValueChange?: (value: string) => void;
  children?: React.ReactNode;
  disabled?: boolean;
  className?: string;
}

export function Select({ value, onValueChange, children, disabled, className = "" }: SelectProps) {
  return (
    <select
      value={value}
      onChange={(e) => onValueChange?.(e.target.value)}
      disabled={disabled}
      className={cn(
        "w-full px-3 py-2 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-900 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-brand-500/30 transition-colors",
        className
      )}
    >
      {children}
    </select>
  );
}

// Passthrough stubs — the native <select> renders everything inline.
export function SelectTrigger({ children }: { children?: React.ReactNode; className?: string }) { return <>{children}</>; }
export function SelectValue({ placeholder }: { placeholder?: string }) { return <option value="">{placeholder ?? ""}</option>; }
export function SelectContent({ children }: { children?: React.ReactNode }) { return <>{children}</>; }
export function SelectItem({ value, children }: { value: string; children: React.ReactNode }) { return <option value={value}>{children}</option>; }
export function SelectGroup({ children }: { children?: React.ReactNode }) { return <>{children}</>; }
export function SelectLabel({ children }: { children?: React.ReactNode }) { return null; }
export function SelectSeparator() { return null; }
