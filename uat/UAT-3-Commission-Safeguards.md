# UAT Test Cases: Commission Safeguards (Fraud Prevention)

**Document Version:** 1.0  
**Date:** 2026-06-10  
**Prepared for:** LitXusDevHub QA Team

---

## Test Scope

This document covers UAT for the 10 critical commission safeguards:
1. Refund/Cancellation Reversal
2. Self-Booking Prevention
3. Code Sharing Prevention
4. Tiered Commission Gaming
5. Markup Price Validation
6. Duplicate Booking Detection
7. Departing Staff Policy
8. Refund Reversal Tracking
9. Tiered Commission Caps
10. Dispute Resolution

---

## Test Data Reference

**Scenario Setup:**
- Tenant A: Travel Pro Inc
- John (15% commission), Jane (8%), Mike (10%, departing)
- Tours: Beach \$500, Mountain \$800, City \$300

---

## SAFEGUARD 1: Refund/Cancellation Reversal

### TEST CASE 3.1.1: Commission Reversal on Cancelled Booking

**Objective:** Verify commission is reversed when booking cancelled before tour completion

**Test Data:**
```
Booking: BOOKING-004
Tour: Beach Resort (\$500)
Agent: John (15%)
Status: Cancelled
Expected Commission Reversal: \$75
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Create booking with John's code | Booking status = "Confirmed" | ☐ Pass ☐ Fail |
| 2 | Check commission accrual | Status = "Accrued", Amount = \$75 | ☐ Pass ☐ Fail |
| 3 | Customer cancels booking | Booking status = "Cancelled" | ☐ Pass ☐ Fail |
| 4 | Check commission status | Status changed to "Reversed" | ☐ Pass ☐ Fail |
| 5 | Check commission amount | Amount = -\$75 (reversed) | ☐ Pass ☐ Fail |
| 6 | Query payout | Reversed commission excluded | ☐ Pass ☐ Fail |

**Expected Result:** Commission automatically reversed, not included in payout

---

### TEST CASE 3.1.2: Commission Finalized Only on Tour Completion

**Objective:** Verify commission doesn't finalize until tour actually completes

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Create booking | Status = "Accrued" | ☐ Pass ☐ Fail |
| 2 | Tour date: June 12 | Booking still "Accrued" | ☐ Pass ☐ Fail |
| 3 | Travel forward to June 12 | Commission status still "Accrued" | ☐ Pass ☐ Fail |
| 4 | Mark tour "Completed" | Commission status → "Finalized" | ☐ Pass ☐ Fail |
| 5 | Check monthly payout | Commission now included | ☐ Pass ☐ Fail |

**Expected Result:** Commission only finalizes on actual tour completion

---

### TEST CASE 3.1.3: Refund After Payout Deducts from Next Month

**Objective:** Verify refunds after payment deduct from next payout

**Precondition:** Commission already paid in June payout

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Commission paid in June payout | Status = "Paid" | ☐ Pass ☐ Fail |
| 2 | Customer requests refund (July) | Refund created | ☐ Pass ☐ Fail |
| 3 | System detects paid commission | Creates reverse accrual | ☐ Pass ☐ Fail |
| 4 | Check July payout calculation | Reversal applied (-\$75) | ☐ Pass ☐ Fail |
| 5 | Agent notified of reversal | Notification sent | ☐ Pass ☐ Fail |

**Expected Result:** Refund adjusts agent's next payout

---

## SAFEGUARD 2: Self-Booking Prevention

### TEST CASE 3.2.1: Staff Cannot Use Own Code

**Objective:** Verify staff cannot book tours using their own referral code

**Precondition:**
- John (STAFF-JOHN-001) logged in

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | John creates booking | Option to add referral code shown | ☐ Pass ☐ Fail |
| 2 | John enters code "STAFF-JOHN-001" | Validation triggers | ☐ Pass ☐ Fail |
| 3 | System checks: StaffId == CodeOwner | Match detected | ☐ Pass ☐ Fail |
| 4 | Booking rejected | Error: "Cannot use own referral code" | ☐ Pass ☐ Fail |
| 5 | Audit logged | Attempted self-booking recorded | ☐ Pass ☐ Fail |
| 6 | Tenant Admin alerted | Notification sent (optional) | ☐ Pass ☐ Fail |

**Expected Result:** Self-booking blocked at system level

---

### TEST CASE 3.2.2: Self-Booking Attempt Tracked

**Objective:** Verify self-booking attempts are audited

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | John attempts self-booking | Blocked as in 3.2.1 | ☐ Pass ☐ Fail |
| 2 | Check audit log | Event logged with timestamp | ☐ Pass ☐ Fail |
| 3 | Check alert system | Tenant notified (if enabled) | ☐ Pass ☐ Fail |
| 4 | Pattern detection | Multiple attempts flagged | ☐ Pass ☐ Fail |

**Expected Result:** All attempts tracked and alertable

---

## SAFEGUARD 3: Code Sharing Prevention

### TEST CASE 3.3.1: Code Usage Audit Trail

**Objective:** Verify code usage is logged with IP/location tracking

**Test Data:**
```
John's Code: STAFF-JOHN-001
Issued from: IP 192.168.1.100 (Office)
Customer booking from: IP 203.0.113.1 (Different country)
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Customer enters code "STAFF-JOHN-001" | Booking created | ☐ Pass ☐ Fail |
| 2 | Check code usage audit | Entry created with metadata | ☐ Pass ☐ Fail |
| 3 | Verify IP logged | Customer's IP recorded | ☐ Pass ☐ Fail |
| 4 | Verify location logged | Geolocation determined | ☐ Pass ☐ Fail |
| 5 | Verify time logged | Timestamp recorded | ☐ Pass ☐ Fail |

