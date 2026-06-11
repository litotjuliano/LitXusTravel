# LitXusTravel Design System
## Modern SaaS Travel Platform UI

**Version:** 1.0  
**Last Updated:** May 2026  
**Status:** Implementation-Ready for Claude Code

---

## 📋 Table of Contents
1. [Brand Identity](#brand-identity)
2. [Color Palette](#color-palette)
3. [Typography](#typography)
4. [Spacing & Layout](#spacing--layout)
5. [Components](#components)
6. [Animations & Motion](#animations--motion)
7. [Responsive Design](#responsive-design)
8. [Dark Mode](#dark-mode)
9. [Accessibility](#accessibility)

---

## Brand Identity

**Platform:** LitXusTravel - Travel Package Distribution Platform  
**Positioning:** Premium, modern, lifestyle-oriented  
**Design Philosophy:** Visual, media-rich, trust-driven  
**Inspiration:** Airbnb, Klook, TourRadar, Booking.com, Stripe

### Design Principles
- **Visual-first:** Large hero images, destination imagery, lifestyle photography
- **Mobile-first:** Responsive from 320px upward
- **Vibrant & Energetic:** Travel-inspired colors that evoke adventure
- **Clean & Minimal:** Whitespace as a feature, not crowding
- **Smooth & Animated:** Engaging micro-interactions throughout
- **Accessible:** WCAG AA compliance minimum
- **Premium Feel:** Elevated typography, refined spacing, quality imagery

---

## Color Palette

### Primary Colors (Vibrant Travel-Inspired)

```
Primary Blue (Ocean/Sky):
  hex: #0066CC
  rgb: 0, 102, 204
  usage: Primary CTA, headers, brand accent

Primary Teal (Tropical/Paradise):
  hex: #00A89A
  rgb: 0, 168, 154
  usage: Secondary actions, highlights

Primary Orange (Sunset/Adventure):
  hex: #FF6B35
  rgb: 255, 107, 53
  usage: Energy, warnings, excitement

Primary Green (Nature/Eco):
  hex: #2ECC71
  rgb: 46, 204, 113
  usage: Success states, eco-friendly tags
```

### Semantic Colors

```
Success:
  light: #E8F8F3
  main: #2ECC71
  dark: #27AE60

Warning:
  light: #FEF3E2
  main: #F39C12
  dark: #E67E22

Error:
  light: #FADBD8
  main: #E74C3C
  dark: #C0392B

Info:
  light: #E3F2FD
  main: #3498DB
  dark: #2980B9
```

### Neutrals (Light Mode Primary)

```
Background:
  primary: #FFFFFF
  secondary: #F8F9FA
  tertiary: #F0F2F5

Text:
  primary: #1A1A1A (near black)
  secondary: #666666 (gray)
  tertiary: #999999 (light gray)
  disabled: #CCCCCC

Border:
  light: #E0E0E0
  medium: #D0D0D0
  dark: #B0B0B0
```

### Tailwind Configuration (tailwind.config.ts)

```typescript
export default {
  theme: {
    colors: {
      // Brand colors
      primary: {
        50: '#F0F7FF',
        100: '#E0EFFF',
        200: '#C2DFFF',
        300: '#93C5FD',
        400: '#60A5FA',
        500: '#3B82F6', // Updated to match #0066CC brand
        600: '#0066CC', // Primary brand blue
        700: '#004BA3',
        800: '#003380',
        900: '#001A4D',
      },
      secondary: {
        50: '#F0FFFE',
        100: '#E0FFFC',
        200: '#C2FFF9',
        300: '#93FFF5',
        400: '#60FFF0',
        500: '#00A89A', // Teal secondary
        600: '#008A7F',
        700: '#006B66',
        800: '#004D4C',
        900: '#003333',
      },
      accent: {
        50: '#FFF5F0',
        100: '#FFEBE0',
        200: '#FFD7C2',
        300: '#FFB893',
        400: '#FF9960',
        500: '#FF6B35', // Orange accent
        600: '#E65A2B',
        700: '#CC491F',
        800: '#B33815',
        900: '#99270A',
      },
      success: '#2ECC71',
      warning: '#F39C12',
      error: '#E74C3C',
      info: '#3498DB',
    },
    extend: {
      spacing: {
        '4px': '0.25rem',
        '8px': '0.5rem',
        '12px': '0.75rem',
        '16px': '1rem',
        '24px': '1.5rem',
        '32px': '2rem',
        '40px': '2.5rem',
        '48px': '3rem',
        '56px': '3.5rem',
        '64px': '4rem',
      },
    },
  },
}
```

---

## Typography

### Font Stack

```css
Primary Font (Headlines, Bold): "Inter", -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif
Secondary Font (Body, Regular): "Inter", -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif
Monospace (Code): "Fira Code", "Courier New", monospace

Font weights:
- Regular: 400
- Medium: 500
- Semi-bold: 600
- Bold: 700
```

### Type Scale

```
Display Large (Hero titles):
  Size: 48px / 3rem
  Line-height: 56px / 3.5rem
  Weight: 700
  Letter-spacing: -1px
  Usage: Page titles, hero sections

Display Medium:
  Size: 36px / 2.25rem
  Line-height: 44px / 2.75rem
  Weight: 700
  Usage: Section headlines

Heading 1:
  Size: 32px / 2rem
  Line-height: 40px / 2.5rem
  Weight: 700
  Usage: Major section titles

Heading 2:
  Size: 24px / 1.5rem
  Line-height: 32px / 2rem
  Weight: 600
  Usage: Subsection titles

Heading 3:
  Size: 20px / 1.25rem
  Line-height: 28px / 1.75rem
  Weight: 600
  Usage: Component titles

Body Large:
  Size: 18px / 1.125rem
  Line-height: 28px / 1.75rem
  Weight: 400
  Usage: Large body text, descriptions

Body Regular:
  Size: 16px / 1rem
  Line-height: 24px / 1.5rem
  Weight: 400
  Usage: Standard body text

Body Small:
  Size: 14px / 0.875rem
  Line-height: 20px / 1.25rem
  Weight: 400
  Usage: Secondary text, labels

Caption:
  Size: 12px / 0.75rem
  Line-height: 16px / 1rem
  Weight: 500
  Usage: Metadata, timestamps
```

### Tailwind Typography Config

```typescript
extend: {
  fontSize: {
    'display-lg': ['48px', { lineHeight: '56px', letterSpacing: '-1px', fontWeight: '700' }],
    'display-md': ['36px', { lineHeight: '44px', fontWeight: '700' }],
    'h1': ['32px', { lineHeight: '40px', fontWeight: '700' }],
    'h2': ['24px', { lineHeight: '32px', fontWeight: '600' }],
    'h3': ['20px', { lineHeight: '28px', fontWeight: '600' }],
    'body-lg': ['18px', { lineHeight: '28px', fontWeight: '400' }],
    'body': ['16px', { lineHeight: '24px', fontWeight: '400' }],
    'body-sm': ['14px', { lineHeight: '20px', fontWeight: '400' }],
    'caption': ['12px', { lineHeight: '16px', fontWeight: '500' }],
  },
}
```

---

## Spacing & Layout

### Spacing Scale (8px base)

```
4px   - xs (tiny gaps, icon spacing)
8px   - sm (component padding)
12px  - md (small spacing)
16px  - lg (standard padding/margin)
24px  - xl (section spacing)
32px  - 2xl (larger gaps)
40px  - 3xl
48px  - 4xl (major section spacing)
56px  - 5xl
64px  - 6xl (hero sections)
```

### Grid System

```
Container max-width: 1280px (xl breakpoint)
Gutter (horizontal padding): 24px (mobile: 16px)
Column count: 12
Gap between items: 16px (mobile: 12px)

Mobile-first breakpoints:
- Mobile: 0px - 639px
- Tablet: 640px - 1023px
- Desktop: 1024px+
```

### Common Layouts

```
Full-width section:
  padding: 48px 24px (mobile: 32px 16px)
  max-width: 1280px
  margin: 0 auto

Content grid:
  display: grid
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr))
  gap: 24px

Hero section:
  min-height: 500px (mobile: 300px)
  display: flex
  align-items: center
  background: gradient or image
  padding: 64px 24px
```

---

## Components

### Button Styles

#### Primary Button (CTA)
```tsx
<button className="
  px-24px py-12px
  bg-primary-600 text-white
  rounded-lg
  font-semibold text-base
  hover:bg-primary-700
  active:bg-primary-800
  transition-colors duration-200
  disabled:opacity-50 disabled:cursor-not-allowed
">
  Book Now
</button>
```

#### Secondary Button
```tsx
<button className="
  px-24px py-12px
  border border-primary-200 text-primary-600
  bg-white
  rounded-lg
  font-semibold text-base
  hover:bg-primary-50
  transition-colors duration-200
">
  Learn More
</button>
```

#### Tertiary Button (Text)
```tsx
<button className="
  px-16px py-8px
  text-primary-600
  font-semibold text-base
  hover:text-primary-700
  underline underline-offset-2
">
  View Details
</button>
```

### Package Card Component

```tsx
<div className="
  bg-white
  rounded-xl
  overflow-hidden
  shadow-md hover:shadow-lg
  transition-shadow duration-300
  border border-gray-200
">
  {/* Image */}
  <div className="relative w-full h-64 overflow-hidden bg-gray-200">
    <Image
      src={packageImage}
      alt={packageTitle}
      className="w-full h-full object-cover hover:scale-105 transition-transform duration-500"
      fill
    />
    {isFeatured && (
      <span className="absolute top-16px right-16px bg-accent-500 text-white px-12px py-8px rounded-full text-caption font-semibold">
        Featured
      </span>
    )}
  </div>

  {/* Content */}
  <div className="p-24px">
    {/* Destination & Rating */}
    <div className="flex items-center justify-between mb-12px">
      <span className="text-caption text-gray-500 uppercase tracking-wide">
        {destination}
      </span>
      <div className="flex items-center gap-4px">
        <Star size={16} className="text-yellow-400 fill-yellow-400" />
        <span className="text-caption font-semibold">{rating}</span>
      </div>
    </div>

    {/* Title */}
    <h3 className="text-h3 font-bold text-gray-900 mb-8px line-clamp-2">
      {packageTitle}
    </h3>

    {/* Description */}
    <p className="text-body-sm text-gray-600 mb-16px line-clamp-2">
      {description}
    </p>

    {/* Duration & Price */}
    <div className="flex items-center justify-between mb-16px">
      <span className="text-body-sm text-gray-600">
        📅 {duration} Days
      </span>
      <span className="text-h2 font-bold text-primary-600">
        RM{price}
      </span>
    </div>

    {/* CTA */}
    <button className="
      w-full
      bg-primary-600 text-white
      px-16px py-12px
      rounded-lg
      font-semibold
      hover:bg-primary-700
      transition-colors duration-200
    ">
      View Package
    </button>
  </div>
</div>
```

### Hero Banner Component

```tsx
<section className="
  relative
  w-full
  h-screen sm:h-96 md:h-[500px]
  overflow-hidden
">
  {/* Background Image */}
  <Image
    src={heroImage}
    alt={heroTitle}
    fill
    className="absolute inset-0 w-full h-full object-cover"
    priority
  />

  {/* Overlay */}
  <div className="absolute inset-0 bg-black/30"></div>

  {/* Content */}
  <div className="
    relative
    h-full
    flex flex-col justify-center
    px-24px sm:px-32px md:px-48px
    max-w-7xl
  ">
    <h1 className="
      text-display-md sm:text-display-lg
      font-bold
      text-white
      mb-16px
      max-w-2xl
    ">
      {heroTitle}
    </h1>

    <p className="
      text-body-lg sm:text-h3
      text-white/90
      max-w-xl
      mb-32px
    ">
      {heroSubtitle}
    </p>

    <div className="flex gap-16px">
      <button className="
        px-32px py-16px
        bg-primary-600 text-white
        rounded-lg
        font-semibold
        hover:bg-primary-700
        transition-colors
      ">
        Explore Now
      </button>
      <button className="
        px-32px py-16px
        border-2 border-white text-white
        rounded-lg
        font-semibold
        hover:bg-white/10
        transition-colors
      ">
        Learn More
      </button>
    </div>
  </div>
</section>
```

### Form Input Component

```tsx
<div className="w-full">
  <label className="
    block text-body-sm font-semibold text-gray-900 mb-8px
  ">
    {label}
    {required && <span className="text-error ml-4px">*</span>}
  </label>

  <input
    type={type}
    placeholder={placeholder}
    className="
      w-full
      px-16px py-12px
      border border-gray-300
      rounded-lg
      text-body
      placeholder:text-gray-400
      focus:outline-none
      focus:ring-2 focus:ring-primary-500 focus:border-transparent
      hover:border-gray-400
      transition-colors
      disabled:bg-gray-100 disabled:text-gray-500 disabled:cursor-not-allowed
    "
  />

  {error && (
    <p className="text-body-sm text-error mt-8px">
      {error}
    </p>
  )}
</div>
```

### Data Table Component (Admin)

```tsx
<div className="overflow-x-auto">
  <table className="w-full">
    <thead className="bg-gray-50 border-b border-gray-200">
      <tr>
        <th className="px-24px py-16px text-left text-body-sm font-semibold text-gray-900">
          Column Header
        </th>
        {/* More headers */}
      </tr>
    </thead>
    <tbody className="divide-y divide-gray-200">
      {data.map((row) => (
        <tr key={row.id} className="hover:bg-gray-50 transition-colors">
          <td className="px-24px py-16px text-body text-gray-900">
            {row.value}
          </td>
        </tr>
      ))}
    </tbody>
  </table>
</div>
```

### Modal/Dialog Component

```tsx
<div className="fixed inset-0 bg-black/50 flex items-center justify-center p-24px z-50">
  <motion.div
    initial={{ opacity: 0, scale: 0.95 }}
    animate={{ opacity: 1, scale: 1 }}
    exit={{ opacity: 0, scale: 0.95 }}
    transition={{ duration: 0.2 }}
    className="
      bg-white
      rounded-xl
      shadow-xl
      max-w-lg w-full
      max-h-[90vh]
      overflow-y-auto
    "
  >
    {/* Header */}
    <div className="flex items-center justify-between p-24px border-b border-gray-200">
      <h2 className="text-h2 font-bold text-gray-900">
        {title}
      </h2>
      <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
        <X size={24} />
      </button>
    </div>

    {/* Content */}
    <div className="p-24px">
      {children}
    </div>

    {/* Footer */}
    <div className="flex gap-12px justify-end p-24px border-t border-gray-200">
      <button onClick={onClose} className="px-24px py-12px border border-gray-300 rounded-lg text-gray-900 font-semibold hover:bg-gray-50">
        Cancel
      </button>
      <button onClick={onSubmit} className="px-24px py-12px bg-primary-600 text-white rounded-lg font-semibold hover:bg-primary-700">
        Submit
      </button>
    </div>
  </motion.div>
</div>
```

---

## Animations & Motion

### Framer Motion Presets

```typescript
// Common easing curves
export const EASING = {
  smooth: [0.4, 0, 0.2, 1],      // Material Design
  ease: [0.25, 0.46, 0.45, 0.94],
  cubic: [0.17, 0.67, 0.83, 0.67],
}

// Common transitions
export const TRANSITIONS = {
  fast: { duration: 0.15, ease: EASING.smooth },
  normal: { duration: 0.2, ease: EASING.smooth },
  slow: { duration: 0.3, ease: EASING.smooth },
  slower: { duration: 0.5, ease: EASING.smooth },
}

// Page transitions
export const pageVariants = {
  hidden: { opacity: 0, y: 20 },
  visible: { opacity: 1, y: 0, transition: TRANSITIONS.normal },
  exit: { opacity: 0, y: -20, transition: TRANSITIONS.fast },
}

// Hover animations
export const hoverScale = {
  whileHover: { scale: 1.05 },
  transition: TRANSITIONS.fast,
}

export const hoverLift = {
  whileHover: { y: -4, boxShadow: '0 20px 25px rgba(0,0,0,0.1)' },
  transition: TRANSITIONS.fast,
}

// Stagger container
export const staggerContainer = {
  hidden: { opacity: 0 },
  visible: {
    opacity: 1,
    transition: {
      staggerChildren: 0.1,
      delayChildren: 0.2,
    },
  },
}

export const staggerItem = {
  hidden: { opacity: 0, y: 20 },
  visible: { opacity: 1, y: 0, transition: TRANSITIONS.normal },
}
```

### Common Animation Patterns

```tsx
// Fade In
<motion.div
  initial={{ opacity: 0 }}
  animate={{ opacity: 1 }}
  transition={{ duration: 0.3 }}
/>

// Slide Up
<motion.div
  initial={{ opacity: 0, y: 20 }}
  animate={{ opacity: 1, y: 0 }}
  transition={{ duration: 0.4 }}
/>

// Smooth Scroll
<motion.div
  initial={{ opacity: 0, x: -50 }}
  whileInView={{ opacity: 1, x: 0 }}
  transition={{ duration: 0.5 }}
  viewport={{ once: true, amount: 0.3 }}
/>

// Image Hover Zoom
<motion.img
  whileHover={{ scale: 1.1 }}
  transition={{ duration: 0.3 }}
/>

// Skeleton Loading
<motion.div
  animate={{ opacity: [0.5, 1, 0.5] }}
  transition={{ duration: 1.5, repeat: Infinity }}
  className="bg-gray-200 rounded"
/>
```

---

## Responsive Design

### Breakpoints (Mobile-First)

```
Mobile:        0px - 639px (sm)
Tablet:        640px - 1023px (md)
Desktop:       1024px - 1279px (lg)
Large Desktop: 1280px+ (xl)
```

### Responsive Typography

```
Display Large:
  Mobile:   36px
  Tablet:   40px
  Desktop:  48px

Heading 1:
  Mobile:   24px
  Tablet:   28px
  Desktop:  32px

Body:
  Mobile:   14px
  Tablet:   15px
  Desktop:  16px
```

### Responsive Layouts

```
Hero Banner:
  Mobile:   h-80 (320px)
  Tablet:   h-96 (384px)
  Desktop:  h-screen or h-[500px]

Grid Cards:
  Mobile:   1 column
  Tablet:   2 columns
  Desktop:  3-4 columns

Sidebar Navigation:
  Mobile:   Drawer/Hamburger
  Desktop:  Fixed/Collapsible sidebar
```

---

## Dark Mode

### Dark Mode Color Mapping

```typescript
// Dark mode adjustments
darkMode: 'class',

theme: {
  extend: {
    colors: {
      // Dark mode overrides
      'dark': {
        'bg-primary': '#0F0F0F',
        'bg-secondary': '#1A1A1A',
        'text-primary': '#FFFFFF',
        'text-secondary': '#E0E0E0',
        'border': '#333333',
      }
    }
  }
}
```

### Dark Mode Component Example

```tsx
<div className="
  bg-white dark:bg-gray-900
  text-gray-900 dark:text-white
  border border-gray-200 dark:border-gray-800
  rounded-lg
  transition-colors duration-200
">
  {/* Content */}
</div>
```

### Dark Mode Toggle

```tsx
'use client'

import { useTheme } from 'next-themes'
import { Moon, Sun } from 'lucide-react'

export function ThemeToggle() {
  const { theme, setTheme } = useTheme()

  return (
    <button
      onClick={() => setTheme(theme === 'dark' ? 'light' : 'dark')}
      className="p-8px hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors"
    >
      {theme === 'dark' ? <Sun size={20} /> : <Moon size={20} />}
    </button>
  )
}
```

---

## Accessibility

### WCAG AA Compliance

#### Color Contrast
```
Normal text: 4.5:1 minimum
Large text:  3:1 minimum
UI elements: 3:1 minimum
```

#### Keyboard Navigation
- All interactive elements must be keyboard accessible
- Tab order must be logical
- Focus indicators must be visible (minimum 3px)
- Escape key closes modals

#### Screen Readers
- Use semantic HTML (button, nav, main, etc.)
- Add aria-labels where needed
- Use aria-live for dynamic updates
- Proper heading hierarchy (h1 → h2 → h3)

#### Image Alt Text
```tsx
<Image
  src={url}
  alt="Descriptive text explaining the image content"
  title="Optional title for hover"
/>
```

#### Form Accessibility
```tsx
<label htmlFor="email">
  Email Address
</label>
<input
  id="email"
  type="email"
  aria-required="true"
  aria-describedby="email-hint"
/>
<p id="email-hint" className="text-caption text-gray-500">
  We'll never share your email
</p>
```

#### Skip Links
```tsx
<a href="#main" className="sr-only focus:not-sr-only">
  Skip to main content
</a>
<main id="main">
  {/* Content */}
</main>
```

### Focus Management

```tsx
// Auto-focus modal
<motion.div
  initial={{ opacity: 0 }}
  animate={{ opacity: 1 }}
  ref={modalRef}
  onAnimationComplete={() => modalRef.current?.focus()}
>
  {/* Modal content */}
</motion.div>
```

---

## Implementation for Claude Code

### Next.js Setup Required:
1. `npm install -D tailwindcss postcss autoprefixer`
2. `npx shadcn-ui@latest init`
3. `npm install framer-motion next-themes`

### File Structure:
```
web/
├── public-website/
│   ├── app/
│   ├── components/
│   │   ├── ui/              (shadcn components)
│   │   ├── sections/        (Hero, Grid, etc.)
│   │   └── common/
│   ├── lib/
│   │   ├── animations.ts    (Framer Motion presets)
│   │   ├── colors.ts        (Color tokens)
│   │   └── classNames.ts
│   └── styles/
│       └── globals.css
│
├── admin-portal/
│   └── (same structure)
│
└── shared-ui/              (Shared design tokens)
    ├── tailwind.config.ts
    ├── colors.ts
    └── animations.ts
```

### Next Steps:
Claude Code will:
1. Setup Next.js with Tailwind + shadcn/ui
2. Implement components from this design system
3. Create pages with animations
4. Connect to .NET API
5. Setup environment variables

---

**Design System Ready for Implementation** ✅

---

# 🎨 LitXusTravel Design System

**Created by:** Lito Juliano  
**Last Updated:** May 30, 2026  
**Version:** 1.0

---

## 🎯 Design Philosophy

LitXusTravel uses a modern, clean design system optimized for:
- SaaS administration dashboards
- Real-time data visualization
- Multi-tenant operations
- Responsive mobile-to-desktop experiences

---

## 🌈 Color Palette

### Primary Colors
- **Primary Blue:** `#0066CC` - Main actions, links, buttons
- **Secondary Teal:** `#00A89A` - Accents, success states, secondary actions
- **Accent Orange:** `#FF6B35` - Warnings, highlights, alerts

### Neutral Colors
- **Dark BG:** `#0f172a` - Primary background
- **Card BG:** `#1a2a4a` - Secondary background
- **Text Primary:** `#e2e8f0` - Main text
- **Text Secondary:** `#94a3b8` - Secondary text
- **Border:** `rgba(255, 255, 255, 0.1)` - Subtle borders

### Status Colors
- **Success:** `#4ade80` - Green (operations successful)
- **Error:** `#f87171` - Red (failures, errors)
- **Warning:** `#facc15` - Yellow (warnings, alerts)
- **Info:** `#60a5fa` - Light Blue (information)

---

## 🔤 Typography

### Font Stack
```
-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif
Monospace: 'Monaco', 'Menlo', 'Ubuntu Mono'
```

### Font Sizes
- **H1 (Page Title):** 32px, Bold
- **H2 (Section Title):** 24px, Bold
- **H3 (Subsection):** 18px, Semi-Bold
- **Body (Default):** 14px, Regular
- **Small (Labels):** 12px, Medium
- **Tiny (Helper):** 11px, Regular

### Line Heights
- **Headings:** 1.2
- **Body:** 1.6
- **Tight:** 1.4

---

## 📐 Spacing System

Base unit: **8px**

```
xs: 4px
sm: 8px
md: 16px
lg: 24px
xl: 32px
2xl: 48px
```

### Common Spacing
- Button padding: `12px 20px` (vertical/horizontal)
- Card padding: `20px`
- Section margin: `40px 0`
- Gap between items: `15px`

---

## 🎨 Components

### Buttons

#### Primary Button
- Background: Linear gradient (#0066CC → #0052A3)
- Color: White
- Padding: `12px 20px`
- Border-radius: `6px`
- Font-weight: `600`
- Hover: `translateY(-2px)` + shadow

#### Secondary Button
- Background: `rgba(255, 255, 255, 0.1)`
- Color: `#cbd5e1`
- Border: `1px solid rgba(255, 255, 255, 0.2)`
- Padding: `12px 20px`
- Border-radius: `6px`
- Hover: Background opacity increase

#### Danger Button
- Background: Linear gradient (#ff6b6b → #ee5a52)
- Color: White
- Padding: `12px 20px`
- Hover: `translateY(-2px)` + shadow

### Cards
- Background: `rgba(255, 255, 255, 0.05)`
- Border: `1px solid rgba(255, 255, 255, 0.1)`
- Border-radius: `12px`
- Padding: `20px`
- Backdrop: `blur(10px)`
- Hover: Slight opacity increase

### Inputs
- Background: `rgba(0, 0, 0, 0.3)`
- Border: `1px solid rgba(255, 255, 255, 0.1)`
- Color: `#e2e8f0`
- Padding: `12px`
- Border-radius: `6px`
- Focus: Border color to primary blue

### Badges
- **Success:** Background `rgba(0, 200, 0, 0.2)`, Color `#4ade80`
- **Error:** Background `rgba(200, 0, 0, 0.2)`, Color `#f87171`
- **Warning:** Background `rgba(255, 165, 0, 0.2)`, Color `#fbbf24`
- **Info:** Background `rgba(0, 102, 204, 0.2)`, Color `#60a5fa`
- Padding: `4px 12px`
- Border-radius: `6px`
- Font-size: `12px`

---

## 🎬 Animations & Transitions

### Default Transitions
- Duration: `0.2s` - `0.3s`
- Timing: `ease` or `ease-in-out`

### Common Animations
- **Button Hover:** Slight upward movement (`translateY(-2px)`)
- **Card Hover:** Subtle lift + opacity change
- **Input Focus:** Border color change
- **Modals:** Fade in with blur backdrop

### Framer Motion Presets (Admin Portal)
```
fadeIn: opacity 0 → 1 (0.2s)
slideUp: y -20px → 0 (0.3s)
slideDown: y 0 → 20px (0.3s)
scaleIn: scale 0.9 → 1 (0.2s)
```

---

## 📱 Responsive Breakpoints

- **Mobile:** 375px - 667px (single column)
- **Tablet:** 768px - 1024px (2 columns)
- **Desktop:** 1025px - 1920px (3+ columns)
- **Wide:** 1921px+ (full-width optimized)

### Layout Adjustments
- **Mobile:** Single column, full-width cards, compact spacing
- **Tablet:** 2-column grid, adjusted padding
- **Desktop:** 3-column grid, optimal spacing

---

## 🌙 Dark Mode

The design system is built on dark mode by default:
- Dark backgrounds reduce eye strain
- Accent colors pop on dark surfaces
- Text contrast meets WCAG AA standards
- No light mode alternative currently

---

## ♿ Accessibility

### Contrast
- Text on background: Minimum 4.5:1 ratio (WCAG AA)
- Focus states: Clear and visible (min 2px outline)
- Color not sole indicator of state

### Keyboard Navigation
- All interactive elements focusable with Tab
- Focus order logical and intuitive
- Form validation error messages clear

### Screen Readers
- Semantic HTML (buttons, links, labels)
- ARIA labels where needed
- Form field associations

---

## 🎯 Usage Guidelines

### When to Use Primary Color
- Main call-to-action buttons
- Links
- Active states
- Progress indicators

### When to Use Secondary Color (Teal)
- Success messages
- Active/running states
- Secondary buttons
- Accent borders

### When to Use Accent Color (Orange)
- Warnings
- Important highlights
- Attention-needed items
- Alert boxes

---

## 📐 Grid System

### 12-Column Grid
- Column width varies by breakpoint
- Gutter: `20px` (10px each side)
- Max-width: `1400px`

### Container Widths
- Mobile: Full width with padding
- Tablet: 90% width
- Desktop: 1400px max

---

## 🎨 Example Component Combinations

### Success Card
```
Background: rgba(0, 200, 0, 0.05)
Border-left: 3px solid #4ade80
Text: #e2e8f0
Icon: ✅
```

### Error Alert
```
Background: rgba(255, 100, 100, 0.05)
Border: 1px solid rgba(255, 100, 100, 0.2)
Text: #f87171
Icon: ⚠️
```

### Info Panel
```
Background: rgba(0, 102, 204, 0.1)
Border-left: 3px solid #0066cc
Text: #cbd5e1
```

---

## 🔄 Component States

### Button States
- **Default:** Base styling
- **Hover:** Transform + shadow
- **Active:** Color shift
- **Disabled:** Opacity 0.5, cursor not-allowed
- **Loading:** Spinner animation

### Input States
- **Default:** Base border
- **Focus:** Primary color border
- **Filled:** Light background
- **Error:** Red border + error message
- **Disabled:** Gray background, cursor not-allowed

### Card States
- **Default:** Base styling
- **Hover:** Slight lift + opacity change
- **Active:** Border highlight
- **Loading:** Skeleton or spinner overlay

---

## 📚 Resources

For implementation details, see:
- Admin Dashboard UI: `UI_SPECS_ADMIN_DASHBOARD.md`
- Public Website UI: `UI_SPECS_PUBLIC_WEBSITE.md`
- Architecture & Code: `CLAUDE.md`

---

**Design System Ready** ✅
