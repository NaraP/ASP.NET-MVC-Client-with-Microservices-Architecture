# 🛒 TechStore — .NET 9 E-Commerce Demo

A full-stack e-commerce demo built with **ASP.NET Core MVC (.NET 9)** and three independent microservice APIs.
Showcases real-world patterns: REST APIs, JWT authentication, session-based cart, multi-service orchestration, and clean MVC architecture.

---

## 🔐 JWT Authentication Flow

```
┌─────────────────────────────────────────────────────────────────┐
│  User submits Login form                                        │
│         │                                                       │
│         ▼                                                       │
│  AccountController.Login()                                      │
│    1. Validates credentials against dummy user store           │
│    2. Issues Cookie → protects MVC pages ([Authorize])         │
│    3. Generates JWT via JwtService → stored in Session         │
│                                                                 │
│  Every API call (Products / Orders / Payments)                  │
│    JwtBearerHandler (DelegatingHandler) reads JWT from Session  │
│    Injects: Authorization: Bearer <token>                       │
│                                                                 │
│  Each API validates the Bearer token on every request          │
│    Invalid/missing token → 401 Unauthorized                    │
└─────────────────────────────────────────────────────────────────┘
```

**Two-layer auth used deliberately:**
- `Cookie` auth → guards MVC pages (browser redirects to login)
- `JWT Bearer` → guards the three REST APIs (returns 401 JSON if missing)

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────┐
│              ECommerce.MVC  :5000                    │
│  Login → Product Catalog → Cart → Checkout → Order  │
│  Issues JWT on login, sends as Bearer to all APIs   │
└───────────┬───────────────┬───────────────┬─────────┘
            │ Bearer token  │ Bearer token  │ Bearer token
   ┌────────▼──────┐ ┌──────▼──────┐ ┌─────▼───────┐
   │ InventoryApi  │ │  OrderApi   │ │ PaymentApi  │
   │    :5001      │ │   :5002     │ │   :5003     │
   │  [Authorize]  │ │ [Authorize] │ │ [Authorize] │
   └───────────────┘ └─────────────┘ └─────────────┘
```

---

## 📁 Key JWT Files

```
ECommerce.MVC/
├── Services/
│   ├── JwtService.cs          ← Generates JWT tokens (HMAC-SHA256)
│   └── JwtBearerHandler.cs    ← DelegatingHandler: injects Bearer into API calls
├── Controllers/
│   └── AccountController.cs   ← Login: issues cookie + JWT; Logout: clears both

ECommerce.InventoryApi/
├── JwtExtensions.cs           ← AddJwtAuth() + AddSwaggerWithJwt() helpers
└── Program.cs                 ← UseAuthentication() + UseAuthorization()

(Same pattern repeated in OrderApi and PaymentApi)
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Run all 4 projects (4 terminals)

```bash
# Terminal 1 — Inventory API
cd src/ECommerce.InventoryApi && dotnet run   # :5001/swagger

# Terminal 2 — Order API
cd src/ECommerce.OrderApi && dotnet run       # :5002/swagger

# Terminal 3 — Payment API
cd src/ECommerce.PaymentApi && dotnet run     # :5003/swagger

# Terminal 4 — MVC Client
cd src/ECommerce.MVC && dotnet run            # :5000
```

---

## 👤 Demo Login Accounts

| Email | Password | Role |
|-------|----------|------|
| demo@example.com | demo | Customer |
| john@example.com | password123 | Customer |
| admin@example.com | admin123 | Admin |

---

## 🔑 JWT Configuration

All projects share the same key (configured in each `appsettings.json`):

```json
"Jwt": {
  "Key":           "TechStore_SuperSecret_JWT_Key_2025_MustBe32CharsLong!",
  "Issuer":        "TechStore",
  "Audience":      "TechStoreApis",
  "ExpiryMinutes": 120
}
```

> ⚠️ In production, store the key in Azure Key Vault or environment variables — never in appsettings.

---

## 🧪 Testing APIs Directly via Swagger

1. Open any API's Swagger UI (e.g. `http://localhost:5001/swagger`)
2. Hit a protected endpoint — you'll get **401 Unauthorized**
3. Get a token: call `POST /api/auth/token` on the MVC app, or copy it from the login response
4. Click **Authorize** in Swagger → paste your Bearer token
5. Re-call the endpoint — now returns **200 OK**

---

## 💳 Payment Testing

| Card Number | Result |
|-------------|--------|
| Any card NOT ending in 0000 | ✅ Success |
| Any card ending in 0000 | ❌ Declined |

---

## 🔧 Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core MVC — .NET 9 |
| APIs | ASP.NET Core Web API — .NET 9 |
| Page Auth | Cookie Authentication |
| API Auth | JWT Bearer (HMAC-SHA256) |
| Token Storage | ASP.NET Core Session |
| Token Injection | DelegatingHandler (JwtBearerHandler) |
| HTTP Client | IHttpClientFactory (typed clients) |
| API Docs | Swagger / OpenAPI with Auth button |
| Frontend | Bootstrap 5.3 + Bootstrap Icons |
| Data | In-memory (no database required) |


