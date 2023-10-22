using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentProgress.Web.Migrations
{
    /// <inheritdoc />
    public partial class PersonSplitName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "People",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "People",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Initials",
                table: "People",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "People");

            migrationBuilder.DropColumn(
                name: "Initials",
                table: "People");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "People",
                newName: "Name");
        }
    }
}
