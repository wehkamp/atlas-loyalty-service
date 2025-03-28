using FluentMigrator;

namespace Loyalty.DatabaseMigrator.Database.Migrations._2024;

[Migration(202411150825)]
public class InitMetaDataTable : UpMigration
{
    public override void Up()
    {
        const string tableName = "audit_document_metadata";
        const string orderNumberColumnName = "order_number";
        const string userNameColumnName = "username";

        Create.Table(tableName)
            .WithColumn("document_id").AsGuid().PrimaryKey()
            .WithColumn("type").AsString().NotNullable()
            .WithColumn("name").AsString().NotNullable()
            .WithColumn("timestamp").AsDateTime().NotNullable()
            .WithColumn(userNameColumnName).AsString().Nullable()
            .WithColumn("correlation_id").AsGuid().NotNullable()
            .WithColumn(orderNumberColumnName).AsString().Nullable();
        
        Create.Index($"{tableName}_{orderNumberColumnName}_idx").OnTable(tableName).OnColumn(orderNumberColumnName);
        Create.Index($"{tableName}_{userNameColumnName}_idx").OnTable(tableName).OnColumn(userNameColumnName);
    }
}
