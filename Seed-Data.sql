-- ============================================================================
-- LitXusTravel Seed Data Script
-- Purpose: Populate database with test data for all scenarios
-- Date: 2026-06-10
-- ============================================================================

-- IMPORTANT: Run in this order:
-- 1. AdminUsers (SuperAdmin, Platform Admin, Tenant Admins)
-- 2. Tenants
-- 3. StaffAgents
-- 4. IndependentAgents
-- 5. CommissionRules
-- 6. Tours
-- 7. Bookings
-- 8. CommissionAccruals

-- ============================================================================
-- 1. SUPERADMIN & ADMIN USERS
-- ============================================================================

-- SuperAdmin (You - LitXus Owner)
INSERT INTO AdminUsers (Id, Name, Email, Role, Scope, IsActive, CreatedAt)
VALUES
(
    '11111111-1111-1111-1111-111111111111',
    'You (LitXus Owner)',
    'litotjuliano@gmail.com',
    'SuperAdmin',
    'Platform',
    1,
    '2026-06-01 10:00:00'
);

-- Platform Admin
INSERT INTO AdminUsers (Id, Name, Email, Role, Scope, IsActive, CreatedAt)
VALUES
(
    '22222222-2222-2222-2222-222222222222',
    'Alice Platform Manager',
    'alice@litxustravel.com',
    'Admin',
    'Platform',
    1,
    '2026-06-02 10:00:00'
);

-- Tenant Admin for Tenant A
INSERT INTO AdminUsers (Id, Name, Email, Role, Scope, AssignedTenantId, IsActive, CreatedAt)
VALUES
(
    '33333333-3333-3333-3333-333333333333',
    'Bob Tenant A Admin',
    'bob@tenanta.com',
    'Admin',
    'Tenant',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    1,
    '2026-06-02 10:00:00'
);

-- Tenant Admin for Tenant B
INSERT INTO AdminUsers (Id, Name, Email, Role, Scope, AssignedTenantId, IsActive, CreatedAt)
VALUES
(
    '44444444-4444-4444-4444-444444444444',
    'Carol Tenant B Admin',
    'carol@tenantb.com',
    'Admin',
    'Tenant',
    'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB',
    1,
    '2026-06-02 10:00:00'
);

-- ============================================================================
-- 2. TENANTS
-- ============================================================================

-- Tenant A: Travel Pro Inc
INSERT INTO Tenants (Id, Name, Email, Phone, Status, CreatedAt)
VALUES
(
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'Travel Pro Inc',
    'contact@travelpro.com',
    '+1-555-0001',
    'Active',
    '2026-06-01 10:00:00'
);

-- Tenant B: Adventure Tours Ltd
INSERT INTO Tenants (Id, Name, Email, Phone, Status, CreatedAt)
VALUES
(
    'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB',
    'Adventure Tours Ltd',
    'info@adventuretours.com',
    '+1-555-0002',
    'Active',
    '2026-06-01 10:00:00'
);

-- ============================================================================
-- 3. STAFF AGENTS (Internal Employees)
-- ============================================================================

-- Tenant A: Staff Agents
-- Agent 1: John Smith (High performer)
INSERT INTO StaffAgents (Id, TenantId, Name, Email, UniqueCode, CodeIssuedAt, CodeExpiresAt, IsActive, JoinedAt, DepartedAt)
VALUES
(
    'STAFF01-0000-0000-0000-000000000001',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'John Smith',
    'john@travelpro.com',
    'STAFF-JOHN-001',
    '2026-06-01 10:00:00',
    '2026-07-01 10:00:00',
    1,
    '2026-05-01 10:00:00',
    NULL
);

-- Agent 2: Jane Doe (Standard performer)
INSERT INTO StaffAgents (Id, TenantId, Name, Email, UniqueCode, CodeIssuedAt, CodeExpiresAt, IsActive, JoinedAt, DepartedAt)
VALUES
(
    'STAFF01-0000-0000-0000-000000000002',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'Jane Doe',
    'jane@travelpro.com',
    'STAFF-JANE-001',
    '2026-06-01 10:00:00',
    '2026-07-01 10:00:00',
    1,
    '2026-04-15 10:00:00',
    NULL
);

