# Admin Portal UI

## Overview

The admin portal is the internal dashboard for SuperAdmins and Admins to manage packages,
tenants, subscriptions, analytics, and settings. It is a Next.js 15 app under `web/admin-portal/`
using shadcn/ui, Framer Motion, and Tailwind CSS with a fixed dark sidebar layout.

Tech: Next.js 15 App Router, shadcn/ui, Framer Motion, Tailwind CSS (`dark:` class strategy).
Design inspiration: Stripe, Linear, Vercel Dashboard.

---

## Page Map

```
/admin                          → Dashboard Overview (stat cards + charts + activity table)
/admin/packages                 → Package list with search, filter, CRUD actions
/admin/packages/[id]           → Package editor (form with sidebar status panel)
/admin/tenants                 → Tenant list with invite, status filter
/admin/tenants/[id]            → Tenant detail (packages, subscription, settings)
/admin/subscriptions           → Subscription plans
/admin/analytics               → Analytics & reports
/admin/billing                 → Billing & invoices
/admin/settings                → Platform settings
/admin/users                   → User management
```

---

## Requirements

### Requirement: SPEC-ADMIN-LAYOUT — Fixed Sidebar Navigation

The admin portal MUST use a fixed left sidebar (256px wide on desktop) with dark background
(`bg-gray-900 dark:bg-gray-950`). On mobile, the sidebar collapses to a hamburger menu.
Navigation items highlight with `bg-primary-600 text-white` when active.

#### Scenario: Desktop sidebar always visible
- Given: Any admin user on a desktop screen (≥ 1024px)
- When: Admin portal page loads
- Then: Left sidebar (256px) is fixed; main content area is offset `md:ml-64`; sidebar shows logo, nav items, and user profile footer

#### Scenario: Mobile sidebar hidden by default
- Given: Admin user on mobile (< 1024px)
- When: Page loads
- Then: Sidebar is hidden; hamburger menu button (`<Menu />` icon) is shown in top-left of header

#### Scenario: Active nav item highlighted
- Given: Admin is on `/admin/packages`
- When: Sidebar renders
- Then: "Packages" NavItem shows `bg-primary-600 text-white`; all other items show `text-gray-300 hover:bg-gray-800`

---

### Requirement: SPEC-ADMIN-DASHBOARD — Dashboard Overview Page

The dashboard MUST show 4 stat cards (Active Tenants, Total Packages, Monthly Revenue, Active
Subscriptions), a revenue line chart with period selector (7d/30d/90d/1y), and a Recent Activity
table. Stat cards MUST use Framer Motion stagger animation on load.

#### Scenario: Stat cards load with stagger animation
- Given: Admin loads the dashboard
- When: Page renders
- Then: Four stat cards animate in sequence (staggerChildren: 0.1s, delayChildren: 0.2s) using `variants={staggerContainer}`; each card shows label, value, percentage change, and trend icon

#### Scenario: Revenue chart period selector
- Given: Dashboard is loaded with default 30d period
- When: Admin clicks "7d" period button
- Then: Chart data updates; selected button shows `bg-primary-600 text-white`; unselected buttons show `bg-gray-100 dark:bg-gray-800`

#### Scenario: Recent activity table shows latest actions
- Given: Recent admin and tenant actions exist
- When: Dashboard loads
- Then: Activity table shows Tenant name, action taken, status badge, and relative timestamp for each recent event

---

### Requirement: SPEC-ADMIN-PACKAGES-LIST — Package Management Page

The packages list MUST show all master packages in a data table with columns: Package Name,
Category, Price, Synced Tenants, Status, Actions. It MUST support text search and category filter.
Row hover shows `hover:bg-gray-50 dark:hover:bg-gray-800/50`.

#### Scenario: Admin searches packages by name
- Given: Admin is on `/admin/packages`
- When: Admin types in the search input
- Then: Table filters in real-time to show only matching packages (client-side filter or query param)

#### Scenario: Admin creates a new package
- Given: Admin clicks "New Package" button
- When: Package editor opens (navigate to `/admin/packages/new` or open modal)
- Then: Empty form renders with all required fields; "New Package" button has `whileHover={{ scale: 1.05 }}` Framer Motion effect

#### Scenario: Actions menu on each package row
- Given: Package list is showing
- When: Admin clicks the ⋮ (ActionMenu) button on a row
- Then: Popover shows: "Edit" → navigates to editor; "Duplicate" → copies package; "Delete" → destructive action with confirmation; danger items show `text-error hover:bg-error/10`

---

### Requirement: SPEC-ADMIN-PACKAGE-EDITOR — Package Editor Page

The editor MUST use a 2-column layout: main content (2/3 width) for form fields, and a
sidebar (1/3 width) for Status, Save, Preview, and SEO panels.
Sections include: Basic Information, Pricing, Media, Itinerary.

#### Scenario: Admin edits and saves a package
- Given: Admin is on `/admin/packages/[id]` with existing package data
- When: Admin updates the title and clicks "Save Changes"
- Then: PUT request sent; success toast shown; package title updates in the breadcrumb

#### Scenario: Package editor status sidebar
- Given: Admin is editing a package
- When: The Status sidebar panel renders
- Then: Shows a select dropdown (Draft / Published / Archived), a "Save Changes" primary button, and a "Preview" secondary button

#### Scenario: Generate Photo button available for all packages (admin and tenant)
- Given: Admin is editing any package (own or synced tenant package) in edit mode
- When: Editor renders
- Then: "Generate Photo" button is visible regardless of whether a tenantId is present; calls the correct API endpoint based on package ownership

---

### Requirement: SPEC-ADMIN-TENANTS-LIST — Tenant Management Page

The tenants list MUST show all tenants with columns: Tenant (avatar + name + email), Subdomain,
Subscription Plan, Joined Date, Status badge, Actions (View Details, Manage Packages, Suspend).

#### Scenario: Admin suspends a tenant
- Given: Admin is on `/admin/tenants`
- When: Admin clicks "Suspend" in the ActionMenu for a tenant
- Then: Confirmation dialog appears; on confirm → PUT request updates tenant status to Suspended; row reflects new status badge

#### Scenario: Subdomain shown as code block
- Given: Tenant has subdomain "travelpro"
- When: Tenant list renders
- Then: Subdomain column shows `<code className="bg-gray-100 dark:bg-gray-800 px-8px py-4px rounded">travelpro.litxustravel.com</code>`

---

## Key Reusable Components

### StatusBadge
```
status="active"   → bg-success/20 text-success
status="pending"  → bg-warning/20 text-warning
status="inactive" → bg-gray-200 dark:bg-gray-700 text-gray-900 dark:text-white
status="Draft"    → bg-yellow-100 text-yellow-800
status="Published"→ bg-green-100 text-green-800
status="Archived" → bg-gray-100 text-gray-600
```

### ActionMenu
- Trigger: `<MoreVertical />` icon button with `p-8px hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg`
- Content: `<Popover align="end">` with `w-48` width
- Danger items: `text-error hover:bg-error/10`

### FormSection
- Container: `p-24px bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl`
- Title: `text-h2 font-bold`
- Children: `space-y-24px`

### Dark Mode Pattern
All panels use: `bg-white dark:bg-gray-900` backgrounds with `border-gray-200 dark:border-gray-800` borders.
Text: `text-gray-900 dark:text-white` for primary, `text-gray-600 dark:text-gray-400` for secondary.
