"use client";

/* Sheet is no longer used — mobile sidebar is handled by SidebarContext + Backdrop */
export function Sheet({ children }: { children: React.ReactNode }) { return <>{children}</>; }
export function SheetContent({ children }: { children: React.ReactNode; side?: string; className?: string; showCloseButton?: boolean }) { return <>{children}</>; }
export function SheetTrigger({ children }: { children: React.ReactNode }) { return <>{children}</>; }
export function SheetClose({ children }: { children?: React.ReactNode }) { return <>{children ?? null}</>; }
export function SheetHeader({ children }: { children: React.ReactNode }) { return <>{children}</>; }
export function SheetFooter({ children }: { children: React.ReactNode }) { return <>{children}</>; }
export function SheetTitle({ children }: { children: React.ReactNode }) { return <>{children}</>; }
export function SheetDescription({ children }: { children: React.ReactNode }) { return <>{children}</>; }
