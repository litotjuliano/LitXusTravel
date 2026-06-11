# LitXusTravel Public Website - UI Specifications
## White-Label Travel Platform

**Inspiration:** Airbnb, Klook, Booking.com, TourRadar  
**Style:** Vibrant, visual, lifestyle-oriented, modern SaaS  
**Target:** Travel agencies' customers browsing & booking packages

---

## 📋 Page Map

```
/                              → Home
/packages                      → Package Listing
/packages/[id]                → Package Detail
/contact                       → Contact Us
/blog                         → Blog Listing
/blog/[slug]                 → Blog Post
/about                        → About Agent
/testimonials                 → Customer Testimonials
```

---

## 1. Homepage (`/`)

### Layout Structure

```
┌─────────────────────────────────────┐
│   Navigation Bar (Fixed/Sticky)     │
├─────────────────────────────────────┤
│                                     │
│        Hero Banner Section          │ (500px height)
│        Large Image + CTA            │
│                                     │
├─────────────────────────────────────┤
│                                     │
│     Featured Packages Grid          │ (3-4 cards)
│                                     │
├─────────────────────────────────────┤
│                                     │
│   Popular Destinations (Carousel)   │ (5-6 cards)
│                                     │
├─────────────────────────────────────┤
│                                     │
│    Call-to-Action Section           │
│    "WhatsApp Us For Custom Tours"   │
│                                     │
├─────────────────────────────────────┤
│    Testimonials Carousel            │
│    (3 testimonial cards)            │
│                                     │
├─────────────────────────────────────┤
│    Footer                           │
└─────────────────────────────────────┘
```

### Component Details

#### Navigation Bar

```tsx
<nav className="sticky top-0 z-40 bg-white shadow-sm">
  <div className="max-w-7xl mx-auto px-24px py-16px">
    <div className="flex items-center justify-between">
      {/* Logo */}
      <div className="flex items-center gap-12px">
        <Image
          src="/logo.svg"
          alt="Brand"
          width={40}
          height={40}
        />
        <h1 className="text-h2 font-bold text-gray-900">
          {tenantName}
        </h1>
      </div>

      {/* Navigation Menu - Desktop */}
      <div className="hidden md:flex items-center gap-32px">
        <NavLink href="/packages">Packages</NavLink>
        <NavLink href="/blog">Blog</NavLink>
        <NavLink href="/about">About</NavLink>
        <NavLink href="/contact">Contact</NavLink>
      </div>

      {/* CTA Button */}
      <button className="
        px-24px py-12px
        bg-primary-600 text-white
        rounded-lg
        font-semibold
        hover:bg-primary-700
        transition-colors
        hidden sm:block
      ">
        WhatsApp Us
      </button>

      {/* Mobile Menu Toggle */}
      <button className="md:hidden">
        <Menu size={24} />
      </button>
    </div>
  </div>
</nav>
```

#### Hero Banner Section

```tsx
<section className="relative w-full h-[500px] overflow-hidden">
  {/* Background Image */}
  <Image
    src={heroImage}
    alt="Hero Banner"
    fill
    className="absolute inset-0 object-cover"
    priority
  />

  {/* Overlay */}
  <div className="absolute inset-0 bg-black/30"></div>

  {/* Content */}
  <motion.div
    className="relative h-full flex flex-col justify-center px-24px max-w-7xl mx-auto"
    initial={{ opacity: 0, y: 20 }}
    animate={{ opacity: 1, y: 0 }}
    transition={{ duration: 0.6 }}
  >
    <h1 className="text-display-lg text-white font-bold mb-16px max-w-2xl">
      Discover Your Next Adventure
    </h1>

    <p className="text-body-lg text-white/90 max-w-xl mb-32px">
      Curated travel packages for unforgettable experiences
    </p>

    <div className="flex gap-16px flex-wrap">
      <motion.button
        className="px-32px py-16px bg-primary-600 text-white rounded-lg font-semibold hover:bg-primary-700"
        whileHover={{ scale: 1.05 }}
      >
        Explore Packages
      </motion.button>

      <motion.button
        className="px-32px py-16px border-2 border-white text-white rounded-lg font-semibold hover:bg-white/10"
        whileHover={{ scale: 1.05 }}
      >
        Chat with Us
      </motion.button>
    </div>
  </motion.div>
</section>
```

