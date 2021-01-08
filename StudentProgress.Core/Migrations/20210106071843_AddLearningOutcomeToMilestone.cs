using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentProgress.Core.Migrations
{
    public partial class AddLearningOutcomeToMilestone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Milestone",
                newName: "Artefact");

            migrationBuilder.AddColumn<string>(
                name: "LearningOutcome",
                table: "Milestone",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Artefact",
                table: "Milestone");

            migrationBuilder.RenameColumn(
                name: "LearningOutcome",
                table: "Milestone",
                newName: "Name");
        }
    }
}
