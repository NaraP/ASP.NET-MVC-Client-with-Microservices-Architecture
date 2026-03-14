-- ============================================================
--  TechStore — Master Seed Runner
--  Runs all three database seed scripts in the correct order.
--
--  Usage (SSMS):    Open and execute this file.
--  Usage (sqlcmd):  sqlcmd -S localhost -E -i 0_Run_All_Seeds.sql
--
--  Order matters:
--    1. Inventory  → products (no dependencies)
--    2. Orders     → orders + items (references product IDs logically)
--    3. Payments   → payments (PaymentReference links to Orders)
-- ============================================================

PRINT '========================================';
PRINT ' TechStore Database Seed — Starting...  ';
PRINT '========================================';
PRINT '';
GO

-- ── Seed 1: Inventory ────────────────────────────────────────────────────────
PRINT '--- [1/3] Seeding TechStore_Inventory ---';
:r 1_Seed_Inventory.sql
GO

-- ── Seed 2: Orders ───────────────────────────────────────────────────────────
PRINT '--- [2/3] Seeding TechStore_Orders ---';
:r 2_Seed_Orders.sql
GO

-- ── Seed 3: Payments ─────────────────────────────────────────────────────────
PRINT '--- [3/3] Seeding TechStore_Payments ---';
:r 3_Seed_Payments.sql
GO

PRINT '';
PRINT '========================================';
PRINT ' ✅ All seeds completed successfully!   ';
PRINT '========================================';
GO
