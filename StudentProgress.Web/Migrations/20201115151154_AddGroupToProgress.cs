using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentProgress.Web.Migrations
{
    public partial class AddGroupToProgress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "StudentGroup",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "ProgressUpdate",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgressUpdate_GroupId",
                table: "ProgressUpdate",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgressUpdate_StudentGroup_GroupId",
                table: "ProgressUpdate",
                column: "GroupId",
                principalTable: "StudentGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgressUpdate_StudentGroup_GroupId",
                table: "ProgressUpdate");

            migrationBuilder.DropIndex(
                name: "IX_ProgressUpdate_GroupId",
                table: "ProgressUpdate");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ProgressUpdate");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "StudentGroup",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
