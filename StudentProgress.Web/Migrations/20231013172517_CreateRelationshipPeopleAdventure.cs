using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentProgress.Web.Migrations;

/// <inheritdoc />
public partial class CreateRelationshipPeopleAdventure : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_People_Adventures_AdventureId",
            table: "People");

        migrationBuilder.DropIndex(
            name: "IX_People_AdventureId",
            table: "People");

        migrationBuilder.DropColumn(
            name: "AdventureId",
            table: "People");

        migrationBuilder.CreateTable(
            name: "AdventurePerson",
            columns: table => new
            {
                AdventuresId = table.Column<int>(type: "INTEGER", nullable: false),
                PeopleId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AdventurePerson", x => new { x.AdventuresId, x.PeopleId });
                table.ForeignKey(
                    name: "FK_AdventurePerson_Adventures_AdventuresId",
                    column: x => x.AdventuresId,
                    principalTable: "Adventures",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AdventurePerson_People_PeopleId",
                    column: x => x.PeopleId,
                    principalTable: "People",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AdventurePerson_PeopleId",
            table: "AdventurePerson",
            column: "PeopleId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AdventurePerson");

        migrationBuilder.AddColumn<int>(
            name: "AdventureId",
            table: "People",
            type: "INTEGER",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_People_AdventureId",
            table: "People",
            column: "AdventureId");

        migrationBuilder.AddForeignKey(
            name: "FK_People_Adventures_AdventureId",
            table: "People",
            column: "AdventureId",
            principalTable: "Adventures",
            principalColumn: "Id");
    }
}
