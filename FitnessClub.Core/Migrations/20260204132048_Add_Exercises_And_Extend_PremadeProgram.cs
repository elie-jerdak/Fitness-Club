using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessClub_Test.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_Exercises_And_Extend_PremadeProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Premade_Program",
                newName: "EquipmentNeeded");

            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                table: "Premade_Program",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "Premade_Program",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Intensity",
                table: "Premade_Program",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfExercises",
                table: "Premade_Program",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PremadeProgramExercises",
                columns: table => new
                {
                    PremadeProgramId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false),
                    NumberOfSets = table.Column<int>(type: "int", nullable: false),
                    NumberOfReps = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremadeProgramExercises", x => new { x.PremadeProgramId, x.ExerciseId });
                    table.ForeignKey(
                        name: "FK_PremadeProgramExercises_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PremadeProgramExercises_Premade_Program_PremadeProgramId",
                        column: x => x.PremadeProgramId,
                        principalTable: "Premade_Program",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PremadeProgramExercises_ExerciseId",
                table: "PremadeProgramExercises",
                column: "ExerciseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PremadeProgramExercises");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropColumn(
                name: "Benefits",
                table: "Premade_Program");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Premade_Program");

            migrationBuilder.DropColumn(
                name: "Intensity",
                table: "Premade_Program");

            migrationBuilder.DropColumn(
                name: "NumberOfExercises",
                table: "Premade_Program");

            migrationBuilder.RenameColumn(
                name: "EquipmentNeeded",
                table: "Premade_Program",
                newName: "Content");

        }
    }
}
