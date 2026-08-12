// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui BearerTokenHandlerTests.
using System.Net;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class BearerTokenHandlerTests
{
    [Fact]
    public async Task SendAsync_ForwardsIdentityLanguageClientIpAndTraceId()
    {
        var session = new TestSession();
        session.SetString(AuthConstants.SessionAccessTokenKey, "token-123");
        session.SetString(AuthConstants.SessionLanguageKey, AuthConstants.LanguageEn);
        var context = new DefaultHttpContext
        {
            TraceIdentifier = "trace-123"
        };
        context.Connection.RemoteIpAddress = IPAddress.Parse("203.0.113.8");
        context.Features.Set<ISessionFeature>(new TestSessionFeature(session));

        var terminalHandler = new CaptureHandler();
        using var handler = new BearerTokenHandler(new HttpContextAccessor { HttpContext = context })
        {
            InnerHandler = terminalHandler
        };
        using var invoker = new HttpMessageInvoker(handler);

        await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://api.test/api/v1/sessions"), CancellationToken.None);

        Assert.NotNull(terminalHandler.Request);
        Assert.Equal("Bearer", terminalHandler.Request.Headers.Authorization?.Scheme);
        Assert.Equal("token-123", terminalHandler.Request.Headers.Authorization?.Parameter);
        Assert.Equal("en", Assert.Single(terminalHandler.Request.Headers.AcceptLanguage).Value);
        Assert.Equal("203.0.113.8", Assert.Single(terminalHandler.Request.Headers.GetValues("X-Forwarded-For")));
        Assert.Equal("trace-123", Assert.Single(terminalHandler.Request.Headers.GetValues("X-Client-Request-Id")));
    }

    private sealed class CaptureHandler : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    private sealed class TestSessionFeature(ISession session) : ISessionFeature
    {
        public ISession Session { get; set; } = session;
    }

    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _values = new(StringComparer.Ordinal);

        public bool IsAvailable => true;
        public string Id => "test-session";
        public IEnumerable<string> Keys => _values.Keys;

        public void Clear() => _values.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _values.Remove(key);

        public void Set(string key, byte[] value) => _values[key] = value;

        public bool TryGetValue(string key, out byte[] value) => _values.TryGetValue(key, out value!);
    }
}
