-- ============================================================
--  TechStore — Payments Database Seed Script
--  Database  : TechStore_Payments
--  Table     : Payments
--  Compatible: SQL Server 2019+ / Azure SQL
--  Run order : 3_Seed_Payments.sql
--  Note      : PaymentReference values must match those in
--              TechStore_Orders — they link the two services.
-- ============================================================

USE master;
GO

-- ── 1. Create database ───────────────────────────────────────────────────────
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TechStore_Payments')
BEGIN
    CREATE DATABASE TechStore_Payments
        COLLATE SQL_Latin1_General_CP1_CI_AS;
    PRINT '✅ Database TechStore_Payments created.';
END
ELSE
    PRINT '⚠️  Database TechStore_Payments already exists — skipping creation.';
GO

USE TechStore_Payments;
GO

-- ── 2. EF Migrations table ───────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId    NVARCHAR(150) NOT NULL CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY,
        ProductVersion NVARCHAR(32)  NOT NULL
    );
END
GO

-- ── 3. Create Payments table ─────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Payments')
BEGIN
    CREATE TABLE Payments (
        PaymentReference NVARCHAR(100) NOT NULL CONSTRAINT PK_Payments PRIMARY KEY,
        UserId           NVARCHAR(50)  NOT NULL,
        Amount           DECIMAL(18,2) NOT NULL,
        Currency         NVARCHAR(10)  NOT NULL DEFAULT 'USD',
        CardLastFour     NVARCHAR(4)   NOT NULL,
        Status           NVARCHAR(20)  NOT NULL,       -- Success | Failed
        FailureReason    NVARCHAR(500) NULL,
        ProcessedAt      DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
        PaymentMethod    NVARCHAR(50)  NOT NULL DEFAULT 'Credit Card',
        OrderId          NVARCHAR(50)  NULL           -- linked post order creation
    );

    CREATE INDEX IX_Payments_UserId      ON Payments(UserId);
    CREATE INDEX IX_Payments_ProcessedAt ON Payments(ProcessedAt DESC);
    CREATE INDEX IX_Payments_Status      ON Payments(Status);
    CREATE INDEX IX_Payments_OrderId     ON Payments(OrderId) WHERE OrderId IS NOT NULL;

    PRINT '✅ Payments table created.';
END
GO

-- ── 4. Seed Payments (idempotent MERGE) ─────────────────────────────────────
-- PaymentReference must match Orders.PaymentReference in TechStore_Orders
-- CardLastFour: 4242 = Visa success, 5555 = Mastercard success
--               0000 = always fails (test card)

