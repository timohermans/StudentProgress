using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace StudentProgress.Core.Migrations
{
    public partial class CreateMilestoneProgress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Milestone_StudentGroup_StudentGroupId",
                table: "Milestone");

            migrationBuilder.AlterColumn<int>(
                name: "StudentGroupId",
                table: "Milestone",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "MilestoneProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MilestoneId = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    ProgressUpdateId = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MilestoneProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MilestoneProgress_Milestone_MilestoneId",
                        column: x => x.MilestoneId,
                        principalTable: "Milestone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MilestoneProgress_ProgressUpdate_ProgressUpdateId",
                        column: x => x.ProgressUpdateId,
                        principalTable: "ProgressUpdate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneProgress_MilestoneId",
                table: "MilestoneProgress",
                column: "MilestoneId");

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneProgress_ProgressUpdateId",
                table: "MilestoneProgress",
                column: "ProgressUpdateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Milestone_StudentGroup_StudentGroupId",
                table: "Milestone",
                column: "StudentGroupId",
                principalTable: "StudentGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Milestone_StudentGroup_StudentGroupId",
                table: "Milestone");

            migrationBuilder.DropTable(
                name: "MilestoneProgress");

            migrationBuilder.AlterColumn<int>(
                name: "StudentGroupId",
                table: "Milestone",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Milestone_StudentGroup_StudentGroupId",
                table: "Milestone",
                column: "StudentGroupId",
                principalTable: "StudentGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
