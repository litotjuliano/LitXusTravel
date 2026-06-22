# UAT Test Cases: Commission Calculations & Workflows

**Document Version:** 1.0  
**Date:** 2026-06-11  
**Prepared for:** LitXusDevHub QA Team

---

## Test Scope

This document covers UAT for complete commission workflows:
- Staff agent booking and commission accrual
- Independent agent reselling
- Monthly commission payout
- Commission statement accuracy

---

## Test Data Reference

**Seed data from `DatabaseSeeder.cs`:**

**Tenants:**
- Tenant A: Travel Pro Inc
- Tenant B: Adventure Tours Ltd

**Staff Agents (Tenant A):**
- John Smith: 15% commission
- Jane Doe: 8% commission
- Mike Johnson: 10% (departing)

**Tours (Tenant A):**
- Beach Resort: \$500
- Mountain Adventure: \$800
- City Tour: \$300

**Expected June Results:**
- John: 2 completed (\$75 + \$120 = \$195), 1 pending (\$45), 1 cancelled (reversed)
- Jane: 2 completed (\$40 + \$64 = \$104)
- Mike: 1 completed (\$30)
- Independent Agent: 2 completed (\$110 + \$160 = \$270)
- **Total June Payout: \$599**

---

## Test Cases

### TEST CASE 4.1: Staff Agent Booking - Accrual to Finalization

**Objective:** Verify complete flow from booking to commission finalization

**Precondition:** John logged in, Beach Resort tour available

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Customer provides John's code: "STAFF-JOHN-001" | Code entered at checkout | ☐ Pass ☐ Fail |
| 2 | Customer selects Beach Resort (\$500) | Tour selected | ☐ Pass ☐ Fail |
| 3 | Customer selects date: June 12, 2026 | Date confirmed | ☐ Pass ☐ Fail |
| 4 | Click "Book" | Booking created | ☐ Pass ☐ Fail |
| 5 | Check booking status | Status = "Confirmed" | ☐ Pass ☐ Fail |
| 6 | Check commission accrual | Created with status "Accrued" | ☐ Pass ☐ Fail |
| 7 | Verify commission amount | \$500 × 15% = \$75 | ☐ Pass ☐ Fail |
| 8 | Navigate to June 12 | Simulate date forward | ☐ Pass ☐ Fail |
| 9 | Mark tour "Completed" | Booking status → "Completed" | ☐ Pass ☐ Fail |
| 10 | Check commission status | Status → "Finalized" | ☐ Pass ☐ Fail |

**Verification:**
```sql
SELECT * FROM CommissionAccruals 
WHERE AgentId = 'STAFF-JOHN' AND SourceId = 'BOOKING-001'
-- Expected: Status = 'Finalized', Amount = 75.0
```

---

### TEST CASE 4.2: Staff Agent Multiple Bookings - Aggregate Commission

**Objective:** Verify multiple bookings sum correctly for monthly payout

**Test Data:**
```
John's June Bookings:
1. Beach (\$500) × 15% = \$75 (completed)
2. Mountain (\$800) × 15% = \$120 (completed)
3. City (\$300) × 15% = \$45 (pending - incomplete)
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Create booking 1 (Beach, completed) | Commission = \$75, Finalized | ☐ Pass ☐ Fail |
| 2 | Create booking 2 (Mountain, completed) | Commission = \$120, Finalized | ☐ Pass ☐ Fail |
| 3 | Create booking 3 (City, pending) | Commission = \$45, Accrued | ☐ Pass ☐ Fail |
| 4 | View John's commission dashboard | Shows all commissions | ☐ Pass ☐ Fail |
| 5 | Check finalized total | \$75 + \$120 = \$195 (ready to pay) | ☐ Pass ☐ Fail |
| 6 | Check pending total | \$45 (waiting for tour completion) | ☐ Pass ☐ Fail |
| 7 | Check balance | \$195 available, \$45 pending | ☐ Pass ☐ Fail |

**Verification:**
```sql
SELECT 
  SUM(CASE WHEN Status = 'Finalized' THEN CommissionAmount ELSE 0 END) as Finalized,
  SUM(CASE WHEN Status = 'Accrued' THEN CommissionAmount ELSE 0 END) as Pending
