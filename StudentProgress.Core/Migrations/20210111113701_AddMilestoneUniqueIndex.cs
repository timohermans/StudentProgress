using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentProgress.Core.Migrations
{
    public partial class AddMilestoneUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Milestone_Artefact_LearningOutcome",
                table: "Milestone",
                columns: new[] { "Artefact", "LearningOutcome" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Milestone_Artefact_LearningOutcome",
                table: "Milestone");
        }
    }
}
