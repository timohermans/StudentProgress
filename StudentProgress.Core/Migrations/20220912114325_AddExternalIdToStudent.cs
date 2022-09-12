using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentProgress.Core.Migrations
{
    public partial class AddExternalIdToStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Student",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Student");
        }
    }
}
