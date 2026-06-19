using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessClub_Test.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringEventsSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DaysOfWeek",
                table: "Calendar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndRecur",
                table: "Calendar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExDates",
                table: "Calendar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "Calendar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "Calendar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RecurrenceRule",
                table: "Calendar",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysOfWeek",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "EndRecur",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "ExDates",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "RecurrenceRule",
                table: "Calendar");
        }
    }
}
