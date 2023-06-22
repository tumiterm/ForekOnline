using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class Thir : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Module",
                table: "StudentAttachments");

            migrationBuilder.CreateTable(
                name: "AssessmentAttachments",
                columns: table => new
                {
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentName = table.Column<int>(type: "int", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Module = table.Column<int>(type: "int", nullable: true),
                    Percentage = table.Column<double>(type: "float", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentAttachments", x => x.AttachmentId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentAttachments");

            migrationBuilder.AddColumn<int>(
                name: "Module",
                table: "StudentAttachments",
                type: "int",
                nullable: true);
        }
    }
}
