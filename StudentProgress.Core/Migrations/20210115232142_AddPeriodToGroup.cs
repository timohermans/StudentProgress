using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentProgress.Core.Migrations
{
    public partial class AddPeriodToGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Period",
                table: "StudentGroup",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Period",
                table: "StudentGroup");
        }
    }
}
