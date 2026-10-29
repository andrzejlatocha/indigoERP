using System.Net;
using System.Text;
using indigoERP.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace indigoERP.Api.Tests;

public sealed class EpicaHrServiceTokenProviderTests : IDisposable
{
    private const string Zmienna = "ServiceClient_indigoERP";
    private readonly string? _poprzedniaWartosc = Environment.GetEnvironmentVariable(Zmienna);

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(Zmienna, _poprzedniaWartosc);
    }

    [Fact]
    public async Task Requests_token_for_indigoERP_to_epicaHR_with_the_service_key_and_caches_it()
    {
        Environment.SetEnvironmentVariable(Zmienna, "sekret-testowy");
        var handler = new FakeGatewayHandler(
            """{"token":"jwt-1","expiresAtUtc":"2099-01-01T00:00:00Z"}""");
        var provider = Utworz(handler, gateway: "https://gateway.test/");

        var pierwszy = await provider.GetTokenAsync(CancellationToken.None);
        var drugi = await provider.GetTokenAsync(CancellationToken.None);

        Assert.Equal("jwt-1", pierwszy);
        Assert.Equal("jwt-1", drugi);
        var zadanie = Assert.Single(handler.Zadania);
        Assert.Equal("https://gateway.test/Service/GetToken?clientApp=indigoERP&targetApp=epicaHR", zadanie.RequestUri?.ToString());
        Assert.Equal("sekret-testowy", zadanie.Headers.GetValues("X-Service-Key").Single());
    }

    [Fact]
    public async Task Returns_null_without_calling_gateway_when_the_service_key_is_missing()
    {
        Environment.SetEnvironmentVariable(Zmienna, null);
        var handler = new FakeGatewayHandler("{}");
        var provider = Utworz(handler, gateway: "https://gateway.test/");

        var token = await provider.GetTokenAsync(CancellationToken.None);

        Assert.Null(token);
        Assert.Empty(handler.Zadania);
    }

    [Fact]
    public async Task Returns_null_when_gateway_rejects_the_key()
    {
        Environment.SetEnvironmentVariable(Zmienna, "zly-sekret");
        var handler = new FakeGatewayHandler("{}", HttpStatusCode.Unauthorized);
        var provider = Utworz(handler, gateway: "https://gateway.test/");

        var token = await provider.GetTokenAsync(CancellationToken.None);

        Assert.Null(token);
    }

    [Fact]
    public async Task Refuses_gateway_url_that_is_not_https()
    {
        Environment.SetEnvironmentVariable(Zmienna, "sekret-testowy");
        var handler = new FakeGatewayHandler("{}");
        var provider = Utworz(handler, gateway: "http://gateway.test/");

        var token = await provider.GetTokenAsync(CancellationToken.None);

        Assert.Null(token);
        Assert.Empty(handler.Zadania);
    }

    private static EpicaHrServiceTokenProvider Utworz(FakeGatewayHandler handler, string gateway)
    {
        var konfiguracja = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["ServiceAuth:GatewayUrl"] = gateway })
            .Build();
        return new EpicaHrServiceTokenProvider(new FakeFactory(handler), konfiguracja,
            NullLogger<EpicaHrServiceTokenProvider>.Instance);
    }

    private sealed class FakeGatewayHandler(string cialo, HttpStatusCode status = HttpStatusCode.OK) : HttpMessageHandler
    {
        public List<HttpRequestMessage> Zadania { get; } = new();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Zadania.Add(request);
            return Task.FromResult(new HttpResponseMessage(status)
            {
                Content = new StringContent(cialo, Encoding.UTF8, "application/json")
            });
        }
    }

    private sealed class FakeFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler, disposeHandler: false);
    }
}
