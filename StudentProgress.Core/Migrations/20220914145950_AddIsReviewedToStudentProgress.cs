using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentProgress.Core.Migrations
{
    public partial class AddIsReviewedToStudentProgress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "ProgressUpdate",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "ProgressUpdate");
        }
    }
}
