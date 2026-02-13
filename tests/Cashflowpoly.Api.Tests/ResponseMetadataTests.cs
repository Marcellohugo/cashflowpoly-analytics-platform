using System.Reflection;
using Cashflowpoly.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class ResponseMetadataTests
{
    [Fact]
    public void Every_Api_Action_Defines_Success_Response_Metadata()
    {
        var missing = GetActionResponseMetadata()
            .Where(item => item.StatusCodes.All(code => code < 200 || code >= 300))
            .Select(item => item.ActionDisplayName)
            .ToList();

        Assert.True(
            missing.Count == 0,
            $"Action tanpa response sukses metadata: {string.Join(", ", missing)}");
    }

    [Fact]
    public void Every_Api_Action_Defines_Error_Response_Metadata()
    {
        var missing = GetActionResponseMetadata()
            .Where(item => item.StatusCodes.All(code => code < 400))
            .Select(item => item.ActionDisplayName)
            .ToList();

        Assert.True(
            missing.Count == 0,
            $"Action tanpa response error metadata: {string.Join(", ", missing)}");
    }

    [Fact]
    public void Every_Api_Action_Exposes_More_Than_200_Status()
    {
        var missing = GetActionResponseMetadata()
            .Where(item => item.StatusCodes.Count == 1 && item.StatusCodes.Contains(StatusCodes.Status200OK))
            .Select(item => item.ActionDisplayName)
            .ToList();

        Assert.True(
            missing.Count == 0,
            $"Action masih hanya 200: {string.Join(", ", missing)}");
    }

    private static IReadOnlyList<ActionMetadata> GetActionResponseMetadata()
    {
        var assembly = typeof(AuthController).Assembly;
        var controllers = assembly.GetTypes()
            .Where(type =>
                typeof(ControllerBase).IsAssignableFrom(type) &&
                type.IsClass &&
                !type.IsAbstract &&
                string.Equals(type.Namespace, "Cashflowpoly.Api.Controllers", StringComparison.Ordinal))
            .OrderBy(type => type.Name);

        var result = new List<ActionMetadata>();

        foreach (var controller in controllers)
        {
            var controllerCodes = controller
                .GetCustomAttributes<ProducesResponseTypeAttribute>(true)
                .Select(attr => attr.StatusCode);

            var actions = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(method => method.GetCustomAttributes<HttpMethodAttribute>(true).Any())
                .OrderBy(method => method.Name);

            foreach (var action in actions)
            {
                var actionCodes = action
                    .GetCustomAttributes<ProducesResponseTypeAttribute>(true)
                    .Select(attr => attr.StatusCode);

                var allCodes = controllerCodes
                    .Concat(actionCodes)
                    .Distinct()
                    .OrderBy(code => code)
                    .ToList();

                result.Add(new ActionMetadata(
                    $"{controller.Name}.{action.Name}",
                    allCodes));
            }
        }

        return result;
    }

    private sealed record ActionMetadata(string ActionDisplayName, IReadOnlyList<int> StatusCodes);
}
