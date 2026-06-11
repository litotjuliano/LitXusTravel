# LitXusTravel UAT List — v1
<!-- STATUS: SENT — 2026-06-01 | Awaiting: uat/incoming/test-report-v1.md -->

## Info
- **Version:** v1
- **Date:** 2026-06-01
- **Prepared By:** LitXusTravel Team
- **Send To:** LitXusDevHub

---

## Changes to Test

| # | Feature / Module | Change Description | Priority |
|---|------------------|--------------------|----------|
| 1 | Booking Form | Fixed booking form validation error | High |
| 2 | Payment / Checkout | Added new payment gateway (FPX) | High |
| 3 | Travel Packages | Updated travel package pricing display | Medium |

---

## Test Scenarios

### Feature 1: Booking Form Validation Fix
- **Steps:**
  1. Open the booking form and submit with empty/invalid fields — verify validation messages appear.
  2. Fill all fields correctly and submit — verify submission succeeds without errors.
  3. Test edge cases: special characters in name fields, past dates, invalid email formats.
- **Expected Result:** Validation errors shown only for invalid inputs; valid submissions process without errors.
- **Test Data Required:** Sample booking details with both valid and invalid field inputs.

---

### Feature 2: New Payment Gateway (FPX)
- **Steps:**
  1. Add a travel package to cart and proceed to checkout.
  2. Verify FPX is listed alongside existing payment methods.
  3. Select FPX and complete a test transaction — verify booking confirmation is created.
  4. Test cancelled/declined FPX — verify graceful error handling and no duplicate bookings.
- **Expected Result:** FPX option visible, successful transactions create bookings, failures handled gracefully.
- **Test Data Required:** FPX sandbox/test credentials; test travel package.

---

### Feature 3: Travel Package Pricing Display Update
- **Steps:**
  1. Go to the travel packages listing page — verify all prices display correctly.
  2. Open individual package detail pages — verify price matches listing and is correctly formatted.
  3. Check currency symbol, decimal formatting, and any promotional/discounted pricing.
  4. Check display on mobile viewport for layout issues.
- **Expected Result:** Prices are accurate, consistently formatted, and display correctly on all screen sizes.
- **Test Data Required:** Packages with standard pricing and at least one with a promotional/discounted price.

---

## Notes to DevHub
- Feature 2 (FPX) is highest risk — prioritise sandbox end-to-end flow.
- Flag any regression issues found outside these 3 changes.
