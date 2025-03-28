using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;

namespace Loyalty.Api.Core;

public class LocalDateTimeModelBinderProvider : IModelBinderProvider
{
    internal const DateTimeStyles SupportedStyles = DateTimeStyles.AllowWhiteSpaces;

    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var modelType = context.Metadata.UnderlyingOrModelType;
        if (modelType == typeof(DateTime))
        {
            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            return new DateTimeModelBinder(SupportedStyles, loggerFactory);
        }

        return null;
    }
}
