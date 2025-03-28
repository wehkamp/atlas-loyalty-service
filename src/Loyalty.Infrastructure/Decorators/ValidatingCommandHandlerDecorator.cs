using FluentValidation;
using Loyalty.Application.Commands;
using Loyalty.Application.Models;

namespace Loyalty.Infrastructure.Decorators;
public class ValidatingCommandHandlerDecorator<TCommand, TResult> : IInMemoryCommandHandler<TCommand, TResult>
    where TCommand : IInMemoryCommand<TResult>
{
    private readonly IInMemoryCommandHandler<TCommand, TResult> _decorated;
    private readonly IEnumerable<IValidator<TCommand>> _validators;

    public ValidatingCommandHandlerDecorator(IInMemoryCommandHandler<TCommand, TResult> decorated, IEnumerable<IValidator<TCommand>> validators)
    {
        _decorated = decorated;
        _validators = validators;
    }

    public async Task<TResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TCommand>(command);

            var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var errors = results
                .SelectMany(r => r.Errors)
                .Where(x => x != null)
                .GroupBy(
                    x => x.PropertyName,
                    x => x.ErrorMessage,
                    (propertyName, errorMessages) => new
                    {
                        Key = propertyName,
                        Values = errorMessages.Distinct().ToArray()
                    })
                .ToDictionary(x => x.Key, x => x.Values);

            if (errors.Any())
            {
                throw new Loyalty.Application.Exceptions.ValidationException(errors);
            }
        }

        return await _decorated.ExecuteAsync(command, cancellationToken);
    }
}

public class ValidatingCommandHandlerDecorator<TCommand> : ValidatingCommandHandlerDecorator<TCommand, Nothing>, IInMemoryCommandHandler<TCommand> where TCommand : IInMemoryCommand
{
    public ValidatingCommandHandlerDecorator(IInMemoryCommandHandler<TCommand, Nothing> decorated, IEnumerable<IValidator<TCommand>> validators) : base(decorated, validators)
    {
    }
}
