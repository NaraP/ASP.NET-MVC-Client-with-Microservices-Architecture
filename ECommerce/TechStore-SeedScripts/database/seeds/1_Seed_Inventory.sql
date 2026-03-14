-- ============================================================
--  TechStore — Inventory Database Seed Script
--  Database  : TechStore_Inventory
--  Table     : Products
--  Compatible: SQL Server 2019+ / Azure SQL
--  Run order : 1_Seed_Inventory.sql
-- ============================================================

USE master;
GO

-- ── 1. Create database if it doesn't exist ───────────────────────────────────
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TechStore_Inventory')
BEGIN
    CREATE DATABASE TechStore_Inventory
        COLLATE SQL_Latin1_General_CP1_CI_AS;
    PRINT '✅ Database TechStore_Inventory created.';
END
ELSE
    PRINT '⚠️  Database TechStore_Inventory already exists — skipping creation.';
GO

USE TechStore_Inventory;
GO

-- ── 2. Create __EFMigrationsHistory (required by EF Core) ───────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_NAME = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId    NVARCHAR(150) NOT NULL CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY,
        ProductVersion NVARCHAR(32)  NOT NULL
    );
    PRINT '✅ __EFMigrationsHistory table created.';
END
GO

-- ── 3. Create Products table ─────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Products')
BEGIN
    CREATE TABLE Products (
        Id          INT              NOT NULL IDENTITY(1,1) CONSTRAINT PK_Products PRIMARY KEY,
        Name        NVARCHAR(200)    NOT NULL,
        Category    NVARCHAR(100)    NOT NULL,
        Price       DECIMAL(18,2)    NOT NULL,
        Stock       INT              NOT NULL DEFAULT 0,
        Description NVARCHAR(500)    NOT NULL DEFAULT '',
        ImageUrl    NVARCHAR(500)    NOT NULL DEFAULT '',
        Rating      DECIMAL(3,1)     NOT NULL DEFAULT 0.0,
        IsActive    BIT              NOT NULL DEFAULT 1,
        CreatedAt   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt   DATETIME2        NULL
    );

    CREATE INDEX IX_Products_Category ON Products(Category);
    CREATE INDEX IX_Products_Name     ON Products(Name);

    PRINT '✅ Products table created with indexes.';
END
ELSE
    PRINT '⚠️  Products table already exists — skipping creation.';
GO

-- ── 4. Seed products (idempotent — safe to re-run) ───────────────────────────
SET IDENTITY_INSERT Products ON;

