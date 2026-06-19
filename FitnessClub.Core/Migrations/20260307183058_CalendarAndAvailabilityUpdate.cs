using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessClub_Test.Core.Migrations
{
    /// <inheritdoc />
    public partial class CalendarAndAvailabilityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ----------------------
            // Availability table fixes
            // ----------------------
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "Availability",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(TimeOnly),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "Availability",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(TimeOnly),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Availability",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Availability",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            // ----------------------
            // CalendarUser join table
            // ----------------------
            migrationBuilder.CreateTable(
                name: "CalendarUser",
                columns: table => new
                {
                    CalendarID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarUser", x => new { x.CalendarID, x.UserID });
                    table.ForeignKey(
                        name: "FK_CalendarUser_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalendarUser_Calendar_CalendarID",
                        column: x => x.CalendarID,
                        principalTable: "Calendar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarUser_UserID",
                table: "CalendarUser",
                column: "UserID");

            // ----------------------
            // Calendar table indexes + FKs
            // ----------------------
            migrationBuilder.CreateIndex(
                name: "IX_Calendar_CoachID",
                table: "Calendar",
                column: "CoachID");

            migrationBuilder.CreateIndex(
                name: "IX_Calendar_ClientID",
                table: "Calendar",
                column: "ClientID");

            migrationBuilder.AddForeignKey(
                name: "FK_Calendar_AspNetUsers_CoachID",
                table: "Calendar",
                column: "CoachID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Calendar_AspNetUsers_ClientID",
                table: "Calendar",
                column: "ClientID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ----------------------
            // Calendar table FKs & indexes
            // ----------------------
            migrationBuilder.DropForeignKey(
                name: "FK_Calendar_AspNetUsers_CoachID",
                table: "Calendar");

            migrationBuilder.DropForeignKey(
                name: "FK_Calendar_AspNetUsers_ClientID",
                table: "Calendar");

            migrationBuilder.DropIndex(name: "IX_Calendar_CoachID", table: "Calendar");
            migrationBuilder.DropIndex(name: "IX_Calendar_ClientID", table: "Calendar");

            // ----------------------
            // CalendarUser join table
            // ----------------------
            migrationBuilder.DropTable(name: "CalendarUser");

            // ----------------------
            // Availability table rollback
            // ----------------------
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "Availability",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "Availability",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Availability",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Availability",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
