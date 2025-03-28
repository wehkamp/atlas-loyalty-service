using Loyalty.Application.Commands;
using Loyalty.Application.Events;
using Loyalty.Application.Queries;
using Loyalty.Domain.Extensions;
using System.Xml;
using System.Xml.Linq;

namespace Loyalty.Application;
public static class ApplicationExtensions
{
    // public static bool IsOk(this IHaveApplicationResponse applicationResponse)
    //     => ApplicationResponse.IsOk(applicationResponse.ResponseCode);

    public static string GetCommandName<TCommand>(this TCommand command)
        where TCommand : ICommand
        => command.GetType().Name;

    public static string GetCommandName<TCommand, TResult>(this TCommand command)
        where TCommand : IInMemoryCommand<TResult> => command.GetType().Name;

    public static string GetEventName<TEvent>(this TEvent @event)
        where TEvent : IEvent => @event.GetType().Name;

    public static string GetQueryName<TQuery, TResult>(this TQuery query)
        where TQuery : IQuery<TResult> => query.GetType().Name;

    // public static OrderId? GetQueryOrderId<TQuery, TResult>(this TQuery query)
    //     where TQuery : IQuery<TResult> => (query as IHaveOrderId)?.OrderId;

    public static async Task<T> OrThrowAsync<T>(this Task<T?> getValue, Exception ex)
        where T : struct
    {
        var value = await getValue;
        if (!value.HasValue)
        {
            throw ex;
        }

        return value.Value;
    }

    // public static bool CanExecute<T>(this IFraudPrincipal principal)
    //     => principal.Roles.Any(role => role.CanExecute<T>());

    // public static bool SuccessFullyProcessed(this BusinessTransaction? businessTransaction)
    //     => businessTransaction?.State is BusinessTransactionState.ApplicationResponseOk or BusinessTransactionState.ApplicationResponseOkSent;

    public static XmlDocument ToXmlDocument(this XDocument xDocument)
    {
        var xmlDocument = new XmlDocument
        {
            PreserveWhitespace = true
        };

        using var stream = new MemoryStream();

        xDocument.Save(stream, SaveOptions.DisableFormatting);

        stream.Seek(0, SeekOrigin.Begin);

        xmlDocument.Load(stream);

        return xmlDocument;
    }
}
