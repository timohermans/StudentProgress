using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentProgress.Web.Migrations
{
    public partial class AddMnemonic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mnemonic",
                table: "StudentGroup",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mnemonic",
                table: "StudentGroup");
        }
    }
}
