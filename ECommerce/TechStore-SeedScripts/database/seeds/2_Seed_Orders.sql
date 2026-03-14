-- ============================================================
--  TechStore — Orders Database Seed Script
--  Database  : TechStore_Orders
--  Tables    : Orders, OrderItems
--  Compatible: SQL Server 2019+ / Azure SQL
--  Run order : 2_Seed_Orders.sql
-- ============================================================

USE master;
GO

-- ── 1. Create database ───────────────────────────────────────────────────────
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TechStore_Orders')
BEGIN
    CREATE DATABASE TechStore_Orders
        COLLATE SQL_Latin1_General_CP1_CI_AS;
    PRINT '✅ Database TechStore_Orders created.';
END
ELSE
    PRINT '⚠️  Database TechStore_Orders already exists — skipping creation.';
GO

USE TechStore_Orders;
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

-- ── 3. Create Orders table ───────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
BEGIN
    CREATE TABLE Orders (
        OrderId           NVARCHAR(50)  NOT NULL CONSTRAINT PK_Orders PRIMARY KEY,
        UserId            NVARCHAR(50)  NOT NULL,
        UserEmail         NVARCHAR(200) NOT NULL,
        TotalAmount       DECIMAL(18,2) NOT NULL,
        Status            NVARCHAR(50)  NOT NULL DEFAULT 'Confirmed',
        CreatedAt         DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
        EstimatedDelivery DATETIME2     NOT NULL,
        ShippingAddress   NVARCHAR(500) NOT NULL,
        PaymentReference  NVARCHAR(100) NOT NULL DEFAULT ''
    );

    CREATE INDEX IX_Orders_UserId    ON Orders(UserId);
    CREATE INDEX IX_Orders_CreatedAt ON Orders(CreatedAt DESC);
    CREATE INDEX IX_Orders_Status    ON Orders(Status);

    PRINT '✅ Orders table created.';
END
GO

-- ── 4. Create OrderItems table ───────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrderItems')
BEGIN
    CREATE TABLE OrderItems (
        Id          INT           NOT NULL IDENTITY(1,1) CONSTRAINT PK_OrderItems PRIMARY KEY,
        OrderId     NVARCHAR(50)  NOT NULL,
        ProductId   INT           NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        Quantity    INT           NOT NULL DEFAULT 1,
        UnitPrice   DECIMAL(18,2) NOT NULL,
        TotalPrice  DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_OrderItems_Orders_OrderId
            FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE
    );

    CREATE INDEX IX_OrderItems_OrderId   ON OrderItems(OrderId);
    CREATE INDEX IX_OrderItems_ProductId ON OrderItems(ProductId);

    PRINT '✅ OrderItems table created.';
END
GO

-- ── 5. Seed Orders (idempotent) ──────────────────────────────────────────────
-- Covers 4 users, multiple statuses, varied date ranges

-- User reference:
--   USR-001 = john@example.com   (John Smith)
--   USR-002 = jane@example.com   (Jane Doe)
--   USR-003 = admin@example.com  (Admin User)
--   USR-004 = demo@example.com   (Demo User)

