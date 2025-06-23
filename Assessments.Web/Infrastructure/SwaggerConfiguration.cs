using Microsoft.OpenApi.Models;
using RodlisteNaturtyper.Data.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Assessments.Web.Infrastructure;

public static class SwaggerConfiguration
{
    private const string Title = "Assessments API";
    private const string SecurityName = "ApiKey";

    public static void AddSwagger(this IServiceCollection services, IWebHostEnvironment builderEnvironment)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(SecurityName, new OpenApiSecurityScheme
            {
                Name = ApiKeyRequiredAttribute.ApiKeyHeader,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "ApiKeyScheme"
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
            options.SchemaFilter<SwaggerIgnoreFilter>();

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = GetTitle(builderEnvironment)
            });
        });
    }

    public static void ConfigureSwagger(this IApplicationBuilder app, IWebHostEnvironment builderEnvironment)
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = GetTitle(builderEnvironment);
            options.DefaultModelsExpandDepth(-1);
        });
    }

    private static string GetTitle(IWebHostEnvironment builderEnvironment)
    {
        var title = Title;

        if (!builderEnvironment.IsProduction())
            title = $"{title} - {builderEnvironment.EnvironmentName}";

        return title;
    }

    private class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiKeyIsRequired = context.MethodInfo.DeclaringType != null &&
                (context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<ApiKeyRequiredAttribute>().Any() ||
                 context.MethodInfo.GetCustomAttributes(true).OfType<ApiKeyRequiredAttribute>().Any());

            if (!apiKeyIsRequired)
                return;

            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });

            operation.Security =
            [
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = SecurityName,
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            ];
        }
    }

    private class SwaggerIgnoreFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.Name != nameof(Assessment))
                return;

            string[] removeProperties = ["state", "createdOn", "isLocked", "lockedBy", "lockedById", "modifiedBy", "modifiedById"];

            foreach (var property in removeProperties)
                schema.Properties.Remove(property);

            foreach (var schemaProperty in schema.Properties)
                schemaProperty.Value.Description = string.Empty;
        }
    }
}