-- Agent 3: Mike Johnson (Part-time, about to leave)
INSERT INTO StaffAgents (Id, TenantId, Name, Email, UniqueCode, CodeIssuedAt, CodeExpiresAt, IsActive, JoinedAt, DepartedAt)
VALUES
(
    'STAFF01-0000-0000-0000-000000000003',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'Mike Johnson',
    'mike@travelpro.com',
    'STAFF-MIKE-001',
    '2026-06-01 10:00:00',
    '2026-07-01 10:00:00',
    1,
    '2026-03-01 10:00:00',
    NULL
);

-- Tenant B: Staff Agents
-- Agent 4: Sarah Williams
INSERT INTO StaffAgents (Id, TenantId, Name, Email, UniqueCode, CodeIssuedAt, CodeExpiresAt, IsActive, JoinedAt, DepartedAt)
VALUES
(
    'STAFF02-0000-0000-0000-000000000004',
    'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB',
    'Sarah Williams',
    'sarah@adventuretours.com',
    'STAFF-SARAH-001',
    '2026-06-01 10:00:00',
    '2026-07-01 10:00:00',
    1,
    '2026-05-10 10:00:00',
    NULL
);

-- ============================================================================
-- 4. INDEPENDENT AGENTS (Freelancers)
-- ============================================================================

-- Independent Agent 1: Travel Influencer
INSERT INTO IndependentAgents (Id, Name, Email, SubscriptionTier, WhiteLabelDomain, IsActive, CreatedAt)
VALUES
(
    'AGENT01-0000-0000-0000-000000000001',
    'Travel Influencer Inc',
    'info@travelinfluencer.com',
    'Premium',
    'travelinfluencer.litxustravel.com',
    1,
    '2026-06-01 10:00:00'
);

-- Independent Agent 2: Wanderlust Travel
INSERT INTO IndependentAgents (Id, Name, Email, SubscriptionTier, WhiteLabelDomain, IsActive, CreatedAt)
VALUES
(
    'AGENT01-0000-0000-0000-000000000002',
    'Wanderlust Travel Agency',
    'contact@wanderlusttravel.com',
    'Standard',
    'wanderlust.litxustravel.com',
    1,
    '2026-06-02 10:00:00'
);

-- ============================================================================
-- 5. COMMISSION RULES
-- ============================================================================

-- Tenant A: Staff Commission Rules (Default 10%)
INSERT INTO CommissionRules (Id, TenantId, AgentId, Trigger, Amount, IsPercentage, PayoutFrequency, AutoApprove, MinimumThreshold, EffectiveFrom, EffectiveTo, IsActive)
VALUES
(
    'RULE-TA-001-0001',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    NULL, -- Default for all staff
    'TourBooked',
    10.0,
    1, -- Percentage
    'Monthly',
    1, -- Auto-approve
    100.0,
    '2026-06-01 00:00:00',
    NULL,
    1
);

-- Tenant A: John Custom Rule (15% - High performer)
INSERT INTO CommissionRules (Id, TenantId, AgentId, Trigger, Amount, IsPercentage, PayoutFrequency, AutoApprove, MinimumThreshold, EffectiveFrom, EffectiveTo, IsActive)
VALUES
(
    'RULE-TA-001-0002',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'STAFF01-0000-0000-0000-000000000001', -- John only
    'TourBooked',
    15.0,
    1, -- Percentage
    'Monthly',
    1,
    100.0,
    '2026-06-01 00:00:00',
    NULL,
    1
);

