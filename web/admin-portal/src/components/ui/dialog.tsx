"use client";

export { Modal as Dialog, Modal as DialogContent } from "@/components/ui/Modal";

export function DialogHeader({ children, className = "" }: { children: React.ReactNode; className?: string }) {
  return <div className={`px-6 pt-6 pb-4 border-b border-gray-200 dark:border-gray-800 ${className}`}>{children}</div>;
}

export function DialogFooter({ children, className = "" }: { children: React.ReactNode; className?: string }) {
  return <div className={`px-6 py-4 border-t border-gray-200 dark:border-gray-800 flex justify-end gap-3 ${className}`}>{children}</div>;
}

export function DialogTitle({ children, className = "" }: { children: React.ReactNode; className?: string }) {
  return <h3 className={`text-lg font-semibold text-gray-900 dark:text-white ${className}`}>{children}</h3>;
}

export function DialogDescription({ children, className = "" }: { children: React.ReactNode; className?: string }) {
  return <p className={`text-sm text-gray-500 dark:text-gray-400 mt-1 ${className}`}>{children}</p>;
}

export function DialogTrigger({ children }: { children: React.ReactNode }) {
  return <>{children}</>;
}

export function DialogClose({ children, onClick }: { children?: React.ReactNode; onClick?: () => void }) {
  return <button onClick={onClick} className="text-gray-400 hover:text-gray-600">{children}</button>;
}
