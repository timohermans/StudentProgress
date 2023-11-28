using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentProgress.Web.Migrations
{
    /// <inheritdoc />
    public partial class CreateObjectives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objective_Quests_QuestId",
                table: "Objective");

            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveProgress_Objective_ObjectiveId",
                table: "ObjectiveProgress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Objective",
                table: "Objective");

            migrationBuilder.RenameTable(
                name: "Objective",
                newName: "Objectives");

            migrationBuilder.RenameIndex(
                name: "IX_Objective_QuestId",
                table: "Objectives",
                newName: "IX_Objectives_QuestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Objectives",
                table: "Objectives",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveProgress_Objectives_ObjectiveId",
                table: "ObjectiveProgress",
                column: "ObjectiveId",
                principalTable: "Objectives",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Objectives_Quests_QuestId",
                table: "Objectives",
                column: "QuestId",
                principalTable: "Quests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveProgress_Objectives_ObjectiveId",
                table: "ObjectiveProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_Objectives_Quests_QuestId",
                table: "Objectives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Objectives",
                table: "Objectives");

            migrationBuilder.RenameTable(
                name: "Objectives",
                newName: "Objective");

            migrationBuilder.RenameIndex(
                name: "IX_Objectives_QuestId",
                table: "Objective",
                newName: "IX_Objective_QuestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Objective",
                table: "Objective",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Objective_Quests_QuestId",
                table: "Objective",
                column: "QuestId",
                principalTable: "Quests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveProgress_Objective_ObjectiveId",
                table: "ObjectiveProgress",
                column: "ObjectiveId",
                principalTable: "Objective",
                principalColumn: "Id");
        }
    }
}
