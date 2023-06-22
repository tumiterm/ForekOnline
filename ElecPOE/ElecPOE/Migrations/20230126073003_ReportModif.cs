using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class ReportModif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Module",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "Phase",
                table: "Results");

            migrationBuilder.CreateTable(
                name: "Finance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phase = table.Column<int>(type: "int", nullable: false),
                    File = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finance", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Finance");

            migrationBuilder.AddColumn<int>(
                name: "Module",
                table: "Results",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Phase",
                table: "Results",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
