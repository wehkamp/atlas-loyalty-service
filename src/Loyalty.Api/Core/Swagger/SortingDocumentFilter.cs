using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Loyalty.Api.Core.Swagger;

public class AddDefaultResponse : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
    }
}

public class AddLabelHeader : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Atlas-Label",
            In = ParameterLocation.Header,
            Description = "The company wide unique label identifier.",
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString(string.Empty)
            }
        });
    }
}
