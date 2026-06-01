using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShopManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditReviewEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_ArchivedAppointments_AppointmentId",
                table: "Reviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_ArchivedAppointments_AppointmentId",
                table: "Reviews",
                column: "AppointmentId",
                principalTable: "ArchivedAppointments",
                principalColumn: "Id");
        }
    }
}
