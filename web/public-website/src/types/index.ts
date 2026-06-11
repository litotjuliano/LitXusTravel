export interface Package {
  id: string
  /** AES-256 encrypted URL-safe slug — computed server-side, never stored */
  slug?: string
  title: string
  description?: string
  shortDescription?: string
  category?: string
  price: number
  currency: string
  durationDays: number
  destination: string
  region?: string
  featuredImageUrl?: string
  imagesJson?: string
  itineraryJson?: string
  highlightsJson?: string
  inclusionsJson?: string
  exclusionsJson?: string
  contactPhone?: string
  contactWhatsapp?: string
  rating?: number
  reviewCount: number
  isPopular: boolean
  isFeatured: boolean
  isCustomized: boolean
  lastSyncedAt: string
  masterPackageId: string
}

export interface ItineraryDay {
  dayNumber: number
  title: string
  description: string
  highlights?: string[]
}

export interface Testimonial {
  id: string
  author: string
  avatar?: string
  text: string
  destination: string
  rating: number
}

export interface InquiryFormData {
  customerName: string
  customerEmail: string
  customerPhone: string
  message: string
  numberOfPax?: number
  preferredTravelDates?: string
  masterPackageId?: string
}
