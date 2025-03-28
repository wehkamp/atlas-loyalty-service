namespace Loyalty.DatabaseMigrator.Database;

public abstract class UpSqlMigration : UpMigration
{
    protected abstract string Sql { get; }

    public override void Up()
    {
        Execute.Sql(Sql);
    }
}