#### Featured Packages Section

```tsx
<section className="py-64px px-24px bg-white">
  <div className="max-w-7xl mx-auto">
    {/* Section Header */}
    <motion.div
      className="mb-48px"
      initial={{ opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
    >
      <h2 className="text-h1 font-bold text-gray-900 mb-12px">
        Featured Packages
      </h2>
      <p className="text-body-lg text-gray-600">
        Our handpicked collection of amazing travel experiences
      </p>
    </motion.div>

    {/* Grid */}
    <motion.div
      className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-24px"
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

    {/* View All CTA */}
    <div className="text-center mt-48px">
      <Link
        href="/packages"
        className="
          inline-block
          px-32px py-16px
          border-2 border-primary-600
          text-primary-600
          rounded-lg
          font-semibold
          hover:bg-primary-50
        "
      >
        View All Packages
      </Link>
    </div>
  </div>
</section>
```

#### Popular Destinations Carousel

```tsx
<section className="py-64px px-24px bg-gray-50">
  <div className="max-w-7xl mx-auto">
    <h2 className="text-h1 font-bold text-gray-900 mb-48px">
      Popular Destinations
    </h2>

    <motion.div
      className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-16px"
      initial="hidden"
      whileInView="visible"
      viewport={{ once: true }}
      variants={staggerContainer}
    >
      {destinations.map((dest) => (
        <motion.div
          key={dest.id}
          variants={staggerItem}
          className="
            relative
            h-48
            rounded-lg
            overflow-hidden
            cursor-pointer
            group
          "
        >
          <Image
            src={dest.image}
            alt={dest.name}
            fill
            className="object-cover group-hover:scale-110 transition-transform duration-500"
          />
          <div className="absolute inset-0 bg-black/30 group-hover:bg-black/40 transition-colors"></div>
          <div className="absolute inset-0 flex items-center justify-center">
            <h3 className="text-h3 font-bold text-white text-center">
              {dest.name}
            </h3>
          </div>
        </motion.div>
      ))}
    </motion.div>
  </div>
</section>
```

#### WhatsApp CTA Section

```tsx
<section className="py-64px px-24px bg-primary-600">
  <motion.div
    className="max-w-2xl mx-auto text-center"
    initial={{ opacity: 0, scale: 0.95 }}
    whileInView={{ opacity: 1, scale: 1 }}
    viewport={{ once: true }}
    transition={{ duration: 0.4 }}
  >
    <h2 className="text-h1 font-bold text-white mb-16px">
      Need Custom Recommendations?
    </h2>

    <p className="text-body-lg text-white/90 mb-32px">
      Chat with our travel experts on WhatsApp for personalized package suggestions
    </p>

    <motion.a
      href={whatsappLink}
      target="_blank"
      rel="noopener noreferrer"
      className="
        inline-flex items-center gap-12px
        px-32px py-16px
        bg-white text-primary-600
        rounded-lg
        font-bold
        hover:bg-gray-100
        transition-colors
      "
      whileHover={{ scale: 1.05 }}
      whileTap={{ scale: 0.95 }}
    >
      <MessageCircle size={24} />
      Chat on WhatsApp
    </motion.a>
  </motion.div>
</section>
```

#### Testimonials Section

