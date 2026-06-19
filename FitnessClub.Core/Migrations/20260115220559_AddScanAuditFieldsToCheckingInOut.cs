using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessClub_Test.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddScanAuditFieldsToCheckingInOut : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "CheckingInOut",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "CheckingInOut",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "QrToken", table: "CheckingInOut");
            migrationBuilder.DropColumn(name: "ScanIp", table: "CheckingInOut");
            migrationBuilder.DropColumn(name: "ScanDevice", table: "CheckingInOut");
            migrationBuilder.DropColumn(name: "Result", table: "CheckingInOut");
            migrationBuilder.DropColumn(name: "Timestamp", table: "CheckingInOut");
        }
    }

}
