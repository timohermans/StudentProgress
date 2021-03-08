using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace StudentProgress.Core.Migrations
{
    public partial class AddProgressTagTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgressTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressTag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgressTagProgressUpdate",
                columns: table => new
                {
                    TagsId = table.Column<int>(type: "integer", nullable: false),
                    UpdatesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressTagProgressUpdate", x => new { x.TagsId, x.UpdatesId });
                    table.ForeignKey(
                        name: "FK_ProgressTagProgressUpdate_ProgressTag_TagsId",
                        column: x => x.TagsId,
                        principalTable: "ProgressTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgressTagProgressUpdate_ProgressUpdate_UpdatesId",
                        column: x => x.UpdatesId,
                        principalTable: "ProgressUpdate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgressTagProgressUpdate_UpdatesId",
                table: "ProgressTagProgressUpdate",
                column: "UpdatesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgressTagProgressUpdate");

            migrationBuilder.DropTable(
                name: "ProgressTag");
        }
    }
}
