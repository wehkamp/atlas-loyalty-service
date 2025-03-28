using Loyalty.Application.Exceptions;
using Loyalty.Domain.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using ApplicationException = Loyalty.Application.Exceptions.ApplicationException;

namespace Loyalty.KafkaProcessors.Core;

/// <summary>
/// Information about an error that occured
/// </summary>
public record ErrorResponse(string Title, int Status, string Detail, IReadOnlyDictionary<string, string[]>? Errors)
{
    /// <summary>
    /// Summarized description of what went wrong
    /// </summary>
    public string Title { get; init; } = Title;

    /// <summary>
    /// More detailed description of what went wrong
    /// </summary>
    public string Detail { get; init; } = Detail;

    /// <summary>
    /// HTTP status code that was returned
    /// </summary>
    public int Status { get; } = Status;

    /// <summary>
    /// Errors that occured
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; } = Errors;
}

public static class ExceptionHandlingExtensions
{
    public static void UseExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerPathFeature>();

                if (feature is not null)
                {
                    var exception = feature.Error;

                    var error = new ErrorResponse(GetTitle(exception), GetStatusCode(exception), exception.Message, GetErrors(exception));

                    if (exception is UnauthorizedException)
                    {
                        logger.LogWarning(exception, exception.Message);
                    }
                    else
                    {
                        logger.LogError(exception, exception.Message);
                    }

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = error.Status;

                    await context.Response.WriteAsJsonAsync(error);
                }
            });
        });
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            ConflictException => StatusCodes.Status409Conflict,
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status422UnprocessableEntity,
            AuthenticationException => StatusCodes.Status401Unauthorized,
            UnauthorizedException => StatusCodes.Status403Forbidden,
            UnauthorizedAccessException => StatusCodes.Status405MethodNotAllowed,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetTitle(Exception exception) =>
        exception switch
        {
            ApplicationException applicationException => applicationException.Title,
            _ => "Server Error"
        };

    private static IReadOnlyDictionary<string, string[]>? GetErrors(Exception exception)
        => exception is ValidationException validationException ? validationException.ErrorsDictionary : null;
}
