using System.Net;
using System.Text;
using indigoERP.Worker.Services;
using Microsoft.Extensions.Configuration;

namespace indigoERP.Worker.Tests;

public class MicrosoftGraphMailServiceTests
{
    [Fact]
    public async Task SendAsync_CreatesDraftAndSendsIt()
    {
        var handler = new RecordingHandler();
        var factory = new TestHttpClientFactory(handler);
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["MicrosoftGraph:TenantId"] = "tenant",
            ["MicrosoftGraph:ClientId"] = "client",
            ["MicrosoftGraph:ClientSecret"] = "secret"
        }).Build();
        var service = new MicrosoftGraphMailService(factory, configuration);

        await service.SendAsync(
            new GraphMailMessage("nadawca@hcz.com.pl", ["odbiorca@hcz.com.pl"], "Temat", "<p>Treść</p>", []),
            CancellationToken.None);

        Assert.Equal(3, handler.Requests.Count);
        Assert.Contains(handler.Requests, item => item.EndsWith("/oauth2/v2.0/token", StringComparison.Ordinal));
        Assert.Contains(handler.Requests, item => item.EndsWith("/messages", StringComparison.Ordinal));
        Assert.Contains(handler.Requests, item => item.EndsWith("/messages/draft-1/send", StringComparison.Ordinal));
    }

    [Fact]
    public async Task SendAsync_RejectsInvalidSender()
    {
        var service = new MicrosoftGraphMailService(
            new TestHttpClientFactory(new RecordingHandler()),
            new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MicrosoftGraph:TenantId"] = "tenant",
                ["MicrosoftGraph:ClientId"] = "client",
                ["MicrosoftGraph:ClientSecret"] = "secret"
            }).Build());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SendAsync(
            new GraphMailMessage("nie adres", ["odbiorca@hcz.com.pl"], "Temat", "Treść", []),
            CancellationToken.None));
    }

    private sealed class TestHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;

        public TestHttpClientFactory(HttpMessageHandler handler)
        {
            _handler = handler;
        }

        public HttpClient CreateClient(string name) => new(_handler, disposeHandler: false);
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public List<string> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request.RequestUri!.AbsoluteUri);

            var content = request.RequestUri.AbsolutePath.EndsWith("/token", StringComparison.Ordinal)
                ? "{\"access_token\":\"token\",\"expires_in\":3600}"
                : request.RequestUri.AbsolutePath.EndsWith("/messages", StringComparison.Ordinal)
                    ? "{\"id\":\"draft-1\"}"
                    : string.Empty;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
        }
    }
}
