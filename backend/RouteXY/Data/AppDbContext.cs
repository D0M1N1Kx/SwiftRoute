using Microsoft.EntityFrameworkCore;
using RouteXY.Api.Entities;

namespace RouteXY.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    public DbSet<CourierLocation> CourierLocations => Set<CourierLocation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // User
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(o => o.TrackingNumber).IsUnique();

            entity.Property(o => o.Status)
                .HasConversion<string>();

            entity.HasOne(o => o.Dispatcher)
                .WithMany(u => u.DispatchedOrders)
                .HasForeignKey(o => o.DispatcherId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Courier)
                .WithMany(u => u.CourierOrders)
                .HasForeignKey(o => o.CourierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Warehouse)
                .WithMany(w => w.Orders)
                .HasForeignKey(o => o.WarehouseId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(o => o.InventoryItem)
                .WithMany(i => i.Orders)
                .HasForeignKey(o => o.InventoryItemId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // OrderStatusHistory
        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.Property(o => o.OldStatus)
                .HasConversion<string>();

            entity.Property(o => o.NewStatus)
                .HasConversion<string>();

            entity.HasOne(h => h.Order)
                .WithMany(o => o.StatusHistory)
                .HasForeignKey(h => h.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(h => h.ChangedByUser)
                .WithMany()
                .HasForeignKey(h => h.ChangedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CourierLocation
        modelBuilder.Entity<CourierLocation>(entity =>
        {
            entity.HasOne(cl => cl.Courier)
                .WithMany(u => u.Locations)
                .HasForeignKey(cl => cl.CourierId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cl => cl.Order)
                .WithMany()
                .HasForeignKey(cl => cl.OrderId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Warehouse
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasMany(w => w.InventoryItems)
                .WithOne(i => i.Warehouse)
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}