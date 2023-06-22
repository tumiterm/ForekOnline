using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class ModPlacement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionDate",
                table: "Placements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Placements",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletionDate",
                table: "Placements");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Placements");
        }
    }
}
