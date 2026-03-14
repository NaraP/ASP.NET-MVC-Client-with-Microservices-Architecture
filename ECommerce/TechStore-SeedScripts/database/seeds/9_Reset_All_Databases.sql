-- ============================================================
--  TechStore — Database Reset Script
--  ⚠️  WARNING: This permanently drops all TechStore databases.
--  Use only in development / local environments.
-- ============================================================

USE master;
GO

-- ── Drop TechStore_Inventory ─────────────────────────────────────────────────
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'TechStore_Inventory')
BEGIN
    ALTER DATABASE TechStore_Inventory SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TechStore_Inventory;
    PRINT '🗑️  TechStore_Inventory dropped.';
END
GO

-- ── Drop TechStore_Orders ────────────────────────────────────────────────────
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'TechStore_Orders')
BEGIN
    ALTER DATABASE TechStore_Orders SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TechStore_Orders;
    PRINT '🗑️  TechStore_Orders dropped.';
END
GO

-- ── Drop TechStore_Payments ──────────────────────────────────────────────────
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'TechStore_Payments')
BEGIN
    ALTER DATABASE TechStore_Payments SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TechStore_Payments;
    PRINT '🗑️  TechStore_Payments dropped.';
END
GO

PRINT '✅ All TechStore databases removed. Run 0_Run_All_Seeds.sql to recreate.';
GO
