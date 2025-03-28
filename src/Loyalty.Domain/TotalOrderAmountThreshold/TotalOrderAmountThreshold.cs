namespace Loyalty.Domain.TotalOrderAmountThreshold;
public class TotalOrderAmountThreshold : AggregateRootEntity
{
    public long Id { get; set; }
    public decimal? SignatureRequiredThreshold { get; set; }
    public decimal? BmcRequiredThreshold { get; set; }
}
