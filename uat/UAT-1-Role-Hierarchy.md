# UAT Test Cases: Role Hierarchy & Authorization

**Document Version:** 1.0  
**Date:** 2026-06-10  
**Prepared for:** LitXusDevHub QA Team

---

## Test Scope

This document covers UAT test cases for the Role Hierarchy system:
- SuperAdmin permissions and audit trail
- Admin user creation and assignment
- Tenant Admin access and scope
- Staff Agent creation and permissions

---

## Prerequisite Data

**Seed Data Required:** Start the API (`dotnet run`) — seed data loads automatically via `DatabaseSeeder.cs`

**Test Users:**
- SuperAdmin: `litotjuliano@gmail.com`
- Platform Admin: `alice@litxustravel.com`
- Tenant A Admin: `bob@tenanta.com`
- Tenant B Admin: `carol@tenantb.com`

---

## Test Cases

### TEST CASE 1.1: SuperAdmin Create Admin User

**Objective:** Verify SuperAdmin can create both Platform and Tenant Admin users

**Preconditions:**
- SuperAdmin logged in
- Navigate to: Admin Dashboard → User Management

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Click "Add New Admin" | Form appears with fields | ☐ Pass ☐ Fail |
| 2 | Select Role: "Admin" | Two scope options appear | ☐ Pass ☐ Fail |
| 3 | Select Scope: "Platform" | Form shows Platform Admin fields | ☐ Pass ☐ Fail |
| 4 | Enter Name: "Test Admin" | Name accepted | ☐ Pass ☐ Fail |
| 5 | Enter Email: "test@admin.com" | Email accepted, validation passes | ☐ Pass ☐ Fail |
| 6 | Click "Create" | Admin created, success message shown | ☐ Pass ☐ Fail |
| 7 | Check audit log | Audit entry shows: Who (SuperAdmin ID), What (Create Admin), When (timestamp), Why (auto) | ☐ Pass ☐ Fail |

**Postconditions:**
- New Platform Admin user created
- Audit log entry recorded
- Email sent to admin (optional: verify inbox)

**Test Data:**
```
Name: Test Admin
Email: test@admin.com
Role: Admin
Scope: Platform
```

---

### TEST CASE 1.2: SuperAdmin Cannot Create Other SuperAdmins

**Objective:** Verify SuperAdmin cannot create other SuperAdmin accounts (only one allowed)

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | SuperAdmin attempts to create Admin | Form appears | ☐ Pass ☐ Fail |
| 2 | Look for "SuperAdmin" role option | Option NOT visible | ☐ Pass ☐ Fail |
| 3 | Try to select "SuperAdmin" via URL hack | Access denied, system error | ☐ Pass ☐ Fail |
| 4 | Check database directly | Only one SuperAdmin exists | ☐ Pass ☐ Fail |

**Expected Result:** SuperAdmin role is not available in the UI or API, preventing accidental duplication.

---

### TEST CASE 1.3: SuperAdmin Create Tenant Admin

**Objective:** Verify SuperAdmin can create Tenant Admin and assign to tenant

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | SuperAdmin creates new Admin | Form shows | ☐ Pass ☐ Fail |
| 2 | Select Scope: "Tenant" | TenantId field appears | ☐ Pass ☐ Fail |
| 3 | Select Tenant: "Travel Pro Inc" | Selection accepted | ☐ Pass ☐ Fail |
| 4 | Enter Name & Email | Validated | ☐ Pass ☐ Fail |
| 5 | Click "Create" | Tenant Admin created | ☐ Pass ☐ Fail |
| 6 | New admin logs in | Can access Travel Pro Inc dashboard | ☐ Pass ☐ Fail |
| 7 | New admin tries to access other tenant | Access denied | ☐ Pass ☐ Fail |

**Test Data:**
```
Name: Test Tenant Admin
Email: test@tenant.com
Role: Admin
Scope: Tenant
AssignedTenant: Travel Pro Inc
```

---

### TEST CASE 1.4: Tenant Admin Can Create Staff Agents

**Objective:** Verify Tenant Admin can create staff agents within their tenant

**Preconditions:**
- Tenant Admin (Bob) logged in for Travel Pro Inc
- Navigate to: Staff Management

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Click "Add Staff Agent" | Form appears | ☐ Pass ☐ Fail |
| 2 | Enter Name: "New Agent" | Accepted | ☐ Pass ☐ Fail |
| 3 | Enter Email: "agent@travelpro.com" | Accepted | ☐ Pass ☐ Fail |
| 4 | Click "Create" | Agent created with auto-generated code | ☐ Pass ☐ Fail |
| 5 | Verify code format | Code is "STAFF-NEWAGENT-001" or similar | ☐ Pass ☐ Fail |
| 6 | Check audit | Tenant Admin action logged | ☐ Pass ☐ Fail |

**Postconditions:**
- Staff agent created in Travel Pro Inc only
- Unique code generated
- Agent receives welcome email

---

### TEST CASE 1.5: Tenant Admin Cannot Access Other Tenant

**Objective:** Verify Tenant Admin is restricted to their assigned tenant

