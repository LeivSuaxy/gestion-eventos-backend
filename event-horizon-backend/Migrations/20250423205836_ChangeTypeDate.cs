using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace event_horizon_backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTypeDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Events",
                type: "time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "Events",
                type: "time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Events",
                type: "time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Events",
                type: "time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Events",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "Events",
                type: "timestamp",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Events",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Events",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "time zone");
        }
    }
}