-- Tenant A: Jane Custom Rule (8% - Part-time)
INSERT INTO CommissionRules (Id, TenantId, AgentId, Trigger, Amount, IsPercentage, PayoutFrequency, AutoApprove, MinimumThreshold, EffectiveFrom, EffectiveTo, IsActive)
VALUES
(
    'RULE-TA-001-0003',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'STAFF01-0000-0000-0000-000000000002', -- Jane only
    'TourBooked',
    8.0,
    1, -- Percentage
    'Monthly',
    1,
    100.0,
    '2026-06-01 00:00:00',
    NULL,
    1
);

-- Tenant B: Staff Commission Rules (Default 12%)
INSERT INTO CommissionRules (Id, TenantId, AgentId, Trigger, Amount, IsPercentage, PayoutFrequency, AutoApprove, MinimumThreshold, EffectiveFrom, EffectiveTo, IsActive)
VALUES
(
    'RULE-TB-001-0001',
    'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB',
    NULL, -- Default
    'TourBooked',
    12.0,
    1, -- Percentage
    'Monthly',
    1,
    100.0,
    '2026-06-01 00:00:00',
    NULL,
    1
);

-- Tenant A: Independent Agent Commission (Travel Influencer)
INSERT INTO CommissionRules (Id, TenantId, AgentId, Trigger, Amount, IsPercentage, PayoutFrequency, AutoApprove, MinimumThreshold, EffectiveFrom, EffectiveTo, IsActive)
VALUES
(
    'RULE-TA-002-0001',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'AGENT01-0000-0000-0000-000000000001', -- Travel Influencer
    'TourBooked',
    20.0,
    1, -- Percentage
    'Monthly',
    1,
    100.0,
    '2026-06-01 00:00:00',
    NULL,
    1
);

-- Tenant B: Independent Agent Commission (Wanderlust)
INSERT INTO CommissionRules (Id, TenantId, AgentId, Trigger, Amount, IsPercentage, PayoutFrequency, AutoApprove, MinimumThreshold, EffectiveFrom, EffectiveTo, IsActive)
VALUES
(
    'RULE-TB-002-0001',
    'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB',
    'AGENT01-0000-0000-0000-000000000002', -- Wanderlust
    'TourBooked',
    15.0,
    1, -- Percentage
    'Monthly',
    1,
    100.0,
    '2026-06-01 00:00:00',
    NULL,
    1
);

-- ============================================================================
-- 6. TOURS
-- ============================================================================

-- Tenant A Tours
INSERT INTO Tours (Id, TenantId, Name, Destination, Description, BasePrice, Duration, Status, CreatedAt)
VALUES
(
    'TOUR-TA-000-0001',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'Beach Paradise Package',
    'Maldives',
    'Luxury beach resort with water activities',
    500.0,
    '7 days',
    'Active',
    '2026-06-01 10:00:00'
);

INSERT INTO Tours (Id, TenantId, Name, Destination, Description, BasePrice, Duration, Status, CreatedAt)
VALUES
(
    'TOUR-TA-000-0002',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'Mountain Adventure',
    'Swiss Alps',
    'Hiking and scenic mountain tours',
    800.0,
    '5 days',
    'Active',
    '2026-06-01 10:00:00'
);

INSERT INTO Tours (Id, TenantId, Name, Destination, Description, BasePrice, Duration, Status, CreatedAt)
VALUES
(
    'TOUR-TA-000-0003',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'City Tour Package',
    'Paris',
    'Cultural and historical city tour',
    300.0,
    '3 days',
    'Active',
    '2026-06-01 10:00:00'
);

-- Tenant B Tours
INSERT INTO Tours (Id, TenantId, Name, Destination, Description, BasePrice, Duration, Status, CreatedAt)
VALUES
(
    'TOUR-TB-000-0001',
    'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB',
    'Desert Safari',
    'Arabian Desert',
    'Camel trekking and desert camping',
    600.0,
    '4 days',
    'Active',
    '2026-06-01 10:00:00'
);

