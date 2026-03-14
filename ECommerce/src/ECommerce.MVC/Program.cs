using ECommerce.MVC.IServices;
using ECommerce.MVC.Middleware;
using ECommerce.MVC.Repositories;
using ECommerce.MVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// ── MVC + Session ────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
});

// ── Cookie auth for MVC pages ────────────────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

// ── Application services ─────────────────────────────────────────────────────
builder.Services.AddScoped<IUserService, UserService>();    // DB-backed user store
builder.Services.AddScoped<IJwtService, JwtService>();     // JWT token generator
builder.Services.AddScoped<JwtBearerHandler>();          // injects Bearer into API calls
builder.Services.AddScoped<IUserAdminService, UserAdminService>();
builder.Services.AddScoped<IUserAdminRepository, UserAdminRepository>();

// ── JWT Bearer handler (injects token into all API HttpClient calls) ─────────
builder.Services.AddScoped<JwtBearerHandler>();

var timeout = TimeSpan.FromSeconds(
    builder.Configuration.GetValue<int>("HttpClient:TimeoutSeconds")
);

builder.Services
    .AddHttpClient<IAuthApiService, AuthApiService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ApiUrls:Auth"]!);
        client.Timeout = timeout;
    })
    .AddHttpMessageHandler<JwtBearerHandler>();

builder.Services
    .AddHttpClient<IAdminApiService, AdminApiService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ApiUrls:Auth"]!);
        client.Timeout = timeout;
    })
    .AddHttpMessageHandler<JwtBearerHandler>();

// Inventory API
builder.Services.AddHttpClient<IInventoryService, InventoryService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:Inventory"]!);
    client.Timeout = timeout;
})
.AddHttpMessageHandler<JwtBearerHandler>();

// Order API
builder.Services.AddHttpClient<IOrderService, OrderService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:Order"]!);
    client.Timeout = timeout;
})
.AddHttpMessageHandler<JwtBearerHandler>();

// Payment API
builder.Services.AddHttpClient<IPaymentService, PaymentService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:Payment"]!);
    client.Timeout = timeout;
})
.AddHttpMessageHandler<JwtBearerHandler>();

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();           // ← session before auth (token lives in session)
app.UseAuthentication();
app.UseAuthorization();
app.UseRequestResponseLogging();   // Custom logging middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
