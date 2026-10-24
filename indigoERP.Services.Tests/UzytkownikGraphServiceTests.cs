using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using indigoERP.Domain.Models._Administrator;
using indigoERP.Service.Services._Administrator;

namespace indigoERP.Services.Tests;

public sealed class UzytkownikGraphServiceTests
{
    [Fact]
    public async Task Select_dane_graph_returns_differences_from_the_api()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new List<UzytkownikEmailGraphModel>
            {
                new() { usrID = 7, usrNazwa = "jan.kowalski", akcja = UzytkownikEmailGraphAkcja.Dodaj, graphEmail = "jan@huta-czestochowa.pl" }
            })
        });
        var provider = new TestAuthenticationStateProvider(new Claim("accessToken", "test-token"));
        var service = new UzytkownikService(TestHttpClientFactory.Create(handler), provider);

        var (response, dane) = await service.SelectDaneGraph();

        Assert.Equal(200, response.Code);
        var pozycja = Assert.Single(dane);
        Assert.Equal(7, pozycja.usrID);
        Assert.Equal(UzytkownikEmailGraphAkcja.Dodaj, pozycja.akcja);
        Assert.EndsWith("Uzytkownik/SelectDaneGraph", handler.Request!.RequestUri!.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Update_dane_graph_sends_the_selected_changes_and_returns_the_summary()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new UzytkownikEmailGraphWynikModel { dodano = 1, usunieto = 1, pominieto = 0 })
        });
        var provider = new TestAuthenticationStateProvider(new Claim("accessToken", "test-token"));
        var service = new UzytkownikService(TestHttpClientFactory.Create(handler), provider);

        var (response, wynik) = await service.UpdateDaneGraph(
        [
            new UzytkownikEmailGraphZmianaModel { usrID = 7, akcja = UzytkownikEmailGraphAkcja.Dodaj },
            new UzytkownikEmailGraphZmianaModel { usrID = 8, akcja = UzytkownikEmailGraphAkcja.Usun }
        ]);

        Assert.Equal(200, response.Code);
        Assert.Equal(1, wynik!.dodano);
        Assert.Equal(1, wynik.usunieto);
        Assert.Equal(HttpMethod.Put, handler.Request!.Method);
        Assert.Equal("Bearer", handler.Request.Headers.Authorization?.Scheme);
        Assert.EndsWith("Uzytkownik/UpdateDaneGraph", handler.Request.RequestUri!.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Update_dane_graph_preserves_an_error_response_without_a_summary()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent("Brak konfiguracji Microsoft Graph.")
        });
        var provider = new TestAuthenticationStateProvider(new Claim("accessToken", "test-token"));
        var service = new UzytkownikService(TestHttpClientFactory.Create(handler), provider);

        var (response, wynik) = await service.UpdateDaneGraph(
            [new UzytkownikEmailGraphZmianaModel { usrID = 7, akcja = UzytkownikEmailGraphAkcja.Dodaj }]);

        Assert.Equal(503, response.Code);
        Assert.Null(wynik);
        Assert.Equal("Brak konfiguracji Microsoft Graph.", response.Message);
    }
}
