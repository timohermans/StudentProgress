using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentProgress.Core.Migrations
{
    public partial class GroupUniqueOnPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentGroup_Name",
                table: "StudentGroup");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroup_Name_Period",
                table: "StudentGroup",
                columns: new[] { "Name", "Period" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Student_Name",
                table: "Student",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentGroup_Name_Period",
                table: "StudentGroup");

            migrationBuilder.DropIndex(
                name: "IX_Student_Name",
                table: "Student");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroup_Name",
                table: "StudentGroup",
                column: "Name",
                unique: true);
        }
    }
}
