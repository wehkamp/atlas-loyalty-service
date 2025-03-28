using Loyalty.Domain.Core;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Loyalty.Infrastructure.External.Database.Context.Interceptors;

public class LabelInterceptor : DbCommandInterceptor
{
    private readonly IOmsAuditTrailScopedContext _omsAuditTrailScopedContext;

    public LabelInterceptor(IOmsAuditTrailScopedContext omsAuditTrailScopedContext)
    {
        _omsAuditTrailScopedContext = omsAuditTrailScopedContext;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = new CancellationToken())
    {
        if (string.IsNullOrWhiteSpace(_omsAuditTrailScopedContext.AtlasLabel.Value))
        {
            throw new InvalidOperationException("Can't preform a database action without a label specified");
        }

        // TODO FIX: This solution does not work in all cases. For example, a `.FirstOrDefaultAsync()` will result in a query like `WHERE x = y LIMIT 1 AND AtlasLabel = 'label'`
        command.CommandText = AddFilterLabel(command.CommandText, _omsAuditTrailScopedContext.AtlasLabel.Value);

        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    private static string AddFilterLabel(string commandCommandText, string labelIdValue)
    {
        if (!commandCommandText.Contains("WHERE"))
        {
            return commandCommandText + $" WHERE AtlasLabel = '{labelIdValue}'";
        }

        return $"{commandCommandText} AND AtlasLabel = '{labelIdValue}'";
    }
}
