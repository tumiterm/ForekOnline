using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class ReportModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phase",
                table: "Finance");

            migrationBuilder.AddColumn<string>(
                name: "StatementName",
                table: "Finance",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatementName",
                table: "Finance");

            migrationBuilder.AddColumn<int>(
                name: "Phase",
                table: "Finance",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
