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
                name: "countries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_countries", x => x.id);
                });

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
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    country_id = table.Column<int>(type: "integer", nullable: false),
                    state_id = table.Column<int>(type: "integer", nullable: true),
                    customer_id = table.Column<int>(type: "integer", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_addresses_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_addresses_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_addresses_states_state_id",
                        column: x => x.state_id,
                        principalTable: "states",
                        principalColumn: "id");
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
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                table: "countries",
                columns: new[] { "id", "code", "is_enabled", "name" },
                values: new object[,]
                {
                    { 1, "AF", true, "Afghanistan" },
                    { 2, "AL", true, "Albania" },
                    { 3, "DZ", true, "Algeria" },
                    { 4, "AS", true, "American Samoa" },
                    { 5, "AD", true, "Andorra" },
                    { 6, "AO", true, "Angola" },
                    { 7, "AI", true, "Anguilla" },
                    { 8, "AQ", true, "Antarctica" },
                    { 9, "AG", true, "Antigua And Barbuda" },
                    { 10, "AR", true, "Argentina" },
                    { 11, "AM", true, "Armenia" },
                    { 12, "AW", true, "Aruba" },
                    { 13, "AU", true, "Australia" },
                    { 14, "AT", true, "Austria" },
                    { 15, "AZ", true, "Azerbaijan" },
                    { 16, "BS", true, "Bahamas" },
                    { 17, "BH", true, "Bahrain" },
                    { 18, "BD", true, "Bangladesh" },
                    { 19, "BB", true, "Barbados" },
                    { 20, "BY", true, "Belarus" },
                    { 21, "BE", true, "Belgium" },
                    { 22, "BZ", true, "Belize" },
                    { 23, "BJ", true, "Benin" },
                    { 24, "BM", true, "Bermuda" },
                    { 25, "BT", true, "Bhutan" },
                    { 26, "BO", true, "Bolivia" },
                    { 27, "BA", true, "Bosnia And Herzegowina" },
                    { 28, "BW", true, "Botswana" },
                    { 29, "BV", true, "Bouvet Island" },
                    { 30, "BR", true, "Brazil" },
                    { 31, "IO", true, "British Indian Ocean Territory" },
                    { 32, "BN", true, "Brunei Darussalam" },
                    { 33, "BG", true, "Bulgaria" },
                    { 34, "BF", true, "Burkina Faso" },
                    { 35, "BI", true, "Burundi" },
                    { 36, "KH", true, "Cambodia" },
                    { 37, "CM", true, "Cameroon" },
                    { 38, "CA", true, "Canada" },
                    { 39, "CV", true, "Cape Verde" },
                    { 40, "KY", true, "Cayman Islands" },
                    { 41, "CF", true, "Central African Republic" },
                    { 42, "TD", true, "Chad" },
                    { 43, "CL", true, "Chile" },
                    { 44, "CN", true, "China" },
                    { 45, "CX", true, "Christmas Island" },
                    { 46, "CC", true, "Cocos (Keeling) Islands" },
                    { 47, "CO", true, "Colombia" },
                    { 48, "KM", true, "Comoros" },
                    { 49, "CG", true, "Congo" },
                    { 50, "CD", true, "Congo, The Democratic Republic Of The" },
                    { 51, "CK", true, "Cook Islands" },
                    { 52, "CR", true, "Costa Rica" },
                    { 53, "CI", true, "Cote D'ivoire" },
                    { 54, "HR", true, "Croatia (Local Name: Hrvatska)" },
                    { 55, "CU", true, "Cuba" },
                    { 56, "CY", true, "Cyprus" },
                    { 57, "CZ", true, "Czech Republic" },
                    { 58, "DK", true, "Denmark" },
                    { 59, "DJ", true, "Djibouti" },
                    { 60, "DM", true, "Dominica" },
                    { 61, "DO", true, "Dominican Republic" },
                    { 62, "TP", true, "East Timor" },
                    { 63, "EC", true, "Ecuador" },
                    { 64, "EG", true, "Egypt" },
                    { 65, "SV", true, "El Salvador" },
                    { 66, "GQ", true, "Equatorial Guinea" },
                    { 67, "ER", true, "Eritrea" },
                    { 68, "EE", true, "Estonia" },
                    { 69, "ET", true, "Ethiopia" },
                    { 70, "FK", true, "Falkland Islands (Malvinas)" },
                    { 71, "FO", true, "Faroe Islands" },
                    { 72, "FJ", true, "Fiji" },
                    { 73, "FI", true, "Finland" },
                    { 74, "FR", true, "France" },
                    { 75, "FX", true, "France, Metropolitan" },
                    { 76, "GF", true, "French Guiana" },
                    { 77, "PF", true, "French Polynesia" },
                    { 78, "TF", true, "French Southern Territories" },
                    { 79, "GA", true, "Gabon" },
                    { 80, "GM", true, "Gambia" },
                    { 81, "GE", true, "Georgia" },
                    { 82, "DE", true, "Germany" },
                    { 83, "GH", true, "Ghana" },
                    { 84, "GI", true, "Gibraltar" },
                    { 85, "GR", true, "Greece" },
                    { 86, "GL", true, "Greenland" },
                    { 87, "GD", true, "Grenada" },
                    { 88, "GP", true, "Guadeloupe" },
                    { 89, "GU", true, "Guam" },
                    { 90, "GT", true, "Guatemala" },
                    { 91, "GN", true, "Guinea" },
                    { 92, "GW", true, "Guinea-bissau" },
                    { 93, "GY", true, "Guyana" },
                    { 94, "HT", true, "Haiti" },
                    { 95, "HM", true, "Heard And Mc Donald Islands" },
                    { 96, "VA", true, "Holy See (Vatican City State)" },
                    { 97, "HN", true, "Honduras" },
                    { 98, "HK", true, "Hong Kong" },
                    { 99, "HU", true, "Hungary" },
                    { 100, "IS", true, "Iceland" },
                    { 101, "IN", true, "India" },
                    { 102, "ID", true, "Indonesia" },
                    { 103, "IR", true, "Iran (Islamic Republic Of)" },
                    { 104, "IQ", true, "Iraq" },
                    { 105, "IE", true, "Ireland" },
                    { 106, "IL", true, "Israel" },
                    { 107, "IT", true, "Italy" },
                    { 108, "JM", true, "Jamaica" },
                    { 109, "JP", true, "Japan" },
                    { 110, "JO", true, "Jordan" },
                    { 111, "KZ", true, "Kazakhstan" },
                    { 112, "KE", true, "Kenya" },
                    { 113, "KI", true, "Kiribati" },
                    { 114, "KP", true, "Korea, Democratic People's Republic Of" },
                    { 115, "KR", true, "Korea, Republic Of" },
                    { 116, "KW", true, "Kuwait" },
                    { 117, "KG", true, "Kyrgyzstan" },
                    { 118, "LA", true, "Lao People's Democratic Republic" },
                    { 119, "LV", true, "Latvia" },
                    { 120, "LB", true, "Lebanon" },
                    { 121, "LS", true, "Lesotho" },
                    { 122, "LR", true, "Liberia" },
                    { 123, "LY", true, "Libyan Arab Jamahiriya" },
                    { 124, "LI", true, "Liechtenstein" },
                    { 125, "LT", true, "Lithuania" },
                    { 126, "LU", true, "Luxembourg" },
                    { 127, "MO", true, "Macau" },
                    { 128, "MK", true, "Macedonia, The Former Yugoslav Republic Of" },
                    { 129, "MG", true, "Madagascar" },
                    { 130, "MW", true, "Malawi" },
                    { 131, "MY", true, "Malaysia" },
                    { 132, "MV", true, "Maldives" },
                    { 133, "ML", true, "Mali" },
                    { 134, "MT", true, "Malta" },
                    { 135, "MH", true, "Marshall Islands" },
                    { 136, "MQ", true, "Martinique" },
                    { 137, "MR", true, "Mauritania" },
                    { 138, "MU", true, "Mauritius" },
                    { 139, "YT", true, "Mayotte" },
                    { 140, "MX", true, "Mexico" },
                    { 141, "FM", true, "Micronesia, Federated States Of" },
                    { 142, "MD", true, "Moldova, Republic Of" },
                    { 143, "MC", true, "Monaco" },
                    { 144, "MN", true, "Mongolia" },
                    { 145, "MS", true, "Montserrat" },
                    { 146, "MA", true, "Morocco" },
                    { 147, "MZ", true, "Mozambique" },
                    { 148, "MM", true, "Myanmar" },
                    { 149, "NA", true, "Namibia" },
                    { 150, "NR", true, "Nauru" },
                    { 151, "NP", true, "Nepal" },
                    { 152, "NL", true, "Netherlands" },
                    { 153, "AN", true, "Netherlands Antilles" },
                    { 154, "NC", true, "New Caledonia" },
                    { 155, "NZ", true, "New Zealand" },
                    { 156, "NI", true, "Nicaragua" },
                    { 157, "NE", true, "Niger" },
                    { 158, "NG", true, "Nigeria" },
                    { 159, "NU", true, "Niue" },
                    { 160, "NF", true, "Norfolk Island" },
                    { 161, "MP", true, "Northern Mariana Islands" },
                    { 162, "NO", true, "Norway" },
                    { 163, "OM", true, "Oman" },
                    { 164, "PK", true, "Pakistan" },
                    { 165, "PW", true, "Palau" },
                    { 166, "PA", true, "Panama" },
                    { 167, "PG", true, "Papua New Guinea" },
                    { 168, "PY", true, "Paraguay" },
                    { 169, "PE", true, "Peru" },
                    { 170, "PH", true, "Philippines" },
                    { 171, "PN", true, "Pitcairn" },
                    { 172, "PL", true, "Poland" },
                    { 173, "PT", true, "Portugal" },
                    { 174, "PR", true, "Puerto Rico" },
                    { 175, "QA", true, "Qatar" },
                    { 176, "RE", true, "Reunion" },
                    { 177, "RO", true, "Romania" },
                    { 178, "RU", true, "Russian Federation" },
                    { 179, "RW", true, "Rwanda" },
                    { 180, "KN", true, "Saint Kitts And Nevis" },
                    { 181, "LC", true, "Saint Lucia" },
                    { 182, "VC", true, "Saint Vincent And The Grenadines" },
                    { 183, "WS", true, "Samoa" },
                    { 184, "SM", true, "San Marino" },
                    { 185, "ST", true, "Sao Tome And Principe" },
                    { 186, "SA", true, "Saudi Arabia" },
                    { 187, "SN", true, "Senegal" },
                    { 188, "SC", true, "Seychelles" },
                    { 189, "SL", true, "Sierra Leone" },
                    { 190, "SG", true, "Singapore" },
                    { 191, "SK", true, "Slovakia (Slovak Republic)" },
                    { 192, "SI", true, "Slovenia" },
                    { 193, "SB", true, "Solomon Islands" },
                    { 194, "SO", true, "Somalia" },
                    { 195, "ZA", true, "South Africa" },
                    { 196, "GS", true, "South Georgia And The South Sandwich Islands" },
                    { 197, "ES", true, "Spain" },
                    { 198, "LK", true, "Sri Lanka" },
                    { 199, "SH", true, "St. Helena" },
                    { 200, "PM", true, "St. Pierre And Miquelon" },
                    { 201, "SD", true, "Sudan" },
                    { 202, "SR", true, "Suriname" },
                    { 203, "SJ", true, "Svalbard And Jan Mayen Islands" },
                    { 204, "SZ", true, "Swaziland" },
                    { 205, "SE", true, "Sweden" },
                    { 206, "CH", true, "Switzerland" },
                    { 207, "SY", true, "Syrian Arab Republic" },
                    { 208, "TW", true, "Taiwan, Province Of China" },
                    { 209, "TJ", true, "Tajikistan" },
                    { 210, "TZ", true, "Tanzania, United Republic Of" },
                    { 211, "TH", true, "Thailand" },
                    { 212, "TG", true, "Togo" },
                    { 213, "TK", true, "Tokelau" },
                    { 214, "TO", true, "Tonga" },
                    { 215, "TT", true, "Trinidad And Tobago" },
                    { 216, "TN", true, "Tunisia" },
                    { 217, "TR", true, "Turkey" },
                    { 218, "TM", true, "Turkmenistan" },
                    { 219, "TC", true, "Turks And Caicos Islands" },
                    { 220, "TV", true, "Tuvalu" },
                    { 221, "UG", true, "Uganda" },
                    { 222, "UA", true, "Ukraine" },
                    { 223, "AE", true, "United Arab Emirates" },
                    { 224, "GB", true, "United Kingdom" },
                    { 225, "US", true, "United States" },
                    { 226, "UM", true, "United States Minor Outlying Islands" },
                    { 227, "UY", true, "Uruguay" },
                    { 228, "UZ", true, "Uzbekistan" },
                    { 229, "VU", true, "Vanuatu" },
                    { 230, "VE", true, "Venezuela" },
                    { 231, "VN", true, "Viet Nam" },
                    { 232, "VG", true, "Virgin Islands (British)" },
                    { 233, "VI", true, "Virgin Islands (U.S.)" },
                    { 234, "WF", true, "Wallis And Futuna Islands" },
                    { 235, "EH", true, "Western Sahara" },
                    { 236, "YE", true, "Yemen" },
                    { 237, "YU", true, "Yugoslavia" },
                    { 238, "ZM", true, "Zambia" },
                    { 239, "ZW", true, "Zimbabwe" }
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

            migrationBuilder.CreateIndex(
                name: "ix_addresses_country_id",
                table: "addresses",
                column: "country_id");

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
                name: "ix_countries_code",
                table: "countries",
                column: "code",
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
                name: "countries");

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
