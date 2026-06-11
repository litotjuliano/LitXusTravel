# UAT Test Cases: Agent Models & Commission Configuration

**Document Version:** 1.0  
**Date:** 2026-06-10  
**Prepared for:** LitXusDevHub QA Team

---

## Test Scope

This document covers UAT for:
- Tenant Staff Agent creation and commission rules
- Independent Agent subscription and white-label website
- Commission configuration (fixed, percentage, tiered)
- Agent code generation and rotation

---

## Test Data Reference

**Tenants:**
- Tenant A: Travel Pro Inc (ID: AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA)
- Tenant B: Adventure Tours Ltd (ID: BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB)

**Staff Agents:**
- John Smith (STAFF-JOHN-001): 15% commission
- Jane Doe (STAFF-JANE-001): 8% commission
- Mike Johnson (STAFF-MIKE-001): 10% commission

**Independent Agents:**
- Travel Influencer: Premium tier, 20% commission for Tenant A
- Wanderlust Travel: Standard tier, 15% commission for Tenant B

---

## Test Cases

### TEST CASE 2.1: Tenant Admin Creates Staff Agent with Custom Commission

**Objective:** Verify staff agent creation with custom commission rate

**Preconditions:**
- Tenant Admin (Bob) logged in for Travel Pro Inc
- Navigate to: Staff Management

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Click "Add Staff Agent" | Form appears | ☐ Pass ☐ Fail |
| 2 | Enter: Name = "John Smith" | Accepted | ☐ Pass ☐ Fail |
| 3 | Enter: Email = "john@travelpro.com" | Accepted | ☐ Pass ☐ Fail |
| 4 | Click "Create" | Agent created | ☐ Pass ☐ Fail |
| 5 | System generates code | Code = "STAFF-JOHN-001" | ☐ Pass ☐ Fail |
| 6 | Navigate to: Commission Settings | Commission config page opens | ☐ Pass ☐ Fail |
| 7 | Assign commission to John | Form appears | ☐ Pass ☐ Fail |
| 8 | Select: Type = "Percentage" | Percentage options show | ☐ Pass ☐ Fail |
| 9 | Enter: Rate = "15%" | Accepted | ☐ Pass ☐ Fail |
| 10 | Select: Trigger = "Per Booking" | Accepted | ☐ Pass ☐ Fail |
| 11 | Click "Apply" | Commission rule saved | ☐ Pass ☐ Fail |

**Expected Result:** Staff agent created with 15% commission rate

**Verification:**
- Query database: Staff agent exists with code
- Verify commission rule applied to John only
- Other staff not affected

---

### TEST CASE 2.2: Staff Agent Receives Unique Code

**Objective:** Verify each staff agent gets a unique, non-transferable code

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Create Staff Agent "Agent A" | Code generated: "STAFF-AGENTA-001" | ☐ Pass ☐ Fail |
| 2 | Create Staff Agent "Agent B" | Code generated: "STAFF-AGENTB-001" | ☐ Pass ☐ Fail |
| 3 | Verify codes are unique | No duplicates in database | ☐ Pass ☐ Fail |
| 4 | Try to manually assign same code | System rejects, error shown | ☐ Pass ☐ Fail |
| 5 | Check code format | Format: STAFF-{FirstName}-{Sequence} | ☐ Pass ☐ Fail |

**Expected Result:** Each agent has unique code, cannot be duplicated

---

### TEST CASE 2.3: Staff Agent Code Rotation

**Objective:** Verify code can be rotated monthly to prevent sharing

**Preconditions:**
- Staff Agent (John) exists with code "STAFF-JOHN-001"

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Navigate to: Agent Management | John's profile opens | ☐ Pass ☐ Fail |
| 2 | Click "Rotate Code" button | Confirmation dialog appears | ☐ Pass ☐ Fail |
| 3 | Confirm rotation | New code generated: "STAFF-JOHN-002" | ☐ Pass ☐ Fail |
| 4 | Check old code expiry | CodeExpiresAt = current + 30 days | ☐ Pass ☐ Fail |
| 5 | Test old code in booking | Booking accepts old code (grace period) | ☐ Pass ☐ Fail |
| 6 | Test new code in booking | Booking accepts new code | ☐ Pass ☐ Fail |
| 7 | After 30 days, test old code | Old code rejected | ☐ Pass ☐ Fail |