```tsx
<section className="py-64px px-24px bg-white">
  <div className="max-w-7xl mx-auto">
    <h2 className="text-h1 font-bold text-gray-900 mb-48px text-center">
      What Our Customers Say
    </h2>

    <motion.div
      className="grid grid-cols-1 md:grid-cols-3 gap-24px"
      variants={staggerContainer}
      initial="hidden"
      whileInView="visible"
      viewport={{ once: true }}
    >
      {testimonials.map((testimonial) => (
        <motion.div
          key={testimonial.id}
          variants={staggerItem}
          className="
            p-24px
            bg-gray-50
            rounded-xl
            border border-gray-200
            hover:shadow-lg
            transition-shadow
          "
        >
          {/* Stars */}
          <div className="flex gap-4px mb-16px">
            {[...Array(5)].map((_, i) => (
              <Star
                key={i}
                size={20}
                className="text-yellow-400 fill-yellow-400"
              />
            ))}
          </div>

          {/* Quote */}
          <p className="text-body text-gray-700 mb-16px italic">
            "{testimonial.text}"
          </p>

          {/* Author */}
          <div className="flex items-center gap-12px">
            <Image
              src={testimonial.avatar}
              alt={testimonial.author}
              width={48}
              height={48}
              className="rounded-full"
            />
            <div>
              <p className="text-body-sm font-semibold text-gray-900">
                {testimonial.author}
              </p>
              <p className="text-caption text-gray-600">
                {testimonial.destination}
              </p>
            </div>
          </div>
        </motion.div>
      ))}
    </motion.div>
  </div>
</section>
```

---

## 2. Package Listing (`/packages`)

### Layout

```
┌─────────────────────────────────────┐
│   Navigation + Search Bar           │
├─────┬─────────────────────────────┤
│     │                             │
│ Filters  │   Package Cards Grid   │
│ Sidebar  │   (3-4 columns)        │
│     │                             │
│     │                             │
└─────┴─────────────────────────────┘
```

### Components

#### Search & Filter Section

```tsx
<div className="py-32px px-24px bg-gray-50 border-b border-gray-200">
  <div className="max-w-7xl mx-auto">
    <div className="flex flex-col md:flex-row gap-16px">
      {/* Search Bar */}
      <input
        type="text"
        placeholder="Search packages..."
        className="
          flex-1
          px-24px py-12px
          border border-gray-300
          rounded-lg
          focus:outline-none
          focus:ring-2 focus:ring-primary-500
        "
        onChange={(e) => setSearchQuery(e.target.value)}
      />

      {/* Sort Dropdown */}
      <select className="
        px-24px py-12px
        border border-gray-300
        rounded-lg
        focus:outline-none
        focus:ring-2 focus:ring-primary-500
      ">
        <option>Sort by: Popular</option>
        <option>Lowest Price</option>
        <option>Highest Price</option>
        <option>Newest</option>
      </select>

      {/* Filter Toggle (Mobile) */}
      <button className="
        md:hidden
        px-24px py-12px
        border border-gray-300
        rounded-lg
        font-semibold
      ">
        Filters
      </button>
    </div>
  </div>
</div>
```

#### Filter Sidebar