A full-stack e-commerce demo built with **ASP.NET Core MVC (.NET 9)** and three independent microservice APIs.
Showcases real-world patterns: REST APIs, session-based cart, JWT-ready auth, multi-service orchestration, and clean MVC architecture.

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────┐
│              ECommerce.MVC  :5000                    │
│  Login → Product Catalog → Cart → Checkout → Order  │
└───────────┬───────────────┬───────────────┬─────────┘
            │               │               │
   ┌────────▼──────┐ ┌──────▼──────┐ ┌─────▼───────┐
   │ InventoryApi  │ │  OrderApi   │ │ PaymentApi  │
   │    :5001      │ │   :5002     │ │   :5003     │
   │  (Products)   │ │  (Orders)   │ │ (Payments)  │
   └───────────────┘ └─────────────┘ └─────────────┘
```

---

## 📁 Project Structure

```
ECommerce/
├── ECommerce.sln
└── src/
    ├── ECommerce.MVC/               ← ASP.NET Core MVC Client
    │   ├── Controllers/
    │   │   ├── AccountController.cs  ← Login / Logout
    │   │   ├── HomeController.cs     ← Product catalog
    │   │   ├── CartController.cs     ← Cart (session-based)
    │   │   └── OrderController.cs   ← Checkout → Payment → Order
    │   ├── Models/ViewModels.cs
    │   ├── Services/
    │   │   ├── ApiServices.cs        ← InventoryService, OrderService, PaymentService
    │   │   └── CartService.cs        ← Session cart helper
    │   └── Views/
    │       ├── Account/Login.cshtml
    │       ├── Home/Index.cshtml     ← Product grid with search & filter
    │       ├── Cart/Index.cshtml
    │       └── Order/
    │           ├── Checkout.cshtml
    │           └── Confirmation.cshtml
    │
    ├── ECommerce.InventoryApi/      ← Products REST API  :5001
    │   └── Controllers/ProductsController.cs
    │
    ├── ECommerce.OrderApi/          ← Orders REST API    :5002
    │   └── Controllers/OrdersController.cs
    │
    └── ECommerce.PaymentApi/        ← Payments REST API  :5003
        └── Controllers/PaymentsController.cs
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Run all 4 projects (4 separate terminals)

```bash
# Terminal 1 — Inventory API (Products)
cd src/ECommerce.InventoryApi
dotnet run
# → http://localhost:5001/swagger

# Terminal 2 — Order API
cd src/ECommerce.OrderApi
dotnet run
# → http://localhost:5002/swagger

# Terminal 3 — Payment API
cd src/ECommerce.PaymentApi
dotnet run
# → http://localhost:5003/swagger

# Terminal 4 — MVC Client App
cd src/ECommerce.MVC
dotnet run
# → http://localhost:5000
```

---

## 👤 Demo Login Accounts

| Email | Password | Role |
|-------|----------|------|
| demo@example.com | demo | Customer |
| john@example.com | password123 | Customer |
| admin@example.com | admin123 | Admin |

---

## 🔄 User Flow

```
1. Login         → /Account/Login
2. Browse        → / (search, filter by category, add to cart)
3. Cart          → /Cart (adjust quantities, remove items)
4. Checkout      → /Order/Checkout (shipping address + card details)
5. Confirmation  → /Order/Confirmation (Order ID + Payment Reference)
```

---

## 💳 Payment Testing

| Card Number | Result |
|-------------|--------|
| Any card NOT ending in 0000 | ✅ Payment Success |
| Any card ending in 0000 | ❌ Payment Declined |

---

## 📡 API Endpoints

### Inventory API — `http://localhost:5001`
| Method | Route | Description |
|--------|-------|-------------|
| GET | /api/products | All products (optional `?category=`) |
| GET | /api/products/{id} | Single product |
| GET | /api/products/categories | List of categories |
| PATCH | /api/products/{id}/stock | Deduct stock |

### Order API — `http://localhost:5002`
| Method | Route | Description |
|--------|-------|-------------|
| POST | /api/orders | Create order → returns Order ID |
| GET | /api/orders/{orderId} | Get order by ID |
| GET | /api/orders/user/{userId} | Orders by user |

### Payment API — `http://localhost:5003`
| Method | Route | Description |
|--------|-------|-------------|
| POST | /api/payments/process | Process payment → returns Payment Reference |
| GET | /api/payments/{ref} | Get payment by reference |

---

## 🛒 Products (12 Dummy Items)

Categories: **Laptops**, **Phones**, **Audio**, **Wearables**, **Tablets**, **Monitors**, **Accessories**, **Gaming**, **Cameras**

---

## 🔧 Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core MVC — .NET 9 |
| APIs | ASP.NET Core Web API — .NET 9 |
| Auth | Cookie Authentication |
| Cart | ASP.NET Core Session |
| HTTP Client | IHttpClientFactory (typed clients) |
| API Docs | Swagger / OpenAPI |
| Frontend | Bootstrap 5.3 + Bootstrap Icons |
| Data | In-memory (no database required) |

---

## 🗺️ Upwork Portfolio Notes

This project demonstrates:
- ✅ Multi-project .NET 9 solution structure
- ✅ Clean separation of concerns (MVC + 3 APIs)
- ✅ Typed HttpClient for service-to-service calls
- ✅ Session-based cart management
- ✅ Cookie authentication with claims
- ✅ REST API design with proper HTTP verbs
- ✅ Swagger documentation on all APIs
- ✅ Responsive UI with Bootstrap 5
- ✅ Real checkout flow: cart → payment → order → confirmation
