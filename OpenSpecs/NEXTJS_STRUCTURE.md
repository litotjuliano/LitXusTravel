# LitXusTravel Next.js Frontend Structure
## Implementation Guide for Claude Code

**Projects:** Public Website, Admin Portal, Tenant Dashboard  
**Framework:** Next.js 14+ with App Router  
**Styling:** Tailwind CSS + shadcn/ui  
**Animation:** Framer Motion  
**State:** React Context + SWR/React Query

---

## Project Organization

```
web/
├── shared-ui/                      (Shared across all apps)
│   ├── components/
│   │   ├── ui/                     (shadcn/ui components)
│   │   │   ├── button.tsx
│   │   │   ├── input.tsx
│   │   │   ├── dialog.tsx
│   │   │   ├── select.tsx
│   │   │   ├── table.tsx
│   │   │   └── ...more
│   │   ├── common/                 (Cross-app components)
│   │   │   ├── Navigation.tsx
│   │   │   ├── Footer.tsx
│   │   │   ├── ThemeToggle.tsx
│   │   │   └── LoadingSpinner.tsx
│   │   └── patterns/               (Common patterns)
│   │       ├── HeroBanner.tsx
│   │       ├── StatCard.tsx
│   │       ├── FormSection.tsx
│   │       └── ActionMenu.tsx
│   ├── lib/
│   │   ├── animations.ts           (Framer Motion presets)
│   │   ├── colors.ts               (Design tokens)
│   │   ├── cn.ts                   (classname helper)
│   │   ├── api-client.ts           (API configuration)
│   │   └── constants.ts
│   ├── hooks/
│   │   ├── useCurrentTenant.ts
│   │   ├── useLocalStorage.ts
│   │   ├── useFetch.ts
│   │   └── useResponsive.ts
│   ├── contexts/
│   │   ├── ThemeContext.tsx        (Dark mode)
│   │   └── TenantContext.tsx       (Tenant info)
│   ├── types/
│   │   ├── api.ts                  (.NET API types)
│   │   ├── domain.ts               (Domain models)
│   │   └── ui.ts                   (UI-specific types)
│   ├── styles/
│   │   └── globals.css
│   └── package.json
│
├── public-website/                 (Agent's customer website)
│   ├── src/
│   │   ├── app/
│   │   │   ├── layout.tsx          (Root layout)
│   │   │   ├── page.tsx            (Home /)
│   │   │   ├── packages/
│   │   │   │   ├── page.tsx        (/packages - listing)
│   │   │   │   └── [id]/
│   │   │   │       └── page.tsx    (/packages/[id] - detail)
│   │   │   ├── blog/
│   │   │   │   ├── page.tsx
│   │   │   │   └── [slug]/page.tsx
│   │   │   ├── about/page.tsx
│   │   │   ├── contact/page.tsx
│   │   │   ├── api/
│   │   │   │   └── inquiries/route.ts    (Route handler for API calls)
│   │   │   ├── error.tsx           (Error boundary)
│   │   │   └── not-found.tsx
│   │   ├── components/
│   │   │   ├── sections/
│   │   │   │   ├── HeroSection.tsx
│   │   │   │   ├── FeaturedPackages.tsx
│   │   │   │   ├── PopularDestinations.tsx
│   │   │   │   ├── TestimonialsSection.tsx
│   │   │   │   ├── CTASection.tsx
│   │   │   │   └── InquiryForm.tsx
│   │   │   ├── PackageCard.tsx     (Reusable)
│   │   │   ├── PackageGrid.tsx
│   │   │   ├── SearchFilters.tsx
│   │   │   └── PackageGallery.tsx
│   │   ├── lib/
│   │   │   └── public-api.ts       (Public API client)
│   │   ├── hooks/
│   │   │   ├── usePackages.ts
│   │   │   └── useInquiries.ts
│   │   └── types/
│   │       └── public.ts
│   ├── public/
│   │   ├── images/
│   │   ├── icons/
│   │   └── favicon.ico
│   ├── package.json
│   ├── tailwind.config.ts
│   ├── postcss.config.js
│   └── next.config.js
│
├── admin-portal/                   (Super admin dashboard)
│   ├── src/
│   │   ├── app/
│   │   │   ├── (dashboard)/
│   │   │   │   ├── layout.tsx      (Admin layout with sidebar)
│   │   │   │   ├── page.tsx        (Dashboard)
│   │   │   │   ├── packages/
│   │   │   │   │   ├── page.tsx    (Package list)
│   │   │   │   │   └── [id]/page.tsx (Editor)
│   │   │   │   ├── tenants/
│   │   │   │   │   ├── page.tsx
│   │   │   │   │   └── [id]/page.tsx
│   │   │   │   ├── subscriptions/page.tsx
│   │   │   │   ├── analytics/page.tsx
│   │   │   │   ├── billing/page.tsx
│   │   │   │   └── settings/page.tsx
│   │   │   ├── auth/
│   │   │   │   ├── login/page.tsx
│   │   │   │   └── forgot-password/page.tsx
│   │   │   └── error.tsx
│   │   ├── components/
│   │   │   ├── layout/
│   │   │   │   ├── Sidebar.tsx
│   │   │   │   ├── TopBar.tsx
│   │   │   │   ├── Navigation.tsx
│   │   │   │   └── UserMenu.tsx
│   │   │   ├── dashboard/
│   │   │   │   ├── StatCard.tsx
│   │   │   │   ├── RevenueChart.tsx
│   │   │   │   ├── RecentActivity.tsx
│   │   │   │   └── QuickActions.tsx
│   │   │   ├── packages/
│   │   │   │   ├── PackageTable.tsx
│   │   │   │   ├── PackageForm.tsx
│   │   │   │   ├── ImageUploader.tsx
│   │   │   │   └── ItineraryBuilder.tsx
│   │   │   ├── tenants/
│   │   │   │   ├── TenantTable.tsx
│   │   │   │   ├── TenantForm.tsx
│   │   │   │   └── InviteDialog.tsx
│   │   │   └── common/
│   │   │       ├── FormSection.tsx
│   │   │       ├── DataTable.tsx
│   │   │       ├── ActionMenu.tsx
│   │   │       └── StatusBadge.tsx
│   │   ├── lib/
│   │   │   ├── admin-api.ts        (Admin API client)
│   │   │   ├── auth.ts             (Auth utils)
│   │   │   └── permissions.ts      (Role-based)
│   │   ├── hooks/
│   │   │   ├── useAdmin.ts
│   │   │   ├── useTenants.ts
│   │   │   ├── usePackages.ts
│   │   │   ├── useAuth.ts
│   │   │   └── useDarkMode.ts
│   │   ├── context/
│   │   │   └── AdminAuthContext.tsx
│   │   └── types/
│   │       └── admin.ts
│   ├── public/
│   ├── package.json
│   ├── tailwind.config.ts
│   ├── postcss.config.js
│   └── next.config.js
│
└── tenant-dashboard/               (Agent/tenant dashboard)
    ├── src/
    │   ├── app/
    │   │   ├── (dashboard)/
    │   │   │   ├── layout.tsx
    │   │   │   ├── page.tsx        (Dashboard overview)
    │   │   │   ├── packages/
    │   │   │   │   ├── page.tsx    (My packages)
    │   │   │   │   └── [id]/page.tsx
    │   │   │   ├── inquiries/
    │   │   │   │   ├── page.tsx
    │   │   │   │   └── [id]/page.tsx
    │   │   │   ├── quotations/
    │   │   │   │   ├── page.tsx
    │   │   │   │   └── [id]/page.tsx
    │   │   │   ├── settings/
    │   │   │   │   ├── page.tsx    (Branding, etc.)
    │   │   │   │   └── website/page.tsx
    │   │   │   ├── reports/page.tsx
    │   │   │   └── payments/page.tsx
    │   │   ├── auth/
    │   │   │   ├── login/page.tsx
    │   │   │   └── register/page.tsx
    │   │   └── error.tsx
    │   ├── components/
    │   │   ├── dashboard/
    │   │   │   ├── QuickStats.tsx
    │   │   │   ├── RecentInquiries.tsx
    │   │   │   ├── PackageSyncStatus.tsx
    │   │   │   └── UpcomingBookings.tsx
    │   │   ├── packages/
    │   │   │   ├── MyPackagesList.tsx
    │   │   │   ├── SyncPackageModal.tsx
    │   │   │   ├── PackageCustomizer.tsx  (Override settings)
    │   │   │   └── MarkupCalculator.tsx
    │   │   ├── inquiries/
    │   │   │   ├── InquiryList.tsx
    │   │   │   ├── InquiryDetail.tsx
    │   │   │   ├── QuotationDraft.tsx
    │   │   │   └── WhatsAppChat.tsx
    │   │   └── common/
    │   │       └── (same as admin)
    │   ├── lib/
    │   │   ├── tenant-api.ts
    │   │   ├── auth.ts
    │   │   └── whatsapp.ts        (WhatsApp integration)
    │   ├── hooks/
    │   │   ├── useTenant.ts
    │   │   ├── useMyPackages.ts
    │   │   ├── useInquiries.ts
    │   │   └── useQuotations.ts
    │   └── types/
    │       └── tenant.ts
    ├── public/
    ├── package.json
    ├── tailwind.config.ts
    ├── postcss.config.js
    └── next.config.js
```