MERGE Products AS target
USING (VALUES
    -- ── Laptops ──────────────────────────────────────────────────────────────
    ( 1, 'MacBook Pro 14"',         'Laptops',     1999.99, 15, 'Apple M3 Pro chip, 18GB RAM, 512GB SSD. The most powerful MacBook ever for professional workflows.',       'https://placehold.co/300x200?text=MacBook+Pro',       4.9, 1, '2025-01-01'),
    ( 2, 'MacBook Air 15"',         'Laptops',     1299.99, 22, 'Apple M3 chip, 8GB RAM, 256GB SSD. Incredibly thin and light with all-day battery life.',                 'https://placehold.co/300x200?text=MacBook+Air+15',    4.7, 1, '2025-01-01'),
    ( 3, 'Dell XPS 15',             'Laptops',     1499.99, 20, 'Intel Core i7-13700H, 16GB RAM, 512GB NVMe SSD, OLED 3.5K touchscreen display.',                         'https://placehold.co/300x200?text=Dell+XPS+15',       4.7, 1, '2025-01-01'),
    ( 4, 'Lenovo ThinkPad X1 Carbon','Laptops',    1349.99, 18, 'Intel Core i7, 16GB RAM, 1TB SSD. Business ultrabook with legendary ThinkPad reliability.',               'https://placehold.co/300x200?text=ThinkPad+X1',       4.6, 1, '2025-01-01'),
    ( 5, 'ASUS ROG Zephyrus G14',   'Laptops',     1599.99, 12, 'AMD Ryzen 9, RTX 4060, 16GB RAM, 1TB SSD. Compact gaming powerhouse with 165Hz display.',                'https://placehold.co/300x200?text=ROG+Zephyrus',      4.8, 1, '2025-01-01'),

    -- ── Phones ───────────────────────────────────────────────────────────────
    ( 6, 'iPhone 16 Pro',           'Phones',      1199.99, 50, 'A18 Pro chip, 256GB, 48MP camera system, Titanium finish, Dynamic Island.',                              'https://placehold.co/300x200?text=iPhone+16+Pro',    4.8, 1, '2025-01-01'),
    ( 7, 'iPhone 16',               'Phones',       799.99, 65, 'A18 chip, 128GB, 48MP main camera, Action button, USB-C.',                                               'https://placehold.co/300x200?text=iPhone+16',        4.6, 1, '2025-01-01'),
    ( 8, 'Samsung Galaxy S25 Ultra','Phones',      1299.99, 40, 'Snapdragon 8 Elite, 12GB RAM, 512GB, 200MP camera, built-in S Pen.',                                      'https://placehold.co/300x200?text=S25+Ultra',        4.8, 1, '2025-01-01'),
    ( 9, 'Samsung Galaxy S25',      'Phones',       999.99, 45, 'Snapdragon 8 Elite, 12GB RAM, 256GB, pro-grade camera with AI features.',                                 'https://placehold.co/300x200?text=Samsung+S25',      4.6, 1, '2025-01-01'),
    (10, 'Google Pixel 9 Pro',      'Phones',      1099.99, 30, 'Google Tensor G4, 12GB RAM, 256GB, Magic Eraser, 7 years of updates.',                                    'https://placehold.co/300x200?text=Pixel+9+Pro',      4.7, 1, '2025-01-01'),

    -- ── Audio ────────────────────────────────────────────────────────────────
    (11, 'Sony WH-1000XM5',         'Audio',        349.99, 30, 'Industry-leading noise cancellation, 30-hour battery, multipoint connection, Hi-Res Audio.',              'https://placehold.co/300x200?text=Sony+WH1000XM5',   4.8, 1, '2025-01-01'),
    (12, 'Apple AirPods Pro 3',     'Audio',        249.99, 55, 'Active Noise Cancellation, Transparency mode, Personalised Spatial Audio, USB-C charging.',               'https://placehold.co/300x200?text=AirPods+Pro+3',    4.7, 1, '2025-01-01'),
    (13, 'Bose QuietComfort 45',    'Audio',        279.99, 25, 'High-fidelity audio with world-class noise cancellation, 24-hour battery.',                                'https://placehold.co/300x200?text=Bose+QC45',        4.6, 1, '2025-01-01'),
    (14, 'Sennheiser Momentum 4',   'Audio',        349.99, 18, '60-hour battery life, adaptive ANC, crystal-clear call quality.',                                          'https://placehold.co/300x200?text=Sennheiser+M4',    4.7, 1, '2025-01-01'),

    -- ── Wearables ────────────────────────────────────────────────────────────
    (15, 'Apple Watch Ultra 2',     'Wearables',    799.99, 25, '49mm Titanium case, GPS + Cellular, 36-hour battery, 100m water resistance, Action button.',              'https://placehold.co/300x200?text=Apple+Watch+Ultra',4.7, 1, '2025-01-01'),
    (16, 'Apple Watch Series 10',   'Wearables',    399.99, 40, 'The thinnest Apple Watch ever. Wider display, faster charging, sleep apnea detection.',                   'https://placehold.co/300x200?text=Watch+Series+10',  4.6, 1, '2025-01-01'),
    (17, 'Samsung Galaxy Watch 7',  'Wearables',    299.99, 35, 'Advanced health tracking, BioActive Sensor, 40-hour battery, Galaxy AI.',                                  'https://placehold.co/300x200?text=Galaxy+Watch+7',   4.5, 1, '2025-01-01'),
    (18, 'Garmin Fenix 8',          'Wearables',    899.99, 15, 'Multi-sport GPS watch, solar charging, AMOLED display, advanced training metrics.',                        'https://placehold.co/300x200?text=Garmin+Fenix+8',   4.8, 1, '2025-01-01'),
    (19, 'Fitbit Charge 6',         'Wearables',    159.99, 60, 'Built-in GPS, heart rate monitoring, stress management score, 7-day battery.',                            'https://placehold.co/300x200?text=Fitbit+Charge+6',  4.3, 1, '2025-01-01'),

    -- ── Tablets ──────────────────────────────────────────────────────────────
    (20, 'iPad Pro 13"',            'Tablets',     1299.99, 18, 'M4 chip, Ultra Retina XDR OLED display, Apple Pencil Pro support, 5G optional.',                          'https://placehold.co/300x200?text=iPad+Pro+13',      4.9, 1, '2025-01-01'),
    (21, 'iPad Pro 12.9"',          'Tablets',     1099.99, 15, 'M4 chip, Liquid Retina XDR display, 256GB, Thunderbolt 4 port.',                                           'https://placehold.co/300x200?text=iPad+Pro+12',      4.9, 1, '2025-01-01'),
    (22, 'iPad Air M2',             'Tablets',      749.99, 28, '11-inch Liquid Retina display, M2 chip, 128GB, 10 hours battery, Apple Pencil compatible.',               'https://placehold.co/300x200?text=iPad+Air+M2',      4.6, 1, '2025-01-01'),
    (23, 'Samsung Galaxy Tab S10+', 'Tablets',      999.99, 20, '12.4" Dynamic AMOLED, Snapdragon 8 Gen 3, 12GB RAM, 256GB, S Pen included.',                              'https://placehold.co/300x200?text=Galaxy+Tab+S10',   4.6, 1, '2025-01-01'),

    -- ── Monitors ─────────────────────────────────────────────────────────────
    (24, 'LG 27" 4K UltraFine',     'Monitors',     699.99, 12, '27" UHD IPS, Nano IPS panel, DCI-P3 98%, USB-C 96W, Thunderbolt 4.',                                     'https://placehold.co/300x200?text=LG+4K+UltraFine',  4.8, 1, '2025-01-01'),
    (25, 'Dell UltraSharp U2723DE', 'Monitors',     749.99, 10, '27" 4K IPS Black, 60Hz, USB-C 90W, built-in KVM switch, 100% sRGB.',                                     'https://placehold.co/300x200?text=Dell+U2723DE',     4.7, 1, '2025-01-01'),
    (26, 'Samsung Odyssey G7',      'Monitors',     599.99, 14, '32" QLED 4K 144Hz, 1ms response time, HDR600, G-Sync compatible.',                                        'https://placehold.co/300x200?text=Odyssey+G7',       4.6, 1, '2025-01-01'),
    (27, 'ASUS ProArt PA32UCG',     'Monitors',    1499.99,  6, '32" 4K 120Hz Mini-LED, Adobe RGB 99.5%, Thunderbolt 4, ideal for creatives.',                             'https://placehold.co/300x200?text=ASUS+ProArt',      4.9, 1, '2025-01-01'),

    -- ── Accessories ──────────────────────────────────────────────────────────
    (28, 'Logitech MX Keys S',      'Accessories',  119.99, 60, 'Low-profile advanced wireless keyboard, perfect keystrokes, backlit, multi-device.',                      'https://placehold.co/300x200?text=MX+Keys+S',        4.7, 1, '2025-01-01'),
    (29, 'Logitech MX Master 3S',   'Accessories',   99.99, 75, '8K DPI sensor, MagSpeed wheel, USB-C, works on glass, 70-day battery.',                                   'https://placehold.co/300x200?text=MX+Master+3S',     4.8, 1, '2025-01-01'),
    (30, 'Razer DeathAdder V3',     'Accessories',   79.99, 80, 'Ergonomic gaming mouse, 30K Focus Pro optical sensor, 90-hour battery.',                                   'https://placehold.co/300x200?text=Razer+DeathAdder', 4.7, 1, '2025-01-01'),
    (31, 'Apple Magic Keyboard',    'Accessories',   99.99, 45, 'Compact wireless keyboard with Touch ID, scissor mechanism, USB-C charging.',                              'https://placehold.co/300x200?text=Magic+Keyboard',   4.5, 1, '2025-01-01'),
    (32, 'Keychron Q1 Pro',         'Accessories',  199.99, 30, 'Wireless mechanical keyboard, hot-swappable, QMK/VIA compatible, aluminium body.',                        'https://placehold.co/300x200?text=Keychron+Q1+Pro',  4.8, 1, '2025-01-01'),
    (33, 'Anker 778 USB-C Hub',     'Accessories',   59.99, 90, '12-in-1 docking station, 4K HDMI, 85W PD, SD card reader, 3x USB-A, USB-C.',                            'https://placehold.co/300x200?text=Anker+778+Hub',    4.6, 1, '2025-01-01'),

    -- ── Gaming ───────────────────────────────────────────────────────────────
    (34, 'PlayStation 5 Slim',      'Gaming',       499.99, 20, 'Next-gen gaming with SSD, 4K@120Hz, ray tracing, DualSense haptic feedback.',                             'https://placehold.co/300x200?text=PS5+Slim',         4.9, 1, '2025-01-01'),
    (35, 'Xbox Series X',           'Gaming',       499.99, 18, '12 teraflops GPU, 1TB SSD, 4K@60Hz (up to 120fps), Quick Resume.',                                        'https://placehold.co/300x200?text=Xbox+Series+X',    4.8, 1, '2025-01-01'),
    (36, 'Nintendo Switch OLED',    'Gaming',       349.99, 35, '7" OLED screen, enhanced audio, 64GB storage, dock for TV play.',                                          'https://placehold.co/300x200?text=Nintendo+Switch',  4.8, 1, '2025-01-01'),
    (37, 'Steam Deck OLED',         'Gaming',       549.99, 22, '7.4" HDR OLED, AMD APU, 512GB NVMe, Wi-Fi 6E. Play your full Steam library.',                            'https://placehold.co/300x200?text=Steam+Deck+OLED',  4.8, 1, '2025-01-01'),
    (38, 'Razer Kishi V2 Pro',      'Gaming',        99.99, 50, 'Mobile gaming controller for iPhone/Android, low latency, haptic feedback.',                               'https://placehold.co/300x200?text=Razer+Kishi+V2',   4.4, 1, '2025-01-01'),

    -- ── Cameras ──────────────────────────────────────────────────────────────
    (39, 'Sony Alpha A7 IV',        'Cameras',      2499.99,  8, '33MP full-frame BSI CMOS, 4K@60fps, 759 phase-detect AF points, 10fps burst.',                           'https://placehold.co/300x200?text=Sony+A7+IV',       4.9, 1, '2025-01-01'),
    (40, 'GoPro Hero 13 Black',     'Cameras',       399.99, 22, '5.3K 60fps video, HyperSmooth 7.0, 10m waterproof, 3-hour battery.',                                     'https://placehold.co/300x200?text=GoPro+Hero13',     4.6, 1, '2025-01-01'),
    (41, 'DJI Osmo Pocket 3',       'Cameras',       519.99, 16, '4K@120fps, 1" CMOS, ActiveTrack 6.0, 3-axis gimbal, 166-minute battery.',                                'https://placehold.co/300x200?text=DJI+Osmo+Pocket3', 4.8, 1, '2025-01-01'),
    (42, 'Canon EOS R50',           'Cameras',       679.99, 14, '24.2MP APS-C, 4K@30fps, DIGIC X processor, Eye Detection AF, compact body.',                             'https://placehold.co/300x200?text=Canon+EOS+R50',    4.6, 1, '2025-01-01'),

    -- ── Smart Home ───────────────────────────────────────────────────────────
    (43, 'Apple HomePod mini',      'Smart Home',    99.99, 50, '360° audio, Siri intelligence, Intercom, Thread smart home hub.',                                          'https://placehold.co/300x200?text=HomePod+mini',     4.4, 1, '2025-01-01'),
    (44, 'Amazon Echo Show 10',     'Smart Home',    249.99, 35, '10.1" HD screen with motion, built-in Zigbee hub, Alexa, 13MP camera.',                                  'https://placehold.co/300x200?text=Echo+Show+10',     4.4, 1, '2025-01-01'),
    (45, 'Philips Hue Starter Kit', 'Smart Home',    199.99, 40, '4x A19 smart bulbs + Bridge. 16 million colors, voice control, app scheduling.',                         'https://placehold.co/300x200?text=Hue+Starter+Kit',  4.6, 1, '2025-01-01'),

    -- ── Networking ───────────────────────────────────────────────────────────
    (46, 'ASUS ZenWiFi Pro ET12',   'Networking',   599.99,  8, 'Tri-band WiFi 6E mesh system, 10Gbps WAN/LAN, 7500 sq ft coverage, 3 pack.',                             'https://placehold.co/300x200?text=ASUS+ZenWiFi',     4.7, 1, '2025-01-01'),
    (47, 'Netgear Orbi RBK863S',    'Networking',   699.99,  6, 'Tri-band WiFi 6 mesh, 6Gbps, covers 5,000 sq ft, 3-pack with satellite.',                                'https://placehold.co/300x200?text=Netgear+Orbi',     4.6, 1, '2025-01-01'),

    -- ── Storage ──────────────────────────────────────────────────────────────
    (48, 'Samsung T9 4TB SSD',      'Storage',      349.99, 25, 'Portable SSD, USB 3.2 Gen 2x2, 2000MB/s read, shock resistant, 3-year warranty.',                        'https://placehold.co/300x200?text=Samsung+T9+SSD',   4.7, 1, '2025-01-01'),
    (49, 'WD Black SN850X 2TB',     'Storage',      169.99, 30, 'NVMe M.2 SSD, PCIe Gen 4, 7300MB/s read, PS5 compatible, gaming storage.',                               'https://placehold.co/300x200?text=WD+SN850X+2TB',    4.8, 1, '2025-01-01'),
    (50, 'Seagate IronWolf 8TB',    'Storage',      179.99, 20, 'NAS HDD, 7200 RPM, 256MB cache, CMR, IronWolf Health Management, 3-year warranty.',                      'https://placehold.co/300x200?text=Seagate+IronWolf', 4.5, 1, '2025-01-01')

) AS source (Id, Name, Category, Price, Stock, Description, ImageUrl, Rating, IsActive, CreatedAt)
ON target.Id = source.Id
WHEN MATCHED THEN
    UPDATE SET
        Name        = source.Name,
        Category    = source.Category,
        Price       = source.Price,
        Stock       = source.Stock,
        Description = source.Description,
        ImageUrl    = source.ImageUrl,
        Rating      = source.Rating,
        IsActive    = source.IsActive,
        UpdatedAt   = GETUTCDATE()
WHEN NOT MATCHED THEN
    INSERT (Id, Name, Category, Price, Stock, Description, ImageUrl, Rating, IsActive, CreatedAt)
    VALUES (source.Id, source.Name, source.Category, source.Price, source.Stock,
            source.Description, source.ImageUrl, source.Rating, source.IsActive, source.CreatedAt);

SET IDENTITY_INSERT Products OFF;
GO

-- ── 5. Register migration so EF Core doesn't re-run it ──────────────────────
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20250101000001_InitialCreate')
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250101000001_InitialCreate', '9.0.0');
GO

-- ── 6. Summary ───────────────────────────────────────────────────────────────
SELECT
    Category,
    COUNT(*)            AS ProductCount,
    MIN(Price)          AS MinPrice,
    MAX(Price)          AS MaxPrice,
    AVG(Price)          AS AvgPrice,
    SUM(Stock)          AS TotalStock
FROM Products
WHERE IsActive = 1
GROUP BY Category
ORDER BY Category;
GO

PRINT '✅ TechStore_Inventory seed complete — ' + CAST((SELECT COUNT(*) FROM Products) AS NVARCHAR) + ' products loaded.';
GO