**Expected Result:** Complete code usage tracking for anomaly detection

---

### TEST CASE 3.3.2: Anomaly Detection - Geographic Impossibility

**Objective:** Alert if code used from impossible locations

**Test Data:**
```
Time 1: Code used from USA (IP 203.0.113.1)
Time 2: Code used from Japan 30 seconds later (impossible)
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Code used in USA | Booking 1 created | ☐ Pass ☐ Fail |
| 2 | Code used in Japan 30 sec later | System detects impossibility | ☐ Pass ☐ Fail |
| 3 | Alert generated | Anomaly flag set | ☐ Pass ☐ Fail |
| 4 | Tenant Admin notified | Alert sent to admin dashboard | ☐ Pass ☐ Fail |

**Expected Result:** Impossible geolocation transitions flagged

---

### TEST CASE 3.3.3: High Volume Usage Alert

**Objective:** Alert if code used abnormally high number of times

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Code used 100 times in one day | Normal bookings created | ☐ Pass ☐ Fail |
| 2 | System detects unusual pattern | Anomaly threshold exceeded | ☐ Pass ☐ Fail |
| 3 | Alert generated | System flags for review | ☐ Pass ☐ Fail |
| 4 | Tenant notified | Alert sent | ☐ Pass ☐ Fail |

**Expected Result:** Unusual volume patterns detected and alerted

---

## SAFEGUARD 4: Tiered Commission Gaming

### TEST CASE 3.4.1: Only Completed Bookings Count Toward Tier

**Objective:** Verify cancelled/pending bookings don't affect tier calculation

**Test Data:**
```
Current Month: 99 completed bookings (99%)
Also has: 10 pending, 5 cancelled
Tier Threshold: 100 for 20%
Question: Will tier be 15% or 20%?
Answer: 15% (only 99 completed count)
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Agent has 99 COMPLETED bookings | Tier = 15% | ☐ Pass ☐ Fail |
| 2 | Agent also has 10 pending bookings | Pending NOT counted | ☐ Pass ☐ Fail |
| 3 | Agent also has 5 cancelled | Cancelled NOT counted | ☐ Pass ☐ Fail |
| 4 | Commission calculated | Uses 15% rate (not 20%) | ☐ Pass ☐ Fail |
| 5 | 10 pending complete | Tier → 20% starting next booking | ☐ Pass ☐ Fail |

