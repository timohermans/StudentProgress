using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentProgress.Core.Migrations
{
    public partial class RemoveUpAndForwardFromStudentGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
                {
                    migrationBuilder.Sql(@"
UPDATE ""ProgressUpdate""
SET ""Feedback"" = concat_ws(E'\n', ""Feedup"", ""Feedback"", ""Feedforward"")
");
                }

            migrationBuilder.DropColumn(
                name: "Feedforward",
                table: "ProgressUpdate");

            migrationBuilder.DropColumn(
                name: "Feedup",
                table: "ProgressUpdate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Feedforward",
                table: "ProgressUpdate",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feedup",
                table: "ProgressUpdate",
                type: "text",
                nullable: true);
        }
    }
}
