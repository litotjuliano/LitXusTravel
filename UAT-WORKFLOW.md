# LitXusTravel — UAT Workflow

## Our Role
LitXusTravel is the **sender**. We write UAT lists and receive feedback from LitXusDevHub.

---

## Folder Structure

```
uat/
  outgoing/    ← We write UAT lists here (DevHub reads these)
  incoming/    ← We read DevHub feedback from here
```

---

## File Naming Convention

| File | Location | Pattern |
|------|----------|---------|
| UAT List (we write) | `uat/outgoing/` | `uat-list-v{N}.md` |
| DevHub Feedback (we read) | `uat/incoming/` | `test-report-v{N}.md` |

---

## Workflow

```
1. We write UAT list → uat/outgoing/uat-list-v{N}.md
2. Notify DevHub: "UAT list ready at LitXusTravel/uat/outgoing/uat-list-v{N}.md"
3. DevHub reviews → writes feedback to our uat/incoming/test-report-v{N}.md
4. We read feedback → fix issues → repeat cycle
5. DevHub signs off → cycle closed
```

---

## UAT List Template

When creating a UAT list, copy this structure:

```markdown
# LitXusTravel UAT List — v{N}

## Info
- **Version:** v{N}
- **Date:** YYYY-MM-DD
- **Prepared By:** LitXusTravel Team
- **Send To:** LitXusDevHub

---

## Changes to Test

| # | Feature / Module | Change Description | Priority |
|---|------------------|--------------------|----------|
| 1 | | | High |

---

## Test Scenarios

### Feature 1: {Name}
- **Steps:**
  1. 
- **Expected Result:** 
- **Test Data Required:** 

---

## Notes to DevHub
- 
```

---

## Reading Feedback

Check `uat/incoming/test-report-v{N}.md` for:
- ✅ Passed — no action needed
- ❌ Failed — fix and notify DevHub for re-test
- 🔄 Re-testing — DevHub is retesting your fix
- ⏳ Pending — not yet tested

---

## Current Cycle

| Version | UAT Sent | Feedback Received | Status |
|---------|----------|-------------------|--------|
| v1 | 2026-06-01 | Pending | ⏳ Awaiting DevHub |
