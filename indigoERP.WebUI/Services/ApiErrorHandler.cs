using System.Net;

namespace indigoERP.WebUI.Services;

/// <summary>Loguje odpowiedzi API z błędem (status i początek treści) oraz timeouty i błędy komunikacji; wyjątki przekazuje dalej.</summary>
public sealed class ApiErrorHandler(ILogger<ApiErrorHandler> logger) : DelegatingHandler
{
    private const int MaxLoggedBodyLength = 2048;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                logger.LogWarning("API zwróciło status {StatusCode} dla {Method} {Uri}. Odpowiedź: {Body}", (int)response.StatusCode,
                    request.Method, request.RequestUri, await ReadBodyPreviewAsync(response, cancellationToken));
            return response;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(ex, "Przekroczono czas oczekiwania na API dla {Method} {Uri}.", request.Method, request.RequestUri);
            throw;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Błąd komunikacji z API dla {Method} {Uri}.", request.Method, request.RequestUri);
            throw;
        }
    }

    private static async Task<string?> ReadBodyPreviewAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0) return null;

        string body = await response.Content.ReadAsStringAsync(cancellationToken);
        return body.Length <= MaxLoggedBodyLength ? body : body[..MaxLoggedBodyLength];
    }
}
