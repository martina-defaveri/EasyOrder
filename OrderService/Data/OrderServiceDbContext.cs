using Microsoft.EntityFrameworkCore;
using OrderService.Domain;

namespace OrderService.Data;

public class OrderServiceDbContext(DbContextOptions<OrderServiceDbContext> options) : DbContext(options)
{
    public virtual DbSet<Order> Orders => Set<Order>();
    public virtual DbSet<OrderProduct> OrderProducts => Set<OrderProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderProduct>()
            .HasKey(op => new { op.OrderId, op.ProductId });

        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderProducts)
            .WithOne()
            .HasForeignKey(op => op.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}