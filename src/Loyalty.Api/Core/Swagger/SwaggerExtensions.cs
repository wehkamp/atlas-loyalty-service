using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Loyalty.Api.Core.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1", new OpenApiInfo
                {
                    Title = "Loyalty Service API",
                    Version = "v1",
                    Description = "Loyalty Service API contains all audit trailing information of orders and users.",
                    Contact = new OpenApiContact { Name = "Juliette & Robin", Url = new Uri("https://wehkampretailgroup.atlassian.net/wiki/spaces/TOM/overview?homepageId=42401795") }
                });
            options.CustomSchemaIds(type => type.ToString());
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            //options.TagActionsBy(t => new[] { SortingDocumentFilter.GetTag("/" + t.RelativePath!) });
            //options.DocumentFilter<SortingDocumentFilter>();
            options.OperationFilter<AddLabelHeader>();
            options.OperationFilter<AddDefaultResponse>();
            //options.CustomOperationIds(e =>
            //{
            //    return $"{e.ActionDescriptor.RouteValues["action"]}";
            //});
            options.SupportNonNullableReferenceTypes();

            //Client ID and Secret
            options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                Name = "Authentication",
                Description = "Basic Authentication",
                Scheme = "basic"
            });

            //Google Auth
            options.AddSecurityDefinition("oauth2",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                            Scopes = new Dictionary<string, string> { { "openid", "openid" }, { "email", "email" } }
                        }
                    },
                    Extensions = new Dictionary<string, IOpenApiExtension> { { "x-tokenName", new OpenApiString("id_token") } }
                });

            //Optional Customer scoping
            options.AddSecurityDefinition("blaze", new OpenApiSecurityScheme { Type = SecuritySchemeType.ApiKey, In = ParameterLocation.Header, Name = "Blaze-Auth-Shopper", Description = "Blaze headers" });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "basic" }, Scheme = "basic" }, Array.Empty<string>() },
                {
                    new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }, Scheme = "oauth2" },
                    new List<string> { "openid", "email" }
                },
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "blaze" } }, Array.Empty<string>() }
            });
        });
        return services;
    }

    public static IApplicationBuilder ConfigureSwaggerUI(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", "Loyalty Service API V1");
            c.EnableTryItOutByDefault();
            c.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { { "nonce", "anyNonceStringHere" } });
            c.OAuthClientId(app.Configuration.GetSection("Authentication:Google:ClientId").Value);
            c.InjectJavascript("/swagger/swagger.js");
        });

        return app;
    }
}
