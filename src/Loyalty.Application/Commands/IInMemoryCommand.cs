using Loyalty.Application.Models;

namespace Loyalty.Application.Commands;
/// <summary>
/// Modify the data while waiting for the result.
/// </summary>
/// <typeparam name="TResult">Result the command must provide</typeparam>
public interface IInMemoryCommand<TResult>
{ }

public interface IInMemoryCommand : IInMemoryCommand<Nothing>
{ }