INSERT INTO Tours (Id, TenantId, Name, Destination, Description, BasePrice, Duration, Status, CreatedAt)
VALUES
(
    'TOUR-TB-000-0002',
    'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB',
    'Jungle Trek',
    'Amazon Rainforest',
    'Wildlife and jungle exploration',
    700.0,
    '6 days',
    'Active',
    '2026-06-01 10:00:00'
);

-- ============================================================================
-- 7. BOOKINGS - TENANT A (STAFF AGENTS)
-- ============================================================================

-- John's Bookings (High performer - 15% commission)
-- Booking 1: June 5 (Completed)
INSERT INTO Bookings (Id, TenantId, TourId, StaffAgentId, CustomerId, BookingPrice, Status, BookedAt, CompletedAt)
VALUES
(
    'BOOKING-001',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'TOUR-TA-000-0001',
    'STAFF01-0000-0000-0000-000000000001', -- John
    'CUST-001',
    500.0,
    'Completed',
    '2026-06-05 10:00:00',
    '2026-06-12 18:00:00'
);

-- Booking 2: June 8 (Completed)
INSERT INTO Bookings (Id, TenantId, TourId, StaffAgentId, CustomerId, BookingPrice, Status, BookedAt, CompletedAt)
VALUES
(
    'BOOKING-002',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'TOUR-TA-000-0002',
    'STAFF01-0000-0000-0000-000000000001', -- John
    'CUST-002',
    800.0,
    'Completed',
    '2026-06-08 10:00:00',
    '2026-06-13 18:00:00'
);

-- Booking 3: June 10 (Pending - not completed yet)
INSERT INTO Bookings (Id, TenantId, TourId, StaffAgentId, CustomerId, BookingPrice, Status, BookedAt, CompletedAt)
VALUES
(
    'BOOKING-003',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'TOUR-TA-000-0003',
    'STAFF01-0000-0000-0000-000000000001', -- John
    'CUST-003',
    300.0,
    'Pending',
    '2026-06-10 10:00:00',
    NULL
);

-- Booking 4: June 15 (Cancelled - should reverse commission)
INSERT INTO Bookings (Id, TenantId, TourId, StaffAgentId, CustomerId, BookingPrice, Status, BookedAt, CompletedAt)
VALUES
(
    'BOOKING-004',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'TOUR-TA-000-0001',
    'STAFF01-0000-0000-0000-000000000001', -- John
    'CUST-004',
    500.0,
    'Cancelled',
    '2026-06-15 10:00:00',
    NULL
);

-- Jane's Bookings (8% commission)
-- Booking 5: June 6 (Completed)
INSERT INTO Bookings (Id, TenantId, TourId, StaffAgentId, CustomerId, BookingPrice, Status, BookedAt, CompletedAt)
VALUES
(
    'BOOKING-005',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'TOUR-TA-000-0001',
    'STAFF01-0000-0000-0000-000000000002', -- Jane
    'CUST-005',
    500.0,
    'Completed',
    '2026-06-06 10:00:00',
    '2026-06-13 18:00:00'
);

-- Booking 6: June 9 (Completed)
INSERT INTO Bookings (Id, TenantId, TourId, StaffAgentId, CustomerId, BookingPrice, Status, BookedAt, CompletedAt)
VALUES
(
    'BOOKING-006',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'TOUR-TA-000-0002',
    'STAFF01-0000-0000-0000-000000000002', -- Jane
    'CUST-006',
    800.0,
    'Completed',
    '2026-06-09 10:00:00',
    '2026-06-14 18:00:00'
);

-- Mike's Bookings (10% default commission - departing staff)
-- Booking 7: June 7 (Completed)
INSERT INTO Bookings (Id, TenantId, TourId, StaffAgentId, CustomerId, BookingPrice, Status, BookedAt, CompletedAt)
VALUES
(
    'BOOKING-007',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'TOUR-TA-000-0003',
    'STAFF01-0000-0000-0000-000000000003', -- Mike
    'CUST-007',
    300.0,
    'Completed',
    '2026-06-07 10:00:00',
    '2026-06-10 18:00:00'
);

