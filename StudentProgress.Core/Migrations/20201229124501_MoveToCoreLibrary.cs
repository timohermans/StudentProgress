using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentProgress.Core.Migrations
{
    public partial class MoveToCoreLibrary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgressUpdate_Student_StudentId",
                table: "ProgressUpdate");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "ProgressUpdate",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroup_Name",
                table: "StudentGroup",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProgressUpdate_Student_StudentId",
                table: "ProgressUpdate",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgressUpdate_Student_StudentId",
                table: "ProgressUpdate");

            migrationBuilder.DropIndex(
                name: "IX_StudentGroup_Name",
                table: "StudentGroup");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "ProgressUpdate",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgressUpdate_Student_StudentId",
                table: "ProgressUpdate",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
