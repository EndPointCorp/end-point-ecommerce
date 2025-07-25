using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EndPointCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultAdminAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "role_id", "user_id" },
                values: new object[] { 1, 999 });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "access_failed_count", "concurrency_stamp", "customer_id", "email", "email_confirmed", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "password_hash", "phone_number", "phone_number_confirmed", "security_stamp", "two_factor_enabled", "user_name" },
                values: new object[] { 999, 0, "11c224f7-0ae4-4a0e-a84f-4c347879cdcb", null, "demoadmin@endpointcommerce.com", true, false, null, "DEMOADMIN@ENDPOINTCOMMERCE.COM", "DEMOADMIN@ENDPOINTCOMMERCE.COM", "AQAAAAIAAYagAAAAECXf2a5iPTohU44T1WF2wHcSCnV30fIxmIgE6cNWwavkO9Hsojvn1lofUFJeekRPZg==", null, false, "UTX2NGNJHCDEVFHGTHIRYFSP6VJUFNMJ", false, "demoadmin@endpointcommerce.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user_roles",
                keyColumns: new[] { "role_id", "user_id" },
                keyValues: new object[] { 1, 999 });

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 999);
        }
    }
}
