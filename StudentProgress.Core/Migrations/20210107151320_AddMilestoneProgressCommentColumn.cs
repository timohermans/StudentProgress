using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentProgress.Core.Migrations
{
    public partial class AddMilestoneProgressCommentColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "MilestoneProgress",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "MilestoneProgress");
        }
    }
}
