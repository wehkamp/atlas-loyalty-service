using System.Runtime.CompilerServices;

namespace Loyalty.Domain.Core;

public interface IOmsAuditTrailScopedContext
{
    /// <summary>
    /// The ID of the scope. This is used to identify the scope.
    /// </summary>
    public Guid ScopeId { get; }

    /// <summary>
    /// Set the label configuration for the scope.
    /// </summary>
    /// <param name="atlasLabel"><see cref="AtlasLabel"/> of the scope</param>
    /// <param name="memberName">The caller of where in the scope this is set.</param>
    /// <param name="fileName">The caller of where in the scope this is set.</param>
    /// <param name="lineNumber">The caller of where in the scope this is set.</param>
    void SetLabel(AtlasLabel atlasLabel, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0);

    AtlasLabel AtlasLabel { get; }

    /// <summary>
    /// The current CorrelationId of the scope. This is used to identify the request in the logs and always has one starting point.
    /// </summary>
    Guid CorrelationId { get; set; }
}
