using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class CorrPla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkplaceModules");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "Placements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Placements",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Placements");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Placements");

            migrationBuilder.CreateTable(
                name: "WorkplaceModules",
                columns: table => new
                {
                    LearnerWorkplaceModulesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Days = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlacementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Progress = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkplaceModules", x => x.LearnerWorkplaceModulesId);
                });
        }
    }
}
