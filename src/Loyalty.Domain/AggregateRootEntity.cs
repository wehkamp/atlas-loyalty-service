namespace Loyalty.Domain;
public abstract class AggregateRootEntity : DomainEntity
{
    /// <summary>
    /// Internal field which holds the version of the order state. This is used to make sure we only persist the order when the version in the database matches.
    /// </summary>
    public uint Version { get; set; }
}
