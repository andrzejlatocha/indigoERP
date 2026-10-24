using System.Net;
using System.Net.Http.Json;
using indigoERP.Domain.ViewModels;
using indigoERP.Service.Services;

namespace indigoERP.Services.Tests;

public sealed class HomeServiceTests
{
    [Fact]
    public async Task SelectAplikacjaByUzytkownik_uses_the_expected_query_contract()
    {
        var handler = new RecordingHandler(_ => JsonResponse<IEnumerable<AplikacjaViewModel>>(
        [
            new AplikacjaViewModel { appID = 132, appNazwa = "Indigo" }
        ]));
        var service = new HomeService(TestHttpClientFactory.Create(handler));

        var result = await service.SelectAplikacjaByUzytkownik(77, "2");

        Assert.Single(result);
        Assert.Equal("https://test.local/api/Home/SelectAplikacjaByUzytkownik?usrID=77&appStatus=2", handler.Request!.RequestUri!.ToString());
    }

    [Fact]
    public async Task SelectAplikacja_returns_an_empty_collection_when_the_api_returns_null()
    {
        var handler = new RecordingHandler(_ => JsonResponse<IEnumerable<AplikacjaViewModel>?>(null));
        var service = new HomeService(TestHttpClientFactory.Create(handler));

        var result = await service.SelectAplikacja("2");

        Assert.Empty(result);
        Assert.EndsWith("Home/SelectAplikacja?appStatus=2", handler.Request!.RequestUri!.ToString(), StringComparison.Ordinal);
    }

    private static HttpResponseMessage JsonResponse<T>(T body) => new(HttpStatusCode.OK) { Content = JsonContent.Create(body) };
}
