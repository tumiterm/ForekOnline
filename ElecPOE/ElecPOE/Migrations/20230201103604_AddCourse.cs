using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class AddCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.CourseId);
                });

            migrationBuilder.CreateTable(
                name: "Module",
                columns: table => new
                {
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseIdFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NQFLevel = table.Column<int>(type: "int", nullable: false),
                    Credit = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.ModuleId);
                    table.ForeignKey(
                        name: "FK_Module_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "CourseId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Module_CourseId",
                table: "Module",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Module");

            migrationBuilder.DropTable(
                name: "Course");
        }
    }
}