**Expected Result:** Only finalized bookings count toward tier

---

### TEST CASE 3.4.2: Minimum Booking Value Threshold

**Objective:** Verify small bookings don't artificially inflate count

**Test Data:**
```
Threshold: Minimum \$50 per booking
Tier 2: 101-500 bookings at 20%
Agent: 99 real bookings + 100 fake \$5 bookings
Expected tier: 15% (only 99 real ones count)
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Agent completes 99 real bookings (\$500 avg) | Counted | ☐ Pass ☐ Fail |
| 2 | Agent creates 100 tiny bookings (\$5 each) | Created | ☐ Pass ☐ Fail |
| 3 | Tier calculation starts | System filters by minimum \$50 | ☐ Pass ☐ Fail |
| 4 | Tier result: 15% (not 20%) | Correct, only 99 counted | ☐ Pass ☐ Fail |

**Expected Result:** Small bookings below threshold excluded from tier calculation

---

### TEST CASE 3.4.3: Monthly Tier Reset

**Objective:** Verify tier counter resets monthly

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | June: Agent has 150 completed bookings | Tier = 20% | ☐ Pass ☐ Fail |
| 2 | June 30: Payout calculated at 20% | Correct | ☐ Pass ☐ Fail |
| 3 | July 1: Month boundary | Tier counter resets | ☐ Pass ☐ Fail |
| 4 | July: Agent at 10 completed bookings | Tier = 15% | ☐ Pass ☐ Fail |

**Expected Result:** Tier resets every month

---

## SAFEGUARD 5: Markup Price Validation

### TEST CASE 3.5.1: Markup Amount Cap

**Objective:** Verify markup cannot exceed configured maximum

**Test Data:**
```
Tour Base Price: \$500
Max Markup Amount: \$50
Max Markup %: 10%
Agent attempts markup: \$100 (exceeds \$50 limit)
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Agent sets markup to \$50 | Accepted (equals limit) | ☐ Pass ☐ Fail |
| 2 | Agent sets markup to \$51 | Rejected: exceeds \$50 limit | ☐ Pass ☐ Fail |
| 3 | Agent sets markup to 10% | Accepted (\$50, equals limit) | ☐ Pass ☐ Fail |
| 4 | Agent sets markup to 11% | Rejected: exceeds 10% limit | ☐ Pass ☐ Fail |
| 5 | Error message clear | User understands why rejected | ☐ Pass ☐ Fail |

**Expected Result:** Both caps enforced, whichever is lower

---

### TEST CASE 3.5.2: Customer Sees Original + Markup Breakdown

**Objective:** Verify transparency in pricing to customer

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Customer views tour on agent site | Pricing shown as: | ☐ Pass ☐ Fail |
| 2 | Breakdown displayed | "Original: \$500 + Agent markup: \$50 = \$550" | ☐ Pass ☐ Fail |
| 3 | Customer understands split | Clear that \$500 goes to tenant | ☐ Pass ☐ Fail |

**Expected Result:** Full price transparency for customer

---

## SAFEGUARD 6: Duplicate Booking Detection

### TEST CASE 3.6.1: Prevent Same Customer, Tour, Date

**Objective:** Verify system prevents duplicate bookings

