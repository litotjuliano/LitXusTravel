# Admin Portal UI

## Overview

The admin portal is the internal dashboard for SuperAdmins and Admins to manage packages,
tenants, subscriptions, analytics, and settings. It is a Next.js 16 app under `web/admin-portal/`
using a TailAdmin-derived component library (no shadcn/ui, no `@base-ui/react`), Framer Motion,
and Tailwind CSS v4 with a fixed always-dark sidebar layout.

Tech: Next.js 16 App Router, TailAdmin (free React template, adapted for Next.js), Framer Motion,
Tailwind CSS v4 (`dark:` class strategy applies to main content only — sidebar is always dark).
Design inspiration: TailAdmin dashboard reference (dark navy sidebar + light content area).

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

### Requirement: SPEC-ADMIN-LAYOUT — Collapsible Dark Sidebar Navigation

The admin portal MUST use a fixed left sidebar that is **always dark** (`bg-gray-dark` / `#1a2231`)
regardless of the light/dark theme toggle — the toggle only affects the main content area and header.
On desktop the sidebar is 290px wide when expanded, collapses to a 90px icon rail, and re-expands
on hover while collapsed. On mobile the sidebar is hidden and opens as a full-width drawer with a
backdrop overlay.

#### Scenario: Desktop sidebar expanded by default
- Given: Any admin user on a desktop screen (≥ 1024px)
- When: Admin portal page loads
- Then: Left sidebar (290px, `bg-gray-dark`) is fixed; main content area is offset `lg:ml-[290px]`; sidebar shows logo, "MENU" section label, nav items with icons, and submenu accordions

#### Scenario: Sidebar collapses to icon rail
- Given: Admin clicks the hamburger toggle in the header on desktop
- When: `SidebarContext.isExpanded` becomes false
- Then: Sidebar narrows to 90px showing icons only; main content offset becomes `lg:ml-[90px]`; hovering the collapsed sidebar temporarily re-expands it to 290px (`isHovered`)

#### Scenario: Mobile sidebar hidden by default, opens as drawer
- Given: Admin user on mobile (< 1024px)
- When: Page loads
- Then: Sidebar is translated off-screen (`-translate-x-full`); hamburger button in header toggles `isMobileOpen`, sliding the sidebar in and rendering a `<Backdrop />` overlay behind it

#### Scenario: Active nav item highlighted
- Given: Admin is on `/admin/packages`
- When: Sidebar renders
- Then: "Packages" NavItem shows `bg-white/[0.08] text-white` with `text-brand-400` icon (the `menu-item-active` / `menu-item-icon-active` utility classes); all other items show `text-gray-400 hover:bg-white/[0.05] hover:text-gray-200`

---

### Requirement: SPEC-ADMIN-HEADER — Sticky Header with Search

The admin portal header MUST be sticky, light-themed (`bg-white dark:bg-gray-900`), and include a
centered search input (hidden below `md`) with a leading search icon and a trailing "⌘K" shortcut
badge. The search input is presentational only in the current implementation (not yet wired to a
command palette or live search).

#### Scenario: Header shows search bar on tablet and above
- Given: Admin views the dashboard on a viewport ≥ 768px
- When: Header renders
- Then: A search input with placeholder "Search or type command..." appears centered between the hamburger toggle and the right-side icon cluster, with a "⌘K" badge anchored to its right edge

#### Scenario: Right-side icon cluster
- Given: Header renders on any viewport
- When: Admin looks at the right side of the header
- Then: Theme toggle (Sun/Moon) → notification bell (with unread dot) → user avatar dropdown appear in that order, each as a 40×40px hover-highlighted button

---

### Requirement: SPEC-ADMIN-LOGIN — Dev Credentials Cheatsheet

The admin-portal login page MUST show a "Dev Credentials" card below the sign-in form listing seed
accounts (Super Admin, Platform Admin, Tenant Admins). Clicking a row MUST populate the email and
password fields via `react-hook-form`'s `setValue()` — the admin still clicks "Sign In" manually;
credentials are not auto-submitted.

#### Scenario: Clicking a credential row fills the form
- Given: Admin is on `/auth/login`
- When: Admin clicks the row for `admin@travelpro.com`
- Then: The email field shows `admin@travelpro.com`; the password field shows `TravelPro@123`; the clicked row is highlighted with `bg-brand-500/10`

#### Scenario: Role badges are color-coded
- Given: The Dev Credentials table renders
- When: Rows display
- Then: Super Admin shows a red-tinted badge, Platform Admin orange, Tenant Admin rows blue — each role tier visually distinct

#### Scenario: Password is discoverable without cluttering the table
- Given: Admin hovers a credential row
- When: The row's `title` attribute is read by the browser tooltip
- Then: Tooltip shows `"Fill: {email} / {password}"` — the password column itself is not rendered in the table

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

All components below are TailAdmin-derived (`src/components/ui/`) — no shadcn/ui, no `@base-ui/react`.

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
- Built on `Dropdown` / `DropdownItem` (`src/components/ui/Dropdown.tsx`) — manual `useState` open/close, not a headless popover lib
- Trigger: `<MoreVertical />` icon button with `dropdown-toggle` class (required for Dropdown's click-outside detection) and `p-2 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg`
- Content: `w-48` panel anchored via the Dropdown component
- Danger items: `text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20`

### Modal
- `src/components/ui/Modal.tsx` — TailAdmin pattern: plain JS, ESC + click-outside handling, **no portal, no focus trap** (acceptable for internal admin tooling)
- API: `isOpen`, `onClose`, `className`, `showCloseButton`, `isFullscreen`
- For fixed-header/scrollable-body/fixed-footer modals (e.g. PackageEditorModal): apply `flex flex-col max-h-[90vh] overflow-hidden` on the Modal's `className`, `shrink-0` on header/footer, `flex-1 overflow-y-auto` on the body section

### Select
- Native `<select>` element wrapped by `src/components/ui/select.tsx`; `SelectTrigger`/`SelectContent`/`SelectItem` are passthrough stubs rendering `<option>` tags — no headless listbox

### FormSection
- Container: `p-6 bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl`
- Title: `text-h2 font-bold`
- Children: `space-y-6`

### Dark Mode Pattern
Sidebar is **exempt** — always `bg-gray-dark`, never toggles. All other panels (header, main content,
cards, modals) use: `bg-white dark:bg-gray-900` backgrounds with `border-gray-200 dark:border-gray-800`
borders. Text: `text-gray-900 dark:text-white` for primary, `text-gray-500 dark:text-gray-400` for secondary.