FROM CommissionAccruals
WHERE AgentId = 'JOHN' AND TenantId = 'TENANT-A' AND MONTH(AccruedAt) = 6
-- Expected: Finalized = 195, Pending = 45
```

---

### TEST CASE 4.3: Staff Agents Different Rates - Correct Commission

**Objective:** Verify different commission rates applied correctly

**Test Data:**
```
John: 15% per booking
Jane: 8% per booking
Mike: 10% per booking (default)
Same tour: \$500
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Create booking with John's code | Commission = \$75 (15% of \$500) | ☐ Pass ☐ Fail |
| 2 | Create booking with Jane's code | Commission = \$40 (8% of \$500) | ☐ Pass ☐ Fail |
| 3 | Create booking with Mike's code | Commission = \$50 (10% of \$500) | ☐ Pass ☐ Fail |
| 4 | Verify all correct in database | Three different amounts | ☐ Pass ☐ Fail |
| 5 | Each agent's dashboard | Shows correct amount | ☐ Pass ☐ Fail |

**Verification:**
```sql
SELECT AgentId, CommissionAmount, CommissionPercentage
FROM CommissionAccruals
WHERE SourceId IN ('BOOKING-001', 'BOOKING-005', 'BOOKING-007')
-- Expected: 75 (15%), 40 (8%), 50 (10%)
```

---

### TEST CASE 4.4: Independent Agent with Markup Commission

**Objective:** Verify commission calculated on full booking price including markup

**Test Data:**
```
Tour Base: \$500
Agent Markup: \$50
Customer Pays: \$550
Commission Rule: 20%
Agent Commission: \$550 × 20% = \$110
Tenant Receives: \$500 (base price only)
Agent Keeps: \$50 (markup) + \$110 (commission) = \$160
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Independent Agent adds \$50 markup | Selling price = \$550 | ☐ Pass ☐ Fail |
| 2 | Customer books at \$550 | Booking created at \$550 | ☐ Pass ☐ Fail |
| 3 | Check commission calculation | \$550 × 20% = \$110 | ☐ Pass ☐ Fail |
| 4 | Check tenant revenue | Query base price only = \$500 | ☐ Pass ☐ Fail |
| 5 | Check agent earnings | Commission (\$110) + Markup (\$50) = \$160 | ☐ Pass ☐ Fail |

**Verification:**
```sql
-- Commission Accrual
SELECT CommissionAmount FROM CommissionAccruals 
WHERE SourceId = 'BOOKING-009'
-- Expected: 110.0

-- Booking Price
SELECT BookingPrice FROM Bookings 
WHERE Id = 'BOOKING-009'
-- Expected: 550.0
```

---

### TEST CASE 4.5: Monthly Payout Processing

**Objective:** Verify June payout calculated and processed correctly

**Precondition:** All June bookings completed and finalized (except pending ones)

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Navigate to: Payout Processing | June payout shown | ☐ Pass ☐ Fail |
| 2 | View payout details | All agents listed | ☐ Pass ☐ Fail |
| 3 | John's payout | \$195 (2 completed, 1 pending excluded) | ☐ Pass ☐ Fail |
| 4 | Jane's payout | \$104 (2 completed) | ☐ Pass ☐ Fail |
| 5 | Mike's payout | \$30 (1 completed) | ☐ Pass ☐ Fail |
| 6 | Independent Agent payout | \$270 (2 completed) | ☐ Pass ☐ Fail |
| 7 | Total payout | \$195 + \$104 + \$30 + \$270 = \$599 | ☐ Pass ☐ Fail |
| 8 | Check minimum threshold | All agents meet \$100 minimum | ☐ Pass ☐ Fail |
| 9 | Submit for payment | Status → "Pending" | ☐ Pass ☐ Fail |
| 10 | Process payment | Status → "Paid" | ☐ Pass ☐ Fail |

**Verification:**
```sql
SELECT 
  AgentId,
  SUM(CommissionAmount) as TotalCommission
