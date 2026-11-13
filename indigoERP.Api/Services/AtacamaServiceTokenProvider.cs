using System.Net.Http.Json;

namespace indigoERP.Api.Services;

/// <summary>
/// Token service-to-service (authorizationSRV, Service/GetToken) dla wywołań indigoERP -> atacamaDMS.Api.
/// Sekret klienta "indigoERP" czytany jest z zmiennej systemowej indigoERP_ServiceClient (ta sama wartość,
/// co w authorizationSRV). Token jest cache'owany do chwili wygaśnięcia.
/// </summary>
public interface IAtacamaServiceTokenProvider
{
    /// <summary>Zwraca token albo null, gdy nie da się go pobrać (brak sekretu, bramka odrzuciła żądanie).</summary>
    Task<string?> GetTokenAsync(CancellationToken cancellationToken);
}

public sealed class AtacamaServiceTokenProvider(
    IHttpClientFactory clients,
    IConfiguration configuration,
    ILogger<AtacamaServiceTokenProvider> logger) : IAtacamaServiceTokenProvider
{
    private const string ClientApp = "indigoERP";
    private const string TargetApp = "atacamaDMS";
    private static readonly TimeSpan ExpiryBuffer = TimeSpan.FromSeconds(30);

    private readonly SemaphoreSlim _blokada = new(1, 1);
    private string? _token;
    private DateTimeOffset _wygasa = DateTimeOffset.MinValue;

    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (_token is not null && DateTimeOffset.UtcNow < _wygasa) return _token;

        await _blokada.WaitAsync(cancellationToken);
        try
        {
            if (_token is not null && DateTimeOffset.UtcNow < _wygasa) return _token;

            var klucz = Environment.GetEnvironmentVariable("indigoERP_ServiceClient") ?? configuration["ServiceAuth:Key"];
            if (string.IsNullOrWhiteSpace(klucz))
            {
                logger.LogError("Brak sekretu indigoERP_ServiceClient. Nie można pobrać tokenu dla {TargetApp}.", TargetApp);
                return null;
            }

            // Adres bramki authorizationSRV: zmienna systemowa GatewayUrl (jak na serwerach), w zastępstwie konfiguracja GatewayUrl.
            var gateway = Environment.GetEnvironmentVariable("GatewayUrl") ?? configuration["GatewayUrl"];
            if (!Uri.TryCreate(gateway, UriKind.Absolute, out var adres) || adres.Scheme != Uri.UriSchemeHttps)
            {
                logger.LogError("Brak poprawnego HTTPS dla zmiennej GatewayUrl.");
                return null;
            }

            if (!adres.AbsolutePath.EndsWith('/'))
            {
                adres = new Uri(adres.AbsoluteUri + "/");
            }

            var client = clients.CreateClient("authorizationSRV");
            client.BaseAddress = adres;

            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"Service/GetToken?clientApp={Uri.EscapeDataString(ClientApp)}&targetApp={Uri.EscapeDataString(TargetApp)}");
            request.Headers.Add("X-Service-Key", klucz);

            using var response = await client.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("authorizationSRV odrzucił pobranie tokenu dla {TargetApp}. Status: {StatusCode}.",
                    TargetApp, response.StatusCode);
                return null;
            }

            var odpowiedz = await response.Content.ReadFromJsonAsync<ServiceTokenResponse>(cancellationToken);
            if (odpowiedz is null || string.IsNullOrWhiteSpace(odpowiedz.Token))
            {
                logger.LogError("authorizationSRV zwrócił pusty token dla {TargetApp}.", TargetApp);
                return null;
            }

            var wygasa = odpowiedz.ExpiresAtUtc - ExpiryBuffer;
            _wygasa = wygasa > DateTimeOffset.UtcNow ? wygasa : DateTimeOffset.UtcNow.AddMinutes(1);
            _token = odpowiedz.Token;
            return _token;
        }
        finally
        {
            _blokada.Release();
        }
    }

    private sealed record ServiceTokenResponse(string Token, DateTimeOffset ExpiresAtUtc);
}
