using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EndPointCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsEnabledToCountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_enabled",
                table: "countries",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_enabled",
                table: "countries");
        }
    }
}
