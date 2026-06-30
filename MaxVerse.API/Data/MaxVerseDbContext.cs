using Microsoft.EntityFrameworkCore;
using MaxVerse.API.Models;

namespace MaxVerse.API.Data;

public class MaxVerseDbContext : DbContext
{
    public MaxVerseDbContext(DbContextOptions<MaxVerseDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<VNPayTransaction> VNPayTransactions => Set<VNPayTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Đảm bảo các cột decimal map đúng kiểu trong SQL Server (tránh warning + sai lệch dữ liệu)
        modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Product>().Property(p => p.DiscountPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OrderDetail>().Property(o => o.UnitPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<VNPayTransaction>().Property(v => v.Amount).HasColumnType("decimal(18,2)");

        // Unique constraints
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Order>().HasIndex(o => o.OrderCode).IsUnique();
        modelBuilder.Entity<Brand>().HasIndex(b => b.BrandName).IsUnique();
        modelBuilder.Entity<Category>().HasIndex(c => c.CategoryName).IsUnique();

        // Tránh xóa cascade nhiều tầng gây lỗi trên SQL Server (User -> Order giữ lại khi cần)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Variant)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CartItem>()
            .HasOne(c => c.Variant)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
            base.OnModelCreating(modelBuilder);

    // Tự động chèn sẵn Roles nếu chưa có
    modelBuilder.Entity<Role>().HasData(
        new Role { RoleId = 1, RoleName = "Admin" },
        new Role { RoleId = 2, RoleName = "User" }
    );
    }
}
