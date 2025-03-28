using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Loyalty.Domain.UnitTests.Mapper;

public static class TestMappingConfiguration
{
    // Api and Worker tests
    public static void Scan<TProgram>()
        where TProgram : class, new()
    {
        // Add the reference to the application. I choose to reference the Program but 
        // never call it so I have the application in our AppDomain so our Mapping rule 
        // scanner can pick up the rules
        _ = new TProgram();

        Scan();
    }

    // Repository tests
    private static void Scan()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(LogLevel.None);
        });
    }
}