MERGE Payments AS target
USING (VALUES

    -- ── Successful payments (linked to seeded orders) ────────────────────────

    -- John Smith (USR-001)
    ('PAY-20250110091500-1001', 'USR-001', 2349.98, 'USD', '4242', 'Success', NULL,
     '2025-01-10T09:14:55', 'Credit Card',  'ORD-20250110-11001'),

    ('PAY-20250205143000-1002', 'USR-001',  449.98, 'USD', '4242', 'Success', NULL,
     '2025-02-05T14:29:55', 'Credit Card',  'ORD-20250205-11002'),

    ('PAY-20250318100000-1003', 'USR-001', 1199.99, 'USD', '4242', 'Success', NULL,
     '2025-03-18T09:59:55', 'Credit Card',  'ORD-20250318-11003'),

    ('PAY-20250402164500-1004', 'USR-001',  629.98, 'USD', '4242', 'Success', NULL,
     '2025-04-02T16:44:55', 'Debit Card',   'ORD-20250402-11004'),

    -- Jane Doe (USR-002)
    ('PAY-20250115110000-2001', 'USR-002', 1449.98, 'USD', '5555', 'Success', NULL,
     '2025-01-15T10:59:55', 'Credit Card',  'ORD-20250115-22001'),

    ('PAY-20250228093000-2002', 'USR-002',  349.99, 'USD', '5555', 'Success', NULL,
     '2025-02-28T09:29:55', 'Credit Card',  'ORD-20250228-22002'),

    ('PAY-20250410130000-2003', 'USR-002', 2799.98, 'USD', '5555', 'Success', NULL,
     '2025-04-10T12:59:55', 'Credit Card',  'ORD-20250410-22003'),

    -- Admin User (USR-003)
    ('PAY-20250120080000-3001', 'USR-003', 3099.98, 'USD', '3782', 'Success', NULL,
     '2025-01-20T07:59:55', 'Credit Card',  'ORD-20250120-33001'),

    ('PAY-20250325152000-3002', 'USR-003',  599.99, 'USD', '3782', 'Success', NULL,
     '2025-03-25T15:19:55', 'Debit Card',   'ORD-20250325-33002'),

    -- Demo User (USR-004)
    ('PAY-20250201103000-4001', 'USR-004', 1699.98, 'USD', '6011', 'Success', NULL,
     '2025-02-01T10:29:55', 'Credit Card',  'ORD-20250201-44001'),

    ('PAY-20250315120000-4002', 'USR-004',  279.98, 'USD', '6011', 'Success', NULL,
     '2025-03-15T11:59:55', 'Debit Card',   'ORD-20250315-44002'),

    ('PAY-20250408170000-4003', 'USR-004',  849.99, 'USD', '6011', 'Success', NULL,
     '2025-04-08T16:59:55', 'Credit Card',  'ORD-20250408-44003'),

    -- ── Failed payment attempts (no linked order) ────────────────────────────

    -- John — declined card attempt before successful order
    ('PAY-20250110090000-FAIL1', 'USR-001', 2349.98, 'USD', '0000', 'Failed',
     'Card declined. Insufficient funds.',
     '2025-01-10T09:00:00', 'Credit Card', NULL),

    -- Jane — wrong CVV attempt
    ('PAY-20250115105000-FAIL2', 'USR-002', 1449.98, 'USD', '1234', 'Failed',
     'Card declined. Please check your card details.',
     '2025-01-15T10:50:00', 'Credit Card', NULL),

    -- Demo — expired card
    ('PAY-20250408165000-FAIL3', 'USR-004',  849.99, 'USD', '9999', 'Failed',
     'Card declined. Card has expired.',
     '2025-04-08T16:50:00', 'Debit Card', NULL),

    -- Anonymous test failure (card ending 0000)
    ('PAY-20250301120000-FAIL4', 'USR-002',  499.99, 'USD', '0000', 'Failed',
     'Card declined. Please check your card details.',
     '2025-03-01T12:00:00', 'Credit Card', NULL),

    -- ── Additional successful payments (browsing history / stress data) ──────

    -- John — older purchases
    ('PAY-20241201090000-1000', 'USR-001',  599.99, 'USD', '4242', 'Success', NULL,
     '2024-12-01T09:00:00', 'Credit Card', NULL),

    ('PAY-20241115140000-1099', 'USR-001', 1099.99, 'USD', '4242', 'Success', NULL,
     '2024-11-15T14:00:00', 'Credit Card', NULL),

    -- Jane — older purchases
    ('PAY-20241205100000-2099', 'USR-002',  799.99, 'USD', '5555', 'Success', NULL,
     '2024-12-05T10:00:00', 'Credit Card', NULL),

    ('PAY-20241020115000-2098', 'USR-002',  349.99, 'USD', '5555', 'Success', NULL,
     '2024-10-20T11:50:00', 'Debit Card',  NULL),

    -- Admin — bulk purchase
    ('PAY-20241210080000-3099', 'USR-003', 4599.97, 'USD', '3782', 'Success', NULL,
     '2024-12-10T08:00:00', 'Credit Card', NULL),

    -- Demo — first ever order
    ('PAY-20241101120000-4099', 'USR-004',  109.99, 'USD', '6011', 'Success', NULL,
     '2024-11-01T12:00:00', 'Debit Card',  NULL)

) AS source (
    PaymentReference, UserId, Amount, Currency, CardLastFour,
    Status, FailureReason, ProcessedAt, PaymentMethod, OrderId
)
ON target.PaymentReference = source.PaymentReference
WHEN MATCHED THEN
    UPDATE SET
        Status        = source.Status,
        FailureReason = source.FailureReason,
        OrderId       = source.OrderId
WHEN NOT MATCHED THEN
    INSERT (PaymentReference, UserId, Amount, Currency, CardLastFour,
            Status, FailureReason, ProcessedAt, PaymentMethod, OrderId)
    VALUES (source.PaymentReference, source.UserId, source.Amount, source.Currency,
            source.CardLastFour, source.Status, source.FailureReason,
            source.ProcessedAt, source.PaymentMethod, source.OrderId);
GO

-- ── 5. Register migration ────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20250101000001_InitialCreate')
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250101000001_InitialCreate', '9.0.0');
GO

-- ── 6. Summary ───────────────────────────────────────────────────────────────
-- Revenue by status
SELECT
    Status,
    COUNT(*)        AS Transactions,
    SUM(Amount)     AS TotalAmount,
    AVG(Amount)     AS AvgAmount
FROM Payments
GROUP BY Status
ORDER BY Status;
GO

-- Revenue by user
SELECT
    UserId,
    COUNT(*)                                    AS Attempts,
    SUM(CASE WHEN Status = 'Success' THEN 1 ELSE 0 END) AS Successful,
    SUM(CASE WHEN Status = 'Failed'  THEN 1 ELSE 0 END) AS Failed,
    SUM(CASE WHEN Status = 'Success' THEN Amount ELSE 0 END) AS TotalPaid
FROM Payments
GROUP BY UserId
ORDER BY TotalPaid DESC;
GO

-- Revenue by payment method
SELECT
    PaymentMethod,
    COUNT(*)    AS Count,
    SUM(Amount) AS Total
FROM Payments
WHERE Status = 'Success'
GROUP BY PaymentMethod;
GO

PRINT '✅ TechStore_Payments seed complete — '
    + CAST((SELECT COUNT(*) FROM Payments WHERE Status = 'Success') AS NVARCHAR) + ' successful, '
    + CAST((SELECT COUNT(*) FROM Payments WHERE Status = 'Failed')  AS NVARCHAR) + ' failed payments loaded.';
GO
