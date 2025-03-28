using FluentMigrator.Runner;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;

namespace Loyalty.DatabaseMigrator.Database;

public interface IDatabaseMigrator
{
    Task MigrateAsync();
}

public class DatabaseMigrator : IDatabaseMigrator
{
    private readonly IDatabase _database;
    private readonly IMigrationRunner _migrationRunner;
    private readonly ILogger<DatabaseMigrator> _logger;

    public DatabaseMigrator(
        IDatabase database,
        IMigrationRunner migrationRunner,
        ILogger<DatabaseMigrator> logger
    )
    {
        _database = database;
        _migrationRunner = migrationRunner;
        _logger = logger;
    }

    public async Task MigrateAsync()
    {
        try
        {
            _logger.LogInformation("Starting database migrator");

            await _database.EnsureExistsAsync();

            // List all migrations we need to have run
            _migrationRunner.ListMigrations();

            var retry = Policy
                .Handle<NpgsqlException>()
                .WaitAndRetry(
                    retryCount: 5,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2,4,8,16,32 sc
                    onRetry: (exception, retryCount, context) => _logger.LogError($"Retry migration: {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.")
                );

            retry.Execute(() => _migrationRunner.MigrateUp());

            _logger.LogInformation("Migrations applied successfully");
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the postresql database");

            throw;
        }
    }
}
