using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace event_horizon_backend.Migrations
{
    /// <inheritdoc />
    public partial class BalanceAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Events",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "AspNetUsers",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "AspNetUsers");
        }
    }
}
