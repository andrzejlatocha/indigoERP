using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Service.Services._Administrator;

namespace indigoERP.Services.Tests;

public sealed class AdministrationServiceTests
{
    [Fact]
    public async Task Aplikacja_insert_uses_bearer_token_and_returns_the_created_identifier()
    {
        var handler = new RecordingHandler(_ => JsonResponse(new AplikacjaDto { appID = 132 }));
        var provider = new TestAuthenticationStateProvider(new Claim("accessToken", "test-token"));
        var service = new AplikacjaService(TestHttpClientFactory.Create(handler), provider);

        var result = await service.Insert(new AplikacjaDto { appID = 132, appNazwa = "Indigo" });

        Assert.Equal(200, result.Code);
        Assert.Equal(132, result.Id);
        Assert.Equal(HttpMethod.Post, handler.Request!.Method);
        Assert.Equal("Bearer", handler.Request.Headers.Authorization?.Scheme);
        Assert.Equal("test-token", handler.Request.Headers.Authorization?.Parameter);
        Assert.EndsWith("Aplikacja/Insert", handler.Request.RequestUri!.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Uzytkownik_unlock_encodes_the_login_name_and_returns_the_api_status()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.NoContent));
        var provider = new TestAuthenticationStateProvider(new Claim("accessToken", "test-token"));
        var service = new UzytkownikService(TestHttpClientFactory.Create(handler), provider);

        var result = await service.OdblokujLogowanie("Zażółć Test");

        Assert.Equal(204, result.Code);
        Assert.Contains("usrNazwa=Za%C5%BC%C3%B3%C5%82%C4%87%20Test", handler.Request!.RequestUri!.OriginalString);
    }

    [Fact]
    public async Task Grupa_insert_member_preserves_a_conflict_response()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.Conflict)
        {
            Content = new StringContent("Taki członek grupy już istnieje.")
        });
        var provider = new TestAuthenticationStateProvider(new Claim("accessToken", "test-token"));
        var service = new GrupaService(TestHttpClientFactory.Create(handler), provider);

        var result = await service.InsertCzlonek(new GrupaCzlonkowieDto { uwgGrupaNadrzednaID = 1, uwgUzytkownikID = 2 });

        Assert.Equal(409, result.Code);
        Assert.Equal("Taki członek grupy już istnieje.", result.Message);
        Assert.Equal(HttpMethod.Post, handler.Request!.Method);
    }

    private static HttpResponseMessage JsonResponse<T>(T body) => new(HttpStatusCode.OK) { Content = JsonContent.Create(body) };
}
