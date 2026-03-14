using ECommerce.AuthApi;
using ECommerce.AuthApi.Data;
using ECommerce.AuthApi.IServices;
using ECommerce.AuthApi.Repository;
using ECommerce.AuthApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers + API explorer ───────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ── Swagger UI with JWT "Authorize" button ────────────────────────────────────
builder.Services.AddSwaggerWithJwt();

// ── JWT Bearer validation ─────────────────────────────────────────────────────
builder.Services.AddJwtAuth(builder.Configuration);

// ── SQL Server (AuthDbContext) ────────────────────────────────────────────────
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

// ── Repository layer (data access) ───────────────────────────────────────────
//    Scoped: one instance per HTTP request, same lifetime as DbContext
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// ── Service layer (business logic) ───────────────────────────────────────────
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Auto-migrate on startup (creates DB + tables + seeds Roles) ───────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
