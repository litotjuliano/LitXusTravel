"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { Loader2, Lock } from "lucide-react"
import { Button } from "@/components/ui/button"
import { tenantApi } from "@/lib/api"
import { toast } from "sonner"

const schema = z.object({
  email:    z.string().email("Enter a valid email"),
  password: z.string().min(6, "Password must be at least 6 characters"),
})
type FormValues = z.infer<typeof schema>

export default function LoginPage() {
  const router = useRouter()
  const [loading, setLoading] = useState(false)

  const { register, handleSubmit, formState: { errors } } = useForm<FormValues>({ resolver: zodResolver(schema) })

  const onSubmit = async (data: FormValues) => {
    setLoading(true)
    try {
      const res = await tenantApi.login(data.email, data.password)
      localStorage.setItem("nexus_token", res.data.accessToken)
      localStorage.setItem("nexus_tenant_id", res.data.tenantId)
      localStorage.setItem("nexus_user_email", res.data.email)
      router.push("/")
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : "Login failed")
    } finally {
      setLoading(false)
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
          <p className="text-white/50 text-sm mt-1">Tenant Portal</p>
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
                placeholder="agent@youragency.com"
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
                className="w-full px-3 py-2.5 text-sm border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
              />
              {errors.password && <p className="text-xs text-destructive mt-1">{errors.password.message}</p>}
            </div>

            <Button
              type="submit"
              disabled={loading}
              className="w-full bg-[--color-brand-blue] hover:bg-blue-700 text-white font-semibold py-2.5"
            >
              {loading ? <><Loader2 size={16} className="animate-spin mr-2" />Signing in...</> : "Sign In"}
            </Button>
          </form>

          <p className="text-xs text-muted-foreground text-center mt-4">
            Test credentials available from your administrator
          </p>
        </div>
      </div>
    </div>
  )
}
