import { useState, useEffect } from "react"
import { publicApi } from "@/lib/api"

export function useTenantName(fallback = "LitXusTravel") {
  const [tenantName, setTenantName] = useState(fallback)

  useEffect(() => {
    publicApi.getWebsite()
      .then(res => { if (res.data?.tenantName) setTenantName(res.data.tenantName) })
      .catch(() => {})
  }, [])

  return tenantName
}
