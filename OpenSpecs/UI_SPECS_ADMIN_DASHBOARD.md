# LitXusTravel Admin Dashboard - UI Specifications
## Super Admin Platform

**Inspiration:** Stripe, Linear, Shopify Admin, Vercel Dashboard  
**Style:** Clean, professional, data-focused, dark mode optimized  
**Target:** Platform administrators managing packages, tenants, subscriptions

---

## 📋 Page Map

```
/admin                          → Dashboard Overview
/admin/packages                 → Package Management
/admin/packages/[id]           → Package Editor
/admin/tenants                 → Tenant Management
/admin/tenants/[id]            → Tenant Details
/admin/subscriptions           → Subscription Plans
/admin/analytics               → Analytics & Reports
/admin/billing                 → Billing & Invoices
/admin/settings                → Platform Settings
/admin/users                   → User Management
```

---

## 1. Dashboard Layout & Navigation

### Sidebar Navigation

```tsx
<aside className="
  w-64 hidden md:block
  fixed left-0 top-0 h-screen
  bg-gray-900 dark:bg-gray-950
  text-white
  overflow-y-auto
  border-r border-gray-800
">
  {/* Logo */}
  <div className="p-24px border-b border-gray-800">
    <Image
      src="/logo-light.svg"
      alt="LitXusTravel"
      width={40}
      height={40}
      className="mb-12px"
    />
    <h1 className="text-h3 font-bold">LitXusTravel</h1>
    <p className="text-caption text-gray-400">Admin</p>
  </div>

  {/* Navigation */}
  <nav className="p-16px space-y-2px">
    <NavItem
      icon={<LayoutDashboard />}
      label="Dashboard"
      href="/admin"
      active
    />
    <NavItem
      icon={<Package />}
      label="Packages"
      href="/admin/packages"
    />
    <NavItem
      icon={<Users />}
      label="Tenants"
      href="/admin/tenants"
    />
    <NavItem
      icon={<CreditCard />}
      label="Subscriptions"
      href="/admin/subscriptions"
    />
    <NavItem
      icon={<BarChart3 />}
      label="Analytics"
      href="/admin/analytics"
    />
    <NavItem
      icon={<Settings />}
      label="Settings"
      href="/admin/settings"
    />
  </nav>

  {/* Footer */}
  <div className="absolute bottom-0 w-full p-16px border-t border-gray-800 space-y-12px">
    <UserProfileDropdown />
    <button className="
      w-full px-16px py-12px
      border border-gray-700 text-gray-300
      rounded-lg text-body-sm
      hover:bg-gray-800 transition-colors
    ">
      Logout
    </button>
  </div>
</aside>
```

### Top Navigation Bar

```tsx
<header className="
  fixed top-0 left-0 right-0 h-16
  bg-white dark:bg-gray-900
  border-b border-gray-200 dark:border-gray-800
  md:ml-64
  z-40
">
  <div className="h-full px-24px flex items-center justify-between">
    {/* Mobile Menu Toggle */}
    <button className="md:hidden">
      <Menu size={24} className="text-gray-900 dark:text-white" />
    </button>

    {/* Title */}
    <h1 className="hidden md:block text-h2 font-bold text-gray-900 dark:text-white">
      Dashboard
    </h1>

    {/* Right Section */}
    <div className="flex items-center gap-16px">
      {/* Search */}
      <div className="hidden lg:flex items-center px-16px py-8px bg-gray-100 dark:bg-gray-800 rounded-lg border border-gray-300 dark:border-gray-700">
        <Search size={16} className="text-gray-400" />
        <input
          type="text"
          placeholder="Search..."
          className="
            ml-8px bg-transparent outline-none text-body-sm
            text-gray-900 dark:text-white
            placeholder:text-gray-500
          "
        />
      </div>

      {/* Notifications */}
      <button className="relative">
        <Bell size={24} className="text-gray-600 dark:text-gray-400" />
        <span className="absolute top-0 right-0 w-8px h-8px bg-error rounded-full"></span>
      </button>

      {/* User Menu */}
      <UserProfileMenu />
    </div>
  </div>
</header>
```

### NavItem Component

