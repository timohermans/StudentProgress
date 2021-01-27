using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentProgress.Core.Migrations
{
    public partial class AddIndexMilestoneUniquePerGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Milestone_Artefact_LearningOutcome",
                table: "Milestone");

            migrationBuilder.DropIndex(
                name: "IX_Milestone_StudentGroupId",
                table: "Milestone");

            migrationBuilder.CreateIndex(
                name: "IX_Milestone_StudentGroupId_Artefact_LearningOutcome",
                table: "Milestone",
                columns: new[] { "StudentGroupId", "Artefact", "LearningOutcome" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Milestone_StudentGroupId_Artefact_LearningOutcome",
                table: "Milestone");

            migrationBuilder.CreateIndex(
                name: "IX_Milestone_Artefact_LearningOutcome",
                table: "Milestone",
                columns: new[] { "Artefact", "LearningOutcome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Milestone_StudentGroupId",
                table: "Milestone",
                column: "StudentGroupId");
        }
    }
}
