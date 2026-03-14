# 🗄️ TechStore — Database Seed Scripts

## Files

| File | Purpose |
|------|---------|
| `0_Run_All_Seeds.sql` | Master runner — executes all 3 seeds in order |
| `1_Seed_Inventory.sql` | Creates & seeds `TechStore_Inventory` (50 products, 10 categories) |
| `2_Seed_Orders.sql` | Creates & seeds `TechStore_Orders` (12 orders, 30 line items) |
| `3_Seed_Payments.sql` | Creates & seeds `TechStore_Payments` (22 payment records) |
| `9_Reset_All_Databases.sql` | ⚠️ Drops all 3 databases (dev only) |

---

## How to run

### Option A — SSMS
1. Open `1_Seed_Inventory.sql` → Execute
2. Open `2_Seed_Orders.sql` → Execute
3. Open `3_Seed_Payments.sql` → Execute

### Option B — sqlcmd (terminal)
```bash
# From the /database/seeds folder:
sqlcmd -S localhost -E -i 1_Seed_Inventory.sql
sqlcmd -S localhost -E -i 2_Seed_Orders.sql
sqlcmd -S localhost -E -i 3_Seed_Payments.sql

# Or run all at once via master runner:
sqlcmd -S localhost -E -i 0_Run_All_Seeds.sql
```

### With username/password
```bash
sqlcmd -S localhost -U sa -P YourPassword -i 1_Seed_Inventory.sql
```

### Azure SQL
```bash
sqlcmd -S yourserver.database.windows.net -U adminuser -P YourPass -i 1_Seed_Inventory.sql
```

---

## What gets seeded

### TechStore_Inventory — 50 Products across 10 categories

| Category | Count |
|----------|-------|
| Laptops | 5 |
| Phones | 5 |
| Audio | 4 |
| Wearables | 5 |
| Tablets | 4 |
| Monitors | 4 |
| Accessories | 6 |
| Gaming | 5 |
| Cameras | 4 |
| Smart Home | 3 |
| Networking | 2 |
| Storage | 3 |

### TechStore_Orders — 12 Orders + 30 Line Items

| User | Orders | Status mix |
|------|--------|------------|
| john@example.com | 4 | Delivered, Shipped, Confirmed |
| jane@example.com | 3 | Delivered, Processing |
| admin@example.com | 2 | Delivered, Shipped |
| demo@example.com | 3 | Delivered, Confirmed |

### TechStore_Payments — 22 Payment Records

- 18 successful payments (all linked to seeded orders + 6 historical)
- 4 failed payment attempts (no linked order)
- Payment references match `Orders.PaymentReference` exactly

---

## Scripts are idempotent

All scripts use `MERGE` statements — safe to re-run without creating duplicates. They will update existing records if values have changed.

---

## Connection strings

Update `appsettings.json` in each API:

```json
// Windows Auth (local dev)
"DefaultConnection": "Server=localhost;Database=TechStore_Inventory;Trusted_Connection=True;TrustServerCertificate=True;"

// SQL Login
"DefaultConnection": "Server=localhost;Database=TechStore_Inventory;User Id=sa;Password=YourPass;TrustServerCertificate=True;"

// Azure SQL
"DefaultConnection": "Server=yourserver.database.windows.net;Database=TechStore_Inventory;User Id=user@server;Password=YourPass;"
```
