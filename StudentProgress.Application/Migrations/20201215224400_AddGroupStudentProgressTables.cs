using FluentMigrator;

namespace StudentProgress.Application.Migrations
{
    [Migration(202012152245)]
    public class AddGroupStudentProgressTables : Migration
    {
        public override void Up()
        {
            Create.Sequence("hibernate_sequence");

            Create.Table("Group")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString().NotNullable().Unique()
                .WithColumn("Mnemonic").AsString().Nullable()
                .WithTimeStamps();

            Create.Table("Student")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString().NotNullable().Unique();

            Create.Table("ProgressUpdate")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Date").AsDateTime()
                .WithColumn("Feedup").AsString().Nullable()
                .WithColumn("Feedback").AsString().Nullable()
                .WithColumn("Feedforward").AsString().Nullable()
                .WithColumn("Progress_feeling").AsInt16()
                .WithColumn("StudentId").AsInt32().NotNullable().ForeignKey("Student", "Id")
                .WithColumn("GroupId").AsInt32().NotNullable().ForeignKey("Group", "Id")
                .WithTimeStamps();

            Create.Table("StudentStudentGroup")
                .WithColumn("GroupId").AsInt32().NotNullable().ForeignKey("Group", "Id")
                .WithColumn("StudentId").AsInt32().NotNullable().ForeignKey("Student", "Id");

            Create.PrimaryKey().OnTable("StudentStudentGroup").Columns("GroupId", "StudentId");
        }

        public override void Down()
        {
        }
    }
}