using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using indigoERP.Domain.ViewModels._Wspolne;
using indigoERP.Service.Services._Wspolne;

namespace indigoERP.Services.Tests;

public sealed class PodgladServiceTests
{
    [Fact]
    public async Task OznaczPowiadomienie_returns_true_and_posts_to_Odczytaj()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.NoContent));
        var service = Utworz(handler);

        var wynik = await service.OznaczPowiadomienie(5);

        Assert.True(wynik);
        Assert.Equal(HttpMethod.Post, handler.Request!.Method);
        Assert.EndsWith("Powiadomienie/Odczytaj/5", handler.Request.RequestUri!.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task OznaczPowiadomienie_returns_false_when_the_api_rejects_the_request()
    {
        var service = Utworz(new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.Forbidden)));

        Assert.False(await service.OznaczPowiadomienie(5));
    }

    [Fact]
    public async Task OznaczPowiadomienie_returns_false_when_the_connection_fails()
    {
        var service = Utworz(new RecordingHandler(_ => throw new HttpRequestException("brak połączenia")));

        Assert.False(await service.OznaczPowiadomienie(5));
    }

    [Fact]
    public async Task OznaczWszystkiePowiadomienia_returns_the_number_of_marked_items()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(3) });
        var service = Utworz(handler);

        var wynik = await service.OznaczWszystkiePowiadomienia(132, 7);

        Assert.Equal(3, wynik);
        Assert.Equal(HttpMethod.Post, handler.Request!.Method);
        Assert.EndsWith("Powiadomienie/OdczytajWszystkie/132/7", handler.Request.RequestUri!.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task OznaczWszystkiePowiadomienia_returns_null_when_the_api_fails()
    {
        var service = Utworz(new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError)));

        Assert.Null(await service.OznaczWszystkiePowiadomienia(132, 7));
    }

    [Fact]
    public async Task OznaczWszystkiePowiadomienia_returns_null_when_the_connection_fails()
    {
        var service = Utworz(new RecordingHandler(_ => throw new HttpRequestException("brak połączenia")));

        Assert.Null(await service.OznaczWszystkiePowiadomienia(132, 7));
    }

    [Fact]
    public async Task PowiadomieniaOstatnie_passes_the_take_limit_in_the_query()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(Array.Empty<PowiadomienieViewModel>())
        });
        var service = Utworz(handler);

        var wynik = await service.PowiadomieniaOstatnie(132, 7, 500);

        Assert.Empty(wynik);
        Assert.EndsWith("Powiadomienie/SelectOstatnie/132/7?take=500", handler.Request!.RequestUri!.ToString(), StringComparison.Ordinal);
    }

    private static PodgladService Utworz(HttpMessageHandler handler) =>
        new(TestHttpClientFactory.Create(handler), new TestAuthenticationStateProvider(new Claim("usrID", "7")));
}
