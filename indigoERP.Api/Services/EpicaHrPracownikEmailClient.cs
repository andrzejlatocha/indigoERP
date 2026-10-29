using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace indigoERP.Api.Services;

/// <summary>
/// Wywołuje epicaHR.Api (PracownikEmail/UstawEmailSluzbowy) tokenem service-to-service (klient indigoERP).
/// Dzięki temu adres e-mail służbowy zmienia się równocześnie w indigoERP i w epicaHR.
/// </summary>
public interface IEpicaHrPracownikEmailClient
{
    /// <summary>Zwraca null przy sukcesie albo komunikat błędu.</summary>
    Task<string?> UstawEmailSluzbowyAsync(int pracownikID, string? email, CancellationToken cancellationToken);
}

public sealed class EpicaHrPracownikEmailClient(
    IHttpClientFactory clients,
    IConfiguration configuration,
    IWebHostEnvironment environment,
    IEpicaHrServiceTokenProvider tokenProvider) : IEpicaHrPracownikEmailClient
{
    public async Task<string?> UstawEmailSluzbowyAsync(int pracownikID, string? email, CancellationToken cancellationToken)
    {
        var baseUrl = configuration["appSettings:EpicaHRApiUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl) && environment.IsDevelopment()) baseUrl = "https://localhost:9130/";
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var address) || address.Scheme != Uri.UriSchemeHttps)
            return "Brak poprawnego HTTPS dla appSettings:EpicaHRApiUrl.";

        var token = await tokenProvider.GetTokenAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
            return "Brak tokenu service-to-service dla epicaHR (sprawdź ServiceClient_indigoERP i nadanie).";

        var client = clients.CreateClient("epicaHR");
        client.BaseAddress = address;

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, $"api/PracownikEmail/UstawEmailSluzbowy/{pracownikID}")
            {
                Content = JsonContent.Create(new { Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim() })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await client.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode) return null;

            var tresc = await response.Content.ReadAsStringAsync(cancellationToken);
            return $"epicaHR HTTP {(int)response.StatusCode}: {tresc}";
        }
        catch (HttpRequestException ex)
        {
            return $"epicaHR niedostępne: {ex.Message}";
        }
    }
}
