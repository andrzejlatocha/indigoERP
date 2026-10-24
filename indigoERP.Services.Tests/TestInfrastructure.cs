using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace indigoERP.Services.Tests;

internal sealed class RecordingHandler(Func<HttpRequestMessage, HttpResponseMessage> response) : HttpMessageHandler
{
    public HttpRequestMessage? Request { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Request = request;
        return Task.FromResult(response(request));
    }
}

internal sealed class TestAuthenticationStateProvider(params Claim[] claims) : AuthenticationStateProvider
{
    private readonly AuthenticationState state = new(new ClaimsPrincipal(new ClaimsIdentity(claims, "Test")));

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(state);
}

internal static class TestHttpClientFactory
{
    public static IHttpClientFactory Create(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://test.local/api/") };
        return new StaticHttpClientFactory(client);
    }

    private sealed class StaticHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }
}