FROM CommissionAccruals
WHERE TenantId = 'TENANT-A' 
  AND MONTH(AccruedAt) = 6 
  AND Status = 'Finalized'
GROUP BY AgentId
-- Expected results as above
```

---

### TEST CASE 4.6: Commission Statement - Agent View

**Objective:** Verify agent can see accurate commission statement

**Precondition:** John logged in to his dashboard

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Navigate to: My Commission | Statement displayed | ☐ Pass ☐ Fail |
| 2 | Current month: June | June details shown | ☐ Pass ☐ Fail |
| 3 | Finalized commissions | \$195 displayed | ☐ Pass ☐ Fail |
| 4 | Pending commissions | \$45 displayed | ☐ Pass ☐ Fail |
| 5 | Available for payout | \$195 highlighted | ☐ Pass ☐ Fail |
| 6 | Upcoming payout date | "June 30" shown | ☐ Pass ☐ Fail |
| 7 | Click "View Details" | Breakdown by booking shown | ☐ Pass ☐ Fail |
| 8 | Each booking details | Tour, date, price, commission | ☐ Pass ☐ Fail |

**Expected Statement:**
```
=== June 2026 Commission Statement ===
Agent: John Smith
Rate: 15% per booking

Completed Bookings:
1. Beach Resort ($500) → $75 commission ✓
2. Mountain Adventure ($800) → $120 commission ✓
   Subtotal: $195 (Finalized)

Pending Bookings:
3. City Tour ($300) → $45 commission (awaiting completion)

Cancelled:
4. Beach Resort ($500) → $75 commission (reversed)

Available for Payout: $195
Pending Finalization: $45
Next Payout: June 30, 2026
```

---

### TEST CASE 4.7: Commission Statement - Tenant View

**Objective:** Verify tenant admin sees all staff commissions

**Precondition:** Bob (Tenant Admin) logged in

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Navigate to: Commission Reports | Report page opens | ☐ Pass ☐ Fail |
| 2 | Select month: June | June report shown | ☐ Pass ☐ Fail |
| 3 | Staff list shown | John, Jane, Mike listed | ☐ Pass ☐ Fail |
| 4 | John's commission | \$195 finalized | ☐ Pass ☐ Fail |
| 5 | Jane's commission | \$104 finalized | ☐ Pass ☐ Fail |
| 6 | Mike's commission | \$30 finalized | ☐ Pass ☐ Fail |
| 7 | Total expense | \$329 (staff total) | ☐ Pass ☐ Fail |
| 8 | Commission as % revenue | Calculate and verify | ☐ Pass ☐ Fail |
| 9 | Click "Download Report" | PDF/CSV generated | ☐ Pass ☐ Fail |

**Expected Report:**
```
=== June 2026 Commission Report ===
Tenant: Travel Pro Inc

Staff Agent Commissions:
1. John Smith (15%)      $195   (Finalized)
2. Jane Doe (8%)         $104   (Finalized)
3. Mike Johnson (10%)     $30   (Finalized)

Freelance Agent Commissions:
4. Travel Influencer (20%) $270  (Finalized)

Total Commission Expense: $599
Total Revenue: ~$4,500 (estimates)
Commission Rate: 13.3%
```

---

### TEST CASE 4.8: Pending Commission Auto-Finalization

**Objective:** Verify pending commission auto-finalizes when tour completes

**Test Data:**
```
Booking 3: City Tour (pending)
Commission: \$45 (Accrued)
Tour Date: June 20
Completion: Will be marked completed
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | View John's dashboard (June 10) | \$45 showing as pending | ☐ Pass ☐ Fail |
| 2 | Navigate to June 20 | Simulate date forward | ☐ Pass ☐ Fail |
| 3 | Mark tour "Completed" | System processes | ☐ Pass ☐ Fail |
| 4 | Check commission status | Changed to "Finalized" | ☐ Pass ☐ Fail |
| 5 | Check John's dashboard | \$45 now finalized | ☐ Pass ☐ Fail |
| 6 | Verify total finalized | Now \$195 + \$45 = \$240 | ☐ Pass ☐ Fail |

