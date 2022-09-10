using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentProgress.Core.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgressTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressTag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Mnemonic = table.Column<string>(type: "TEXT", nullable: true),
                    Period = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValue: new DateTime(1, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Milestone",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LearningOutcome = table.Column<string>(type: "TEXT", nullable: false),
                    Artefact = table.Column<string>(type: "TEXT", nullable: false),
                    StudentGroupId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Milestone_StudentGroup_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgressUpdate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Feedback = table.Column<string>(type: "TEXT", nullable: true),
                    ProgressFeeling = table.Column<int>(type: "INTEGER", nullable: false),
                    GroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressUpdate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressUpdate_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgressUpdate_StudentGroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "StudentGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentStudentGroup",
                columns: table => new
                {
                    StudentGroupsId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentStudentGroup", x => new { x.StudentGroupsId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_StudentStudentGroup_Student_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentStudentGroup_StudentGroup_StudentGroupsId",
                        column: x => x.StudentGroupsId,
                        principalTable: "StudentGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MilestoneProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MilestoneId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    ProgressUpdateId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProgressTagProgressUpdate",
                columns: table => new
                {
                    TagsId = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatesId = table.Column<int>(type: "INTEGER", nullable: false)
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
                name: "IX_Milestone_StudentGroupId_Artefact_LearningOutcome",
                table: "Milestone",
                columns: new[] { "StudentGroupId", "Artefact", "LearningOutcome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneProgress_MilestoneId",
                table: "MilestoneProgress",
                column: "MilestoneId");

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneProgress_ProgressUpdateId",
                table: "MilestoneProgress",
                column: "ProgressUpdateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressTagProgressUpdate_UpdatesId",
                table: "ProgressTagProgressUpdate",
                column: "UpdatesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressUpdate_GroupId",
                table: "ProgressUpdate",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressUpdate_StudentId",
                table: "ProgressUpdate",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Key",
                table: "Settings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Student_Name",
                table: "Student",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroup_Name_Period",
                table: "StudentGroup",
                columns: new[] { "Name", "Period" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentStudentGroup_StudentsId",
                table: "StudentStudentGroup",
                column: "StudentsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MilestoneProgress");

            migrationBuilder.DropTable(
                name: "ProgressTagProgressUpdate");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "StudentStudentGroup");

            migrationBuilder.DropTable(
                name: "Milestone");

            migrationBuilder.DropTable(
                name: "ProgressTag");

            migrationBuilder.DropTable(
                name: "ProgressUpdate");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "StudentGroup");
        }
    }
}
