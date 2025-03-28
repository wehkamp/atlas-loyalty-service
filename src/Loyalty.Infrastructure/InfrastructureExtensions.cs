using Loyalty.Domain.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace Loyalty.Infrastructure;
public static class InfrastructureExtensions
{
    public static void UseLabel(this IServiceScope serviceScope, AtlasLabel atlasLabel, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
    {
        var omsAuditTrailScopedContext = serviceScope.ServiceProvider.GetRequiredService<IOmsAuditTrailScopedContext>();
        omsAuditTrailScopedContext.SetLabel(
            atlasLabel: atlasLabel,
            memberName: memberName,
            fileName: fileName,
            lineNumber: lineNumber
        );
    }
}