**Verification:**
```sql
SELECT Status FROM CommissionAccruals 
WHERE SourceId = 'BOOKING-003'
-- Before: Accrued
-- After: Finalized
```

---

### TEST CASE 4.9: Cancelled Booking Commission Removal

**Objective:** Verify cancelled bookings remove accrued commission

**Test Data:**
```
Booking 4: Beach Resort (cancelled)
Commission: \$75 (was accrued)
Status Change: Cancelled
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | View commission for BOOKING-004 | Status = "Accrued", Amount = \$75 | ☐ Pass ☐ Fail |
| 2 | Cancel the booking | Booking status → "Cancelled" | ☐ Pass ☐ Fail |
| 3 | Check commission status | Status → "Reversed" | ☐ Pass ☐ Fail |
| 4 | Check commission amount | Appears as -\$75 or hidden | ☐ Pass ☐ Fail |
| 5 | Check John's finalized total | Still \$195 (cancelled not included) | ☐ Pass ☐ Fail |

**Verification:**
```sql
SELECT Status, CommissionAmount FROM CommissionAccruals 
WHERE SourceId = 'BOOKING-004'
-- Expected: Status = 'Reversed'
```

---

### TEST CASE 4.10: Direct Booking (No Code) - No Commission

**Objective:** Verify direct bookings without code generate no commission

**Test Data:**
```
Booking 8: Direct customer (no staff code)
Tour: Beach Resort (\$500)
Commission: None (should be \$0)
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Customer books without code | Booking created normally | ☐ Pass ☐ Fail |
| 2 | Check StaffAgentId | NULL or empty | ☐ Pass ☐ Fail |
| 3 | Check commission accrual | No record created | ☐ Pass ☐ Fail |
| 4 | Check payout | Not included in staff payouts | ☐ Pass ☐ Fail |
| 5 | Revenue to tenant | Full \$500 (no commission) | ☐ Pass ☐ Fail |

**Verification:**
```sql
SELECT * FROM CommissionAccruals 
WHERE SourceId = 'BOOKING-008'
-- Expected: No rows returned
```

---

## UAT Summary

| Test Case | Scenario | Status |
|-----------|----------|--------|
| 4.1 | Booking → Accrual → Finalization | ☐ |
| 4.2 | Multiple Bookings Aggregate | ☐ |
| 4.3 | Different Agent Rates | ☐ |
| 4.4 | Independent Agent Markup | ☐ |
| 4.5 | Monthly Payout | ☐ |
| 4.6 | Agent Commission Statement | ☐ |
| 4.7 | Tenant Commission Report | ☐ |
| 4.8 | Auto-Finalization | ☐ |
| 4.9 | Cancelled Booking | ☐ |
| 4.10 | Direct Booking (No Code) | ☐ |

**Overall Status:** ☐ PASS ☐ FAIL ☐ BLOCKED

---

## Data Validation Queries

Run these after payout to verify correctness:

```sql
-- Total finalized commission for June
SELECT SUM(CommissionAmount) 
FROM CommissionAccruals
WHERE MONTH(AccruedAt) = 6 AND Status IN ('Finalized', 'Paid')
-- Expected: 599.0

-- Breakdown by agent
SELECT AgentId, SUM(CommissionAmount) as Total
FROM CommissionAccruals
WHERE MONTH(AccruedAt) = 6 AND Status IN ('Finalized', 'Paid')
GROUP BY AgentId
-- Expected: John=195, Jane=104, Mike=30, Travel Influencer=270

-- Verify no reversed included
SELECT COUNT(*) FROM CommissionAccruals
WHERE Status = 'Reversed'
-- Expected: 1 (only BOOKING-004)

-- Verify pending not included
SELECT COUNT(*) FROM CommissionAccruals
WHERE Status = 'Accrued'
-- Expected: Should be small number or 0 if all completed
```

---

## Sign-Off

| Role | Name | Date | Status |
|------|------|------|--------|
| QA Lead | _______ | _______ | ☐ Approved |
| Dev Lead | _______ | _______ | ☐ Reviewed |
| Product Owner | _______ | _______ | ☐ Accepted |

