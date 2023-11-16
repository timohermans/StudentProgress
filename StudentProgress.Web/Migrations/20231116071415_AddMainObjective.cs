using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentProgress.Web.Migrations;

/// <inheritdoc />
public partial class AddMainObjective : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "MainObjective",
            table: "QuestLines",
            type: "TEXT",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "MainObjective",
            table: "QuestLines");
    }
}
