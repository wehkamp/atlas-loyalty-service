namespace Loyalty.Domain.ArticleGroupCode;
public class ArticleGroupCode : AggregateRootEntity
{
    public long Id { get; set; }
    public required string Code { get; set; }
    public decimal? SignatureRequiredThreshold { get; set; }
    public decimal? BmcRequiredThreshold { get; set; }
}