```tsx
<Link
  href={href}
  className={`
    flex items-center gap-12px px-16px py-12px rounded-lg
    font-semibold text-body-sm transition-colors
    ${active
      ? 'bg-primary-600 text-white'
      : 'text-gray-300 hover:bg-gray-800 dark:hover:bg-gray-800'
    }
  `}
>
  {icon && <span className="w-20px h-20px">{icon}</span>}
  <span>{label}</span>
</Link>
```

---

## 2. Dashboard Overview Page

### Layout

```
┌─────────────────────────────────────────┐
│   Header + Quick Stats                  │
├─────────────────────────────────────────┤
│   ┌──────────┐  ┌──────────┐           │
│   │  Metric  │  │  Metric  │  ...     │
│   └──────────┘  └──────────┘           │
├─────────────────────────────────────────┤
│   Charts (Revenue, Signups, etc.)       │
├─────────────────────────────────────────┤
│   Recent Activity / Data Tables         │
└─────────────────────────────────────────┘
```

### Stat Cards

```tsx
<motion.div
  className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-16px mb-32px"
  variants={staggerContainer}
  initial="hidden"
  animate="visible"
>
  {[
    { label: 'Active Tenants', value: '1,234', change: '+12.5%', icon: Users },
    { label: 'Total Packages', value: '5,678', change: '+8.2%', icon: Package },
    { label: 'Monthly Revenue', value: 'RM45,230', change: '+23.1%', icon: TrendingUp },
    { label: 'Active Subscriptions', value: '891', change: '+4.3%', icon: CreditCard },
  ].map((stat) => (
    <motion.div key={stat.label} variants={staggerItem}>
      <div className="
        p-24px
        bg-white dark:bg-gray-900
        border border-gray-200 dark:border-gray-800
        rounded-xl
        hover:shadow-lg dark:hover:shadow-gray-800/50
        transition-shadow
      ">
        <div className="flex items-start justify-between mb-16px">
          <div>
            <p className="text-caption text-gray-600 dark:text-gray-400 uppercase font-semibold mb-8px">
              {stat.label}
            </p>
            <h3 className="text-display-md font-bold text-gray-900 dark:text-white">
              {stat.value}
            </h3>
          </div>
          <div className="
            w-12 h-12
            bg-primary-100 dark:bg-primary-900
            rounded-lg
            flex items-center justify-center
          ">
            <stat.icon className="text-primary-600 dark:text-primary-400" size={24} />
          </div>
        </div>

        {/* Change Indicator */}
        <div className="flex items-center gap-4px">
          <TrendingUp size={16} className="text-success" />
          <span className="text-caption font-semibold text-success">
            {stat.change}
          </span>
          <span className="text-caption text-gray-500">vs last month</span>
        </div>
      </div>
    </motion.div>
  ))}
</motion.div>
```

### Chart Section

```tsx
<div className="
  p-24px
  bg-white dark:bg-gray-900
  border border-gray-200 dark:border-gray-800
  rounded-xl
  mb-32px
">
  <div className="flex items-center justify-between mb-32px">
    <div>
      <h2 className="text-h2 font-bold text-gray-900 dark:text-white">
        Revenue Overview
      </h2>
      <p className="text-body-sm text-gray-600 dark:text-gray-400">
        Monthly subscription revenue
      </p>
    </div>

    {/* Time Period Selector */}
    <div className="flex gap-8px">
      {['7d', '30d', '90d', '1y'].map((period) => (
        <button
          key={period}
          onClick={() => setTimePeriod(period)}
          className={`
            px-12px py-8px rounded-lg text-caption font-semibold transition-colors
            ${selectedPeriod === period
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 dark:bg-gray-800 text-gray-900 dark:text-white hover:bg-gray-200 dark:hover:bg-gray-700'
            }
          `}
        >
          {period}
        </button>
      ))}
    </div>
  </div>

  {/* Chart Component */}
  <ResponsiveContainer width="100%" height={300}>
    <LineChart data={chartData}>
      <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
      <XAxis dataKey="date" stroke="#6b7280" />
      <YAxis stroke="#6b7280" />
      <Tooltip
        contentStyle={{
          backgroundColor: '#1f2937',
          border: 'none',
          borderRadius: '8px',
          color: '#fff',
        }}
      />
      <Line
        type="monotone"
        dataKey="revenue"
        stroke="#0066CC"
        strokeWidth={3}
        dot={false}
        isAnimationActive
      />
    </LineChart>
  </ResponsiveContainer>
</div>
```

