using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class LP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonPlan_LessonPlanConfig_LessonPlanConfigsId",
                table: "LessonPlan");

            migrationBuilder.DropIndex(
                name: "IX_LessonPlan_LessonPlanConfigsId",
                table: "LessonPlan");

            migrationBuilder.DropColumn(
                name: "LessonPlanConfigsId",
                table: "LessonPlan");

            migrationBuilder.AddColumn<Guid>(
                name: "LessonPlanId",
                table: "LessonPlanConfig",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlanConfig_LessonPlanId",
                table: "LessonPlanConfig",
                column: "LessonPlanId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPlanConfig_LessonPlan_LessonPlanId",
                table: "LessonPlanConfig",
                column: "LessonPlanId",
                principalTable: "LessonPlan",
                principalColumn: "LessonPlanId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonPlanConfig_LessonPlan_LessonPlanId",
                table: "LessonPlanConfig");

            migrationBuilder.DropIndex(
                name: "IX_LessonPlanConfig_LessonPlanId",
                table: "LessonPlanConfig");

            migrationBuilder.DropColumn(
                name: "LessonPlanId",
                table: "LessonPlanConfig");

            migrationBuilder.AddColumn<Guid>(
                name: "LessonPlanConfigsId",
                table: "LessonPlan",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlan_LessonPlanConfigsId",
                table: "LessonPlan",
                column: "LessonPlanConfigsId");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPlan_LessonPlanConfig_LessonPlanConfigsId",
                table: "LessonPlan",
                column: "LessonPlanConfigsId",
                principalTable: "LessonPlanConfig",
                principalColumn: "Id");
        }
    }
}