-- Direct Customer Booking (No staff code)
-- Booking 8: June 11 (Completed - no commission)
INSERT INTO Bookings (Id, TenantId, TourId, StaffAgentId, CustomerId, BookingPrice, Status, BookedAt, CompletedAt)
VALUES
(
    'BOOKING-008',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'TOUR-TA-000-0001',
    NULL, -- No staff
    'CUST-008',
    500.0,
    'Completed',
    '2026-06-11 10:00:00',
    '2026-06-18 18:00:00'
);

-- ============================================================================
-- 8. BOOKINGS - INDEPENDENT AGENTS
-- ============================================================================

-- Travel Influencer bookings (20% commission)
-- Booking 9: June 4 (Completed)
INSERT INTO Bookings (Id, TenantId, TourId, IndependentAgentId, CustomerId, BookingPrice, Status, BookedAt, CompletedAt)
VALUES
(
    'BOOKING-009',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'TOUR-TA-000-0001',
    'AGENT01-0000-0000-0000-000000000001', -- Travel Influencer
    'CUST-009',
    550.0, -- $500 base + $50 markup
    'Completed',
    '2026-06-04 10:00:00',
    '2026-06-11 18:00:00'
);

-- Booking 10: June 12 (Completed)
INSERT INTO Bookings (Id, TenantId, TourId, IndependentAgentId, CustomerId, BookingPrice, Status, BookedAt, CompletedAt)
VALUES
(
    'BOOKING-010',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'TOUR-TA-000-0002',
    'AGENT01-0000-0000-0000-000000000001', -- Travel Influencer
    'CUST-010',
    800.0, -- No markup
    'Completed',
    '2026-06-12 10:00:00',
    '2026-06-17 18:00:00'
);

-- ============================================================================
-- 9. COMMISSION ACCRUALS (Results from bookings)
-- ============================================================================

-- John's completed bookings (15% rate)
-- Commission 1: Booking 001, $500 × 15% = $75
INSERT INTO CommissionAccruals (Id, AgentId, TenantId, CommissionRuleId, TriggerType, SourceId, CommissionAmount, CommissionPercentage, BaseAmount, Status, AccruedAt, PaidAt, PayoutId, DisputeTicketId)
VALUES
(
    'COMM-001',
    'STAFF01-0000-0000-0000-000000000001',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'RULE-TA-001-0002',
    'TourBooked',
    'BOOKING-001',
    75.0,
    15.0,
    500.0,
    'Finalized',
    '2026-06-05 10:00:00',
    '2026-07-01 10:00:00',
    'PAYOUT-TA-001',
    NULL
);

-- Commission 2: Booking 002, $800 × 15% = $120
INSERT INTO CommissionAccruals (Id, AgentId, TenantId, CommissionRuleId, TriggerType, SourceId, CommissionAmount, CommissionPercentage, BaseAmount, Status, AccruedAt, PaidAt, PayoutId, DisputeTicketId)
VALUES
(
    'COMM-002',
    'STAFF01-0000-0000-0000-000000000001',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'RULE-TA-001-0002',
    'TourBooked',
    'BOOKING-002',
    120.0,
    15.0,
    800.0,
    'Finalized',
    '2026-06-08 10:00:00',
    '2026-07-01 10:00:00',
    'PAYOUT-TA-001',
    NULL
);

-- Commission 3: Booking 003, $300 × 15% = $45 (PENDING - not completed)
INSERT INTO CommissionAccruals (Id, AgentId, TenantId, CommissionRuleId, TriggerType, SourceId, CommissionAmount, CommissionPercentage, BaseAmount, Status, AccruedAt, PaidAt, PayoutId, DisputeTicketId)
VALUES
(
    'COMM-003',
    'STAFF01-0000-0000-0000-000000000001',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'RULE-TA-001-0002',
    'TourBooked',
    'BOOKING-003',
    45.0,
    15.0,
    300.0,
    'Accrued',
    '2026-06-10 10:00:00',
    NULL,
    NULL,
    NULL
);