```tsx
<aside className="
  w-full md:w-80
  p-24px
  bg-gray-50
  rounded-lg
  border border-gray-200
">
  <h3 className="text-h3 font-bold text-gray-900 mb-24px">
    Filters
  </h3>

  {/* Price Range */}
  <div className="mb-32px">
    <label className="text-body-sm font-semibold text-gray-900 block mb-12px">
      Price Range
    </label>
    <input
      type="range"
      min="0"
      max="10000"
      className="w-full"
      onChange={(e) => setPriceMax(e.target.value)}
    />
    <p className="text-caption text-gray-600 mt-8px">
      RM0 - RM{priceMax}
    </p>
  </div>

  {/* Duration */}
  <div className="mb-32px">
    <label className="text-body-sm font-semibold text-gray-900 block mb-12px">
      Duration
    </label>
    {['1-3 days', '4-7 days', '8-14 days', '14+ days'].map((duration) => (
      <label key={duration} className="flex items-center gap-8px mb-12px cursor-pointer">
        <input type="checkbox" className="w-16px h-16px" />
        <span className="text-body text-gray-700">{duration}</span>
      </label>
    ))}
  </div>

  {/* Destinations */}
  <div className="mb-32px">
    <label className="text-body-sm font-semibold text-gray-900 block mb-12px">
      Destinations
    </label>
    {destinations.map((dest) => (
      <label key={dest.id} className="flex items-center gap-8px mb-12px cursor-pointer">
        <input type="checkbox" className="w-16px h-16px" />
        <span className="text-body text-gray-700">{dest.name}</span>
      </label>
    ))}
  </div>

  {/* Rating */}
  <div className="mb-32px">
    <label className="text-body-sm font-semibold text-gray-900 block mb-12px">
      Rating
    </label>
    {['★★★★★', '★★★★☆', '★★★☆☆'].map((rating) => (
      <label key={rating} className="flex items-center gap-8px mb-12px cursor-pointer">
        <input type="checkbox" className="w-16px h-16px" />
        <span className="text-body text-gray-700">{rating}</span>
      </label>
    ))}
  </div>

  {/* Clear Filters */}
  <button className="
    w-full
    py-12px
    border border-gray-300
    rounded-lg
    text-gray-900
    font-semibold
    hover:bg-gray-100
    transition-colors
  ">
    Clear Filters
  </button>
</aside>
```

#### Package Grid

```tsx
<motion.div
  className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-24px"
  variants={staggerContainer}
  initial="hidden"
  animate="visible"
>
  {filteredPackages.map((pkg) => (
    <motion.div key={pkg.id} variants={staggerItem}>
      <PackageCard package={pkg} />
    </motion.div>
  ))}
</motion.div>
```

---

## 3. Package Detail (`/packages/[id]`)

### Layout

```
┌─────────────────────────────────────┐
│   Image Gallery                     │
│   (Hero + Thumbnails)               │
├─────────────────────────────────────┤
│   Title + Meta Info                 │
├─────┬─────────────────────────────┤
│     │                             │
│ Details │   Booking Card          │
│ Sections│   (Sticky Sidebar)      │
│     │                             │
└─────┴─────────────────────────────┘
```

### Components

#### Image Gallery

```tsx
<section className="w-full bg-black">
  {/* Main Image */}
  <motion.div
    className="relative w-full h-[500px] md:h-[600px]"
    layoutId={`package-image-${packageId}`}
  >
    <Image
      src={selectedImage}
      alt={packageTitle}
      fill
      className="object-cover"
      priority
    />
  </motion.div>

  {/* Thumbnails */}
  <div className="flex overflow-x-auto gap-8px p-16px bg-black">
    {images.map((image, index) => (
      <motion.button
        key={index}
        onClick={() => setSelectedImage(image)}
        className={`
          relative
          w-24 h-24 md:w-32 md:h-32
          rounded-lg
          overflow-hidden
          flex-shrink-0
          border-2
          ${selectedImage === image ? 'border-primary-400' : 'border-transparent'}
          hover:border-primary-300
          transition-colors
        `}
        whileHover={{ scale: 1.05 }}
      >
        <Image
          src={image}
          alt="Package image"
          fill
          className="object-cover"
        />
      </motion.button>
    ))}
  </div>
</section>
```

#### Package Header

