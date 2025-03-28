namespace Loyalty.Infrastructure.External.Database.Context.Entities;

public abstract class BaseEntity
{
    public uint Version { get; set; }

    public DateTime CreationDateTime { get; set; }

    public DateTime MutationDateTime { get; set; }
}

public abstract class BaseStateEntity : BaseEntity
{
    public string State { get; set; } = default!;
}

/// <summary>
/// Marks the entity as an Aggregate Root.
///
/// Marking entities as an aggregate root will force the version (xmin) in the database to change, as well
/// as updating the MutationDateTime any time the entity (or any of its children) is saved. You can view the
/// DomainStateInterceptor for more information.
/// </summary>
public interface IAggregateRootEntity
{
    public uint Version { get; }
}