**Test Data:**
```
Customer: CUST-001
Tour: Beach Resort
Date: June 15, 2026
Status 1: Confirmed (first booking)
Status 2: Try to book again - should reject
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Customer books Beach Resort for June 15 | Booking created | ☐ Pass ☐ Fail |
| 2 | Customer tries to book same again | System detects duplicate | ☐ Pass ☐ Fail |
| 3 | Booking rejected | Error: "You already have this booking" | ☐ Pass ☐ Fail |
| 4 | Suggestion provided | System offers different date | ☐ Pass ☐ Fail |

**Expected Result:** Duplicate bookings prevented with user-friendly message

---

### TEST CASE 3.6.2: Allow Different Dates

**Objective:** Verify customer can book same tour on different dates

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Customer books Beach Resort for June 15 | Success | ☐ Pass ☐ Fail |
| 2 | Customer books Beach Resort for July 15 | Success | ☐ Pass ☐ Fail |
| 3 | Both bookings exist | No conflict | ☐ Pass ☐ Fail |

**Expected Result:** Same tour, different dates allowed

---

## SAFEGUARD 7: Departing Staff Policy

### TEST CASE 3.7.1: Commission Locked When Staff Leaves

**Objective:** Verify commission handling for departing staff

**Precondition:** Mike (departing June 30) has pending commission

**Test Data:**
```
Mike's Pending Commission: \$500 (not yet paid)
Mike's Last Day: June 30, 2026
Tenant Policy: Auto-pay departing staff
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Mike's status: Active (June 29) | Eligible for payout | ☐ Pass ☐ Fail |
| 2 | Mike's departure date set: June 30 | Status: "Departing" | ☐ Pass ☐ Fail |
| 3 | Tenant configures policy | Select "Auto-pay" | ☐ Pass ☐ Fail |
| 4 | June payout cycle runs | Mike included in payout | ☐ Pass ☐ Fail |
| 5 | Commission paid: \$500 | Mike receives full amount | ☐ Pass ☐ Fail |
| 6 | July payout cycle runs | Mike NOT included | ☐ Pass ☐ Fail |

**Expected Result:** Departing staff receive final payout based on policy

---

### TEST CASE 3.7.2: Departing Staff Options

**Objective:** Verify tenant can choose departing staff policy

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Navigate to: Staff Settings | Policy options shown | ☐ Pass ☐ Fail |
| 2 | Option A: "Auto-pay" | Pay all pending commission | ☐ Pass ☐ Fail |
| 3 | Option B: "Escrow (30 days)" | Hold for 30 days after departure | ☐ Pass ☐ Fail |
| 4 | Option C: "Forfeit" | Don't pay pending commission | ☐ Pass ☐ Fail |
| 5 | Save selection | Policy applied | ☐ Pass ☐ Fail |

**Expected Result:** Tenant controls departing staff commission policy

---

## SAFEGUARD 8: Refund Reversal Tracking

### TEST CASE 3.8.1: Audit Trail Shows Commission Journey

**Objective:** Verify full audit trail of commission changes

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | View commission detail page | Full history shown | ☐ Pass ☐ Fail |
| 2 | Entry 1: "Created" | Date: Booking date | ☐ Pass ☐ Fail |
| 3 | Entry 2: "Finalized" | Date: Tour completion | ☐ Pass ☐ Fail |
| 4 | Entry 3: "Paid" | Date: Payout processed | ☐ Pass ☐ Fail |
| 5 | Entry 4: "Refunded" | Date: Refund processed | ☐ Pass ☐ Fail |
| 6 | Final balance | Net amount after all adjustments | ☐ Pass ☐ Fail |

**Expected Result:** Complete transparent audit trail

---

## SAFEGUARD 9: Tiered Commission Caps

### TEST CASE 3.9.1: Maximum Tier Commission Enforced

**Objective:** Verify tier rates cannot exceed system cap

