namespace Loyalty.Application.Queries;
/// <summary>
/// Retrieve the data, need to be real time and hence need some creativity to somewhat scale.
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IQuery<TResult> : IQuery { }

public interface IQuery { }
