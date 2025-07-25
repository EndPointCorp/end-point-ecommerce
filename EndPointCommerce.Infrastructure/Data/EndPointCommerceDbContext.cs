using EndPointCommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Infrastructure.Data;

public class EndPointCommerceDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<CategoryImage> CategoryImages { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    public DbSet<Quote> Quotes { get; set; }
    public DbSet<QuoteItem> QuoteItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Coupon> Coupons { get; set; }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<IdentityUserClaim<int>> UserClaims { get; set; }
    public DbSet<IdentityRole<int>> Roles { get; set; }
    public DbSet<IdentityRoleClaim<int>> RoleClaims { get; set; }
    public DbSet<IdentityUserRole<int>> UserRoles { get; set; }

    public DbSet<Address> Addresses { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<State> States { get; set; }

    public DbSet<SiteContent> SiteContents { get; set; }

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
                Id = 999,
                UserName = "demoadmin@endpointcommerce.com",
                NormalizedUserName = "DEMOADMIN@ENDPOINTCOMMERCE.COM",
                Email = "demoadmin@endpointcommerce.com",
                NormalizedEmail = "DEMOADMIN@ENDPOINTCOMMERCE.COM",
                EmailConfirmed = true,
                ConcurrencyStamp = "11c224f7-0ae4-4a0e-a84f-4c347879cdcb",
                SecurityStamp = "UTX2NGNJHCDEVFHGTHIRYFSP6VJUFNMJ",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                PasswordHash = "AQAAAAIAAYagAAAAECXf2a5iPTohU44T1WF2wHcSCnV30fIxmIgE6cNWwavkO9Hsojvn1lofUFJeekRPZg=="
            }
        );

        modelBuilder.Entity<IdentityUserRole<int>>().HasData(
            new IdentityUserRole<int> { UserId = 999, RoleId = 1 }
        );

        modelBuilder.Entity<Country>().HasData(
            new Country { Id = 1, Code = "AF", Name = "Afghanistan" },
            new Country { Id = 2, Code = "AL", Name = "Albania" },
            new Country { Id = 3, Code = "DZ", Name = "Algeria" },
            new Country { Id = 4, Code = "AS", Name = "American Samoa" },
            new Country { Id = 5, Code = "AD", Name = "Andorra" },
            new Country { Id = 6, Code = "AO", Name = "Angola" },
            new Country { Id = 7, Code = "AI", Name = "Anguilla" },
            new Country { Id = 8, Code = "AQ", Name = "Antarctica" },
            new Country { Id = 9, Code = "AG", Name = "Antigua And Barbuda" },
            new Country { Id = 10, Code = "AR", Name = "Argentina" },
            new Country { Id = 11, Code = "AM", Name = "Armenia" },
            new Country { Id = 12, Code = "AW", Name = "Aruba" },
            new Country { Id = 13, Code = "AU", Name = "Australia" },
            new Country { Id = 14, Code = "AT", Name = "Austria" },
            new Country { Id = 15, Code = "AZ", Name = "Azerbaijan" },
            new Country { Id = 16, Code = "BS", Name = "Bahamas" },
            new Country { Id = 17, Code = "BH", Name = "Bahrain" },
            new Country { Id = 18, Code = "BD", Name = "Bangladesh" },
            new Country { Id = 19, Code = "BB", Name = "Barbados" },
            new Country { Id = 20, Code = "BY", Name = "Belarus" },
            new Country { Id = 21, Code = "BE", Name = "Belgium" },
            new Country { Id = 22, Code = "BZ", Name = "Belize" },
            new Country { Id = 23, Code = "BJ", Name = "Benin" },
            new Country { Id = 24, Code = "BM", Name = "Bermuda" },
            new Country { Id = 25, Code = "BT", Name = "Bhutan" },
            new Country { Id = 26, Code = "BO", Name = "Bolivia" },
            new Country { Id = 27, Code = "BA", Name = "Bosnia And Herzegowina" },
            new Country { Id = 28, Code = "BW", Name = "Botswana" },
            new Country { Id = 29, Code = "BV", Name = "Bouvet Island" },
            new Country { Id = 30, Code = "BR", Name = "Brazil" },
            new Country { Id = 31, Code = "IO", Name = "British Indian Ocean Territory" },
            new Country { Id = 32, Code = "BN", Name = "Brunei Darussalam" },
            new Country { Id = 33, Code = "BG", Name = "Bulgaria" },
            new Country { Id = 34, Code = "BF", Name = "Burkina Faso" },
            new Country { Id = 35, Code = "BI", Name = "Burundi" },
            new Country { Id = 36, Code = "KH", Name = "Cambodia" },
            new Country { Id = 37, Code = "CM", Name = "Cameroon" },
            new Country { Id = 38, Code = "CA", Name = "Canada" },
            new Country { Id = 39, Code = "CV", Name = "Cape Verde" },
            new Country { Id = 40, Code = "KY", Name = "Cayman Islands" },
            new Country { Id = 41, Code = "CF", Name = "Central African Republic" },
            new Country { Id = 42, Code = "TD", Name = "Chad" },
            new Country { Id = 43, Code = "CL", Name = "Chile" },
            new Country { Id = 44, Code = "CN", Name = "China" },
            new Country { Id = 45, Code = "CX", Name = "Christmas Island" },
            new Country { Id = 46, Code = "CC", Name = "Cocos (Keeling) Islands" },
            new Country { Id = 47, Code = "CO", Name = "Colombia" },
            new Country { Id = 48, Code = "KM", Name = "Comoros" },
            new Country { Id = 49, Code = "CG", Name = "Congo" },
            new Country { Id = 50, Code = "CD", Name = "Congo, The Democratic Republic Of The" },
            new Country { Id = 51, Code = "CK", Name = "Cook Islands" },
            new Country { Id = 52, Code = "CR", Name = "Costa Rica" },
            new Country { Id = 53, Code = "CI", Name = "Cote D'ivoire" },
            new Country { Id = 54, Code = "HR", Name = "Croatia (Local Name: Hrvatska)" },
            new Country { Id = 55, Code = "CU", Name = "Cuba" },
            new Country { Id = 56, Code = "CY", Name = "Cyprus" },
            new Country { Id = 57, Code = "CZ", Name = "Czech Republic" },
            new Country { Id = 58, Code = "DK", Name = "Denmark" },
            new Country { Id = 59, Code = "DJ", Name = "Djibouti" },
            new Country { Id = 60, Code = "DM", Name = "Dominica" },
            new Country { Id = 61, Code = "DO", Name = "Dominican Republic" },
            new Country { Id = 62, Code = "TP", Name = "East Timor" },
            new Country { Id = 63, Code = "EC", Name = "Ecuador" },
            new Country { Id = 64, Code = "EG", Name = "Egypt" },
            new Country { Id = 65, Code = "SV", Name = "El Salvador" },
            new Country { Id = 66, Code = "GQ", Name = "Equatorial Guinea" },
            new Country { Id = 67, Code = "ER", Name = "Eritrea" },
            new Country { Id = 68, Code = "EE", Name = "Estonia" },
            new Country { Id = 69, Code = "ET", Name = "Ethiopia" },
            new Country { Id = 70, Code = "FK", Name = "Falkland Islands (Malvinas)" },
            new Country { Id = 71, Code = "FO", Name = "Faroe Islands" },
            new Country { Id = 72, Code = "FJ", Name = "Fiji" },
            new Country { Id = 73, Code = "FI", Name = "Finland" },
            new Country { Id = 74, Code = "FR", Name = "France" },
            new Country { Id = 75, Code = "FX", Name = "France, Metropolitan" },
            new Country { Id = 76, Code = "GF", Name = "French Guiana" },
            new Country { Id = 77, Code = "PF", Name = "French Polynesia" },
            new Country { Id = 78, Code = "TF", Name = "French Southern Territories" },
            new Country { Id = 79, Code = "GA", Name = "Gabon" },
            new Country { Id = 80, Code = "GM", Name = "Gambia" },
            new Country { Id = 81, Code = "GE", Name = "Georgia" },
            new Country { Id = 82, Code = "DE", Name = "Germany" },
            new Country { Id = 83, Code = "GH", Name = "Ghana" },
            new Country { Id = 84, Code = "GI", Name = "Gibraltar" },
            new Country { Id = 85, Code = "GR", Name = "Greece" },
            new Country { Id = 86, Code = "GL", Name = "Greenland" },
            new Country { Id = 87, Code = "GD", Name = "Grenada" },
            new Country { Id = 88, Code = "GP", Name = "Guadeloupe" },
            new Country { Id = 89, Code = "GU", Name = "Guam" },
            new Country { Id = 90, Code = "GT", Name = "Guatemala" },
            new Country { Id = 91, Code = "GN", Name = "Guinea" },
            new Country { Id = 92, Code = "GW", Name = "Guinea-bissau" },
            new Country { Id = 93, Code = "GY", Name = "Guyana" },
            new Country { Id = 94, Code = "HT", Name = "Haiti" },
            new Country { Id = 95, Code = "HM", Name = "Heard And Mc Donald Islands" },
            new Country { Id = 96, Code = "VA", Name = "Holy See (Vatican City State)" },
            new Country { Id = 97, Code = "HN", Name = "Honduras" },
            new Country { Id = 98, Code = "HK", Name = "Hong Kong" },
            new Country { Id = 99, Code = "HU", Name = "Hungary" },
            new Country { Id = 100, Code = "IS", Name = "Iceland" },
            new Country { Id = 101, Code = "IN", Name = "India" },
            new Country { Id = 102, Code = "ID", Name = "Indonesia" },
            new Country { Id = 103, Code = "IR", Name = "Iran (Islamic Republic Of)" },
            new Country { Id = 104, Code = "IQ", Name = "Iraq" },
            new Country { Id = 105, Code = "IE", Name = "Ireland" },
            new Country { Id = 106, Code = "IL", Name = "Israel" },
            new Country { Id = 107, Code = "IT", Name = "Italy" },
            new Country { Id = 108, Code = "JM", Name = "Jamaica" },
            new Country { Id = 109, Code = "JP", Name = "Japan" },
            new Country { Id = 110, Code = "JO", Name = "Jordan" },
            new Country { Id = 111, Code = "KZ", Name = "Kazakhstan" },
            new Country { Id = 112, Code = "KE", Name = "Kenya" },
            new Country { Id = 113, Code = "KI", Name = "Kiribati" },
            new Country { Id = 114, Code = "KP", Name = "Korea, Democratic People's Republic Of" },
            new Country { Id = 115, Code = "KR", Name = "Korea, Republic Of" },
            new Country { Id = 116, Code = "KW", Name = "Kuwait" },
            new Country { Id = 117, Code = "KG", Name = "Kyrgyzstan" },
            new Country { Id = 118, Code = "LA", Name = "Lao People's Democratic Republic" },
            new Country { Id = 119, Code = "LV", Name = "Latvia" },
            new Country { Id = 120, Code = "LB", Name = "Lebanon" },
            new Country { Id = 121, Code = "LS", Name = "Lesotho" },
            new Country { Id = 122, Code = "LR", Name = "Liberia" },
            new Country { Id = 123, Code = "LY", Name = "Libyan Arab Jamahiriya" },
            new Country { Id = 124, Code = "LI", Name = "Liechtenstein" },
            new Country { Id = 125, Code = "LT", Name = "Lithuania" },
            new Country { Id = 126, Code = "LU", Name = "Luxembourg" },
            new Country { Id = 127, Code = "MO", Name = "Macau" },
            new Country { Id = 128, Code = "MK", Name = "Macedonia, The Former Yugoslav Republic Of" },
            new Country { Id = 129, Code = "MG", Name = "Madagascar" },
            new Country { Id = 130, Code = "MW", Name = "Malawi" },
            new Country { Id = 131, Code = "MY", Name = "Malaysia" },
            new Country { Id = 132, Code = "MV", Name = "Maldives" },
            new Country { Id = 133, Code = "ML", Name = "Mali" },
            new Country { Id = 134, Code = "MT", Name = "Malta" },
            new Country { Id = 135, Code = "MH", Name = "Marshall Islands" },
            new Country { Id = 136, Code = "MQ", Name = "Martinique" },
            new Country { Id = 137, Code = "MR", Name = "Mauritania" },
            new Country { Id = 138, Code = "MU", Name = "Mauritius" },
            new Country { Id = 139, Code = "YT", Name = "Mayotte" },
            new Country { Id = 140, Code = "MX", Name = "Mexico" },
            new Country { Id = 141, Code = "FM", Name = "Micronesia, Federated States Of" },
            new Country { Id = 142, Code = "MD", Name = "Moldova, Republic Of" },
            new Country { Id = 143, Code = "MC", Name = "Monaco" },
            new Country { Id = 144, Code = "MN", Name = "Mongolia" },
            new Country { Id = 145, Code = "MS", Name = "Montserrat" },
            new Country { Id = 146, Code = "MA", Name = "Morocco" },
            new Country { Id = 147, Code = "MZ", Name = "Mozambique" },
            new Country { Id = 148, Code = "MM", Name = "Myanmar" },
            new Country { Id = 149, Code = "NA", Name = "Namibia" },
            new Country { Id = 150, Code = "NR", Name = "Nauru" },
            new Country { Id = 151, Code = "NP", Name = "Nepal" },
            new Country { Id = 152, Code = "NL", Name = "Netherlands" },
            new Country { Id = 153, Code = "AN", Name = "Netherlands Antilles" },
            new Country { Id = 154, Code = "NC", Name = "New Caledonia" },
            new Country { Id = 155, Code = "NZ", Name = "New Zealand" },
            new Country { Id = 156, Code = "NI", Name = "Nicaragua" },
            new Country { Id = 157, Code = "NE", Name = "Niger" },
            new Country { Id = 158, Code = "NG", Name = "Nigeria" },
            new Country { Id = 159, Code = "NU", Name = "Niue" },
            new Country { Id = 160, Code = "NF", Name = "Norfolk Island" },
            new Country { Id = 161, Code = "MP", Name = "Northern Mariana Islands" },
            new Country { Id = 162, Code = "NO", Name = "Norway" },
            new Country { Id = 163, Code = "OM", Name = "Oman" },
            new Country { Id = 164, Code = "PK", Name = "Pakistan" },
            new Country { Id = 165, Code = "PW", Name = "Palau" },
            new Country { Id = 166, Code = "PA", Name = "Panama" },
            new Country { Id = 167, Code = "PG", Name = "Papua New Guinea" },
            new Country { Id = 168, Code = "PY", Name = "Paraguay" },
            new Country { Id = 169, Code = "PE", Name = "Peru" },
            new Country { Id = 170, Code = "PH", Name = "Philippines" },
            new Country { Id = 171, Code = "PN", Name = "Pitcairn" },
            new Country { Id = 172, Code = "PL", Name = "Poland" },
            new Country { Id = 173, Code = "PT", Name = "Portugal" },
            new Country { Id = 174, Code = "PR", Name = "Puerto Rico" },
            new Country { Id = 175, Code = "QA", Name = "Qatar" },
            new Country { Id = 176, Code = "RE", Name = "Reunion" },
            new Country { Id = 177, Code = "RO", Name = "Romania" },
            new Country { Id = 178, Code = "RU", Name = "Russian Federation" },
            new Country { Id = 179, Code = "RW", Name = "Rwanda" },
            new Country { Id = 180, Code = "KN", Name = "Saint Kitts And Nevis" },
            new Country { Id = 181, Code = "LC", Name = "Saint Lucia" },
            new Country { Id = 182, Code = "VC", Name = "Saint Vincent And The Grenadines" },
            new Country { Id = 183, Code = "WS", Name = "Samoa" },
            new Country { Id = 184, Code = "SM", Name = "San Marino" },
            new Country { Id = 185, Code = "ST", Name = "Sao Tome And Principe" },
            new Country { Id = 186, Code = "SA", Name = "Saudi Arabia" },
            new Country { Id = 187, Code = "SN", Name = "Senegal" },
            new Country { Id = 188, Code = "SC", Name = "Seychelles" },
            new Country { Id = 189, Code = "SL", Name = "Sierra Leone" },
            new Country { Id = 190, Code = "SG", Name = "Singapore" },
            new Country { Id = 191, Code = "SK", Name = "Slovakia (Slovak Republic)" },
            new Country { Id = 192, Code = "SI", Name = "Slovenia" },
            new Country { Id = 193, Code = "SB", Name = "Solomon Islands" },
            new Country { Id = 194, Code = "SO", Name = "Somalia" },
            new Country { Id = 195, Code = "ZA", Name = "South Africa" },
            new Country { Id = 196, Code = "GS", Name = "South Georgia And The South Sandwich Islands" },
            new Country { Id = 197, Code = "ES", Name = "Spain" },
            new Country { Id = 198, Code = "LK", Name = "Sri Lanka" },
            new Country { Id = 199, Code = "SH", Name = "St. Helena" },
            new Country { Id = 200, Code = "PM", Name = "St. Pierre And Miquelon" },
            new Country { Id = 201, Code = "SD", Name = "Sudan" },
            new Country { Id = 202, Code = "SR", Name = "Suriname" },
            new Country { Id = 203, Code = "SJ", Name = "Svalbard And Jan Mayen Islands" },
            new Country { Id = 204, Code = "SZ", Name = "Swaziland" },
            new Country { Id = 205, Code = "SE", Name = "Sweden" },
            new Country { Id = 206, Code = "CH", Name = "Switzerland" },
            new Country { Id = 207, Code = "SY", Name = "Syrian Arab Republic" },
            new Country { Id = 208, Code = "TW", Name = "Taiwan, Province Of China" },
            new Country { Id = 209, Code = "TJ", Name = "Tajikistan" },
            new Country { Id = 210, Code = "TZ", Name = "Tanzania, United Republic Of" },
            new Country { Id = 211, Code = "TH", Name = "Thailand" },
            new Country { Id = 212, Code = "TG", Name = "Togo" },
            new Country { Id = 213, Code = "TK", Name = "Tokelau" },
            new Country { Id = 214, Code = "TO", Name = "Tonga" },
            new Country { Id = 215, Code = "TT", Name = "Trinidad And Tobago" },
            new Country { Id = 216, Code = "TN", Name = "Tunisia" },
            new Country { Id = 217, Code = "TR", Name = "Turkey" },
            new Country { Id = 218, Code = "TM", Name = "Turkmenistan" },
            new Country { Id = 219, Code = "TC", Name = "Turks And Caicos Islands" },
            new Country { Id = 220, Code = "TV", Name = "Tuvalu" },
            new Country { Id = 221, Code = "UG", Name = "Uganda" },
            new Country { Id = 222, Code = "UA", Name = "Ukraine" },
            new Country { Id = 223, Code = "AE", Name = "United Arab Emirates" },
            new Country { Id = 224, Code = "GB", Name = "United Kingdom" },
            new Country { Id = 225, Code = "US", Name = "United States" },
            new Country { Id = 226, Code = "UM", Name = "United States Minor Outlying Islands" },
            new Country { Id = 227, Code = "UY", Name = "Uruguay" },
            new Country { Id = 228, Code = "UZ", Name = "Uzbekistan" },
            new Country { Id = 229, Code = "VU", Name = "Vanuatu" },
            new Country { Id = 230, Code = "VE", Name = "Venezuela" },
            new Country { Id = 231, Code = "VN", Name = "Viet Nam" },
            new Country { Id = 232, Code = "VG", Name = "Virgin Islands (British)" },
            new Country { Id = 233, Code = "VI", Name = "Virgin Islands (U.S.)" },
            new Country { Id = 234, Code = "WF", Name = "Wallis And Futuna Islands" },
            new Country { Id = 235, Code = "EH", Name = "Western Sahara" },
            new Country { Id = 236, Code = "YE", Name = "Yemen" },
            new Country { Id = 237, Code = "YU", Name = "Yugoslavia" },
            new Country { Id = 238, Code = "ZM", Name = "Zambia" },
            new Country { Id = 239, Code = "ZW", Name = "Zimbabwe" }
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
