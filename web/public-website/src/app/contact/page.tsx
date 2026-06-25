import { Metadata } from "next"
import { Mail, Phone, MapPin, MessageCircle } from "lucide-react"
import Navbar from "@/components/layout/Navbar"
import Footer from "@/components/layout/Footer"
import InquiryForm from "@/components/forms/InquiryForm"

export const metadata: Metadata = {
  title: "Contact Us — LitXusTravel",
  description: "Get in touch with our travel experts for personalised package recommendations.",
}

const CONTACT_INFO = [
  { icon: Phone,          label: "Phone",    value: "+60 12-345 6789" },
  { icon: Mail,           label: "Email",    value: "hello@litxustravel.com" },
  { icon: MapPin,         label: "Address",  value: "Kuala Lumpur, Malaysia" },
  { icon: MessageCircle,  label: "WhatsApp", value: "+60 12-345 6789" },
]

export default function ContactPage() {
  return (
    <>
      <Navbar />
      <main className="flex-1 bg-gray-50">
        {/* Header */}
        <section className="bg-(--color-brand-blue) py-16 px-4 sm:px-6 text-center">
          <h1 className="text-3xl sm:text-4xl font-bold text-white mb-3">Contact Us</h1>
          <p className="text-white/80 text-lg max-w-xl mx-auto">
            Our travel experts are ready to help you plan your perfect trip.
          </p>
        </section>

        <section className="max-w-6xl mx-auto px-4 sm:px-6 py-16">
          <div className="grid grid-cols-1 lg:grid-cols-5 gap-12">
            {/* Contact info */}
            <div className="lg:col-span-2 space-y-6">
              <div>
                <h2 className="text-2xl font-bold mb-2">Get In Touch</h2>
                <p className="text-muted-foreground text-sm leading-relaxed">
                  Fill in the form or reach out directly via WhatsApp for the fastest response.
                </p>
              </div>

              <div className="space-y-4">
                {CONTACT_INFO.map(({ icon: Icon, label, value }) => (
                  <div key={label} className="flex items-start gap-4">
                    <div className="w-10 h-10 rounded-lg bg-blue-100 flex items-center justify-center shrink-0">
                      <Icon size={18} className="text-(--color-brand-blue)" />
                    </div>
                    <div>
                      <p className="text-xs font-semibold text-muted-foreground uppercase">{label}</p>
                      <p className="text-sm font-medium text-foreground">{value}</p>
                    </div>
                  </div>
                ))}
              </div>

              <a
                href={`https://wa.me/${process.env.NEXT_PUBLIC_WHATSAPP_NUMBER ?? "601234567890"}?text=Hi%2C%20I%27d%20like%20to%20enquire%20about%20travel%20packages`}
                target="_blank"
                rel="noopener noreferrer"
                className="inline-flex items-center gap-2 bg-(--color-brand-success) hover:bg-green-600 text-white font-bold px-6 py-3 rounded-xl transition-colors text-sm"
              >
                <MessageCircle size={18} />
                Chat on WhatsApp
              </a>
            </div>

            {/* Form */}
            <div className="lg:col-span-3 bg-white rounded-2xl border border-border shadow-sm p-8">
              <h3 className="text-xl font-bold mb-6">Send Us a Message</h3>
              <InquiryForm />
            </div>
          </div>
        </section>
      </main>
      <Footer />
    </>
  )
}
