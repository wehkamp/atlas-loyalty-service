namespace Loyalty.DatabaseMigrator.Database;

public abstract class UpAsyncMigration : UpMigration
{
    public override void Up()
    {
        // Asynchronous migration logic
        Task.Run(UpAsync).Wait();
    }

    protected abstract Task UpAsync();
}