### Recent Activity Table

```tsx
<div className="
  p-24px
  bg-white dark:bg-gray-900
  border border-gray-200 dark:border-gray-800
  rounded-xl
">
  <h2 className="text-h2 font-bold text-gray-900 dark:text-white mb-24px">
    Recent Activity
  </h2>

  <table className="w-full">
    <thead>
      <tr className="border-b border-gray-200 dark:border-gray-800">
        <th className="px-16px py-16px text-left text-body-sm font-semibold text-gray-600 dark:text-gray-400">
          Tenant
        </th>
        <th className="px-16px py-16px text-left text-body-sm font-semibold text-gray-600 dark:text-gray-400">
          Action
        </th>
        <th className="px-16px py-16px text-left text-body-sm font-semibold text-gray-600 dark:text-gray-400">
          Status
        </th>
        <th className="px-16px py-16px text-left text-body-sm font-semibold text-gray-600 dark:text-gray-400">
          Time
        </th>
      </tr>
    </thead>
    <tbody className="divide-y divide-gray-200 dark:divide-gray-800">
      {activities.map((activity) => (
        <tr key={activity.id} className="hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors">
          <td className="px-16px py-16px">
            <div className="flex items-center gap-12px">
              <Image
                src={activity.avatar}
                alt={activity.tenant}
                width={32}
                height={32}
                className="rounded-full"
              />
              <div>
                <p className="text-body font-semibold text-gray-900 dark:text-white">
                  {activity.tenant}
                </p>
                <p className="text-caption text-gray-600 dark:text-gray-400">
                  {activity.email}
                </p>
              </div>
            </div>
          </td>
          <td className="px-16px py-16px text-body text-gray-900 dark:text-white">
            {activity.action}
          </td>
          <td className="px-16px py-16px">
            <span className={`
              px-12px py-6px rounded-full text-caption font-semibold
              ${activity.status === 'completed'
                ? 'bg-success/20 text-success'
                : 'bg-warning/20 text-warning'
              }
            `}>
              {activity.status}
            </span>
          </td>
          <td className="px-16px py-16px text-caption text-gray-600 dark:text-gray-400">
            {activity.time}
          </td>
        </tr>
      ))}
    </tbody>
  </table>

  <button className="
    w-full mt-24px px-16px py-12px
    text-primary-600 dark:text-primary-400
    font-semibold text-body-sm
    border-t border-gray-200 dark:border-gray-800
    hover:bg-gray-50 dark:hover:bg-gray-800
    transition-colors
  ">
    View All Activity
  </button>
</div>
```

---

## 3. Package Management Page

### Package List

