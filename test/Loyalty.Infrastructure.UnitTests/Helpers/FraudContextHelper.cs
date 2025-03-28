using Loyalty.Application.Services;
using Loyalty.Infrastructure.External.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace Loyalty.Infrastructure.UnitTests.Helpers;

public static class FraudContextHelper
{
    public record FraudContextMock
    {
        public required Mock<IDbContextFactory<OmsCoreAuditTrailReaderDbContext>> FraudDbReaderContextMock { get; init; }
        public required Mock<IDbContextFactory<OmsCoreAuditTrailWriterDbContext>> FraudDbWriterContextMock { get; init; }
        public required Mock<IDateTimeProvider> DateTimeProviderMock { get; init; }
    }

    public static FraudContextMock Create(DateTime? now = null)
        => CreateAsync(now).GetAwaiter().GetResult();

    public static async Task<FraudContextMock> CreateAsync(DateTime? now = null)
    {
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(m => m.GetNow()).Returns(() => now ?? DateTime.Now);

        string databaseName = $"FraudInMemory-{Guid.NewGuid()}";

        DbContextOptions<OmsCoreAuditTrailDbContext> CreateFraudDbContextOptions()
        {
            return new DbContextOptionsBuilder<OmsCoreAuditTrailDbContext>()
                .UseInMemoryDatabase(databaseName)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
        }

        var fraudDbContextOptions = CreateFraudDbContextOptions();

        var fraudDbReaderContextFactory = new Mock<IDbContextFactory<OmsCoreAuditTrailReaderDbContext>>();
        fraudDbReaderContextFactory
            .Setup(factory => factory.CreateDbContextAsync(new CancellationToken()))
            .ReturnsAsync(() => new OmsCoreAuditTrailReaderDbContext(fraudDbContextOptions));
        fraudDbReaderContextFactory
            .Setup(factory => factory.CreateDbContext())
            .Returns(() => new OmsCoreAuditTrailReaderDbContext(fraudDbContextOptions));

        var fraudDbWriterContextFactory = new Mock<IDbContextFactory<OmsCoreAuditTrailWriterDbContext>>();
        fraudDbWriterContextFactory
            .Setup(factory => factory.CreateDbContextAsync(new CancellationToken()))
            .ReturnsAsync(() => new OmsCoreAuditTrailWriterDbContext(fraudDbContextOptions));
        fraudDbWriterContextFactory
            .Setup(factory => factory.CreateDbContext())
            .Returns(() => new OmsCoreAuditTrailWriterDbContext(fraudDbContextOptions));

        var fraudContextMock = new FraudContextMock
        {
            FraudDbReaderContextMock = fraudDbReaderContextFactory,
            FraudDbWriterContextMock = fraudDbWriterContextFactory,
            DateTimeProviderMock = dateTimeProviderMock,
        };

        return await Task.FromResult(fraudContextMock);
    }
}