"use client"

import { Toaster as SonnerToaster, type ToasterProps } from "sonner"

export function Toaster({ position = "bottom-right", ...props }: ToasterProps) {
  return <SonnerToaster richColors position={position} {...props} />
}
