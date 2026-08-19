// Fungsi file: Memastikan readiness UI mengikuti status readiness API.
using System.Net;
using Cashflowpoly.Ui.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class ApiHealthCheckTests
{
    [Theory]
    [InlineData(HttpStatusCode.OK, HealthStatus.Healthy)]
    [InlineData(HttpStatusCode.ServiceUnavailable, HealthStatus.Unhealthy)]
    public async Task CheckHealthAsync_ReflectsApiStatus(HttpStatusCode apiStatus, HealthStatus expected)
    {
        var check = new ApiHealthCheck(new StubClientFactory(apiStatus));

        var result = await check.CheckHealthAsync(new HealthCheckContext(), TestContext.Current.CancellationToken);

        Assert.Equal(expected, result.Status);
    }

    private sealed class StubClientFactory(HttpStatusCode statusCode) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(new StubHandler(statusCode))
        {
            BaseAddress = new Uri("http://api.test/")
        };
    }

    private sealed class StubHandler(HttpStatusCode statusCode) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(statusCode));
    }
}
