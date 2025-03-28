using Loyalty.Application.Exceptions;
using Loyalty.Domain.Core.Extensions;
using Loyalty.Infrastructure.External.Database.Context.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Loyalty.Infrastructure.External.Database.Context.Mappings;

namespace Loyalty.Infrastructure.External.Database.Context;

public class OmsCoreAuditTrailDbContext : DbContext
{
    public OmsCoreAuditTrailDbContext(DbContextOptions<OmsCoreAuditTrailDbContext> options) : base(options) { }
    public OmsCoreAuditTrailDbContext(DbContextOptions<OmsCoreAuditTrailReaderDbContext> options) : base(options) { }
    public OmsCoreAuditTrailDbContext(DbContextOptions<OmsCoreAuditTrailWriterDbContext> options) : base(options) { }

    public DbSet<AuditDocumentMetadata> AuditDocumentMetadatas { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplyMappings(modelBuilder);

        UseUtcToLocalDatesConverters(modelBuilder.Model);
    }

    private static void ApplyMappings(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AuditDocumentMetadataMapping());
    }

    private static void UseUtcToLocalDatesConverters(IMutableModel model)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v.ToLocalTime() : DateTime.SpecifyKind(v, DateTimeKind.Local), v => v);

        model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetProperties())
            .Where(property => property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
            .ForEach(property => property.SetValueConverter(dateTimeConverter));
    }

    public async Task<TResult> WithChangeTrackingTransactionAsync<TResult>(Func<Task<TResult>> action, CancellationToken cancellationToken)
    {
        var strategy = Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database.BeginTransactionAsync(cancellationToken);

            try
            {
                ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

                var result = await action();

                ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                throw new ConcurrencyException(ex);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
        });
    }
}
