using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class AlterLP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAppoved",
                table: "LessonPlan");

            migrationBuilder.AlterColumn<string>(
                name: "IsApprovedBy",
                table: "LessonPlan",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Approval",
                table: "LessonPlan",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approval",
                table: "LessonPlan");

            migrationBuilder.AlterColumn<Guid>(
                name: "IsApprovedBy",
                table: "LessonPlan",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAppoved",
                table: "LessonPlan",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
