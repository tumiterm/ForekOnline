using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class ddCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Module_Placements_PlacementId",
                table: "Module");

            migrationBuilder.DropIndex(
                name: "IX_Module_PlacementId",
                table: "Module");

            migrationBuilder.DropColumn(
                name: "PlacementId",
                table: "Module");

            migrationBuilder.AddColumn<Guid>(
                name: "Module",
                table: "Placements",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Module",
                table: "Placements");

            migrationBuilder.AddColumn<Guid>(
                name: "PlacementId",
                table: "Module",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Module_PlacementId",
                table: "Module",
                column: "PlacementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Module_Placements_PlacementId",
                table: "Module",
                column: "PlacementId",
                principalTable: "Placements",
                principalColumn: "PlacementId");
        }
    }
}
