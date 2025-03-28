using Loyalty.Api.Core.Authentication;
using Loyalty.Application.Repositories;
using Loyalty.Domain.Core;
using Loyalty.Infrastructure.External.Kafka.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Loyalty.Api.Core.Middleware;

public class LabelIdMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly string[] _excludePaths = new[]
    {
        "/health",
        "/metrics",
        "/startz",
        "/readyz",
        "/status",
        "/swagger",
        "/favicon.ico",
    };

    public LabelIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IOmsAuditTrailScopedContext omsAuditTrailScopedContext,
        ILabelRepository labelRepository,
        ILogger<LabelIdMiddleware> logger
    )
    {
        var requestPath = context.Request.Path;

        if (requestPath.Equals("/", StringComparison.CurrentCultureIgnoreCase))
        {
            // Do nothing 
        }
        else if (!_excludePaths.Any(path => requestPath.StartsWithSegments(path)))
        {
            var headers = AtlasHeaders.FromHeaders(context.Request.Headers);
            if (headers.LabelId == null)
            {
                logger.LogError("Request received without `{atlasLabel}` or the old `Blaze-AtlasLabel` header  on {Url}", AtlasHeaders.AtlasLabel, context.Request.Path);

                throw new NoLabelHeaderSetException("Blaze-AtlasLabel", AtlasHeaders.AtlasLabel);
            }

            var label = labelRepository.GetLabel(headers.LabelId);
            if (label == null)
            {
                throw new LabelNotFoundException(headers.LabelId);
            }

            omsAuditTrailScopedContext.SetLabel(label.Value);
        }

        await _next(context);
    }
}
