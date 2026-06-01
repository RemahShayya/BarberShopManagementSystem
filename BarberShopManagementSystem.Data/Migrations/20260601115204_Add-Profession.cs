using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShopManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProfession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProfessionId",
                table: "Services",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Professions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProfessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeProfessions_AspNetUsers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeProfessions_Professions_ProfessionId",
                        column: x => x.ProfessionId,
                        principalTable: "Professions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_ProfessionId",
                table: "Services",
                column: "ProfessionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfessions_EmployeeId",
                table: "EmployeeProfessions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfessions_ProfessionId",
                table: "EmployeeProfessions",
                column: "ProfessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Professions_ProfessionId",
                table: "Services",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Professions_ProfessionId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "EmployeeProfessions");

            migrationBuilder.DropTable(
                name: "Professions");

            migrationBuilder.DropIndex(
                name: "IX_Services_ProfessionId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "Services");
        }
    }
}