**Preconditions:**
- Tenant A Admin (Bob) logged in
- Travel Pro Inc dashboard open

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Try to access Adventure Tours dashboard via URL | Access denied, redirected | ☐ Pass ☐ Fail |
| 2 | Try to view Adventure Tours staff list | Error: "Unauthorized" | ☐ Pass ☐ Fail |
| 3 | Try to create staff in Adventure Tours | API call rejected | ☐ Pass ☐ Fail |
| 4 | Check system logs | Unauthorized access attempt logged | ☐ Pass ☐ Fail |

**Expected Result:** Strict tenant isolation enforced at all levels.

---

### TEST CASE 1.6: SuperAdmin Audit Trail - Comprehensive

**Objective:** Verify all SuperAdmin actions are logged with full context

**Preconditions:**
- SuperAdmin (You) logged in
- Navigate to: Admin Dashboard → Audit Logs

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Perform action: Create Admin | Action completed | ☐ Pass ☐ Fail |
| 2 | View Audit Log | Entry exists for this action | ☐ Pass ☐ Fail |
| 3 | Check entry details: Who | Shows SuperAdmin user ID | ☐ Pass ☐ Fail |
| 4 | Check entry details: What | Shows "Create Admin" action | ☐ Pass ☐ Fail |
| 5 | Check entry details: When | Shows exact timestamp | ☐ Pass ☐ Fail |
| 6 | Check entry details: Where | Shows Tenant ID (if applicable) | ☐ Pass ☐ Fail |
| 7 | Check entry details: Before/After | Shows state change (JSON) | ☐ Pass ☐ Fail |
| 8 | Check entry details: IP Address | Shows source IP | ☐ Pass ☐ Fail |

**Audit Entry Format:**
```json
{
  "id": "AUDIT-001",
  "superAdminId": "11111111-1111-1111-1111-111111111111",
  "action": "CreateAdmin",
  "timestamp": "2026-06-10 14:30:00",
  "affectedEntityType": "AdminUser",
  "affectedEntityId": "NEW-ADMIN-ID",
  "reason": "Testing",
  "beforeState": "{}",
  "afterState": {"id": "NEW-ADMIN-ID", "name": "Test Admin", ...},
  "clientIp": "192.168.1.1"
}
```

**Postcondition:** Audit trail is complete and non-editable.

---

### TEST CASE 1.7: Platform Admin Cannot Create Admins

**Objective:** Verify only SuperAdmin can create other Admins

**Preconditions:**
- Platform Admin (Alice) logged in

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Navigate to: User Management | Page accessible | ☐ Pass ☐ Fail |
| 2 | Look for "Add Admin" button | Button NOT visible | ☐ Pass ☐ Fail |
| 3 | Try to access via URL | Access denied | ☐ Pass ☐ Fail |
| 4 | Try via API directly | API returns 403 Forbidden | ☐ Pass ☐ Fail |

**Expected Result:** Platform Admin cannot create or manage other admins.

---

### TEST CASE 1.8: Staff Agent Cannot Access Admin Functions

**Objective:** Verify staff agents have no admin privileges

**Preconditions:**
- Staff Agent (John) logged in to white-label site
- Logged in as: john@travelpro.com

**Test Steps:**

| # | Step | Expected Result | Status |
|---|------|-----------------|--------|
| 1 | Try to access admin dashboard | Access denied, redirected | ☐ Pass ☐ Fail |
| 2 | Try to create other staff | Endpoint not available | ☐ Pass ☐ Fail |
| 3 | Try to access commission rules | Read-only, cannot edit | ☐ Pass ☐ Fail |
| 4 | Check available functions | Only booking/tour related | ☐ Pass ☐ Fail |

**Expected Result:** Staff agents see only their own booking and commission dashboard.

---

## UAT Summary

| Test Case | Status | Notes |
|-----------|--------|-------|
| 1.1 - Create Admin | ☐ | Expected to pass |
| 1.2 - Cannot Create SuperAdmin | ☐ | Expected to pass |
| 1.3 - Create Tenant Admin | ☐ | Expected to pass |
| 1.4 - Tenant Admin Creates Staff | ☐ | Expected to pass |
| 1.5 - Tenant Isolation | ☐ | Expected to pass |
| 1.6 - Audit Trail | ☐ | Expected to pass |
| 1.7 - Platform Admin Limitations | ☐ | Expected to pass |
| 1.8 - Staff Agent Limitations | ☐ | Expected to pass |

**Overall Status:** ☐ PASS ☐ FAIL ☐ BLOCKED

---

## Known Issues / Observations

(To be filled by QA team during testing)

```
1. [Issue]: 
   Severity: [Critical/High/Medium/Low]
   Steps to reproduce:
   Expected: 
   Actual:
   Workaround: 

```

---

## Sign-Off

| Role | Name | Date | Status |
|------|------|------|--------|
| QA Lead | _______ | _______ | ☐ Approved |
| Dev Lead | _______ | _______ | ☐ Reviewed |
| Product Owner | _______ | _______ | ☐ Accepted |

