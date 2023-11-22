using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentProgress.Web.Migrations
{
    /// <inheritdoc />
    public partial class RenameObjectiveInQuest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objective_Quest_QuestId",
                table: "Objective");

            migrationBuilder.DropForeignKey(
                name: "FK_Quest_QuestLines_QuestLineId",
                table: "Quest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quest",
                table: "Quest");

            migrationBuilder.DropColumn(
                name: "MainObjective",
                table: "QuestLines");

            migrationBuilder.RenameTable(
                name: "Quest",
                newName: "Quests");

            migrationBuilder.RenameColumn(
                name: "ObjectiveMain",
                table: "Quests",
                newName: "MainObjective");

            migrationBuilder.RenameIndex(
                name: "IX_Quest_QuestLineId",
                table: "Quests",
                newName: "IX_Quests_QuestLineId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quests",
                table: "Quests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Objective_Quests_QuestId",
                table: "Objective",
                column: "QuestId",
                principalTable: "Quests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_QuestLines_QuestLineId",
                table: "Quests",
                column: "QuestLineId",
                principalTable: "QuestLines",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objective_Quests_QuestId",
                table: "Objective");

            migrationBuilder.DropForeignKey(
                name: "FK_Quests_QuestLines_QuestLineId",
                table: "Quests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quests",
                table: "Quests");

            migrationBuilder.RenameTable(
                name: "Quests",
                newName: "Quest");

            migrationBuilder.RenameColumn(
                name: "MainObjective",
                table: "Quest",
                newName: "ObjectiveMain");

            migrationBuilder.RenameIndex(
                name: "IX_Quests_QuestLineId",
                table: "Quest",
                newName: "IX_Quest_QuestLineId");

            migrationBuilder.AddColumn<string>(
                name: "MainObjective",
                table: "QuestLines",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quest",
                table: "Quest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Objective_Quest_QuestId",
                table: "Objective",
                column: "QuestId",
                principalTable: "Quest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quest_QuestLines_QuestLineId",
                table: "Quest",
                column: "QuestLineId",
                principalTable: "QuestLines",
                principalColumn: "Id");
        }
    }
}
