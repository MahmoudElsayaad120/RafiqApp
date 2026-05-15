using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class removeUserTable4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_User_UserId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_User_UserId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_User_UserId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_UserId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Patients",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Doctors",
                newName: "userId");

            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ChatMessages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_User_DoctorId",
                table: "User",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_User_PatientId",
                table: "User",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_User_UserId",
                table: "ChatMessages",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Doctors_DoctorId",
                table: "User",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Patients_PatientId",
                table: "User",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_User_UserId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Doctors_DoctorId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Patients_PatientId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_DoctorId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_PatientId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Patients",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Doctors",
                newName: "UserId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Patients",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Doctors",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ChatMessages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_UserId",
                table: "Patients",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_User_UserId",
                table: "ChatMessages",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_User_UserId",
                table: "Doctors",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_User_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
