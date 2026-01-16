using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NeonGadgetStore.Api.Models;
using System.Text.Json;

namespace NeonGadgetStore.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<WishlistEntry> Wishlist => Set<WishlistEntry>();
    public DbSet<ViewedEntry> ViewedHistory => Set<ViewedEntry>();
    
    // Thêm DbSet mới theo báo cáo
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>()
            .Property(p => p.Images)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrWhiteSpace(v)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );

        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId);

        builder.Entity<Profile>()
            .HasIndex(p => p.Username)
            .IsUnique();

        builder.Entity<Profile>()
            .HasIndex(p => p.UserId)
            .IsUnique();

        builder.Entity<Product>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        builder.Entity<Category>()
            .HasIndex(c => c.Slug)
            .IsUnique();

        builder.Entity<WishlistEntry>()
            .HasIndex(w => new { w.UserId, w.ProductId })
            .IsUnique();

        builder.Entity<ViewedEntry>()
            .HasIndex(v => new { v.UserId, v.ProductId })
            .IsUnique();

        // Cấu hình cho Permission
        builder.Entity<Permission>()
            .HasIndex(p => p.PermissionModule);

        // Cấu hình cho RolePermission
        builder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // Cấu hình cho Payment
        builder.Entity<Payment>()
            .HasIndex(p => p.PaymentStatus);

        // Cấu hình cho ShoppingCart
        builder.Entity<ShoppingCart>()
            .HasMany(sc => sc.CartItems)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId);

        // Cấu hình cho CartItem
        builder.Entity<CartItem>()
            .HasIndex(ci => new { ci.CartId, ci.ProductId })
            .IsUnique();

        // Cấu hình cho Review
        builder.Entity<Review>()
            .HasIndex(r => r.ProductId);
        builder.Entity<Review>()
            .HasIndex(r => r.UserId);
    }
}
