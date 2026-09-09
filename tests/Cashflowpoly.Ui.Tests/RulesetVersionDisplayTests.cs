// Fungsi file: Menguji pemilihan versi terbaru dan histori pada detail set aturan.
using System.Net;
using System.Net.Http.Json;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Controllers;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class RulesetVersionDisplayTests
{
    [Theory]
    [InlineData(null, 2)]
    [InlineData(1, 1)]
    public async Task Details_DisplaysLatestSavedVersion_UnlessHistoryIsSelected(int? requestedVersion, int expectedVersion)
    {
        using var api = new RulesetApi();
        using var services = new ServiceCollection().AddLogging().AddDistributedMemoryCache().AddSession().BuildServiceProvider();
        var context = new DefaultHttpContext
        {
            Session = services.GetRequiredService<ISessionStore>()
                .Create(Guid.NewGuid().ToString(), TimeSpan.FromMinutes(20), TimeSpan.FromSeconds(10), () => true, true)
        };
        var controller = new RulesetsController(api)
        {
            ControllerContext = new ControllerContext { HttpContext = context },
            TempData = new TempDataDictionary(context, api)
        };

        var result = await controller.Details(api.RulesetId, requestedVersion, null, null, TestContext.Current.CancellationToken);

        var model = Assert.IsType<RulesetDetailViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.NotNull(model.Components);
        Assert.Equal(expectedVersion, model.Components.Version);
        Assert.Contains($"/components?version={expectedVersion}", api.Paths);
    }

    private sealed class RulesetApi : HttpMessageHandler, IHttpClientFactory, ITempDataProvider
    {
        public Guid RulesetId { get; } = Guid.NewGuid();
        public List<string> Paths { get; } = [];
        public HttpClient CreateClient(string name) => new(this, disposeHandler: false) { BaseAddress = new Uri("http://localhost/") };
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = request.RequestUri!;
            var isComponents = uri.AbsolutePath.EndsWith("/components", StringComparison.Ordinal);
            Paths.Add(isComponents ? "/components" + uri.Query : uri.AbsolutePath);
            var content = isComponents
                ? JsonContent.Create(new RulesetComponentsResponse(RulesetId, Guid.NewGuid(), uri.Query == "?version=2" ? 2 : 1, "PEMULA"))
                : JsonContent.Create(new RulesetDetailResponse(RulesetId, "Edited ruleset", null, [], Version: 2));
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content });
        }
    }
}