-- Commission 4: Booking 004, $500 × 15% = $75 (CANCELLED - should be reversed)
INSERT INTO CommissionAccruals (Id, AgentId, TenantId, CommissionRuleId, TriggerType, SourceId, CommissionAmount, CommissionPercentage, BaseAmount, Status, AccruedAt, PaidAt, PayoutId, DisputeTicketId)
VALUES
(
    'COMM-004',
    'STAFF01-0000-0000-0000-000000000001',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'RULE-TA-001-0002',
    'TourBooked',
    'BOOKING-004',
    75.0,
    15.0,
    500.0,
    'Reversed',
    '2026-06-15 10:00:00',
    NULL,
    NULL,
    NULL
);

-- Jane's completed bookings (8% rate)
-- Commission 5: Booking 005, $500 × 8% = $40
INSERT INTO CommissionAccruals (Id, AgentId, TenantId, CommissionRuleId, TriggerType, SourceId, CommissionAmount, CommissionPercentage, BaseAmount, Status, AccruedAt, PaidAt, PayoutId, DisputeTicketId)
VALUES
(
    'COMM-005',
    'STAFF01-0000-0000-0000-000000000002',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'RULE-TA-001-0003',
    'TourBooked',
    'BOOKING-005',
    40.0,
    8.0,
    500.0,
    'Finalized',
    '2026-06-06 10:00:00',
    '2026-07-01 10:00:00',
    'PAYOUT-TA-001',
    NULL
);

-- Commission 6: Booking 006, $800 × 8% = $64
INSERT INTO CommissionAccruals (Id, AgentId, TenantId, CommissionRuleId, TriggerType, SourceId, CommissionAmount, CommissionPercentage, BaseAmount, Status, AccruedAt, PaidAt, PayoutId, DisputeTicketId)
VALUES
(
    'COMM-006',
    'STAFF01-0000-0000-0000-000000000002',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'RULE-TA-001-0003',
    'TourBooked',
    'BOOKING-006',
    64.0,
    8.0,
    800.0,
    'Finalized',
    '2026-06-09 10:00:00',
    '2026-07-01 10:00:00',
    'PAYOUT-TA-001',
    NULL
);

-- Mike's booking (10% default - departing)
-- Commission 7: Booking 007, $300 × 10% = $30
INSERT INTO CommissionAccruals (Id, AgentId, TenantId, CommissionRuleId, TriggerType, SourceId, CommissionAmount, CommissionPercentage, BaseAmount, Status, AccruedAt, PaidAt, PayoutId, DisputeTicketId)
VALUES
(
    'COMM-007',
    'STAFF01-0000-0000-0000-000000000003',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'RULE-TA-001-0001',
    'TourBooked',
    'BOOKING-007',
    30.0,
    10.0,
    300.0,
    'Finalized',
    '2026-06-07 10:00:00',
    '2026-07-01 10:00:00',
    'PAYOUT-TA-001',
    NULL
);

-- Direct booking (no commission)
-- No commission accrual for BOOKING-008

-- Travel Influencer bookings (20% rate)
-- Commission 9: Booking 009, $550 × 20% = $110
INSERT INTO CommissionAccruals (Id, AgentId, TenantId, CommissionRuleId, TriggerType, SourceId, CommissionAmount, CommissionPercentage, BaseAmount, Status, AccruedAt, PaidAt, PayoutId, DisputeTicketId)
VALUES
(
    'COMM-009',
    'AGENT01-0000-0000-0000-000000000001',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'RULE-TA-002-0001',
    'TourBooked',
    'BOOKING-009',
    110.0,
    20.0,
    550.0,
    'Finalized',
    '2026-06-04 10:00:00',
    '2026-07-01 10:00:00',
    'PAYOUT-IND-001',
    NULL
);

