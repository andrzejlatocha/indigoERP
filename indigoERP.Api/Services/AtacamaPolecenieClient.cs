using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using indigoERP.Domain.ViewModels._Wspolne;

namespace indigoERP.Api.Services;

/// <summary>
/// Wywołania atacamaDMS.Api dla przepisów wewnętrznych (biblioteka 13). Token serwisowy z rolą HCZ-atacamaDMS-Integracja-indigoERP.
/// Adres bazowy z konfiguracji AtacamaDms:BaseUrl (np. https://app.hcz.com.pl:9210/api/).
/// </summary>
public interface IAtacamaPolecenieClient
{
    /// <summary>Lista przepisów (pliki PDF biblioteki 13); null, gdy brak tokenu serwisowego.</summary>
    Task<IReadOnlyList<PrzepisWewnetrznyViewModel>?> SelectPubliczneAsync(string? szukaj, CancellationToken cancellationToken);

    /// <summary>Plik PDF przepisu; null, gdy plik nie istnieje w bibliotece 13 albo brak tokenu.</summary>
    Task<byte[]?> PobierzPdfAsync(int pliID, CancellationToken cancellationToken);
}

public sealed class AtacamaPolecenieClient(
    IHttpClientFactory clients,
    IAtacamaServiceTokenProvider tokens,
    IConfiguration configuration,
    ILogger<AtacamaPolecenieClient> logger) : IAtacamaPolecenieClient
{
    private const string NazwaKlienta = "atacamaDMSPolecenia";

    public async Task<IReadOnlyList<PrzepisWewnetrznyViewModel>?> SelectPubliczneAsync(string? szukaj, CancellationToken cancellationToken)
    {
        var sciezka = "PrzepisyWewnetrzne/SelectPubliczne";

        if (!string.IsNullOrWhiteSpace(szukaj))
        {
            sciezka += "?szukaj=" + Uri.EscapeDataString(szukaj.Trim());
        }

        using var odpowiedz = await WyslijAsync(sciezka, cancellationToken);

        if (odpowiedz == null)
        {
            return null;
        }

        var lista = await odpowiedz.Content.ReadFromJsonAsync<List<PrzepisWewnetrznyViewModel>>(cancellationToken);

        return lista ?? [];
    }

    public async Task<byte[]?> PobierzPdfAsync(int pliID, CancellationToken cancellationToken)
    {
        using var odpowiedz = await WyslijAsync($"PrzepisyWewnetrzne/Pdf/{pliID}", cancellationToken);

        if (odpowiedz == null || odpowiedz.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        return await odpowiedz.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    // Zwraca null, gdy nie ma tokenu serwisowego (wtedy controller odpowiada 503).
    // Błąd HTTP inny niż 404 zostaje rzucony jako HttpRequestException (controller odpowiada 502).
    private async Task<HttpResponseMessage?> WyslijAsync(string sciezka, CancellationToken cancellationToken)
    {
        var token = await tokens.GetTokenAsync(cancellationToken);

        if (token == null)
        {
            return null;
        }

        var baza = configuration["AtacamaDms:BaseUrl"];

        if (!Uri.TryCreate(baza, UriKind.Absolute, out var adres) || adres.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException("Brak poprawnego HTTPS w AtacamaDms:BaseUrl.");
        }

        if (!adres.AbsolutePath.EndsWith('/'))
        {
            adres = new Uri(adres.AbsoluteUri + "/");
        }

        var klient = clients.CreateClient(NazwaKlienta);
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(adres, sciezka));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var odpowiedz = await klient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (odpowiedz.StatusCode == HttpStatusCode.NotFound)
        {
            return odpowiedz;
        }

        if (!odpowiedz.IsSuccessStatusCode)
        {
            logger.LogError("atacamaDMS.Api zwrócił {StatusCode} dla {Sciezka}.", (int)odpowiedz.StatusCode, sciezka);
            odpowiedz.Dispose();
            throw new HttpRequestException($"atacamaDMS.Api zwrócił {(int)odpowiedz.StatusCode}.", null, odpowiedz.StatusCode);
        }

        return odpowiedz;
    }
}
