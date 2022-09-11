using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentProgress.Core.Migrations
{
    public partial class AddAvatarPathToStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarPath",
                table: "Student",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarPath",
                table: "Student");
        }
    }
}
