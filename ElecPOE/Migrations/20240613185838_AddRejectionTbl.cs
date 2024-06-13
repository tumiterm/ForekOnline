﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectionTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rejections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    NextSteps = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FollowUpDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rejections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rejections_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rejections_ApplicationId",
                table: "Rejections",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rejections");
        }
    }
}