---

## Key Implementation Files

### Shared UI Components (shadcn/ui)

```bash
# Install shadcn/ui components in each app:
npx shadcn-ui@latest add button
npx shadcn-ui@latest add input
npx shadcn-ui@latest add select
npx shadcn-ui@latest add dialog
npx shadcn-ui@latest add table
npx shadcn-ui@latest add dropdown-menu
npx shadcn-ui@latest add card
npx shadcn-ui@latest add alert
npx shadcn-ui@latest add toast
```

### Tailwind Configuration

```typescript
// tailwind.config.ts (shared template)
import type { Config } from "tailwindcss"

const config = {
  darkMode: ["class"],
  content: [
    "./src/pages/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/components/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/app/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#F0F7FF',
          600: '#0066CC',
          // ... full palette
        },
        secondary: {
          500: '#00A89A',
          // ...
        },
        accent: {
          500: '#FF6B35',
          // ...
        },
      },
      fontFamily: {
        inter: ['var(--font-inter)'],
      },
    },
  },
  plugins: [require("tailwindcss-animate")],
} satisfies Config

export default config
```

### API Client Configuration

```typescript
// lib/api-client.ts
import axios, { AxiosInstance } from 'axios'

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api'

export const createApiClient = (accessToken?: string): AxiosInstance => {
  const client = axios.create({
    baseURL: API_BASE_URL,
    headers: {
      'Content-Type': 'application/json',
      ...(accessToken && { 'Authorization': `Bearer ${accessToken}` }),
    },
  })

  // Error interceptor
  client.interceptors.response.use(
    (response) => response,
    (error) => {
      if (error.response?.status === 401) {
        // Handle unauthorized - redirect to login
        window.location.href = '/auth/login'
      }
      return Promise.reject(error)
    }
  )

  return client
}

// Usage in components
const publicApi = createApiClient()
const adminApi = createApiClient(adminToken)
```

