using ECommerce.PaymentApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.PaymentApi.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

    public DbSet<PaymentRecord> Payments => Set<PaymentRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PaymentRecord>(entity =>
        {
            entity.HasKey(p => p.PaymentReference);
            entity.HasIndex(p => p.UserId);
            entity.HasIndex(p => p.ProcessedAt);
            entity.HasIndex(p => p.Status);
        });
    }
}
