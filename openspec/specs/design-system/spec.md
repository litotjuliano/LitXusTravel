# Design System

## Overview

LitXusTravel uses a unified design system across both the public website and admin portal.
The system is implemented in Tailwind CSS and applied via shadcn/ui components. The design
philosophy is: visual-first, mobile-first, accessible (WCAG AA), and smooth (Framer Motion animations).

---

## Brand Colors

```
Primary Blue  (Ocean/Sky):       #0066CC   — primary CTAs, headers, links, active states
Secondary Teal (Tropical):       #00A89A   — secondary actions, success highlights
Accent Orange (Sunset):          #FF6B35   — warnings, energy, attention highlights
Success Green:                   #2ECC71   — success states, eco-friendly tags
```

### Tailwind Mapping

```
primary-600  = #0066CC   (primary brand blue — use for buttons, active nav)
secondary-500 = #00A89A  (teal — use for secondary actions)
accent-500   = #FF6B35   (orange — use for warnings, highlights)
success      = #2ECC71
warning      = #F39C12
error        = #E74C3C
info         = #3498DB
```

### Semantic Colors (dark mode defaults)

```
Background primary:    #0F0F0F  (dark) / #FFFFFF  (light)
Background secondary:  #1A1A1A  (dark) / #F8F9FA  (light)
Text primary:          #FFFFFF  (dark) / #1A1A1A  (light)
Text secondary:        #E0E0E0  (dark) / #666666  (light)
Border:                #333333  (dark) / #E0E0E0  (light)
```

---

## Typography

Font family: **Inter** (primary and body), fallback: `-apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif`
Monospace: `"Fira Code", "Courier New", monospace`

### Type Scale

```
display-lg  48px / 56px lh  weight 700   letter-spacing -1px   — hero titles
display-md  36px / 44px lh  weight 700                          — section headlines
h1          32px / 40px lh  weight 700                          — page titles
h2          24px / 32px lh  weight 600                          — subsection titles
h3          20px / 28px lh  weight 600                          — component titles
body-lg     18px / 28px lh  weight 400                          — large descriptions
body        16px / 24px lh  weight 400                          — standard text
body-sm     14px / 20px lh  weight 400                          — secondary text, labels
caption     12px / 16px lh  weight 500                          — metadata, timestamps
```

---

## Spacing Scale (8px base)

```
4px  (0.25rem)  — xs: tiny gaps, icon spacing
8px  (0.5rem)   — sm: component inner spacing
12px (0.75rem)  — md: small gaps
16px (1rem)     — lg: standard padding/margin (Tailwind: `p-4` or custom `px-16px`)
24px (1.5rem)   — xl: section spacing, card padding
32px (2rem)     — 2xl: larger gaps
40px (2.5rem)   — 3xl
48px (3rem)     — 4xl: major section spacing
64px (4rem)     — 6xl: hero sections
```

---

## Layout & Grid

```
Container max-width: 1280px
Horizontal gutter:   24px desktop / 16px mobile
Column system:       12 columns
Gap:                 24px desktop / 12px mobile

Breakpoints (mobile-first):
  Mobile:   0px   – 639px   (sm)
  Tablet:   640px – 1023px  (md)
  Desktop:  1024px – 1279px (lg)
  Large:    1280px+          (xl)

Hero section:
  min-height: 500px  (mobile: 320px)
  padding: 64px 24px

Admin sidebar:
  width: 256px (fixed, desktop); drawer on mobile
```

---

## Animations (Framer Motion)

```typescript
// Easing
smooth: [0.4, 0, 0.2, 1]

// Transitions
fast:   { duration: 0.15, ease: smooth }
normal: { duration: 0.2,  ease: smooth }
slow:   { duration: 0.3,  ease: smooth }

// Page transitions
pageVariants: {
  hidden:  { opacity: 0, y: 20 },
  visible: { opacity: 1, y: 0, transition: normal },
  exit:    { opacity: 0, y: -20, transition: fast }
}

// Stagger (dashboard stat cards)
staggerContainer: { staggerChildren: 0.1, delayChildren: 0.2 }
staggerItem:      { hidden: { opacity: 0, y: 20 }, visible: { opacity: 1, y: 0 } }

// Button hover
whileHover: { scale: 1.05 }
whileTap:   { scale: 0.95 }

// Card hover lift
hoverLift: { y: -4, boxShadow: "0 20px 25px rgba(0,0,0,0.1)" }
```

---

## Component Patterns

### Primary Button
```
px-24px py-12px  bg-primary-600 text-white  rounded-lg  font-semibold
hover: bg-primary-700   active: bg-primary-800
disabled: opacity-50 cursor-not-allowed
Framer: whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}
```

### Secondary Button
```
px-24px py-12px  border border-primary-200  text-primary-600  bg-white  rounded-lg
hover: bg-primary-50
```

### Package Card (public website)
```
bg-white  rounded-xl  overflow-hidden  shadow-md hover:shadow-lg  border border-gray-200
Image: h-64 object-cover, hover:scale-105 duration-500
Content: p-24px  title: text-h3 font-bold  price: text-h2 text-primary-600
```

### Form Input
```
w-full  px-16px py-12px  border border-gray-300  rounded-lg  text-body
focus: ring-2 ring-primary-500 border-transparent
error: border-error ring-error
disabled: bg-gray-100 text-gray-500 cursor-not-allowed
```

### Modal/Dialog
```
Backdrop: fixed inset-0 bg-black/50 z-50
Container: bg-white rounded-xl shadow-xl max-w-lg w-full max-h-[90vh] overflow-y-auto
Animation: initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} duration 0.2s
Header: p-24px border-b  |  Content: p-24px  |  Footer: p-24px border-t flex gap-12px justify-end
```

### Status Badge
```
Active/Published: bg-success/20 text-success
Pending/Warning:  bg-warning/20 text-warning
Inactive/Draft:   bg-gray-200 dark:bg-gray-700 text-gray-900 dark:text-white
Danger/Error:     bg-error/20 text-error
Base: px-12px py-6px rounded-full text-caption font-semibold
```

---

## Accessibility Requirements

- Minimum contrast ratio: 4.5:1 for normal text, 3:1 for large text and UI elements (WCAG AA)
- All interactive elements keyboard accessible; tab order logical
- Focus indicators: minimum 3px visible outline
- Escape key closes all modals
- Semantic HTML: `<button>`, `<nav>`, `<main>`, `<h1>`...`<h3>` hierarchy
- Images: always include descriptive `alt` text
- Forms: `<label>` associated to every `<input>` via `htmlFor`
- Dynamic updates: use `aria-live` regions

---

## File Locations

```
web/admin-portal/
  src/
    lib/
      animations.ts   — Framer Motion presets (EASING, TRANSITIONS, pageVariants, stagger*)
    tailwind.config.ts — color tokens (primary-600=#0066CC, secondary-500=#00A89A, accent-500=#FF6B35)

web/public-website/
  src/
    lib/
      animations.ts   — shared animation presets
    tailwind.config.ts — same color tokens
```