### Environment Variables

```
# .env.local (each app)
NEXT_PUBLIC_API_URL=http://localhost:5000/api
NEXT_PUBLIC_TENANT_SUBDOMAIN=localhost
NEXT_PUBLIC_APP_NAME=LitXusTravel Public

# Admin portal
NEXT_PUBLIC_API_URL=http://localhost:5000/api
NEXT_SECRET_ADMIN_KEY=xxx
```

### Package.json Template

```json
{
  "name": "public-website",
  "version": "1.0.0",
  "private": true,
  "scripts": {
    "dev": "next dev",
    "build": "next build",
    "start": "next start",
    "lint": "next lint",
    "type-check": "tsc --noEmit"
  },
  "dependencies": {
    "next": "^14.0.0",
    "react": "^18.3.0",
    "react-dom": "^18.3.0",
    "framer-motion": "^10.16.0",
    "tailwindcss": "^3.4.0",
    "shadcn-ui": "^0.0.4",
    "@radix-ui/react-dialog": "^1.1.1",
    "@radix-ui/react-dropdown-menu": "^2.0.5",
    "@radix-ui/react-popover": "^1.0.6",
    "axios": "^1.6.0",
    "next-themes": "^0.2.1",
    "lucide-react": "^0.305.0",
    "recharts": "^2.10.0",
    "zod": "^3.22.0"
  },
  "devDependencies": {
    "@types/node": "^20.0.0",
    "@types/react": "^18.3.0",
    "@types/react-dom": "^18.3.0",
    "typescript": "^5.3.0",
    "postcss": "^8.4.31",
    "autoprefixer": "^10.4.16",
    "tailwindcss-animate": "^1.0.6",
    "eslint": "^8.54.0",
    "eslint-config-next": "^14.0.0"
  }
}
```

