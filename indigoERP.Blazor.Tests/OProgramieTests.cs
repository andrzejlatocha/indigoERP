using System.Net;
using System.Text;
using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using indigoERP.Blazor.OProgramie;
using Xunit;

namespace indigoERP.Blazor.Tests;

public class OProgramieTests
{
    private static readonly OProgramieNowosc[] PrzykladoweNowosci =
    {
        new() { nowID = 1, nowData = new DateTime(2026, 8, 10), nowWersja = "1.0", nowTytul = "Najstarsza", nowOpis = "<p>Opis A</p>" },
        new()
        {
            nowID = 3, nowData = new DateTime(2026, 8, 11, 18, 30, 0), nowWersja = "1.2", nowTytul = "Najnowsza", tnoOpis = "Zmiana",
            nowOpis = "<p><b>Pogrubione</b></p><script>alert(1)</script><img src=x onerror=alert(2)><a href=\"javascript:alert(3)\">link</a>"
        },
        new() { nowID = 2, nowData = new DateTime(2026, 8, 11, 9, 0, 0), nowWersja = "1.1", nowTytul = "Srodkowa", nowOpis = "<p>Opis B</p>" }
    };

    [Fact]
    public void Sanitize_RemovesScriptsEventHandlersAndJavascriptUrls()
    {
        var result = OProgramieHtml.Sanitize(
            "<p>ok</p><script>alert(1)</script><img src=x onerror=alert(2)><a href=\"javascript:alert(3)\">link</a><iframe src=\"x\"></iframe>");

        Assert.Contains("<p>ok</p>", result);
        Assert.DoesNotContain("<script", result, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("onerror", result, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("javascript:", result, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<iframe", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Sanitize_KeepsFormattingAndHandlesEmpty()
    {
        Assert.Equal("<h3>Tytuł</h3><ul><li>a</li></ul>", OProgramieHtml.Sanitize("<h3>Tytuł</h3><ul><li>a</li></ul>"));
        Assert.Equal(string.Empty, OProgramieHtml.Sanitize(null));
        Assert.Equal(string.Empty, OProgramieHtml.Sanitize("   "));
    }

    [Fact]
    public async Task Client_ReturnsNewestFirstAndCallsAppEndpoint()
    {
        var handler = new StubHandler(HttpStatusCode.OK, System.Text.Json.JsonSerializer.Serialize(PrzykladoweNowosci));
        var client = CreateClient(handler);

        var result = await client.GetAsync(240);

        Assert.Equal(new[] { 3, 2, 1 }, result.Select(n => n.nowID));
        Assert.EndsWith("Nowosc/SelectByAplikacja/240?nowStatus=2", handler.LastRequestUri?.ToString());
    }

    [Fact]
    public async Task Client_ThrowsHttpRequestException_WhenApiFails()
    {
        var client = CreateClient(new StubHandler(HttpStatusCode.InternalServerError, "{}"));

        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync(240));
    }

    [Fact]
    public void AddOProgramie_UsesConfiguredIndigoApiUrlInDevelopment()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["appSettings:IndigoApiUrl"] = "https://indigo.test/Api" })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOProgramie(configuration, new FakeEnvironment(Environments.Development));

        using var provider = services.BuildServiceProvider();
        var http = provider.GetRequiredService<IHttpClientFactory>().CreateClient(OProgramieNowosciClient.ClientName);

        Assert.Equal(new Uri("https://indigo.test/Api/"), http.BaseAddress);
    }

    [Fact]
    public void Nowosci_AreRenderedInGivenOrderWithTitleLinks()
    {
        using var context = new BunitContext();
        Register(context, new FakeNowosci(PrzykladoweNowosci.OrderByDescending(n => n.nowData).ThenByDescending(n => n.nowID).ToArray()));

        var cut = context.Render<OProgramieNowosci>();

        var rows = cut.FindAll("tbody tr");
        Assert.Equal(3, rows.Count);
        Assert.Contains("Najnowsza", rows[0].TextContent);
        Assert.Contains("Najstarsza", rows[2].TextContent);
        Assert.Equal(3, cut.FindAll("button.op-link").Count);
    }