**Expected Result:** Code rotates, old code has 30-day grace period

---

### TEST CASE 2.4: Commission Rule Types - Fixed Amount

**Objective:** Verify staff can have fixed amount commission

**Test Data:**
```
CommissionRule:
  Type: Fixed Amount
  Amount: $50
  Trigger: Per Booking
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Create commission rule: \$50 per booking | Rule saved | ☐ Pass ☐ Fail |
| 2 | Customer books \$500 tour | Booking created | ☐ Pass ☐ Fail |
| 3 | Tour completes | Commission = \$50 (fixed) | ☐ Pass ☐ Fail |
| 4 | Customer books \$800 tour | Booking created | ☐ Pass ☐ Fail |
| 5 | Tour completes | Commission = \$50 (still fixed) | ☐ Pass ☐ Fail |

**Expected Result:** Commission is flat \$50 regardless of booking price

---

### TEST CASE 2.5: Commission Rule Types - Percentage

**Objective:** Verify staff can have percentage-based commission

**Test Data:**
```
CommissionRule:
  Type: Percentage
  Rate: 15%
  Trigger: Per Booking
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Create commission rule: 15% per booking | Rule saved | ☐ Pass ☐ Fail |
| 2 | Customer books \$500 tour | Booking created | ☐ Pass ☐ Fail |
| 3 | Tour completes | Commission = \$75 (15% of \$500) | ☐ Pass ☐ Fail |
| 4 | Customer books \$800 tour | Booking created | ☐ Pass ☐ Fail |
| 5 | Tour completes | Commission = \$120 (15% of \$800) | ☐ Pass ☐ Fail |

**Expected Result:** Commission scales with booking price

---

### TEST CASE 2.6: Commission Rule Types - Tiered

**Objective:** Verify tiered commission (volume-based)