-- Commission 10: Booking 010, $800 × 20% = $160
INSERT INTO CommissionAccruals (Id, AgentId, TenantId, CommissionRuleId, TriggerType, SourceId, CommissionAmount, CommissionPercentage, BaseAmount, Status, AccruedAt, PaidAt, PayoutId, DisputeTicketId)
VALUES
(
    'COMM-010',
    'AGENT01-0000-0000-0000-000000000001',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'RULE-TA-002-0001',
    'TourBooked',
    'BOOKING-010',
    160.0,
    20.0,
    800.0,
    'Finalized',
    '2026-06-12 10:00:00',
    '2026-07-01 10:00:00',
    'PAYOUT-IND-001',
    NULL
);

-- ============================================================================
-- 10. COMMISSION PAYOUTS (Monthly settlements)
-- ============================================================================

-- Tenant A June Payout
INSERT INTO CommissionPayouts (Id, AgentId, TenantId, PayoutPeriodStart, PayoutPeriodEnd, CommissionAccrualIds, TotalAmount, Status, ProcessedAt, TransactionId)
VALUES
(
    'PAYOUT-TA-001',
    NULL, -- Payout for multiple agents
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    '2026-06-01 00:00:00',
    '2026-06-30 23:59:59',
    'COMM-001,COMM-002,COMM-005,COMM-006,COMM-007',
    329.0,
    'Paid',
    '2026-07-01 10:00:00',
    'TXN-TA-JUN-001'
);

-- Independent Agent June Payout
INSERT INTO CommissionPayouts (Id, AgentId, TenantId, PayoutPeriodStart, PayoutPeriodEnd, CommissionAccrualIds, TotalAmount, Status, ProcessedAt, TransactionId)
VALUES
(
    'PAYOUT-IND-001',
    'AGENT01-0000-0000-0000-000000000001',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    '2026-06-01 00:00:00',
    '2026-06-30 23:59:59',
    'COMM-009,COMM-010',
    270.0,
    'Paid',
    '2026-07-01 10:00:00',
    'TXN-IND-JUN-001'
);

-- ============================================================================
-- SUMMARY OF TEST DATA
-- ============================================================================
/*
SUPERADMIN:
  ├─ You (litotjuliano@gmail.com)

ADMINS:
  ├─ Alice (Platform Admin)
  ├─ Bob (Tenant A Admin)
  └─ Carol (Tenant B Admin)

TENANTS:
  ├─ Tenant A: Travel Pro Inc
  └─ Tenant B: Adventure Tours Ltd

STAFF AGENTS (Tenant A):
  ├─ John Smith (STAFF-JOHN-001): 15% commission
  ├─ Jane Doe (STAFF-JANE-001): 8% commission
  └─ Mike Johnson (STAFF-MIKE-001): 10% commission (departing)

INDEPENDENT AGENTS:
  ├─ Travel Influencer (20% for Tenant A)
  └─ Wanderlust Travel (15% for Tenant B)

BOOKINGS (Tenant A - June):
  ├─ John: 4 bookings (2 completed, 1 pending, 1 cancelled)
  ├─ Jane: 2 bookings (both completed)
  ├─ Mike: 1 booking (completed)
  ├─ Direct: 1 booking (no commission)
  └─ Influencer: 2 bookings (both completed)

COMMISSIONS (Finalized & Paid):
  ├─ John: $195 finalized, $45 pending, $75 reversed
  ├─ Jane: $104 finalized
  ├─ Mike: $30 finalized
  ├─ Travel Influencer: $270 finalized
  └─ Total June Payout: $599

TEST SCENARIOS COVERED:
  ✓ High performer with custom rate (John 15%)
  ✓ Standard performer (Jane 8%)
  ✓ Departing staff (Mike)
  ✓ Pending commission (not completed)
  ✓ Cancelled booking (reversed commission)
  ✓ Direct customer (no code, no commission)
  ✓ Independent agent with markup (Influencer)
  ✓ Commission finalization on tour completion
  ✓ Monthly payout processing
*/