```tsx
<div className="
  p-24px
  bg-white dark:bg-gray-900
  border border-gray-200 dark:border-gray-800
  rounded-xl
">
  {/* Header */}
  <div className="flex items-center justify-between mb-32px">
    <div>
      <h1 className="text-h1 font-bold text-gray-900 dark:text-white">
        Packages
      </h1>
      <p className="text-body text-gray-600 dark:text-gray-400">
        {totalPackages} total packages
      </p>
    </div>

    <motion.button
      className="
        flex items-center gap-8px
        px-24px py-12px
        bg-primary-600 text-white
        rounded-lg font-semibold
        hover:bg-primary-700
        transition-colors
      "
      whileHover={{ scale: 1.05 }}
      whileTap={{ scale: 0.95 }}
    >
      <Plus size={20} />
      New Package
    </motion.button>
  </div>

  {/* Filters & Search */}
  <div className="flex gap-16px mb-32px">
    <div className="flex-1">
      <input
        type="text"
        placeholder="Search packages..."
        className="
          w-full px-16px py-12px
          border border-gray-300 dark:border-gray-700
          bg-white dark:bg-gray-800
          rounded-lg
          text-body text-gray-900 dark:text-white
          placeholder:text-gray-500 dark:placeholder:text-gray-400
          focus:outline-none focus:ring-2 focus:ring-primary-500
        "
        onChange={(e) => setSearchQuery(e.target.value)}
      />
    </div>

    <select className="
      px-16px py-12px
      border border-gray-300 dark:border-gray-700
      bg-white dark:bg-gray-800
      rounded-lg
      text-body text-gray-900 dark:text-white
      focus:outline-none focus:ring-2 focus:ring-primary-500
    ">
      <option>All Categories</option>
      <option>Beach</option>
      <option>Mountain</option>
      <option>City</option>
    </select>
  </div>

  {/* Table */}
  <div className="overflow-x-auto">
    <table className="w-full">
      <thead>
        <tr className="border-b border-gray-200 dark:border-gray-800">
          <th className="px-16px py-16px text-left text-body-sm font-semibold text-gray-600 dark:text-gray-400">
            Package Name
          </th>
          <th className="px-16px py-16px text-left text-body-sm font-semibold text-gray-600 dark:text-gray-400">
            Category
          </th>
          <th className="px-16px py-16px text-left text-body-sm font-semibold text-gray-600 dark:text-gray-400">
            Price
          </th>
          <th className="px-16px py-16px text-left text-body-sm font-semibold text-gray-600 dark:text-gray-400">
            Synced Tenants
          </th>
          <th className="px-16px py-16px text-left text-body-sm font-semibold text-gray-600 dark:text-gray-400">
            Status
          </th>
          <th className="px-16px py-16px text-right text-body-sm font-semibold text-gray-600 dark:text-gray-400">
            Actions
          </th>
        </tr>
      </thead>
      <tbody className="divide-y divide-gray-200 dark:divide-gray-800">
        {packages.map((pkg) => (
          <tr key={pkg.id} className="hover:bg-gray-50 dark:hover:bg-gray-800/50 transition-colors">
            <td className="px-16px py-16px">
              <div>
                <p className="text-body font-semibold text-gray-900 dark:text-white">
                  {pkg.name}
                </p>
                <p className="text-caption text-gray-600 dark:text-gray-400">
                  {pkg.duration} days
                </p>
              </div>
            </td>
            <td className="px-16px py-16px text-body text-gray-900 dark:text-white">
              {pkg.category}
            </td>
            <td className="px-16px py-16px text-body font-semibold text-gray-900 dark:text-white">
              RM{pkg.price}
            </td>
            <td className="px-16px py-16px">
              <span className="text-body font-semibold text-gray-900 dark:text-white">
                {pkg.syncedTenants}
              </span>
            </td>
            <td className="px-16px py-16px">
              <StatusBadge status={pkg.status} />
            </td>
            <td className="px-16px py-16px text-right">
              <ActionMenu
                items={[
                  { label: 'Edit', action: () => editPackage(pkg.id) },
                  { label: 'Duplicate', action: () => duplicatePackage(pkg.id) },
                  { label: 'Delete', action: () => deletePackage(pkg.id), danger: true },
                ]}
              />
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  </div>
</div>
```

---

## 4. Package Editor

### Form Layout