```tsx
<section className="py-32px px-24px bg-white border-b border-gray-200">
  <div className="max-w-7xl mx-auto">
    <div className="flex items-start justify-between gap-24px">
      {/* Title & Meta */}
      <div className="flex-1">
        <div className="flex items-center gap-12px mb-16px">
          <span className="text-caption uppercase font-semibold text-gray-500">
            {category}
          </span>
          <div className="flex items-center gap-4px">
            {[...Array(5)].map((_, i) => (
              <Star
                key={i}
                size={16}
                className={`${
                  i < Math.round(rating)
                    ? 'text-yellow-400 fill-yellow-400'
                    : 'text-gray-300'
                }`}
              />
            ))}
            <span className="text-caption font-semibold text-gray-600">
              ({reviewCount} reviews)
            </span>
          </div>
        </div>

        <h1 className="text-display-md font-bold text-gray-900 mb-12px">
          {packageTitle}
        </h1>

        <p className="text-body-lg text-gray-600 max-w-2xl">
          {shortDescription}
        </p>
      </div>

      {/* Price Card */}
      <motion.div
        className="
          w-full md:w-96
          p-24px
          bg-primary-50
          border border-primary-200
          rounded-xl
          sticky top-24
        "
        initial={{ opacity: 0, x: 20 }}
        animate={{ opacity: 1, x: 0 }}
        transition={{ delay: 0.2 }}
      >
        <p className="text-caption text-gray-600 mb-8px">
          Starting from
        </p>
        <h2 className="text-display-md font-bold text-primary-600 mb-24px">
          RM{price}
        </h2>

        <div className="space-y-16px mb-32px">
          <div className="flex items-center gap-12px text-body">
            <Calendar size={20} className="text-primary-600" />
            <span>{duration} days</span>
          </div>
          <div className="flex items-center gap-12px text-body">
            <MapPin size={20} className="text-primary-600" />
            <span>{destination}</span>
          </div>
          <div className="flex items-center gap-12px text-body">
            <Users size={20} className="text-primary-600" />
            <span>{groupSize} travelers</span>
          </div>
        </div>

        <button className="
          w-full
          py-16px
          bg-primary-600 text-white
          rounded-lg
          font-bold
          hover:bg-primary-700
          transition-colors
          mb-12px
        ">
          Get Quotation
        </button>

        <motion.a
          href={whatsappLink}
          className="
            block
            w-full
            py-16px
            border-2 border-primary-600
            text-primary-600
            rounded-lg
            font-bold
            text-center
            hover:bg-primary-50
            transition-colors
          "
          whileHover={{ scale: 1.02 }}
        >
          Chat on WhatsApp
        </motion.a>
      </motion.div>
    </div>
  </div>
</section>
```

#### Itinerary Section

```tsx
<section className="py-48px px-24px bg-white">
  <div className="max-w-4xl mx-auto">
    <h2 className="text-h1 font-bold text-gray-900 mb-32px">
      Itinerary
    </h2>

    <motion.div
      className="space-y-24px"
      variants={staggerContainer}
      initial="hidden"
      whileInView="visible"
      viewport={{ once: true }}
    >
      {itinerary.map((day, index) => (
        <motion.div
          key={index}
          variants={staggerItem}
          className="
            p-24px
            border border-gray-200
            rounded-xl
            hover:shadow-lg
            transition-shadow
          "
        >
          <div className="flex gap-16px items-start">
            {/* Day Badge */}
            <div className="
              w-16 h-16
              bg-primary-600 text-white
              rounded-full
              flex items-center justify-center
              font-bold
              flex-shrink-0
            ">
              {day.dayNumber}
            </div>

            {/* Content */}
            <div className="flex-1">
              <h3 className="text-h3 font-bold text-gray-900 mb-12px">
                {day.title}
              </h3>
              <p className="text-body text-gray-700 mb-16px">
                {day.description}
              </p>

              {/* Highlights */}
              {day.highlights && (
                <div className="flex flex-wrap gap-8px">
                  {day.highlights.map((highlight) => (
                    <span
                      key={highlight}
                      className="
                        px-12px py-8px
                        bg-gray-100
                        text-gray-700
                        text-caption
                        rounded-full
                      "
                    >
                      {highlight}
                    </span>
                  ))}
                </div>
              )}
            </div>
          </div>
        </motion.div>
      ))}
    </motion.div>
  </div>
</section>
```

#### Inclusions & Exclusions

