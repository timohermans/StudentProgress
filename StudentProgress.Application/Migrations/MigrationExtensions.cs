using FluentMigrator;
using FluentMigrator.Builders.Create.Table;

namespace StudentProgress.Application.Migrations
{
    internal static class MigrationExtensions
    {
        public static ICreateTableColumnOptionOrWithColumnSyntax WithIdColumn(this ICreateTableWithColumnSyntax tableWithColumnSyntax)
        {
            return tableWithColumnSyntax
                .WithColumn("id")
                .AsInt32()
                .NotNullable()
                .PrimaryKey()
                .Identity();
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax WithTimeStamps(this ICreateTableWithColumnSyntax tableWithColumnSyntax)
        {
            return tableWithColumnSyntax
                .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("ModifiedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);
        }
    }
}