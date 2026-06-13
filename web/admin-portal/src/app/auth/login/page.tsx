"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { Loader2, Lock, WifiOff } from "lucide-react"
import { Button } from "@/components/ui/button"
import { adminApi } from "@/lib/api"
import { toast } from "sonner"

const schema = z.object({
  email:    z.string().email("Enter a valid email"),
  password: z.string().min(6, "Password must be at least 6 characters"),
})
type FormValues = z.infer<typeof schema>

export default function LoginPage() {
  const router = useRouter()
  const [loading, setLoading] = useState(false)
  const [serverDown, setServerDown] = useState(false)

  const { register, handleSubmit, formState: { errors } } = useForm<FormValues>({ resolver: zodResolver(schema) })

  const onSubmit = async (data: FormValues) => {
    setLoading(true)
    setServerDown(false)
    try {
      const res = await adminApi.login(data.email, data.password)
      localStorage.setItem("nexus_token", res.data.accessToken)
      localStorage.removeItem("user_info")
      setLoading(false)
      router.push("/")
    } catch (err: unknown) {
      setLoading(false)
      if (err instanceof Error && err.message === "NETWORK_ERROR") {
        setServerDown(true)
        return
      }
      toast.error(err instanceof Error ? err.message : "Login failed")
    }
  }

  return (
    <div className="min-h-screen bg-[--color-sidebar] flex items-center justify-center px-4">
      <div className="w-full max-w-sm">
        {/* Logo */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-14 h-14 rounded-2xl bg-[--color-brand-blue] mb-4">
            <Lock size={24} className="text-white" />
          </div>
          <h1 className="text-2xl font-bold text-white">LitXusTravel</h1>
          <p className="text-white/50 text-sm mt-1">Admin Portal</p>
        </div>

        {/* Form */}
        <div className="bg-card border border-border rounded-2xl p-6 shadow-xl">
          <h2 className="text-lg font-semibold text-foreground mb-5">Sign In</h2>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Email</label>
              <input
                {...register("email")}
                type="email"
                placeholder="superadmin@litxustravel.com"
                autoComplete="email"
                className="w-full px-3 py-2.5 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
              />
              {errors.email && <p className="text-xs text-destructive mt-1">{errors.email.message}</p>}
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground mb-1.5">Password</label>
              <input
                {...register("password")}
                type="password"
                placeholder="••••••••"
                autoComplete="current-password"
                className="w-full px-3 py-2.5 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
              />
              {errors.password && <p className="text-xs text-destructive mt-1">{errors.password.message}</p>}
            </div>

            <Button
              type="submit"
              disabled={loading}
              className="w-full bg-[--color-brand-blue] hover:bg-blue-700 text-white font-semibold py-2.5"
            >
              {loading ? (
                <><Loader2 size={16} className="animate-spin mr-2" />Signing in...</>
              ) : (
                "Sign In"
              )}
            </Button>

            {serverDown && (
              <div className="flex items-center gap-2 mt-3 px-3 py-2.5 rounded-lg bg-destructive/10 border border-destructive/30 text-destructive text-sm">
                <WifiOff size={15} className="shrink-0" />
                <span>Server unavailable. Start the API and try again.</span>
              </div>
            )}
          </form>

          <div className="text-xs text-muted-foreground text-center mt-6 space-y-1 border-t border-border pt-4">
            <p className="font-semibold text-foreground mb-2">Dev Credentials:</p>
            <p><span className="text-[--color-brand-blue]">SuperAdmin:</span> superadmin@litxustravel.com / SuperAdmin@123</p>
            <p><span className="text-[--color-brand-blue]">Admin:</span> admin@litxustravel.com / Admin@123</p>
            <p className="text-muted-foreground text-[10px] mt-3 space-y-1">
              <span className="block"><span className="text-[--color-brand-blue]">Tenant Admins:</span></span>
              <span className="block">admin@travelpro.com / TravelPro@123</span>
              <span className="block">admin@wanderlust.com / Wanderlust@123</span>
              <span className="block">admin@adventureseek.com / Adventure@123</span>
            </p>
            <p className="text-muted-foreground text-[10px] mt-3 space-y-1">
              <span className="block"><span className="text-[--color-brand-blue]">Tenant URLs (lvh.me):</span></span>
              <span className="block"><a href="http://travelpro.lvh.me:3001" target="_blank" rel="noreferrer" className="underline hover:text-[--color-brand-blue]">travelpro.lvh.me:3001</a> — Travel Pro</span>
              <span className="block"><a href="http://wanderlust.lvh.me:3001" target="_blank" rel="noreferrer" className="underline hover:text-[--color-brand-blue]">wanderlust.lvh.me:3001</a> — Wanderlust Tours</span>
              <span className="block"><a href="http://adventure.lvh.me:3002" target="_blank" rel="noreferrer" className="underline hover:text-[--color-brand-blue]">adventure.lvh.me:3002</a> — Adventure Seekers</span>
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}
