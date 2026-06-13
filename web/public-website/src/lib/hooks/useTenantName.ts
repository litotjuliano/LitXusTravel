import { useState, useEffect } from "react"
import { publicApi } from "@/lib/api"

export function useTenantName(fallback = "LitXusTravel") {
  const [tenantName, setTenantName] = useState(fallback)

  useEffect(() => {
    const subdomain = window.location.hostname.split(".")[0]
    console.log("[useTenantName] subdomain:", subdomain)
    publicApi.getWebsite()
      .then(res => {
        console.log("[useTenantName] response:", res.data)
        if (res.data?.tenantName) setTenantName(res.data.tenantName)
      })
      .catch(err => {
        console.error("[useTenantName] error:", err)
      })
  }, [])

  return tenantName
}
