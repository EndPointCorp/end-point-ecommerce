using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EndPointCommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryToAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_addresses_states_state_id",
                table: "addresses");

            migrationBuilder.AlterColumn<int>(
                name: "state_id",
                table: "addresses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "country_id",
                table: "addresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_addresses_country_id",
                table: "addresses",
                column: "country_id");

            migrationBuilder.AddForeignKey(
                name: "fk_addresses_countries_country_id",
                table: "addresses",
                column: "country_id",
                principalTable: "countries",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_addresses_states_state_id",
                table: "addresses",
                column: "state_id",
                principalTable: "states",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_addresses_countries_country_id",
                table: "addresses");

            migrationBuilder.DropForeignKey(
                name: "fk_addresses_states_state_id",
                table: "addresses");

            migrationBuilder.DropIndex(
                name: "ix_addresses_country_id",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "country_id",
                table: "addresses");

            migrationBuilder.AlterColumn<int>(
                name: "state_id",
                table: "addresses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_addresses_states_state_id",
                table: "addresses",
                column: "state_id",
                principalTable: "states",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
