using System.Reflection;
using Cashflowpoly.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cashflowpoly.Api.Infrastructure;

public sealed class StandardResponseOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var httpMethod = context.ApiDescription.HttpMethod?.ToUpperInvariant() ?? "GET";
        var hasRouteId = context.ApiDescription.RelativePath?.Contains('{') == true;

        var allowAnonymous = HasAttribute<AllowAnonymousAttribute>(context.MethodInfo) ||
                             HasAttribute<AllowAnonymousAttribute>(context.MethodInfo.DeclaringType);
        var hasAuthorize = HasAttribute<AuthorizeAttribute>(context.MethodInfo) ||
                           HasAttribute<AuthorizeAttribute>(context.MethodInfo.DeclaringType);
        var requiresAuth = hasAuthorize && !allowAnonymous;

        EnsureSuccessResponse(operation, httpMethod);
        AddErrorResponse(operation, context, "400", "Validasi request gagal.");

        if (requiresAuth)
        {
            AddErrorResponse(operation, context, "401", "Akses membutuhkan autentikasi.");
            AddErrorResponse(operation, context, "403", "Role tidak diizinkan.");
        }

        if (hasRouteId || httpMethod is "PUT" or "PATCH" or "DELETE")
        {
            AddErrorResponse(operation, context, "404", "Resource tidak ditemukan.");
        }

        if (httpMethod is "POST" or "PUT" or "PATCH")
        {
            AddErrorResponse(operation, context, "409", "Terjadi konflik data.");
            AddErrorResponse(operation, context, "422", "Aturan domain tidak terpenuhi.");
        }

        AddErrorResponse(operation, context, "429", "Terlalu banyak request.");
        AddErrorResponse(operation, context, "500", "Terjadi kesalahan pada server.");
    }

    private static void EnsureSuccessResponse(OpenApiOperation operation, string httpMethod)
    {
        operation.Responses ??= new OpenApiResponses();

        if (operation.Responses.Keys.Any(key => key.StartsWith("2", StringComparison.Ordinal)))
        {
            return;
        }

        var successCode = httpMethod switch
        {
            "POST" => "201",
            "DELETE" => "204",
            _ => "200"
        };

        operation.Responses[successCode] = new OpenApiResponse { Description = "Berhasil." };
    }

    private static void AddErrorResponse(
        OpenApiOperation operation,
        OperationFilterContext context,
        string statusCode,
        string description)
    {
        operation.Responses ??= new OpenApiResponses();

        if (operation.Responses.ContainsKey(statusCode))
        {
            return;
        }

        var schema = context.SchemaGenerator.GenerateSchema(typeof(ErrorResponse), context.SchemaRepository);
        operation.Responses[statusCode] = new OpenApiResponse
        {
            Description = description,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new()
                {
                    Schema = schema
                }
            }
        };
    }

    private static bool HasAttribute<T>(MemberInfo? memberInfo) where T : Attribute
    {
        return memberInfo?.GetCustomAttributes(typeof(T), true).Any() == true;
    }
}
