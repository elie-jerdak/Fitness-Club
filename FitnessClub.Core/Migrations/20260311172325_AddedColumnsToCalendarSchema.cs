using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessClub_Test.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddedColumnsToCalendarSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundColor",
                table: "Calendar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Calendar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Display",
                table: "Calendar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBackground",
                table: "Calendar",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "Display",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "IsBackground",
                table: "Calendar");
        }
    }
}
