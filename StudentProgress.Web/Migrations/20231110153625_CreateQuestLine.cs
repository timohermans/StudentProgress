using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentProgress.Web.Migrations;

/// <inheritdoc />
public partial class CreateQuestLine : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Quest_QuestLine_QuestLineId",
            table: "Quest");

        migrationBuilder.DropForeignKey(
            name: "FK_QuestLine_Adventures_AdventureId",
            table: "QuestLine");

        migrationBuilder.DropPrimaryKey(
            name: "PK_QuestLine",
            table: "QuestLine");

        migrationBuilder.RenameTable(
            name: "QuestLine",
            newName: "QuestLines");

        migrationBuilder.RenameIndex(
            name: "IX_QuestLine_AdventureId",
            table: "QuestLines",
            newName: "IX_QuestLines_AdventureId");

        migrationBuilder.AlterColumn<int>(
            name: "AdventureId",
            table: "QuestLines",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "INTEGER",
            oldNullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "PK_QuestLines",
            table: "QuestLines",
            column: "Id");

        migrationBuilder.CreateIndex(
            name: "IX_QuestLines_Name",
            table: "QuestLines",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_QuestLines_Order",
            table: "QuestLines",
            column: "Order",
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "FK_Quest_QuestLines_QuestLineId",
            table: "Quest",
            column: "QuestLineId",
            principalTable: "QuestLines",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_QuestLines_Adventures_AdventureId",
            table: "QuestLines",
            column: "AdventureId",
            principalTable: "Adventures",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Quest_QuestLines_QuestLineId",
            table: "Quest");

        migrationBuilder.DropForeignKey(
            name: "FK_QuestLines_Adventures_AdventureId",
            table: "QuestLines");

        migrationBuilder.DropPrimaryKey(
            name: "PK_QuestLines",
            table: "QuestLines");

        migrationBuilder.DropIndex(
            name: "IX_QuestLines_Name",
            table: "QuestLines");

        migrationBuilder.DropIndex(
            name: "IX_QuestLines_Order",
            table: "QuestLines");

        migrationBuilder.RenameTable(
            name: "QuestLines",
            newName: "QuestLine");

        migrationBuilder.RenameIndex(
            name: "IX_QuestLines_AdventureId",
            table: "QuestLine",
            newName: "IX_QuestLine_AdventureId");

        migrationBuilder.AlterColumn<int>(
            name: "AdventureId",
            table: "QuestLine",
            type: "INTEGER",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AddPrimaryKey(
            name: "PK_QuestLine",
            table: "QuestLine",
            column: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Quest_QuestLine_QuestLineId",
            table: "Quest",
            column: "QuestLineId",
            principalTable: "QuestLine",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_QuestLine_Adventures_AdventureId",
            table: "QuestLine",
            column: "AdventureId",
            principalTable: "Adventures",
            principalColumn: "Id");
    }
}
