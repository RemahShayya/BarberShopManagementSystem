using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShopManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class SomeUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewEmailSent",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ReviewToken",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "ArchivedAppointments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BarberId",
                table: "ArchivedAppointments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ArchivedAppointments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "ReviewEmailSent",
                table: "ArchivedAppointments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewId",
                table: "ArchivedAppointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewToken",
                table: "ArchivedAppointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedAppointments_BarberId",
                table: "ArchivedAppointments",
                column: "BarberId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedAppointments_CustomerId",
                table: "ArchivedAppointments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedAppointments_ReviewId",
                table: "ArchivedAppointments",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedAppointments_ServiceId",
                table: "ArchivedAppointments",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArchivedAppointments_AspNetUsers_BarberId",
                table: "ArchivedAppointments",
                column: "BarberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArchivedAppointments_AspNetUsers_CustomerId",
                table: "ArchivedAppointments",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArchivedAppointments_Reviews_ReviewId",
                table: "ArchivedAppointments",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArchivedAppointments_Services_ServiceId",
                table: "ArchivedAppointments",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArchivedAppointments_AspNetUsers_BarberId",
                table: "ArchivedAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_ArchivedAppointments_AspNetUsers_CustomerId",
                table: "ArchivedAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_ArchivedAppointments_Reviews_ReviewId",
                table: "ArchivedAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_ArchivedAppointments_Services_ServiceId",
                table: "ArchivedAppointments");

            migrationBuilder.DropIndex(
                name: "IX_ArchivedAppointments_BarberId",
                table: "ArchivedAppointments");

            migrationBuilder.DropIndex(
                name: "IX_ArchivedAppointments_CustomerId",
                table: "ArchivedAppointments");

            migrationBuilder.DropIndex(
                name: "IX_ArchivedAppointments_ReviewId",
                table: "ArchivedAppointments");

            migrationBuilder.DropIndex(
                name: "IX_ArchivedAppointments_ServiceId",
                table: "ArchivedAppointments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ArchivedAppointments");

            migrationBuilder.DropColumn(
                name: "ReviewEmailSent",
                table: "ArchivedAppointments");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "ArchivedAppointments");

            migrationBuilder.DropColumn(
                name: "ReviewToken",
                table: "ArchivedAppointments");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "ArchivedAppointments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "BarberId",
                table: "ArchivedAppointments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<bool>(
                name: "ReviewEmailSent",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReviewToken",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
