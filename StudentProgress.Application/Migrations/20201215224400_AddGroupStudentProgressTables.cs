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
                .WithColumn("id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("name").AsString().NotNullable().Unique()
                .WithColumn("mnemonic").AsString().Nullable()
                .WithColumn("created_date").AsDateTime().NotNullable() // TODO: fix dit
                .WithColumn("updated_date").AsDateTime().NotNullable()
                ;

            Create.Table("Student")
                .WithColumn("id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("name").AsString().NotNullable().Unique();

            Create.Table("ProgressUpdate")
                .WithColumn("id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("date").AsDateTime()
                .WithColumn("feedup").AsString()
                .WithColumn("feedback").AsString()
                .WithColumn("feedforward").AsString()
                .WithColumn("progress_feeling").AsInt16()
                .WithColumn("created_date").AsDate().NotNullable()
                .WithColumn("updated_date").AsDate().NotNullable()
                .WithColumn("student_id").AsInt32().NotNullable().ForeignKey("Student", "id")
                .WithColumn("group_id").AsInt32().NotNullable().ForeignKey("Group", "id");

            Create.Table("StudentStudentGroup")
                .WithColumn("group_id").AsInt32().NotNullable().ForeignKey("Group", "id")
                .WithColumn("student_id").AsInt32().NotNullable().ForeignKey("Student", "id")
                
                ;

            Create.PrimaryKey().OnTable("StudentStudentGroup").Columns("group_id", "student_id");
        }

        public override void Down()
        {
        }
    }
}