**Test Data:**
```
System Cap: 30% maximum commission
Tenant tries: Tier 3 = 35%
Expected: Rejected or capped to 30%
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Tenant creates tiered rule | Tier options shown | ☐ Pass ☐ Fail |
| 2 | Try to set Tier 3: 35% | Validation error | ☐ Pass ☐ Fail |
| 3 | Error message | "Maximum 30% allowed" | ☐ Pass ☐ Fail |
| 4 | Set Tier 3: 30% | Accepted | ☐ Pass ☐ Fail |

**Expected Result:** Commission rates capped at system maximum

---

## SAFEGUARD 10: Dispute Resolution

### TEST CASE 3.10.1: SuperAdmin Creates Dispute Ticket

**Objective:** Verify dispute resolution workflow instead of direct override

**Test Data:**
```
Issue: Commission calculated at 15% instead of 20%
Impact: Agent underpaid by \$200
Fix: Recalculate using correct 20% rate
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | SuperAdmin identifies discrepancy | Math verified | ☐ Pass ☐ Fail |
| 2 | Navigate to: Disputes | Create dispute option shown | ☐ Pass ☐ Fail |
| 3 | Create Dispute Ticket | Form appears | ☐ Pass ☐ Fail |
| 4 | Enter description | "System applied 15% instead of 20%" | ☐ Pass ☐ Fail |
| 5 | Enter proposed fix | "Recalculate as 20% = \$500 (not \$375)" | ☐ Pass ☐ Fail |
| 6 | Select reason code | "Miscalculation" | ☐ Pass ☐ Fail |
| 7 | Submit ticket | Status = "Open" | ☐ Pass ☐ Fail |
| 8 | Tenant notified | Email sent to Tenant Admin | ☐ Pass ☐ Fail |

**Expected Result:** Dispute created with full documentation

---

### TEST CASE 3.10.2: Tenant Admin Reviews Dispute

**Objective:** Verify tenant approves or rejects dispute resolution

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Tenant Admin receives notification | Email with dispute link | ☐ Pass ☐ Fail |
| 2 | Reviews dispute details | Original vs. proposed amounts clear | ☐ Pass ☐ Fail |
| 3 | Verifies calculation | \$375 (15%) vs \$500 (20%) | ☐ Pass ☐ Fail |
| 4 | Clicks "Approve" | Status → "Approved" | ☐ Pass ☐ Fail |
| 5 | System recalculates automatically | New total: \$500 | ☐ Pass ☐ Fail |
| 6 | Adjustment scheduled | For next payout (\$125 difference) | ☐ Pass ☐ Fail |
| 7 | Agent notified | Dispute resolution email sent | ☐ Pass ☐ Fail |

**Expected Result:** Transparent, audited dispute resolution

---

### TEST CASE 3.10.3: No Direct SuperAdmin Override

**Objective:** Verify SuperAdmin cannot directly modify commission amount

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | SuperAdmin attempts direct edit | Commission amount field | ☐ Pass ☐ Fail |
| 2 | Try to change amount | Field is read-only | ☐ Pass ☐ Fail |
| 3 | Try via API | API returns 403 Forbidden | ☐ Pass ☐ Fail |
| 4 | Only option available | Dispute resolution workflow | ☐ Pass ☐ Fail |

**Expected Result:** Direct modification disabled, only dispute workflow available

---

## UAT Summary

| Safeguard | Test Case | Status | Critical? |
|-----------|-----------|--------|-----------|
| 1. Refund Reversal | 3.1.1-3 | ☐ | YES |
| 2. Self-Booking | 3.2.1-2 | ☐ | YES |
| 3. Code Sharing | 3.3.1-3 | ☐ | YES |
| 4. Tiered Gaming | 3.4.1-3 | ☐ | YES |
| 5. Markup Cap | 3.5.1-2 | ☐ | YES |
| 6. Duplicate Booking | 3.6.1-2 | ☐ | YES |
| 7. Departing Staff | 3.7.1-2 | ☐ | MEDIUM |
| 8. Refund Tracking | 3.8.1 | ☐ | YES |
| 9. Commission Caps | 3.9.1 | ☐ | MEDIUM |
| 10. Dispute Resolution | 3.10.1-3 | ☐ | YES |

**Overall Status:** ☐ PASS ☐ FAIL ☐ BLOCKED

**Critical Failures:** If any "YES" safeguard fails, entire feature must be blocked

---

## Sign-Off

| Role | Name | Date | Status |
|------|------|------|--------|
| QA Lead | _______ | _______ | ☐ Approved |
| Security Lead | _______ | _______ | ☐ Approved |
| Dev Lead | _______ | _______ | ☐ Reviewed |
| Product Owner | _______ | _______ | ☐ Accepted |

