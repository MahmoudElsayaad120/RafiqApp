using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotification0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_PatientId",
                table: "notifications",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_Patients_PatientId",
                table: "notifications",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_Patients_PatientId",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_PatientId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "notifications");
        }
    }
}
