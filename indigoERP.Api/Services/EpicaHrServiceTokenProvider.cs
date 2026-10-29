using System.Net.Http.Json;

namespace indigoERP.Api.Services;

/// <summary>
/// Token service-to-service (authorizationSRV, Service/GetToken) dla wywołań indigoERP -> epicaHR.
/// Sekret klienta "indigoERP" czytany jest z zmiennej systemowej ServiceClient_indigoERP (ta sama wartość,
/// co w authorizationSRV). Token jest cache'owany do chwili wygaśnięcia.
/// </summary>
public interface IEpicaHrServiceTokenProvider
{
    /// <summary>Zwraca token albo null, gdy nie da się go pobrać (brak sekretu, bramka odrzuciła żądanie).</summary>
    Task<string?> GetTokenAsync(CancellationToken cancellationToken);
}

public sealed class EpicaHrServiceTokenProvider(
    IHttpClientFactory clients,
    IConfiguration configuration,
    ILogger<EpicaHrServiceTokenProvider> logger) : IEpicaHrServiceTokenProvider
{
    private const string ClientApp = "indigoERP";
    private const string TargetApp = "epicaHR";
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

            var klucz = Environment.GetEnvironmentVariable("ServiceClient_indigoERP") ?? configuration["ServiceAuth:Key"];
            if (string.IsNullOrWhiteSpace(klucz))
            {
                logger.LogError("Brak sekretu ServiceClient_indigoERP. Nie można pobrać tokenu dla {TargetApp}.", TargetApp);
                return null;
            }

            var gateway = configuration["ServiceAuth:GatewayUrl"];
            if (!Uri.TryCreate(gateway, UriKind.Absolute, out var adres) || adres.Scheme != Uri.UriSchemeHttps)
            {
                logger.LogError("Brak poprawnego HTTPS dla ServiceAuth:GatewayUrl.");
                return null;
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
