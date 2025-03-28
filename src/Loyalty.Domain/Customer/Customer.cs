namespace Loyalty.Domain.Customer;
public class Customer : AggregateRootEntity
{
    public long Id { get; set; }
    public required string CustomerId { get; set; }
    public required string Label { get; set; }
}