MERGE Orders AS target
USING (VALUES

    -- ── John Smith (USR-001) — 4 orders ─────────────────────────────────────
    ('ORD-20250110-11001', 'USR-001', 'john@example.com', 2349.98, 'Delivered',
     '2025-01-10T09:15:00', '2025-01-15T18:00:00',
     'John Smith, 123 Oak Street, New York, NY 10001, United States',
     'PAY-20250110091500-1001'),

    ('ORD-20250205-11002', 'USR-001', 'john@example.com', 449.98, 'Delivered',
     '2025-02-05T14:30:00', '2025-02-10T18:00:00',
     'John Smith, 123 Oak Street, New York, NY 10001, United States',
     'PAY-20250205143000-1002'),

    ('ORD-20250318-11003', 'USR-001', 'john@example.com', 1199.99, 'Shipped',
     '2025-03-18T10:00:00', '2025-03-23T18:00:00',
     'John Smith, 123 Oak Street, New York, NY 10001, United States',
     'PAY-20250318100000-1003'),

    ('ORD-20250402-11004', 'USR-001', 'john@example.com', 629.98, 'Confirmed',
     '2025-04-02T16:45:00', '2025-04-07T18:00:00',
     'John Smith, 123 Oak Street, New York, NY 10001, United States',
     'PAY-20250402164500-1004'),

    -- ── Jane Doe (USR-002) — 3 orders ───────────────────────────────────────
    ('ORD-20250115-22001', 'USR-002', 'jane@example.com', 1449.98, 'Delivered',
     '2025-01-15T11:00:00', '2025-01-20T18:00:00',
     'Jane Doe, 456 Maple Avenue, Los Angeles, CA 90210, United States',
     'PAY-20250115110000-2001'),

    ('ORD-20250228-22002', 'USR-002', 'jane@example.com', 349.99, 'Delivered',
     '2025-02-28T09:30:00', '2025-03-05T18:00:00',
     'Jane Doe, 456 Maple Avenue, Los Angeles, CA 90210, United States',
     'PAY-20250228093000-2002'),

    ('ORD-20250410-22003', 'USR-002', 'jane@example.com', 2799.98, 'Processing',
     '2025-04-10T13:00:00', '2025-04-15T18:00:00',
     'Jane Doe, 456 Maple Avenue, Los Angeles, CA 90210, United States',
     'PAY-20250410130000-2003'),

    -- ── Admin User (USR-003) — 2 orders ─────────────────────────────────────
    ('ORD-20250120-33001', 'USR-003', 'admin@example.com', 3099.98, 'Delivered',
     '2025-01-20T08:00:00', '2025-01-25T18:00:00',
     'Admin User, 789 Pine Road, Austin, TX 78701, United States',
     'PAY-20250120080000-3001'),

    ('ORD-20250325-33002', 'USR-003', 'admin@example.com', 599.99, 'Shipped',
     '2025-03-25T15:20:00', '2025-03-30T18:00:00',
     'Admin User, 789 Pine Road, Austin, TX 78701, United States',
     'PAY-20250325152000-3002'),

    -- ── Demo User (USR-004) — 3 orders ──────────────────────────────────────
    ('ORD-20250201-44001', 'USR-004', 'demo@example.com', 1699.98, 'Delivered',
     '2025-02-01T10:30:00', '2025-02-06T18:00:00',
     'Demo User, 321 Birch Lane, Chicago, IL 60601, United States',
     'PAY-20250201103000-4001'),

    ('ORD-20250315-44002', 'USR-004', 'demo@example.com', 279.98, 'Delivered',
     '2025-03-15T12:00:00', '2025-03-20T18:00:00',
     'Demo User, 321 Birch Lane, Chicago, IL 60601, United States',
     'PAY-20250315120000-4002'),

    ('ORD-20250408-44003', 'USR-004', 'demo@example.com', 849.99, 'Confirmed',
     '2025-04-08T17:00:00', '2025-04-13T18:00:00',
     'Demo User, 321 Birch Lane, Chicago, IL 60601, United States',
     'PAY-20250408170000-4003')

) AS source (
    OrderId, UserId, UserEmail, TotalAmount, Status,
    CreatedAt, EstimatedDelivery, ShippingAddress, PaymentReference
)
ON target.OrderId = source.OrderId
WHEN MATCHED THEN
    UPDATE SET
        Status            = source.Status,
        TotalAmount       = source.TotalAmount,
        ShippingAddress   = source.ShippingAddress,
        PaymentReference  = source.PaymentReference
WHEN NOT MATCHED THEN
    INSERT (OrderId, UserId, UserEmail, TotalAmount, Status, CreatedAt, EstimatedDelivery, ShippingAddress, PaymentReference)
    VALUES (source.OrderId, source.UserId, source.UserEmail, source.TotalAmount, source.Status,
            source.CreatedAt, source.EstimatedDelivery, source.ShippingAddress, source.PaymentReference);
GO

-- ── 6. Seed OrderItems ───────────────────────────────────────────────────────
-- Clear and re-insert (items are always tied to their order)
DELETE FROM OrderItems WHERE OrderId IN (
    'ORD-20250110-11001','ORD-20250205-11002','ORD-20250318-11003','ORD-20250402-11004',
    'ORD-20250115-22001','ORD-20250228-22002','ORD-20250410-22003',
    'ORD-20250120-33001','ORD-20250325-33002',
    'ORD-20250201-44001','ORD-20250315-44002','ORD-20250408-44003'
);

INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, UnitPrice, TotalPrice)
VALUES
    -- ORD-20250110-11001: John — MacBook Pro + AirPods Pro
    ('ORD-20250110-11001',  1, 'MacBook Pro 14"',      1, 1999.99, 1999.99),
    ('ORD-20250110-11001', 12, 'Apple AirPods Pro 3',  1,  249.99,  249.99),

    -- ORD-20250205-11002: John — Sony WH-1000XM5 + Logitech MX Keys S
    ('ORD-20250205-11002', 11, 'Sony WH-1000XM5',      1,  349.99,  349.99),
    ('ORD-20250205-11002', 28, 'Logitech MX Keys S',   1,   99.99,   99.99),

    -- ORD-20250318-11003: John — iPhone 16 Pro
    ('ORD-20250318-11003',  6, 'iPhone 16 Pro',        1, 1199.99, 1199.99),

    -- ORD-20250402-11004: John — Razer DeathAdder V3 + Keychron Q1 Pro
    ('ORD-20250402-11004', 30, 'Razer DeathAdder V3',  1,   79.99,   79.99),
    ('ORD-20250402-11004', 32, 'Keychron Q1 Pro',      1,  199.99,  199.99),
    ('ORD-20250402-11004', 33, 'Anker 778 USB-C Hub',  1,   59.99,   59.99),
    ('ORD-20250402-11004', 28, 'Logitech MX Keys S',   3,  119.99,  359.97),

    -- ORD-20250115-22001: Jane — iPad Pro 13" + Apple Pencil compatible
    ('ORD-20250115-22001', 20, 'iPad Pro 13"',         1, 1299.99, 1299.99),
    ('ORD-20250115-22001', 16, 'Apple Watch Series 10',1,  399.99,  399.99),

    -- ORD-20250228-22002: Jane — Nintendo Switch OLED
    ('ORD-20250228-22002', 36, 'Nintendo Switch OLED', 1,  349.99,  349.99),

    -- ORD-20250410-22003: Jane — Sony Alpha A7 IV + GoPro Hero 13
    ('ORD-20250410-22003', 39, 'Sony Alpha A7 IV',     1, 2499.99, 2499.99),
    ('ORD-20250410-22003', 40, 'GoPro Hero 13 Black',  1,  399.99,  399.99),

    -- ORD-20250120-33001: Admin — MacBook Pro + LG 4K Monitor + MX Keys S
    ('ORD-20250120-33001',  1, 'MacBook Pro 14"',      1, 1999.99, 1999.99),
    ('ORD-20250120-33001', 24, 'LG 27" 4K UltraFine',  1,  699.99,  699.99),
    ('ORD-20250120-33001', 28, 'Logitech MX Keys S',   2,  119.99,  239.98),
    ('ORD-20250120-33001', 29, 'Logitech MX Master 3S',1,   99.99,   99.99),

    -- ORD-20250325-33002: Admin — Samsung Odyssey G7 Monitor
    ('ORD-20250325-33002', 26, 'Samsung Odyssey G7',   1,  599.99,  599.99),

    -- ORD-20250201-44001: Demo — Dell XPS 15 + Sony Headphones
    ('ORD-20250201-44001',  3, 'Dell XPS 15',          1, 1499.99, 1499.99),
    ('ORD-20250201-44001', 11, 'Sony WH-1000XM5',      1,  349.99,  349.99),

    -- ORD-20250315-44002: Demo — Fitbit Charge 6 + Anker Hub
    ('ORD-20250315-44002', 19, 'Fitbit Charge 6',      1,  159.99,  159.99),
    ('ORD-20250315-44002', 33, 'Anker 778 USB-C Hub',  2,   59.99,  119.98),

    -- ORD-20250408-44003: Demo — Steam Deck OLED
    ('ORD-20250408-44003', 37, 'Steam Deck OLED',      1,  549.99,  549.99),
    ('ORD-20250408-44003', 38, 'Razer Kishi V2 Pro',   1,   99.99,   99.99),
    ('ORD-20250408-44003', 30, 'Razer DeathAdder V3',  2,   79.99,  159.98),
    ('ORD-20250408-44003', 49, 'WD Black SN850X 2TB',  1,  169.99,  169.99);
GO

-- ── 7. Register migration ────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20250101000001_InitialCreate')
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250101000001_InitialCreate', '9.0.0');
GO

-- ── 8. Summary ───────────────────────────────────────────────────────────────
SELECT
    o.Status,
    COUNT(DISTINCT o.OrderId)  AS OrderCount,
    COUNT(i.Id)                AS TotalItems,
    SUM(o.TotalAmount)         AS TotalRevenue,
    AVG(o.TotalAmount)         AS AvgOrderValue
FROM Orders o
LEFT JOIN OrderItems i ON o.OrderId = i.OrderId
GROUP BY o.Status
ORDER BY o.Status;
GO

SELECT
    o.UserId,
    o.UserEmail,
    COUNT(DISTINCT o.OrderId) AS Orders,
    SUM(o.TotalAmount)        AS TotalSpent
FROM Orders o
GROUP BY o.UserId, o.UserEmail
ORDER BY TotalSpent DESC;
GO

PRINT '✅ TechStore_Orders seed complete — '
    + CAST((SELECT COUNT(*) FROM Orders)     AS NVARCHAR) + ' orders, '
    + CAST((SELECT COUNT(*) FROM OrderItems) AS NVARCHAR) + ' line items loaded.';
GO