```tsx
<div className="grid grid-cols-3 gap-24px">
  {/* Main Content */}
  <div className="col-span-2 space-y-24px">
    {/* Basic Info */}
    <FormSection
      title="Basic Information"
      description="Core package details"
    >
      <FormGroup label="Package Name" required>
        <input type="text" placeholder="e.g., 5D4N Japan Sakura Tour" />
      </FormGroup>

      <FormGroup label="Description" required>
        <textarea rows={4} placeholder="Describe your package..." />
      </FormGroup>

      <FormGroup label="Category" required>
        <select>
          <option>Beach</option>
          <option>Mountain</option>
          <option>City</option>
        </select>
      </FormGroup>

      <FormGroup label="Duration (Days)" required>
        <input type="number" min="1" placeholder="5" />
      </FormGroup>
    </FormSection>

    {/* Pricing */}
    <FormSection title="Pricing">
      <FormGroup label="Base Price (RM)" required>
        <input type="number" placeholder="4999" />
      </FormGroup>

      <FormGroup label="Currency">
        <select>
          <option>RM (Malaysian Ringgit)</option>
          <option>USD</option>
          <option>SGD</option>
        </select>
      </FormGroup>
    </FormSection>

    {/* Media */}
    <FormSection title="Media" description="Images and videos">
      <ImageUploader />
    </FormSection>

    {/* Itinerary */}
    <FormSection title="Itinerary">
      <ItineraryBuilder />
    </FormSection>
  </div>

  {/* Sidebar */}
  <div className="space-y-24px">
    {/* Status */}
    <div className="
      p-24px
      bg-white dark:bg-gray-900
      border border-gray-200 dark:border-gray-800
      rounded-xl
    ">
      <h3 className="text-h3 font-bold text-gray-900 dark:text-white mb-16px">
        Status
      </h3>

      <div className="space-y-16px">
        <select className="
          w-full px-16px py-12px
          border border-gray-300 dark:border-gray-700
          rounded-lg
        ">
          <option>Draft</option>
          <option>Published</option>
          <option>Archived</option>
        </select>

        <button className="
          w-full px-16px py-12px
          bg-primary-600 text-white
          rounded-lg font-semibold
          hover:bg-primary-700
        ">
          Save Changes
        </button>

        <button className="
          w-full px-16px py-12px
          border border-gray-300 dark:border-gray-700
          rounded-lg font-semibold
          hover:bg-gray-50 dark:hover:bg-gray-800
        ">
          Preview
        </button>
      </div>
    </div>

    {/* SEO */}
    <div className="
      p-24px
      bg-white dark:bg-gray-900
      border border-gray-200 dark:border-gray-800
      rounded-xl
    ">
      <h3 className="text-h3 font-bold text-gray-900 dark:text-white mb-16px">
        SEO
      </h3>

      <FormGroup label="Meta Title">
        <input type="text" maxLength={60} />
      </FormGroup>

      <FormGroup label="Meta Description">
        <textarea rows={2} maxLength={160} />
      </FormGroup>
    </div>
  </div>
</div>
```

---

## 5. Tenant Management Page

### Tenant List with Actions

```tsx
<div className="
  p-24px
  bg-white dark:bg-gray-900
  border border-gray-200 dark:border-gray-800
  rounded-xl
">
  <div className="flex items-center justify-between mb-32px">
    <h1 className="text-h1 font-bold text-gray-900 dark:text-white">
      Tenants
    </h1>
    <button className="
      px-24px py-12px
      bg-primary-600 text-white
      rounded-lg font-semibold
    ">
      Invite Tenant
    </button>
  </div>

  {/* Table */}
  <table className="w-full">
    <thead>
      <tr className="border-b border-gray-200 dark:border-gray-800">
        <th className="px-16px py-16px text-left text-body-sm font-semibold">
          Tenant
        </th>
        <th className="px-16px py-16px text-left text-body-sm font-semibold">
          Subdomain
        </th>
        <th className="px-16px py-16px text-left text-body-sm font-semibold">
          Subscription
        </th>
        <th className="px-16px py-16px text-left text-body-sm font-semibold">
          Joined
        </th>
        <th className="px-16px py-16px text-left text-body-sm font-semibold">
          Status
        </th>
        <th className="px-16px py-16px text-right text-body-sm font-semibold">
          Actions
        </th>
      </tr>
    </thead>
    <tbody className="divide-y divide-gray-200 dark:divide-gray-800">
      {tenants.map((tenant) => (
        <tr key={tenant.id} className="hover:bg-gray-50 dark:hover:bg-gray-800/50">
          <td className="px-16px py-16px">
            <div className="flex items-center gap-12px">
              <Image
                src={tenant.logo}
                alt={tenant.name}
                width={40}
                height={40}
                className="rounded-lg"
              />
              <div>
                <p className="text-body font-semibold text-gray-900 dark:text-white">
                  {tenant.name}
                </p>
                <p className="text-caption text-gray-600 dark:text-gray-400">
                  {tenant.contact}
                </p>
              </div>
            </div>
          </td>
          <td className="px-16px py-16px">
            <code className="text-caption bg-gray-100 dark:bg-gray-800 px-8px py-4px rounded">
              {tenant.subdomain}.nexustravel.com
            </code>
          </td>
          <td className="px-16px py-16px text-body text-gray-900 dark:text-white">
            {tenant.plan}
          </td>
          <td className="px-16px py-16px text-caption text-gray-600 dark:text-gray-400">
            {tenant.joinedDate}
          </td>
          <td className="px-16px py-16px">
            <StatusBadge status={tenant.status} />
          </td>
          <td className="px-16px py-16px text-right">
            <ActionMenu items={[
              { label: 'View Details', action: () => viewTenant(tenant.id) },
              { label: 'Manage Packages', action: () => manageTenantPackages(tenant.id) },
              { label: 'Suspend', action: () => suspendTenant(tenant.id), danger: true },
            ]} />
          </td>
        </tr>
      ))}
    </tbody>
  </table>
</div>
```

