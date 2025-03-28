using FluentMigrator;
using Npgsql;

namespace Loyalty.DatabaseMigrator.Database;

public abstract class UpMigration : Migration
{
    public object? GetColumnValue(string columnName, string query)
    {
        // Connect to the PostgreSQL database
        var connectionString = ConnectionString.ToString();
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            // Create a command with parameters
            using (var command = new NpgsqlCommand(query, connection))
            {
                // Execute the query
                using (var reader = command.ExecuteReader())
                {
                    // Read the results
                    while (reader.Read())
                    {
                        return reader[columnName]; // Access the column value
                    }
                }
            }
        }

        return null;
    }

    public override void Down()
    { }
}
