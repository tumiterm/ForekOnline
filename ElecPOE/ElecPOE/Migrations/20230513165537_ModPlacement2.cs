using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class ModPlacement2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompletionDate",
                table: "Placements",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Placements",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Placements");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Placements",
                newName: "CompletionDate");
        }
    }
}
