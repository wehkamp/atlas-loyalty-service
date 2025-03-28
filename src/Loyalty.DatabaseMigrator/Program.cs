using FluentMigrator.Runner;
using Loyalty.DatabaseMigrator.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Loyalty.DatabaseMigrator;

public static class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureServices(ConfigureCustomServices);

        var app = builder.Build();

        await UpdateDatabase(app.Services);
    }

    private static async Task UpdateDatabase(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var databaseMigrator = scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>();
        await databaseMigrator.MigrateAsync();
    }

    private static void ConfigureCustomServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
    {
        // A subset of services is configured as compared to the main Loyalty.Api, just what is required to run the migrations
        services.AddScoped<IDatabase, Database.Database>();
        services.AddScoped<IDatabaseMigrator, Database.DatabaseMigrator>();

        var connectionString = hostBuilderContext.Configuration.GetConnectionString(IDatabase.ConnectionStringPostgresSection);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ApplicationException($"Expected to find the Init Container it's own connection string what's not configured at: ConnectionStrings:{connectionString}");
        }

        // Migration
        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(UpMigration).Assembly)
                .For.Migrations()
                .For.EmbeddedResources()
            )
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(true);
    }
}
