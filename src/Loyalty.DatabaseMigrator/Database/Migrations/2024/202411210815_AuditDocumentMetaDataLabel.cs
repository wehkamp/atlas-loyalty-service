using FluentMigrator;

namespace Loyalty.DatabaseMigrator.Database.Migrations._2024;

[Migration(202411210815)]
public class AuditDocumentMetaDataLabel : UpMigration
{
    public override void Up()
    {
        const string tableName = "audit_document_metadata";
        const string atlasLabelColumnName = "atlas_label";
        const string timestampColumnName = "timestamp";

        // Add the label to the column as well
        Alter.Table(tableName)
            .AddColumn(atlasLabelColumnName)
            .AsString()
            .NotNullable();

        // Add an index on the label
        Create.Index($"{tableName}_{atlasLabelColumnName}_idx").OnTable(tableName).OnColumn(atlasLabelColumnName);
        Create.Index($"{tableName}_{timestampColumnName}_idx").OnTable(tableName).OnColumn(timestampColumnName);
    }
}
