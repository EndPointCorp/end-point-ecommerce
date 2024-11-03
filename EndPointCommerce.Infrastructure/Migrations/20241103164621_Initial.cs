using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EndPointCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "coupons",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    discount = table.Column<decimal>(type: "numeric", nullable: false),
                    is_discount_fixed = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_coupons", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payment_methods",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_methods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_claims", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    normalized_name = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "site_contents",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_site_contents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "states",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    abbreviation = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_states", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claims", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: true),
                    user_name = table.Column<string>(type: "text", nullable: true),
                    normalized_user_name = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    normalized_email = table.Column<string>(type: "text", nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    street = table.Column<string>(type: "text", nullable: false),
                    street_two = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "text", nullable: false),
                    zip_code = table.Column<string>(type: "text", nullable: false),
                    state_id = table.Column<int>(type: "integer", nullable: false),
                    customer_id = table.Column<int>(type: "integer", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_addresses_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_addresses_states_state_id",
                        column: x => x.state_id,
                        principalTable: "states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quotes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: true),
                    coupon_id = table.Column<int>(type: "integer", nullable: true),
                    is_open = table.Column<bool>(type: "boolean", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    shipping_address_id = table.Column<int>(type: "integer", nullable: true),
                    billing_address_id = table.Column<int>(type: "integer", nullable: true),
                    tax = table.Column<decimal>(type: "numeric", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quotes", x => x.id);
                    table.ForeignKey(
                        name: "fk_quotes_addresses_billing_address_id",
                        column: x => x.billing_address_id,
                        principalTable: "addresses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_quotes_addresses_shipping_address_id",
                        column: x => x.shipping_address_id,
                        principalTable: "addresses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_quotes_coupons_coupon_id",
                        column: x => x.coupon_id,
                        principalTable: "coupons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_quotes_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    coupon_id = table.Column<int>(type: "integer", nullable: true),
                    quote_id = table.Column<int>(type: "integer", nullable: true),
                    subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    discount = table.Column<decimal>(type: "numeric", nullable: false),
                    tax = table.Column<decimal>(type: "numeric", nullable: false),
                    total = table.Column<decimal>(type: "numeric", nullable: false),
                    shipping_address_id = table.Column<int>(type: "integer", nullable: false),
                    billing_address_id = table.Column<int>(type: "integer", nullable: false),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    payment_method_id = table.Column<int>(type: "integer", nullable: false),
                    payment_method_nonce_value = table.Column<string>(type: "text", nullable: true),
                    payment_method_nonce_descriptor = table.Column<string>(type: "text", nullable: true),
                    payment_transaction_id = table.Column<string>(type: "text", nullable: true),
                    tracking_number = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_orders_addresses_billing_address_id",
                        column: x => x.billing_address_id,
                        principalTable: "addresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_orders_addresses_shipping_address_id",
                        column: x => x.shipping_address_id,
                        principalTable: "addresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_orders_coupons_coupon_id",
                        column: x => x.coupon_id,
                        principalTable: "coupons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_orders_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_orders_order_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "order_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_orders_payment_methods_payment_method_id",
                        column: x => x.payment_method_id,
                        principalTable: "payment_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_orders_quotes_quote_id",
                        column: x => x.quote_id,
                        principalTable: "quotes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    url_key = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    main_image_id = table.Column<int>(type: "integer", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false),
                    meta_title = table.Column<string>(type: "text", nullable: true),
                    meta_keywords = table.Column<string>(type: "text", nullable: true),
                    meta_description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    discriminator = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_images", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    url_key = table.Column<string>(type: "text", nullable: true),
                    sku = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_in_stock = table.Column<bool>(type: "boolean", nullable: false),
                    is_discounted = table.Column<bool>(type: "boolean", nullable: false),
                    base_price = table.Column<decimal>(type: "numeric", nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    short_description = table.Column<string>(type: "text", nullable: true),
                    weight = table.Column<decimal>(type: "numeric", nullable: true),
                    category_id = table.Column<int>(type: "integer", nullable: true),
                    main_image_id = table.Column<int>(type: "integer", nullable: true),
                    thumbnail_image_id = table.Column<int>(type: "integer", nullable: true),
                    search_vector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: true)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description", "short_description", "meta_description", "meta_keywords", "sku" }),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false),
                    meta_title = table.Column<string>(type: "text", nullable: true),
                    meta_keywords = table.Column<string>(type: "text", nullable: true),
                    meta_description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_products_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_products_product_images_main_image_id",
                        column: x => x.main_image_id,
                        principalTable: "images",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_products_product_images_thumbnail_image_id",
                        column: x => x.thumbnail_image_id,
                        principalTable: "images",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "quote_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quote_id = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quote_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_quote_items_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quote_items_quotes_quote_id",
                        column: x => x.quote_id,
                        principalTable: "quotes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    quote_item_id = table.Column<int>(type: "integer", nullable: true),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric", nullable: false),
                    discount = table.Column<decimal>(type: "numeric", nullable: false),
                    total = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_items_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_items_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_items_quote_items_quote_item_id",
                        column: x => x.quote_item_id,
                        principalTable: "quote_items",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "order_statuses",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Processing" },
                    { 3, "Invoiced" },
                    { 4, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "payment_methods",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Credit Card" },
                    { 2, "Free Order" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { 1, "832e598d-6660-4c6f-a48a-9bed25a49671", "Admin", "ADMIN" },
                    { 2, "8a80a6c6-dae7-48a0-9b5b-841564ba537d", "Customer", "CUSTOMER" }
                });

            migrationBuilder.InsertData(
                table: "site_contents",
                columns: new[] { "id", "content", "name" },
                values: new object[,]
                {
                    { 1, "", "homepage_content" },
                    { 2, "", "site_message" }
                });

            migrationBuilder.InsertData(
                table: "states",
                columns: new[] { "id", "abbreviation", "name" },
                values: new object[,]
                {
                    { 1, "AL", "Alabama" },
                    { 2, "AK", "Alaska" },
                    { 3, "AZ", "Arizona" },
                    { 4, "AR", "Arkansas" },
                    { 5, "CA", "California" },
                    { 6, "CO", "Colorado" },
                    { 7, "CT", "Connecticut" },
                    { 8, "DE", "Delaware" },
                    { 9, "FL", "Florida" },
                    { 10, "GA", "Georgia" },
                    { 11, "HI", "Hawaii" },
                    { 12, "ID", "Idaho" },
                    { 13, "IL", "Illinois" },
                    { 14, "IN", "Indiana" },
                    { 15, "IA", "Iowa" },
                    { 16, "KS", "Kansas" },
                    { 17, "KY", "Kentucky" },
                    { 18, "LA", "Louisiana" },
                    { 19, "ME", "Maine" },
                    { 20, "MD", "Maryland" },
                    { 21, "MA", "Massachusetts" },
                    { 22, "MI", "Michigan" },
                    { 23, "MN", "Minnesota" },
                    { 24, "MS", "Mississippi" },
                    { 25, "MO", "Missouri" },
                    { 26, "MT", "Montana" },
                    { 27, "NE", "Nebraska" },
                    { 28, "NV", "Nevada" },
                    { 29, "NH", "New Hampshire" },
                    { 30, "NJ", "New Jersey" },
                    { 31, "NM", "New Mexico" },
                    { 32, "NY", "New York" },
                    { 33, "NC", "North Carolina" },
                    { 34, "ND", "North Dakota" },
                    { 35, "OH", "Ohio" },
                    { 36, "OK", "Oklahoma" },
                    { 37, "OR", "Oregon" },
                    { 38, "PA", "Pennsylvania" },
                    { 39, "RI", "Rhode Island" },
                    { 40, "SC", "South Carolina" },
                    { 41, "SD", "South Dakota" },
                    { 42, "TN", "Tennessee" },
                    { 43, "TX", "Texas" },
                    { 44, "UT", "Utah" },
                    { 45, "VT", "Vermont" },
                    { 46, "VA", "Virginia" },
                    { 47, "WA", "Washington" },
                    { 48, "WV", "West Virginia" },
                    { 49, "WI", "Wisconsin" },
                    { 50, "WY", "Wyoming" },
                    { 51, "DC", "District of Columbia" }
                });

            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "role_id", "user_id" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "access_failed_count", "concurrency_stamp", "customer_id", "email", "email_confirmed", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "password_hash", "phone_number", "phone_number_confirmed", "security_stamp", "two_factor_enabled", "user_name" },
                values: new object[] { 1, 0, "99982048-369f-4174-b277-40c991417169", null, "epadmin@endpointcorp.com", true, true, null, "EPADMIN@ENDPOINTCORP.COM", "EPADMIN", "AQAAAAIAAYagAAAAEGYF8Zctt9SJG5HeU2g1GKqqJxU2tNQkUDgRJzC0BcaeinhhfvRkuQQKsjmoFrrHoQ==", null, false, "abd3ee70-301d-4822-9953-69e8b4be3c88", false, "epadmin" });

            migrationBuilder.CreateIndex(
                name: "ix_addresses_customer_id",
                table: "addresses",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_addresses_state_id",
                table: "addresses",
                column: "state_id");

            migrationBuilder.CreateIndex(
                name: "ix_categories_main_image_id",
                table: "categories",
                column: "main_image_id");

            migrationBuilder.CreateIndex(
                name: "ix_categories_name",
                table: "categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_categories_url_key",
                table: "categories",
                column: "url_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_coupons_code",
                table: "coupons",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_images_product_id",
                table: "images",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_items_order_id",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_items_product_id",
                table: "order_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_items_quote_item_id",
                table: "order_items",
                column: "quote_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_billing_address_id",
                table: "orders",
                column: "billing_address_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_coupon_id",
                table: "orders",
                column: "coupon_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_payment_method_id",
                table: "orders",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_quote_id",
                table: "orders",
                column: "quote_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_shipping_address_id",
                table: "orders",
                column: "shipping_address_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_status_id",
                table: "orders",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_category_id",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_main_image_id",
                table: "products",
                column: "main_image_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_name",
                table: "products",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_products_search_vector",
                table: "products",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_products_thumbnail_image_id",
                table: "products",
                column: "thumbnail_image_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_url_key",
                table: "products",
                column: "url_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_quote_items_product_id",
                table: "quote_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_quote_items_quote_id",
                table: "quote_items",
                column: "quote_id");

            migrationBuilder.CreateIndex(
                name: "ix_quotes_billing_address_id",
                table: "quotes",
                column: "billing_address_id");

            migrationBuilder.CreateIndex(
                name: "ix_quotes_coupon_id",
                table: "quotes",
                column: "coupon_id");

            migrationBuilder.CreateIndex(
                name: "ix_quotes_customer_id",
                table: "quotes",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_quotes_shipping_address_id",
                table: "quotes",
                column: "shipping_address_id");

            migrationBuilder.CreateIndex(
                name: "ix_site_contents_name",
                table: "site_contents",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_states_abbreviation",
                table: "states",
                column: "abbreviation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_customer_id",
                table: "users",
                column: "customer_id");

            migrationBuilder.AddForeignKey(
                name: "fk_categories_category_images_main_image_id",
                table: "categories",
                column: "main_image_id",
                principalTable: "images",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_images_products_product_id",
                table: "images",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_categories_category_images_main_image_id",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "fk_products_product_images_main_image_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "fk_products_product_images_thumbnail_image_id",
                table: "products");

            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "site_contents");

            migrationBuilder.DropTable(
                name: "user_claims");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "quote_items");

            migrationBuilder.DropTable(
                name: "order_statuses");

            migrationBuilder.DropTable(
                name: "payment_methods");

            migrationBuilder.DropTable(
                name: "quotes");

            migrationBuilder.DropTable(
                name: "addresses");

            migrationBuilder.DropTable(
                name: "coupons");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "states");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
