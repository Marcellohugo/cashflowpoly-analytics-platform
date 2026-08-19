// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui AuthControllerResilienceTests.
using System.Text;
using Cashflowpoly.Ui.Controllers;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class AuthControllerResilienceTests
{
    [Fact]
    public async Task Login_WhenApiIsUnavailable_ShouldReturnLoginViewWithError()
    {
        var controller = CreateController();

        var result = await controller.Login(new LoginViewModel
        {
            Username = "instructor",
            Password = "password123"
        });

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<LoginViewModel>(view.Model);
        Assert.Equal("Login", view.ViewName);
        Assert.Equal(string.Empty, model.Password);
        Assert.False(string.IsNullOrWhiteSpace(model.ErrorMessage));
    }

    [Fact]
    public async Task Register_WhenApiIsUnavailable_ShouldReturnRegisterViewWithError()
    {
        var controller = CreateController();

        var result = await controller.Register(new RegisterViewModel
        {
            DisplayName = "Instructor",
            Username = "instructor",
            Password = "password123",
            ConfirmPassword = "password123"
        });

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<RegisterViewModel>(view.Model);
        Assert.True(string.IsNullOrEmpty(view.ViewName));
        Assert.Equal(string.Empty, model.Password);
        Assert.Equal(string.Empty, model.ConfirmPassword);
        Assert.False(string.IsNullOrWhiteSpace(model.ErrorMessage));
    }

    private static AuthController CreateController()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Features.Set<ISessionFeature>(new TestSessionFeature(new TestSession()));

        return new AuthController(new ThrowingHttpClientFactory())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    private sealed class ThrowingHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient(new ThrowingHandler())
            {
                BaseAddress = new Uri("http://localhost:5041")
            };
        }
    }

    private sealed class ThrowingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new HttpRequestException("API unavailable");
        }
    }

    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _values = new();

        public IEnumerable<string> Keys => _values.Keys;
        public string Id { get; } = Guid.NewGuid().ToString("N");
        public bool IsAvailable => true;

        public void Clear() => _values.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _values.Remove(key);
        public void Set(string key, byte[] value) => _values[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _values.TryGetValue(key, out value!);

        public void SetString(string key, string value)
        {
            Set(key, Encoding.UTF8.GetBytes(value));
        }
    }

    private sealed class TestSessionFeature : ISessionFeature
    {
        public TestSessionFeature(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; set; }
    }
}