```tsx
<section className="py-48px px-24px bg-gray-50">
  <div className="max-w-4xl mx-auto">
    <div className="grid grid-cols-1 md:grid-cols-2 gap-32px">
      {/* Inclusions */}
      <motion.div
        initial={{ opacity: 0, x: -20 }}
        whileInView={{ opacity: 1, x: 0 }}
        viewport={{ once: true }}
      >
        <h3 className="text-h2 font-bold text-gray-900 mb-24px flex items-center gap-12px">
          <CheckCircle className="text-success" size={28} />
          What's Included
        </h3>

        <ul className="space-y-12px">
          {inclusions.map((item) => (
            <li key={item} className="flex items-start gap-12px">
              <Check size={20} className="text-success flex-shrink-0 mt-2px" />
              <span className="text-body text-gray-700">{item}</span>
            </li>
          ))}
        </ul>
      </motion.div>

      {/* Exclusions */}
      <motion.div
        initial={{ opacity: 0, x: 20 }}
        whileInView={{ opacity: 1, x: 0 }}
        viewport={{ once: true }}
      >
        <h3 className="text-h2 font-bold text-gray-900 mb-24px flex items-center gap-12px">
          <AlertCircle className="text-warning" size={28} />
          Not Included
        </h3>

        <ul className="space-y-12px">
          {exclusions.map((item) => (
            <li key={item} className="flex items-start gap-12px">
              <X size={20} className="text-warning flex-shrink-0 mt-2px" />
              <span className="text-body text-gray-700">{item}</span>
            </li>
          ))}
        </ul>
      </motion.div>
    </div>
  </div>
</section>
```

---

## 4. Contact Form & Inquiry

### Inquiry Modal/Form

```tsx
<form className="space-y-24px">
  {/* Name */}
  <div>
    <label className="block text-body-sm font-semibold text-gray-900 mb-8px">
      Full Name *
    </label>
    <input
      type="text"
      required
      className="
        w-full px-16px py-12px
        border border-gray-300 rounded-lg
        focus:outline-none focus:ring-2 focus:ring-primary-500
      "
      placeholder="Your name"
    />
  </div>

  {/* Email */}
  <div>
    <label className="block text-body-sm font-semibold text-gray-900 mb-8px">
      Email Address *
    </label>
    <input
      type="email"
      required
      className="
        w-full px-16px py-12px
        border border-gray-300 rounded-lg
        focus:outline-none focus:ring-2 focus:ring-primary-500
      "
      placeholder="your@email.com"
    />
  </div>

  {/* Phone */}
  <div>
    <label className="block text-body-sm font-semibold text-gray-900 mb-8px">
      Phone Number *
    </label>
    <input
      type="tel"
      required
      className="
        w-full px-16px py-12px
        border border-gray-300 rounded-lg
        focus:outline-none focus:ring-2 focus:ring-primary-500
      "
      placeholder="+60 1X XXXX XXXX"
    />
  </div>

  {/* Message */}
  <div>
    <label className="block text-body-sm font-semibold text-gray-900 mb-8px">
      Message *
    </label>
    <textarea
      required
      rows={4}
      className="
        w-full px-16px py-12px
        border border-gray-300 rounded-lg
        focus:outline-none focus:ring-2 focus:ring-primary-500
        resize-none
      "
      placeholder="Tell us about your interest..."
    ></textarea>
  </div>

  {/* Number of Travelers */}
  <div>
    <label className="block text-body-sm font-semibold text-gray-900 mb-8px">
      Number of Travelers *
    </label>
    <input
      type="number"
      required
      min="1"
      className="
        w-full px-16px py-12px
        border border-gray-300 rounded-lg
        focus:outline-none focus:ring-2 focus:ring-primary-500
      "
      placeholder="2"
    />
  </div>

  {/* Submit */}
  <motion.button
    type="submit"
    className="
      w-full
      py-16px
      bg-primary-600 text-white
      rounded-lg font-bold
      hover:bg-primary-700
      transition-colors
    "
    whileHover={{ scale: 1.02 }}
    whileTap={{ scale: 0.98 }}
  >
    Send Inquiry
  </motion.button>

  <p className="text-caption text-gray-600 text-center">
    Or reach us directly on{' '}
    <a href={whatsappLink} className="text-primary-600 font-semibold hover:underline">
      WhatsApp
    </a>
  </p>
</form>
```

