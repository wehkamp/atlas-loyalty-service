using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Loyalty.DatabaseMigrator.Database;

public interface IDatabase
{
    const string ConnectionStringPostgresSection = "Postgres";

    Task<NpgsqlConnection> CreateConnectionAsync();

    Task EnsureExistsAsync();

    string ConnectionString { get; }
}

public class Database : IDatabase
{
    private readonly string _connectionString;
    private readonly ILogger<Database> _logger;
    private readonly NpgsqlConnectionStringBuilder _connectionStringBuilder;

    public string ConnectionString => _connectionString;

    public Database(IConfiguration configuration, ILogger<Database> logger)
    {
        var connectionString = configuration.GetConnectionString(IDatabase.ConnectionStringPostgresSection);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ApplicationException($"ConnectionStrings is missing a valid connection string with key '{IDatabase.ConnectionStringPostgresSection}'");
        }

        _connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        if (string.IsNullOrWhiteSpace(_connectionStringBuilder.Database))
        {
            throw new ApplicationException($"Connection string is missing a valid database name in the connection string '{IDatabase.ConnectionStringPostgresSection}'");
        }

        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task EnsureExistsAsync()
    {
        // Clone the connection string builder and change the database to 'postgres' or any other default database
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
            Database = "postgres" // Use a default database to connect to the instance
        };

        using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync();

        // Use a parameterized query to safely check if the target database exists
        var command = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = @dbName", connection);
        command.Parameters.AddWithValue("dbName", _connectionStringBuilder.Database ?? throw new InvalidOperationException("The connection string is missing a valid database name."));

        var found = (int?)await command.ExecuteScalarAsync();

        if (found is null)
        {
            _logger.LogInformation("Database {database} doesn't exist", _connectionStringBuilder.Database);

            // Ensure database name is valid before creating it
            var databaseName = _connectionStringBuilder.Database;
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new InvalidOperationException("The connection string is missing a valid database name.");
            }

            // Create the target database if it doesn't exist, using a parameterized command for safety
            var createCommand = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", connection);
            await createCommand.ExecuteNonQueryAsync();

            _logger.LogInformation("Database {database} created", databaseName);
        }
    }

    public async Task<NpgsqlConnection> CreateConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionStringBuilder.ConnectionString);

        await connection.OpenAsync();

        return connection;
    }
}
