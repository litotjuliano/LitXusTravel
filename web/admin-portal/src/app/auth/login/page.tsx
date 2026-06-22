"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { Loader2, Lock, WifiOff, KeyRound } from "lucide-react"
import { Button } from "@/components/ui/button"
import { adminApi } from "@/lib/api"
import { toast } from "sonner"

const schema = z.object({
  email:    z.string().email("Enter a valid email"),
  password: z.string().min(6, "Password must be at least 6 characters"),
})
type FormValues = z.infer<typeof schema>

const DEV_CREDENTIALS = [
  { email: "superadmin@litxustravel.com", password: "SuperAdmin@123", role: "Super Admin",         color: "text-red-400   border-red-500/30   bg-red-500/10"   },
  { email: "admin@litxustravel.com",       password: "Admin@123",      role: "Platform Admin",      color: "text-orange-400 border-orange-500/30 bg-orange-500/10" },
  { email: "admin@travelpro.com",          password: "TravelPro@123",  role: "Tenant Admin — TravelPro",   color: "text-blue-400  border-blue-500/30  bg-blue-500/10"  },
  { email: "admin@wanderlust.com",         password: "Wanderlust@123", role: "Tenant Admin — Wanderlust",  color: "text-blue-400  border-blue-500/30  bg-blue-500/10"  },
  { email: "admin@adventureseek.com",      password: "Adventure@123",  role: "Tenant Admin — AdventureSeek", color: "text-blue-400 border-blue-500/30 bg-blue-500/10" },
]

export default function LoginPage() {
  const router = useRouter()
  const [loading, setLoading] = useState(false)
  const [serverDown, setServerDown] = useState(false)
  const [filledEmail, setFilledEmail] = useState<string | null>(null)

  const { register, handleSubmit, setValue, formState: { errors } } = useForm<FormValues>({
    resolver: zodResolver(schema),
  })

  const fillCredentials = (email: string, password: string) => {
    setValue("email", email, { shouldValidate: true })
    setValue("password", password, { shouldValidate: true })
    setFilledEmail(email)
  }

  const onSubmit = async (data: FormValues) => {
    setLoading(true)
    setServerDown(false)
    try {
      const res = await adminApi.login(data.email, data.password)
      localStorage.setItem("litxus_token", res.data.accessToken)
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
    <div className="min-h-screen bg-gray-dark flex items-center justify-center px-4 py-10">
      <div className="w-full max-w-sm">

        {/* Logo */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-14 h-14 rounded-2xl bg-brand-500 mb-4">
            <Lock size={24} className="text-white" />
          </div>
          <h1 className="text-2xl font-bold text-white">LitXusTravel</h1>
          <p className="text-white/50 text-sm mt-1">Admin Portal</p>
        </div>

        {/* Login card */}
        <div className="bg-gray-800 border border-gray-700 rounded-2xl p-6 shadow-xl">
          <h2 className="text-lg font-semibold text-white mb-5">Sign In</h2>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-300 mb-1.5">Email</label>
              <input
                {...register("email")}
                type="email"
                placeholder="superadmin@litxustravel.com"
                autoComplete="email"
                className="w-full px-3 py-2.5 text-sm border border-gray-600 rounded-lg bg-gray-900 text-white placeholder:text-gray-500 focus:outline-none focus:ring-2 focus:ring-brand-500/50 transition-colors"
              />
              {errors.email && <p className="text-xs text-red-400 mt-1">{errors.email.message}</p>}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-300 mb-1.5">Password</label>
              <input
                {...register("password")}
                type="password"
                placeholder="••••••••"
                autoComplete="current-password"
                className="w-full px-3 py-2.5 text-sm border border-gray-600 rounded-lg bg-gray-900 text-white placeholder:text-gray-500 focus:outline-none focus:ring-2 focus:ring-brand-500/50 transition-colors"
              />
              {errors.password && <p className="text-xs text-red-400 mt-1">{errors.password.message}</p>}
            </div>

            <Button
              type="submit"
              disabled={loading}
              className="w-full bg-brand-500 hover:bg-brand-600 text-white font-semibold py-2.5 rounded-lg transition-colors"
            >
              {loading ? (
                <><Loader2 size={16} className="animate-spin mr-2" />Signing in...</>
              ) : (
                "Sign In"
              )}
            </Button>

            {serverDown && (
              <div className="flex items-center gap-2 px-3 py-2.5 rounded-lg bg-red-500/10 border border-red-500/30 text-red-400 text-sm">
                <WifiOff size={15} className="shrink-0" />
                <span>Server unavailable. Start the API and try again.</span>
              </div>
            )}
          </form>
        </div>

        {/* Dev credentials cheatsheet */}
        <div className="mt-3 bg-gray-800/80 border border-gray-700 rounded-2xl overflow-hidden shadow-lg">
          {/* Header */}
          <div className="flex items-center gap-2 px-4 py-2.5 border-b border-gray-700 bg-gray-800">
            <KeyRound size={13} className="text-yellow-400 shrink-0" />
            <span className="text-xs font-semibold text-gray-400">Dev Credentials</span>
            <span className="ml-auto text-[10px] font-medium text-gray-500 bg-gray-700 border border-gray-600 px-2 py-0.5 rounded">
              click row to fill
            </span>
          </div>

          {/* Table */}
          <table className="w-full text-xs">
            <thead>
              <tr className="border-b border-gray-700">
                <th className="text-left px-4 py-2 font-semibold text-gray-400">Email</th>
                <th className="text-left px-3 py-2 font-semibold text-gray-400">Role</th>
              </tr>
            </thead>
            <tbody>
              {DEV_CREDENTIALS.map(({ email, password, role, color }) => {
                const isActive = filledEmail === email
                return (
                  <tr
                    key={email}
                    onClick={() => fillCredentials(email, password)}
                    title={`Fill: ${email} / ${password}`}
                    className={`border-b border-gray-700/50 cursor-pointer transition-colors ${
                      isActive
                        ? "bg-brand-500/10"
                        : "hover:bg-white/[0.04]"
                    }`}
                  >
                    <td className="px-4 py-2.5 font-mono text-gray-300 truncate max-w-[180px]">
                      {email}
                    </td>
                    <td className="px-3 py-2.5">
                      <span className={`inline-block px-2 py-0.5 rounded text-[10px] font-semibold border ${color}`}>
                        {role}
                      </span>
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>

          {/* Password hint */}
          <div className="px-4 py-2 bg-gray-800/60 border-t border-gray-700">
            <p className="text-[11px] text-gray-500">
              Password shown in tooltip · Row highlights when filled
            </p>
          </div>
        </div>

      </div>
    </div>
  )
}
