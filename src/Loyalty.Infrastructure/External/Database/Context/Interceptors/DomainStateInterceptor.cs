using Loyalty.Application.Services;
using Loyalty.Infrastructure.External.Database.Context.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using static Microsoft.EntityFrameworkCore.EntityState;

namespace Loyalty.Infrastructure.External.Database.Context.Interceptors;
/// <summary>
/// This interceptor is responsible for setting the CreationDateTime and MutationDateTime on entities
/// automatically. It also handles the versioning of entities, if the useManualVersioning flag is set.
///
/// When an entity is marked as an IAggregateRootEntity, the MutationDateTime will always be updated,
/// even if the entity is not modified. This is to ensure that the version (xmin) in the database changes
/// when any child entity is updated (for example: an orderline update causes the order to be updated with a new
/// timestamp, indicating the entity as a whole has changed).
/// </summary>
public class DomainStateInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly bool _useManualVersioning;

    public DomainStateInterceptor(
        IDateTimeProvider dateTimeProvider,
        bool useManualVersioning = false
    )
    {
        _dateTimeProvider = dateTimeProvider;
        _useManualVersioning = useManualVersioning;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            OnBeforeSaving(eventData.Context.ChangeTracker);
        }

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken)
    {
        if (eventData.Context is not null)
        {
            OnBeforeSaving(eventData.Context.ChangeTracker);

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void OnBeforeSaving(ChangeTracker changeTracker)
    {
        var entries = changeTracker.Entries();
        var now = _dateTimeProvider.GetNow();

        // TODO/FIXME: changeTracker.Entries() returns all entities loaded into the context, even if they are not updated.
        //             This means that the MutationDateTime will be updated on aggregate roots, even if they are not modified.
        //             We have previously seen concurrency exceptions with this behavior because catalogue items were marked
        //             as an aggregate root, but very frequently used in many processes, causing these conflicts.
        //             This could be fixed by making sure aggregate roots are updated when any child entity is modified,
        //             but this required bookkeeping to see which child entities belong to which aggregate root.
        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity trackable)
            {
                if (trackable is IAggregateRootEntity)
                {
                    // Always force a change on the aggregate root, so the version (xmin) in the
                    // database changes
                    trackable.MutationDateTime = now;
                    entry.Property("MutationDateTime").IsModified = true;
                }

                switch (entry.State)
                {
                    case Added:
                        {
                            trackable.CreationDateTime = now;
                            trackable.MutationDateTime = now;

                            break;
                        }
                    case Modified:
                        {
                            trackable.MutationDateTime = now;
                            entry.Property("CreationDateTime").IsModified = false;
                            break;
                        }
                }

                if (_useManualVersioning && entry.State is Added or Modified)
                {
                    trackable.Version++;
                }
            }
        }
    }
}