    [Fact]
    public void Title_Click_OpensDialogWithSanitizedHtml_AndCloseHidesIt()
    {
        using var context = new BunitContext();
        Register(context, new FakeNowosci(new[] { PrzykladoweNowosci[1] }));
        var cut = context.Render<OProgramieNowosci>();

        cut.Find("button.op-link").Click();

        var content = cut.Find(".op-modal-tresc").InnerHtml;
        Assert.Contains("<b>Pogrubione</b>", content);
        Assert.DoesNotContain("<script", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("onerror", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Najnowsza", cut.Find("#op-modal-tytul").TextContent);

        cut.Find("button.op-btn").Click();

        Assert.Empty(cut.FindAll(".op-modal"));
    }

    [Fact]
    public void Nowosci_ShowsEmptyMessage_WhenNoNews()
    {
        using var context = new BunitContext();
        Register(context, new FakeNowosci(Array.Empty<OProgramieNowosc>()));

        var cut = context.Render<OProgramieNowosci>();

        Assert.Contains("Brak opublikowanych nowości", cut.Find(".op-empty").TextContent);
    }

    [Fact]
    public void Nowosci_ShowsWarning_WhenApiUnavailable()
    {
        using var context = new BunitContext();
        Register(context, new FakeNowosci(null, new HttpRequestException("brak połączenia")));

        var cut = context.Render<OProgramieNowosci>();

        Assert.Contains("Nie udało się pobrać nowości", cut.Find(".op-warn").TextContent);
    }

    [Fact]
    public void Nowosci_ShowsInfo_WhenAppIdMissing()
    {
        using var context = new BunitContext();
        Register(context, new FakeNowosci(PrzykladoweNowosci), appId: null);

        var cut = context.Render<OProgramieNowosci>();

        Assert.Contains("brak numeru aplikacji", cut.Find(".op-warn").TextContent);
    }

    [Fact]
    public void Strona_ShowsNameVersionAndAppId()
    {
        using var context = new BunitContext();
        Register(context, new FakeNowosci(Array.Empty<OProgramieNowosc>()), appId: "240", version: "3.26.1103.10000");

        var cut = context.Render<OProgramieStrona>(parameters => parameters
            .Add(p => p.Nazwa, "testApp")
            .Add(p => p.Haslo, "Hasło testowe")
            .Add(p => p.Opis, "Opis testowy"));

        Assert.Contains("testApp", cut.Markup);
        Assert.Contains("Hasło testowe", cut.Find("h1").TextContent);
        Assert.Contains("3.26.1103.10000", cut.Find(".op-facts").TextContent);
        Assert.Contains("240", cut.Find(".op-facts").TextContent);
        Assert.Equal(3, cut.FindAll(".op-feature").Count);
    }

    private static void Register(BunitContext context, IOProgramieNowosci source, string? appId = "240", string version = "1.0.0")
    {
        var settings = new Dictionary<string, string?> { ["appSettings:Version"] = version };

        if (appId is not null)
        {
            settings["appSettings:AppID"] = appId;
        }

        context.Services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(settings).Build());
        context.Services.AddSingleton<IHostEnvironment>(new FakeEnvironment(Environments.Development));
        context.Services.AddSingleton(source);
    }

    private static OProgramieNowosciClient CreateClient(HttpMessageHandler handler)
    {
        var services = new ServiceCollection();
        services.AddHttpClient(OProgramieNowosciClient.ClientName, http => http.BaseAddress = new Uri("https://indigo.test/Api/"))
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var factory = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();

        return new OProgramieNowosciClient(factory, NullLogger<OProgramieNowosciClient>.Instance);
    }

    private sealed class FakeNowosci : IOProgramieNowosci
    {
        private readonly IReadOnlyList<OProgramieNowosc>? list;
        private readonly Exception? error;

        public FakeNowosci(IReadOnlyList<OProgramieNowosc>? list, Exception? error = null)
        {
            this.list = list;
            this.error = error;
        }

        public Task<IReadOnlyList<OProgramieNowosc>> GetAsync(int appId, CancellationToken cancellationToken = default)
            => error is null ? Task.FromResult(list ?? Array.Empty<OProgramieNowosc>()) : Task.FromException<IReadOnlyList<OProgramieNowosc>>(error);
    }

    private sealed class FakeEnvironment : IHostEnvironment
    {
        public FakeEnvironment(string name) => EnvironmentName = name;

        public string EnvironmentName { get; set; }

        public string ApplicationName { get; set; } = "test";

        public string ContentRootPath { get; set; } = string.Empty;

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode status;
        private readonly string body;

        public StubHandler(HttpStatusCode status, string body)
        {
            this.status = status;
            this.body = body;
        }

        public Uri? LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri;

            return Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") });
        }
    }
}
