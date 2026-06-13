import type { NextConfig } from "next"

const nextConfig: NextConfig = {
  allowedDevOrigins: ["travelpro.lvh.me", "wanderlust.lvh.me", "adventure.lvh.me"],
  turbopack: {
    root: __dirname,
  },
  images: {
    remotePatterns: [
      { protocol: "https", hostname: "images.unsplash.com" },
      { protocol: "https", hostname: "plus.unsplash.com" },
    ],
  },
}

export default nextConfig
