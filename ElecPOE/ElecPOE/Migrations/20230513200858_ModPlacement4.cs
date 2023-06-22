using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class ModPlacement4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
