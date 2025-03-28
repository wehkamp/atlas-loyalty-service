namespace Loyalty.Application.Queries;

public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken);
}