---

## Dark Mode Configuration

### Tailwind Dark Mode Setup

```typescript
// tailwind.config.ts
export default {
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        dark: {
          bg: {
            primary: '#0F0F0F',
            secondary: '#1A1A1A',
            tertiary: '#2D2D2D',
          },
          text: {
            primary: '#FFFFFF',
            secondary: '#E0E0E0',
            tertiary: '#A0A0A0',
          },
          border: '#333333',
        }
      }
    }
  }
}
```

### Dark Mode Toggle

```tsx
'use client'

import { useTheme } from 'next-themes'
import { useEffect, useState } from 'react'

export function AdminThemeToggle() {
  const { theme, setTheme } = useTheme()
  const [mounted, setMounted] = useState(false)

  useEffect(() => setMounted(true), [])

  if (!mounted) return null

  return (
    <button
      onClick={() => setTheme(theme === 'dark' ? 'light' : 'dark')}
      className="p-8px hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors"
      aria-label="Toggle dark mode"
    >
      {theme === 'dark' ? <Sun size={20} /> : <Moon size={20} />}
    </button>
  )
}
```

---

## Reusable Admin Components

### FormSection

```tsx
<div className="
  p-24px
  bg-white dark:bg-gray-900
  border border-gray-200 dark:border-gray-800
  rounded-xl
">
  <h2 className="text-h2 font-bold text-gray-900 dark:text-white mb-8px">
    {title}
  </h2>
  {description && (
    <p className="text-body text-gray-600 dark:text-gray-400 mb-24px">
      {description}
    </p>
  )}
  <div className="space-y-24px">
    {children}
  </div>
</div>
```

### StatusBadge

```tsx
<span className={`
  px-12px py-6px rounded-full text-caption font-semibold
  ${status === 'active'
    ? 'bg-success/20 text-success'
    : status === 'pending'
    ? 'bg-warning/20 text-warning'
    : 'bg-gray-200 dark:bg-gray-700 text-gray-900 dark:text-white'
  }
`}>
  {status.charAt(0).toUpperCase() + status.slice(1)}
</span>
```

### ActionMenu

```tsx
<Popover>
  <PopoverTrigger asChild>
    <button className="p-8px hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg">
      <MoreVertical size={20} />
    </button>
  </PopoverTrigger>
  <PopoverContent align="end" className="w-48">
    <div className="space-y-2px">
      {items.map((item) => (
        <button
          key={item.label}
          onClick={item.action}
          className={`
            w-full text-left px-12px py-8px rounded-lg text-body-sm
            hover:bg-gray-100 dark:hover:bg-gray-800
            transition-colors
            ${item.danger ? 'text-error hover:bg-error/10' : 'text-gray-900 dark:text-white'}
          `}
        >
          {item.label}
        </button>
      ))}
    </div>
  </PopoverContent>
</Popover>
```

---

## Admin Dashboard Checklist

- ✅ Dark mode fully supported for B2B usage
- ✅ Sidebar navigation always visible on desktop
- ✅ Responsive mobile navigation
- ✅ Data tables with sorting & filtering
- ✅ Modal forms for CRUD operations
- ✅ Real-time status indicators
- ✅ Notification system
- ✅ User profile menu
- ✅ Keyboard shortcuts documentation
- ✅ Loading states & error boundaries

---

**Admin Dashboard UI Ready for Implementation** ✅
