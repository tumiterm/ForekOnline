using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class AttachD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonPlanConfig");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "LessonPlan");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "LessonPlan");

            migrationBuilder.AddColumn<string>(
                name: "Document",
                table: "LessonPlan",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAppoved",
                table: "LessonPlan",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "IsApprovedBy",
                table: "LessonPlan",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "LessonPlan",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Document",
                table: "LessonPlan");

            migrationBuilder.DropColumn(
                name: "IsAppoved",
                table: "LessonPlan");

            migrationBuilder.DropColumn(
                name: "IsApprovedBy",
                table: "LessonPlan");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "LessonPlan");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "LessonPlan",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "LessonPlan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LessonPlanConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionPlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssessmentStrategy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Consumables = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LecturerName = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LessonPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Simulation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPlanConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonPlanConfig_LessonPlan_LessonPlanId",
                        column: x => x.LessonPlanId,
                        principalTable: "LessonPlan",
                        principalColumn: "LessonPlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlanConfig_LessonPlanId",
                table: "LessonPlanConfig",
                column: "LessonPlanId",
                unique: true);
        }
    }
}
