using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EndPointCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDefaultAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "access_failed_count", "concurrency_stamp", "customer_id", "email", "email_confirmed", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "password_hash", "phone_number", "phone_number_confirmed", "security_stamp", "two_factor_enabled", "user_name" },
                values: new object[] { 1, 0, "99982048-369f-4174-b277-40c991417169", null, "epadmin@endpointcorp.com", true, true, null, "EPADMIN@ENDPOINTCORP.COM", "EPADMIN", "AQAAAAIAAYagAAAAEGYF8Zctt9SJG5HeU2g1GKqqJxU2tNQkUDgRJzC0BcaeinhhfvRkuQQKsjmoFrrHoQ==", null, false, "abd3ee70-301d-4822-9953-69e8b4be3c88", false, "epadmin" });
        }
    }
}
