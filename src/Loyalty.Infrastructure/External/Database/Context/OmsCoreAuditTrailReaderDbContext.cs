using Loyalty.Infrastructure.External.Database.Context.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace Loyalty.Infrastructure.External.Database.Context;
public class OmsCoreAuditTrailReaderDbContext : OmsCoreAuditTrailDbContext
{
    public OmsCoreAuditTrailReaderDbContext(DbContextOptions<OmsCoreAuditTrailDbContext> options) : base(options)
    {
    }

    [ActivatorUtilitiesConstructor]
    public OmsCoreAuditTrailReaderDbContext(DbContextOptions<OmsCoreAuditTrailReaderDbContext> options) : base(options)
    {
    }

    public override int SaveChanges()
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override void AddRange(params object[] entities)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override void AddRange(IEnumerable<object> entities)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override void AttachRange(params object[] entities)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override void AttachRange(IEnumerable<object> entities)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override void UpdateRange(params object[] entities)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override void UpdateRange(IEnumerable<object> entities)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override void RemoveRange(params object[] entities)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }

    public override void RemoveRange(IEnumerable<object> entities)
    {
        throw new DbActionsAreOnlyAllowedOnWriterContextException();
    }
}
