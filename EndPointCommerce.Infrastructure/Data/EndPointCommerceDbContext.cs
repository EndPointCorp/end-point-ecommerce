using EndPointCommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Infrastructure.Data;

public class EndPointCommerceDbContext : DbContext
{
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<Quote> Quotes { get; set; }
    public DbSet<QuoteItem> QuoteItems { get; set; }
    public DbSet<SiteContent> SiteContents { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<CategoryImage> CategoryImages { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<IdentityUserClaim<int>> UserClaims { get; set; }
    public DbSet<IdentityRole<int>> Roles { get; set; }
    public DbSet<IdentityRoleClaim<int>> RoleClaims { get; set; }
    public DbSet<IdentityUserRole<int>> UserRoles { get; set; }

    public EndPointCommerceDbContext (DbContextOptions<EndPointCommerceDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add the full-text search vector field to Products
        modelBuilder.Entity<Product>()
            .HasGeneratedTsVectorColumn(
                p => p.SearchVector!,
                "english",  // Text search config
                p => new {
                    p.Name,
                    p.Description,
                    p.ShortDescription,
                    p.MetaDescription,
                    p.MetaKeywords,
                    p.Sku
                })  // Included properties
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN"); // Index method on the search vector (GIN or GIST)

        modelBuilder.Entity<IdentityUserRole<int>>().HasKey(k => new { k.UserId, k.RoleId });

        modelBuilder.Entity<Quote>()
            .HasOne(q => q.BillingAddress)
            .WithMany()
            .HasForeignKey(q => q.BillingAddressId);

        modelBuilder.Entity<Quote>()
            .HasOne(q => q.ShippingAddress)
            .WithMany()
            .HasForeignKey(q => q.ShippingAddressId);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole<int>>().HasData(
            new IdentityRole<int> {
                Id = 1, Name = User.ADMIN_ROLE, NormalizedName = User.ADMIN_ROLE.ToUpper(),
                ConcurrencyStamp = "832e598d-6660-4c6f-a48a-9bed25a49671"
            },
            new IdentityRole<int> {
                Id = 2, Name = User.CUSTOMER_ROLE, NormalizedName = User.CUSTOMER_ROLE.ToUpper(),
                ConcurrencyStamp = "8a80a6c6-dae7-48a0-9b5b-841564ba537d"
            }
        );

        modelBuilder.Entity<User>().HasData(
            new User {
                Id = 1,
                UserName = "epadmin",
                NormalizedUserName = "EPADMIN",
                Email = "epadmin@endpointcorp.com",
                NormalizedEmail = "EPADMIN@ENDPOINTCORP.COM",
                EmailConfirmed = true,
                ConcurrencyStamp = "99982048-369f-4174-b277-40c991417169",
                SecurityStamp = "abd3ee70-301d-4822-9953-69e8b4be3c88",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                // The password is "Password123"
                PasswordHash = "AQAAAAIAAYagAAAAEGYF8Zctt9SJG5HeU2g1GKqqJxU2tNQkUDgRJzC0BcaeinhhfvRkuQQKsjmoFrrHoQ=="
            }
        );

        modelBuilder.Entity<IdentityUserRole<int>>().HasData(
            new IdentityUserRole<int> { UserId = 1, RoleId = 1 }
        );

        modelBuilder.Entity<State>().HasData(
            new State { Id = 1, Abbreviation = "AL", Name = "Alabama" },
            new State { Id = 2, Abbreviation = "AK", Name = "Alaska" },
            new State { Id = 3, Abbreviation = "AZ", Name = "Arizona" },
            new State { Id = 4, Abbreviation = "AR", Name = "Arkansas" },
            new State { Id = 5, Abbreviation = "CA", Name = "California" },
            new State { Id = 6, Abbreviation = "CO", Name = "Colorado" },
            new State { Id = 7, Abbreviation = "CT", Name = "Connecticut" },
            new State { Id = 8, Abbreviation = "DE", Name = "Delaware" },
            new State { Id = 9, Abbreviation = "FL", Name = "Florida" },
            new State { Id = 10, Abbreviation = "GA", Name = "Georgia" },
            new State { Id = 11, Abbreviation = "HI", Name = "Hawaii" },
            new State { Id = 12, Abbreviation = "ID", Name = "Idaho" },
            new State { Id = 13, Abbreviation = "IL", Name = "Illinois" },
            new State { Id = 14, Abbreviation = "IN", Name = "Indiana" },
            new State { Id = 15, Abbreviation = "IA", Name = "Iowa" },
            new State { Id = 16, Abbreviation = "KS", Name = "Kansas" },
            new State { Id = 17, Abbreviation = "KY", Name = "Kentucky" },
            new State { Id = 18, Abbreviation = "LA", Name = "Louisiana" },
            new State { Id = 19, Abbreviation = "ME", Name = "Maine" },
            new State { Id = 20, Abbreviation = "MD", Name = "Maryland" },
            new State { Id = 21, Abbreviation = "MA", Name = "Massachusetts" },
            new State { Id = 22, Abbreviation = "MI", Name = "Michigan" },
            new State { Id = 23, Abbreviation = "MN", Name = "Minnesota" },
            new State { Id = 24, Abbreviation = "MS", Name = "Mississippi" },
            new State { Id = 25, Abbreviation = "MO", Name = "Missouri" },
            new State { Id = 26, Abbreviation = "MT", Name = "Montana" },
            new State { Id = 27, Abbreviation = "NE", Name = "Nebraska" },
            new State { Id = 28, Abbreviation = "NV", Name = "Nevada" },
            new State { Id = 29, Abbreviation = "NH", Name = "New Hampshire" },
            new State { Id = 30, Abbreviation = "NJ", Name = "New Jersey" },
            new State { Id = 31, Abbreviation = "NM", Name = "New Mexico" },
            new State { Id = 32, Abbreviation = "NY", Name = "New York" },
            new State { Id = 33, Abbreviation = "NC", Name = "North Carolina" },
            new State { Id = 34, Abbreviation = "ND", Name = "North Dakota" },
            new State { Id = 35, Abbreviation = "OH", Name = "Ohio" },
            new State { Id = 36, Abbreviation = "OK", Name = "Oklahoma" },
            new State { Id = 37, Abbreviation = "OR", Name = "Oregon" },
            new State { Id = 38, Abbreviation = "PA", Name = "Pennsylvania" },
            new State { Id = 39, Abbreviation = "RI", Name = "Rhode Island" },
            new State { Id = 40, Abbreviation = "SC", Name = "South Carolina" },
            new State { Id = 41, Abbreviation = "SD", Name = "South Dakota" },
            new State { Id = 42, Abbreviation = "TN", Name = "Tennessee" },
            new State { Id = 43, Abbreviation = "TX", Name = "Texas" },
            new State { Id = 44, Abbreviation = "UT", Name = "Utah" },
            new State { Id = 45, Abbreviation = "VT", Name = "Vermont" },
            new State { Id = 46, Abbreviation = "VA", Name = "Virginia" },
            new State { Id = 47, Abbreviation = "WA", Name = "Washington" },
            new State { Id = 48, Abbreviation = "WV", Name = "West Virginia" },
            new State { Id = 49, Abbreviation = "WI", Name = "Wisconsin" },
            new State { Id = 50, Abbreviation = "WY", Name = "Wyoming" },
            new State { Id = 51, Abbreviation = "DC", Name = "District of Columbia" }
        );

        modelBuilder.Entity<OrderStatus>().HasData(
            new OrderStatus { Id = 1, Name = OrderStatus.PENDING },
            new OrderStatus { Id = 2, Name = OrderStatus.PROCESSING },
            new OrderStatus { Id = 3, Name = OrderStatus.INVOICED },
            new OrderStatus { Id = 4, Name = OrderStatus.CANCELLED }
        );

        modelBuilder.Entity<PaymentMethod>().HasData(
            new PaymentMethod { Id = 1, Name = PaymentMethod.CREDIT_CARD },
            new PaymentMethod { Id = 2, Name = PaymentMethod.FREE_ORDER }
        );

        modelBuilder.Entity<SiteContent>().HasData(
            new SiteContent { Id = 1, Name = "homepage_content", Content = string.Empty },
            new SiteContent { Id = 2, Name = "site_message", Content = string.Empty }
        );
    }
}
