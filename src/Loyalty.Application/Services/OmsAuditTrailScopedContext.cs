using Loyalty.Domain.Core;
using Microsoft.Extensions.Logging;

namespace Loyalty.Application.Services;

public class OmsAuditTrailScopedContext : IOmsAuditTrailScopedContext
{
    private readonly ILogger<OmsAuditTrailScopedContext> _logger;
    public Guid ScopeId { get; }

    public OmsAuditTrailScopedContext(ILogger<OmsAuditTrailScopedContext> logger)
    {
        ScopeId = Guid.NewGuid();
        _logger = logger;
        _logger.LogDebug("{omsAuditTrailScopedContext}-{id} | Creating", nameof(OmsAuditTrailScopedContext), ScopeId);
    }

    private Guid _correlationId = Guid.Empty;
    public Guid CorrelationId
    {
        get => _correlationId;
        set
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("Correlation id cannot be empty", nameof(value));
            }

            _correlationId = value;
        }
    }

    private AtlasLabel? _label;
    public AtlasLabel AtlasLabel => _label!.Value;

    public void SetLabel(AtlasLabel label, string memberName = "", string fileName = "", int lineNumber = 0)
    {
        _logger.LogDebug("{omsAuditTrailScopedContext}-{id} | Setting | labelId: {labelId} from {fileName}({lineNumber}):{memberName}", nameof(OmsAuditTrailScopedContext), ScopeId, label, fileName, lineNumber, memberName);
        _label = label;
    }

    ~OmsAuditTrailScopedContext()
    {
        _logger.LogDebug("{omsAuditTrailScopedContext}-{id} | Destorying", nameof(OmsAuditTrailScopedContext), ScopeId);
    }
}
