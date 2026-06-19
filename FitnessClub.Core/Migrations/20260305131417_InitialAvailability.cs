using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessClub_Test.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Subscription_Payment",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Day",
                table: "Availability",
                type: "date",
                unicode: false,
                maxLength: 10,
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_ClientId",
                table: "Feedback",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Client_ClientId",
                table: "Feedback",
                column: "ClientId",
                principalTable: "Client",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Client_ClientId",
                table: "Feedback");

            migrationBuilder.DropIndex(
                name: "IX_Feedback_ClientId",
                table: "Feedback");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Subscription_Payment",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Day",
                table: "Availability",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldUnicode: false,
                oldMaxLength: 10);
        }
    }
}
