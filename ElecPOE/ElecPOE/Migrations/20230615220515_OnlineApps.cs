using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElecPOE.Migrations
{
    /// <inheritdoc />
    public partial class OnlineApps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Selection = table.Column<int>(type: "int", nullable: false),
                    PassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudyPermitCategory = table.Column<int>(type: "int", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicantAddressAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantSurname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantTitle = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    HighestQualification = table.Column<int>(type: "int", nullable: false),
                    IDPassDoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HighestQualDoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResidenceDoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_Applications_Address_ApplicantAddressAddressId",
                        column: x => x.ApplicantAddressAddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medicals",
                columns: table => new
                {
                    MedicalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disability = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicals", x => x.MedicalId);
                });

            migrationBuilder.CreateTable(
                name: "Guardians",
                columns: table => new
                {
                    GuardianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Relationship = table.Column<int>(type: "int", nullable: false),
                    Cellphone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDDoc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guardians", x => x.GuardianId);
                    table.ForeignKey(
                        name: "FK_Guardians_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantAddressAddressId",
                table: "Applications",
                column: "ApplicantAddressAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_ApplicationId",
                table: "Guardians",
                column: "ApplicationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guardians");

            migrationBuilder.DropTable(
                name: "Medicals");

            migrationBuilder.DropTable(
                name: "Applications");
        }
    }
}