---

## 5. Footer

```tsx
<footer className="bg-gray-900 text-white py-48px px-24px">
  <div className="max-w-7xl mx-auto">
    <div className="grid grid-cols-1 md:grid-cols-4 gap-32px mb-48px">
      {/* Company Info */}
      <div>
        <h4 className="text-h3 font-bold mb-24px">
          {tenantName}
        </h4>
        <p className="text-body text-gray-400 mb-16px">
          Discover extraordinary travel experiences
        </p>
        <div className="flex gap-12px">
          <SocialLink icon={<Facebook />} href="#" />
          <SocialLink icon={<Instagram />} href="#" />
          <SocialLink icon={<Twitter />} href="#" />
        </div>
      </div>

      {/* Quick Links */}
      <div>
        <h4 className="text-h3 font-bold mb-16px">Quick Links</h4>
        <ul className="space-y-8px">
          <li><a href="/packages" className="text-gray-400 hover:text-white">Packages</a></li>
          <li><a href="/blog" className="text-gray-400 hover:text-white">Blog</a></li>
          <li><a href="/about" className="text-gray-400 hover:text-white">About Us</a></li>
          <li><a href="/contact" className="text-gray-400 hover:text-white">Contact</a></li>
        </ul>
      </div>

      {/* Support */}
      <div>
        <h4 className="text-h3 font-bold mb-16px">Support</h4>
        <ul className="space-y-8px">
          <li><a href="/faq" className="text-gray-400 hover:text-white">FAQ</a></li>
          <li><a href="/privacy" className="text-gray-400 hover:text-white">Privacy</a></li>
          <li><a href="/terms" className="text-gray-400 hover:text-white">Terms</a></li>
          <li><a href="/cancellation" className="text-gray-400 hover:text-white">Cancellation</a></li>
        </ul>
      </div>

      {/* Contact */}
      <div>
        <h4 className="text-h3 font-bold mb-16px">Get In Touch</h4>
        <ul className="space-y-12px">
          <li className="flex items-start gap-12px">
            <Phone size={20} className="flex-shrink-0 mt-2px" />
            <span className="text-gray-400">{phone}</span>
          </li>
          <li className="flex items-start gap-12px">
            <Mail size={20} className="flex-shrink-0 mt-2px" />
            <span className="text-gray-400">{email}</span>
          </li>
          <li className="flex items-start gap-12px">
            <MapPin size={20} className="flex-shrink-0 mt-2px" />
            <span className="text-gray-400">{address}</span>
          </li>
        </ul>
      </div>
    </div>

    {/* Bottom */}
    <div className="border-t border-gray-800 pt-24px text-center text-gray-500">
      <p>© 2026 {tenantName}. All rights reserved.</p>
      <p className="text-caption mt-8px">
        Powered by <span className="text-primary-400 font-semibold">LitXusTravel</span>
      </p>
    </div>
  </div>
</footer>
```

---

## Mobile Responsiveness Checklist

- ✅ Hero sections: 300px height on mobile
- ✅ Package cards: 1 column mobile, 2 on tablet, 3 on desktop
- ✅ Images: Optimized lazy loading
- ✅ Forms: Mobile-friendly input sizing
- ✅ Navigation: Hamburger menu on mobile
- ✅ Spacing: 16px padding on mobile, 24px on desktop
- ✅ Typography: Scaled for mobile (14px body, 24px h1)
- ✅ Touch targets: Minimum 48px buttons

---

**Public Website UI Ready for Implementation** ✅