**Test Data:**
```
CommissionRule (Independent Agent):
  Tier 1 (0-100 bookings): 15%
  Tier 2 (101-500 bookings): 20%
  Tier 3 (500+ bookings): 25%
  Booking price: $500 avg
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Agent at 50 completed bookings | Rate = 15% | ☐ Pass ☐ Fail |
| 2 | Agent completes booking | Commission = \$500 × 15% = \$75 | ☐ Pass ☐ Fail |
| 3 | Agent reaches 150 bookings | Rate = 20% (moved to tier 2) | ☐ Pass ☐ Fail |
| 4 | Agent completes booking | Commission = \$500 × 20% = \$100 | ☐ Pass ☐ Fail |
| 5 | Monthly reset | Counter resets for next month | ☐ Pass ☐ Fail |

**Expected Result:** Tier changes based on monthly completed bookings

---

### TEST CASE 2.7: SuperAdmin Creates Independent Agent

**Objective:** Verify SuperAdmin can create independent agent with white-label website

**Preconditions:**
- SuperAdmin logged in
- Navigate to: Independent Agents

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Click "Add Independent Agent" | Form appears | ☐ Pass ☐ Fail |
| 2 | Enter Name: "Travel Influencer" | Accepted | ☐ Pass ☐ Fail |
| 3 | Enter Email: "info@influencer.com" | Accepted | ☐ Pass ☐ Fail |
| 4 | Select Subscription: "Premium" | Accepted | ☐ Pass ☐ Fail |
| 5 | Click "Create" | Agent created | ☐ Pass ☐ Fail |
| 6 | System provisions website | Domain = "travelinfluencer.litxustravel.com" | ☐ Pass ☐ Fail |
| 7 | SSL certificate provisioned | Website accessible via HTTPS | ☐ Pass ☐ Fail |
| 8 | Agent receives credentials | Email sent with login link | ☐ Pass ☐ Fail |

**Expected Result:** Independent agent created with white-label website

---

### TEST CASE 2.8: Tenant Authorizes Independent Agent

**Objective:** Verify tenant can authorize freelance agent to resell their tours

**Preconditions:**
- Independent Agent (Travel Influencer) created
- Tenant Admin logged in for Travel Pro Inc

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Navigate to: Settings → Freelance Agents | List appears | ☐ Pass ☐ Fail |
| 2 | Click "Add Agent" | Selection dialog appears | ☐ Pass ☐ Fail |
| 3 | Select: "Travel Influencer" | Agent highlighted | ☐ Pass ☐ Fail |
| 4 | Set Commission: 20% | Accepted | ☐ Pass ☐ Fail |
| 5 | Set Markup Allowed: Yes, \$50 max | Accepted | ☐ Pass ☐ Fail |
| 6 | Click "Authorize" | Agent added to tenant | ☐ Pass ☐ Fail |
| 7 | Agent logs in | Can see Travel Pro tours | ☐ Pass ☐ Fail |

**Expected Result:** Agent authorized and can resell tours

---

### TEST CASE 2.9: Independent Agent Configures Markup

**Objective:** Verify agent can markup tours within tenant limits

**Preconditions:**
- Independent Agent authorized for Tenant A
- Markup allowed: \$50 max, 10% max
- Tour base price: \$500

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Agent accesses catalog | Travel Pro tours visible | ☐ Pass ☐ Fail |
| 2 | Agent selects "Beach Tour" | \$500 base price shown | ☐ Pass ☐ Fail |
| 3 | Agent adds \$50 markup | Selling price = \$550 | ☐ Pass ☐ Fail |
| 4 | Try to add \$100 markup | Rejected: exceeds \$50 limit | ☐ Pass ☐ Fail |
| 5 | Customer books at \$550 | Booking created | ☐ Pass ☐ Fail |
| 6 | Verify earnings | Commission = \$550 × 20% = \$110 (on full price) | ☐ Pass ☐ Fail |
| 7 | Verify tenant earnings | Tenant gets \$500 base (no markup share) | ☐ Pass ☐ Fail |

**Expected Result:** Markup applied within limits, commission on full price

---

### TEST CASE 2.10: Default Commission vs. Custom Commission

**Objective:** Verify tenant's default rule applies to all staff, can be overridden per agent

**Test Data:**
```
Default Rule: 10% for all staff
John Custom: 15% (override)
Jane Custom: 8% (override)
Mike: Uses default 10%
```

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Query default commission rule | Found: 10% for all staff | ☐ Pass ☐ Fail |
| 2 | Query John's rule | Found: 15% (custom) | ☐ Pass ☐ Fail |
| 3 | Query Jane's rule | Found: 8% (custom) | ☐ Pass ☐ Fail |
| 4 | Query Mike's rule | Found: 10% (default) | ☐ Pass ☐ Fail |
| 5 | Create booking for Mike | Commission = \$500 × 10% = \$50 | ☐ Pass ☐ Fail |
| 6 | Create booking for John | Commission = \$500 × 15% = \$75 | ☐ Pass ☐ Fail |

**Expected Result:** Custom overrides applied per agent, default used otherwise

---

## UAT Summary

| Test Case | Status | Notes |
|-----------|--------|-------|
| 2.1 - Create Staff Agent | ☐ | Expected to pass |
| 2.2 - Unique Agent Code | ☐ | Expected to pass |
| 2.3 - Code Rotation | ☐ | Expected to pass |
| 2.4 - Fixed Commission | ☐ | Expected to pass |
| 2.5 - Percentage Commission | ☐ | Expected to pass |
| 2.6 - Tiered Commission | ☐ | Expected to pass |
| 2.7 - Create Independent Agent | ☐ | Expected to pass |
| 2.8 - Authorize Agent | ☐ | Expected to pass |
| 2.9 - Markup Configuration | ☐ | Expected to pass |
| 2.10 - Default vs Custom | ☐ | Expected to pass |

**Overall Status:** ☐ PASS ☐ FAIL ☐ BLOCKED

---

## Known Issues / Observations

(To be filled by QA team)

---

## Sign-Off

| Role | Name | Date | Status |
|------|------|------|--------|
| QA Lead | _______ | _______ | ☐ Approved |
| Dev Lead | _______ | _______ | ☐ Reviewed |
| Product Owner | _______ | _______ | ☐ Accepted |

