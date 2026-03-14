using ECommerce.AuthApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.AuthApi.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<AppUser>      Users         => Set<AppUser>();
    public DbSet<Role>         Roles         => Set<Role>();
    public DbSet<UserRole>     UserRoles     => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── AppUser ──────────────────────────────────────────────────────────
        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique().HasDatabaseName("UX_Users_Email");
            e.HasIndex(u => u.IsActive);
            e.HasIndex(u => u.CreatedAt);
        });

        // ── Role ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<Role>(e =>
        {
            e.HasIndex(r => r.Name).IsUnique().HasDatabaseName("UX_Roles_Name");

            // Seed system roles
            e.HasData(
                new Role { Id = 1, Name = "Customer", Description = "Standard customer — browse and purchase products.",         IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Role { Id = 2, Name = "Admin",    Description = "Full platform access — manage users, products, orders.",    IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Role { Id = 3, Name = "Manager",  Description = "Manage inventory and orders; no user administration.",     IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Role { Id = 4, Name = "Support",  Description = "View orders and payment records; read-only access.",       IsActive = true, CreatedAt = new DateTime(2025, 1, 1) }
            );
        });

        // ── UserRole (join table) ─────────────────────────────────────────────
        modelBuilder.Entity<UserRole>(e =>
        {
            e.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique()
             .HasDatabaseName("UX_UserRoles_UserId_RoleId");

            e.HasOne(ur => ur.User)
             .WithMany(u => u.UserRoles)
             .HasForeignKey(ur => ur.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(ur => ur.Role)
             .WithMany(r => r.UserRoles)
             .HasForeignKey(ur => ur.RoleId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── RefreshToken ──────────────────────────────────────────────────────
        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasIndex(rt => rt.Token).IsUnique();
            e.HasIndex(rt => rt.UserId);
            e.HasIndex(rt => rt.ExpiresAt);

            e.HasOne(rt => rt.User)
             .WithMany()
             .HasForeignKey(rt => rt.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
