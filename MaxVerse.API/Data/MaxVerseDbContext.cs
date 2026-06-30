using Microsoft.EntityFrameworkCore;
using MaxVerse.API.Models.Common;
using MaxVerse.API.Models.Users;
using MaxVerse.API.Models.Payment;
using MaxVerse.API.Models.Product;
using MaxVerse.API.Models.Order;


namespace MaxVerse.API.Data;

public class MaxVerseDbContext : DbContext
{
    public MaxVerseDbContext(DbContextOptions<MaxVerseDbContext> options)
        : base(options)
    {
    }

    #region Common

    public DbSet<Country> Countries => Set<Country>();

    #endregion

    #region Users

    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
    public DbSet<Role> Roles => Set<Role>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    #endregion

    #region Payments

    public DbSet<PaymentType> PaymentTypes => Set<PaymentType>();
    public DbSet<UserPaymentMethod> UserPaymentMethods => Set<UserPaymentMethod>();

    #endregion

    #region Products

    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductItem> ProductItems => Set<ProductItem>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Variation> Variations => Set<Variation>();
    public DbSet<VariationOption> VariationOptions => Set<VariationOption>();
    public DbSet<ProductConfiguration> ProductConfigurations => Set<ProductConfiguration>();

    #endregion

    #region Orders

    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<ShoppingCartItem> ShoppingCartItems => Set<ShoppingCartItem>();
    public DbSet<ShopOrder> ShopOrders => Set<ShopOrder>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<UserReview> UserReviews => Set<UserReview>();
    public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
    public DbSet<ShippingMethod> ShippingMethods => Set<ShippingMethod>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // User -> Role (N:1)
        modelBuilder.Entity<User>()
            .HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // User -> RefreshToken (1:N)
        modelBuilder.Entity<RefreshToken>()
            .HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique Role Name
        modelBuilder.Entity<Role>()
            .HasIndex(x => x.Name)
            .IsUnique();
        modelBuilder.Entity<Role>().HasData(
    new Role
    {
        Id = 1,
        Name = "Admin",
        Description = "System Administrator",
        CreatedAt = new DateTime(2026, 1, 1)
    },
    new Role
    {
        Id = 2,
        Name = "Customer",
        Description = "Customer",
        CreatedAt = new DateTime(2026, 1, 1)
    }
);
        //-------------------------------------------------
        // Composite Keys
        //-------------------------------------------------

        modelBuilder.Entity<UserAddress>()
            .HasKey(x => new
            {
                x.UserId,
                x.AddressId
            });

        modelBuilder.Entity<ProductConfiguration>()
            .HasKey(x => new
            {
                x.ProductItemId,
                x.VariationOptionId
            });

        //-------------------------------------------------
        // Self Reference
        //-------------------------------------------------

        modelBuilder.Entity<ProductCategory>()
            .HasOne(x => x.ParentCategory)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        //-------------------------------------------------
        // One To One
        //-------------------------------------------------

        modelBuilder.Entity<User>()
            .HasOne(x => x.ShoppingCart)
            .WithOne(x => x.User)
            .HasForeignKey<ShoppingCart>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        //-------------------------------------------------
        // UserAddress
        //-------------------------------------------------

        modelBuilder.Entity<UserAddress>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserAddresses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserAddress>()
            .HasOne(x => x.Address)
            .WithMany(x => x.UserAddresses)
            .HasForeignKey(x => x.AddressId)
            .OnDelete(DeleteBehavior.Cascade);

        //-------------------------------------------------
        // ProductConfiguration
        //-------------------------------------------------

        modelBuilder.Entity<ProductConfiguration>()
            .HasOne(x => x.ProductItem)
            .WithMany(x => x.Configurations)
            .HasForeignKey(x => x.ProductItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProductConfiguration>()
            .HasOne(x => x.VariationOption)
            .WithMany(x => x.ProductConfigurations)
            .HasForeignKey(x => x.VariationOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        //-------------------------------------------------
        // Restrict Delete
        //-------------------------------------------------

        modelBuilder.Entity<ShopOrder>()
            .HasOne(x => x.User)
            .WithMany(x => x.Orders)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderLine>()
            .HasOne(x => x.ProductItem)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserReview>()
            .HasOne(x => x.ProductItem)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShoppingCartItem>()
            .HasOne(x => x.ProductItem)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        //-------------------------------------------------
        // Decimal Precision
        //-------------------------------------------------

        modelBuilder.Entity<ProductItem>()
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ProductItem>()
            .Property(x => x.SalePrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ProductItem>()
            .Property(x => x.Weight)
            .HasPrecision(10, 2);

        modelBuilder.Entity<ShippingMethod>()
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderLine>()
            .Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderLine>()
            .Property(x => x.LineTotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ShopOrder>()
            .Property(x => x.Subtotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ShopOrder>()
            .Property(x => x.ShippingFee)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ShopOrder>()
            .Property(x => x.DiscountAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ShopOrder>()
            .Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        //-------------------------------------------------
        // Unique Index
        //-------------------------------------------------

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<ProductItem>()
            .HasIndex(x => x.SKU)
            .IsUnique();

        modelBuilder.Entity<Brand>()
            .HasIndex(x => x.BrandName)
            .IsUnique();

        modelBuilder.Entity<ProductCategory>()
            .HasIndex(x => x.CategoryName)
            .IsUnique();

        modelBuilder.Entity<PaymentType>()
            .HasIndex(x => x.Value)
            .IsUnique();

        modelBuilder.Entity<OrderStatus>()
            .HasIndex(x => x.Status)
            .IsUnique();
    }
}