---

## Key Patterns for Claude Code

### Data Fetching with SWR

```typescript
// hooks/usePackages.ts
import useSWR from 'swr'
import { publicApi } from '@/lib/api-client'

export function usePackages(filters?: object) {
  const { data, error, isLoading } = useSWR(
    ['packages', filters],
    async ([_key, filters]) => {
      const response = await publicApi.get('/packages', { params: filters })
      return response.data
    },
    { revalidateOnFocus: false }
  )

  return {
    packages: data?.items || [],
    total: data?.total || 0,
    isLoading,
    error,
  }
}
```

### Form Handling with Zod

```typescript
// components/InquiryForm.tsx
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'

const inquirySchema = z.object({
  name: z.string().min(2, 'Name is required'),
  email: z.string().email('Invalid email'),
  phone: z.string().min(10, 'Valid phone required'),
  message: z.string().min(10, 'Message must be at least 10 characters'),
  travelers: z.number().min(1, 'At least 1 traveler'),
})

type InquiryFormData = z.infer<typeof inquirySchema>

export function InquiryForm() {
  const { register, handleSubmit, formState: { errors } } = 
    useForm<InquiryFormData>({
      resolver: zodResolver(inquirySchema),
    })

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      {/* Form fields */}
    </form>
  )
}
```

### Animation Patterns (Framer Motion)

```typescript
// components/sections/FeaturedPackages.tsx
import { motion } from 'framer-motion'
import { staggerContainer, staggerItem } from '@/lib/animations'

export function FeaturedPackages() {
  return (
    <motion.div
      className="grid grid-cols-1 md:grid-cols-3 gap-24px"
      variants={staggerContainer}
      initial="hidden"
      whileInView="visible"
      viewport={{ once: true, amount: 0.1 }}
    >
      {packages.map((pkg) => (
        <motion.div key={pkg.id} variants={staggerItem}>
          <PackageCard package={pkg} />
        </motion.div>
      ))}
    </motion.div>
  )
}
```

---

## Deployment Instructions

### Vercel Deployment

```bash
# Login to Vercel
npm install -g vercel
vercel login

# Deploy each app
cd web/public-website
vercel

# Set environment variables in Vercel dashboard
# NEXT_PUBLIC_API_URL = https://api.nexustravel.com
```

### Environment-Specific URLs

```
Development:  localhost:3000 → localhost:5000 API
Staging:      staging.nexustravel.com → staging-api.nexustravel.com
Production:   app.nexustravel.com → api.nexustravel.com
              [agent].nexustravel.com → api.nexustravel.com
```

---

## Performance Optimizations

- Image optimization with `next/image`
- Code splitting with dynamic imports
- Font optimization with `next/font`
- Route prefetching
- SWR caching strategies
- Vercel Analytics integration

---

**Next.js Structure Ready for Claude Code Implementation** ✅
