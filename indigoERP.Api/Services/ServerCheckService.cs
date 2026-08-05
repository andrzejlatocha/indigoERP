using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace indigoERP.Api.Services
{
    public class ServerCheckService : IServerCheckService
    {
        private const int TimeoutMs = 3000;
        private const int HttpTimeoutMs = 10000;
        private const int DefaultSqlPort = 1433;
        private const int DefaultOraclePort = 1521;

        private readonly IHttpClientFactory _httpClientFactory;

        public ServerCheckService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<(bool IsAlive, int? ResponseMs, string? Error)> CheckAsync(string typKOD, string ip, int? port, CancellationToken cancellationToken)
        {
            return typKOD switch
            {
                "WIN" => await CheckPingAsync(ExtractHost(ip), cancellationToken),
                "SQL" => await CheckTcpAsync(ExtractHost(ip), port ?? DefaultSqlPort, cancellationToken),
                "ORA" => await CheckTcpAsync(ExtractHost(ip), port ?? DefaultOraclePort, cancellationToken),
                "API" or "WEB" or "BLZ" => await CheckHttpAsync(ip, port, cancellationToken),
                _ => (false, null, $"Nieznany typ serwera '{typKOD}'.")
            };
        }

        // Wspólne dla API/WEB/BLZ — zwykły HTTP GET, nieważne czy pod adresem jest REST API, klasyczna
        // strona WebForms czy aplikacja Blazor. Pole "IP / host" musi zawierać PEŁNY adres URL wraz ze
        // schematem (http:// albo https://) — część monitorowanych stron działa po http, część po https,
        // więc nie da się tego odgadnąć/dopisać automatycznie (domyślne https:// byłoby błędne dla http).
        // Jeśli w polu Port podano wartość, NADPISUJE port w URL (np. adres "http://host" + port 9130
        // → sprawdzane jest "http://host:9130") — bez tego pole Port byłoby bez znaczenia.
        private async Task<(bool, int?, string?)> CheckHttpAsync(string url, int? port, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                {
                    return (false, null, $"Pole IP/host musi być pełnym adresem URL z http:// lub https:// (podano: '{url}').");
                }

                if (port.HasValue)
                {
                    uri = new UriBuilder(uri) { Port = port.Value }.Uri;
                }

                var httpClient = _httpClientFactory.CreateClient(nameof(ServerCheckService));
                httpClient.Timeout = TimeSpan.FromMilliseconds(HttpTimeoutMs);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 indigoERP-Monitoring/1.0");
                httpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                httpClient.DefaultRequestHeaders.ExpectContinue = false;

                using var response = await httpClient.GetAsync(uri, cancellationToken);
                stopwatch.Stop();

                // 4xx (np. 401 Unauthorized na stronie logowania, 404 na adresie bez trasy pod "/") oznacza,
                // że serwer ODPOWIEDZIAŁ poprawnie — żyje, tylko akurat odmówił/nie znalazł tego konkretnego
                // żądania. Traktujemy to jako pełny sukces (bez komunikatu) — tylko 5xx (błąd po stronie
                // serwera/aplikacji) pokazujemy jako "padło" z informacją o kodzie.
                var isAlive = (int)response.StatusCode < 500;
                var isClientError = (int)response.StatusCode is >= 400 and < 500;
                var info = (response.IsSuccessStatusCode || isClientError) ? null : $"HTTP {(int)response.StatusCode} {response.ReasonPhrase} ({uri})";

                return (isAlive, (int)stopwatch.ElapsedMilliseconds, info);
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return (false, null, $"Przekroczono czas oczekiwania na odpowiedź ({HttpTimeoutMs / 1000}s).");
            }
            catch (Exception ex)
            {
                return (false, null, BuildErrorMessage(ex));
            }
        }

        // Ping/TCP-probe oczekują samej nazwy hosta/IP — jeśli ktoś przez pomyłkę wpisał pełny URL
        // (np. "https://intranet.hcz.com.pl" dla serwera SQL/WIN), wyciągamy sam host zamiast
        // rzucać błędem "nie można rozwiązać nazwy hosta".
        private static string ExtractHost(string value)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out var uri) && !string.IsNullOrEmpty(uri.Host)
                ? uri.Host
                : value;
        }

        // HttpRequestException (błędy certyfikatu SSL, DNS, connection refused itd.) ma najbardziej
        // przydatny opis w InnerException — sam ex.Message to często ogólnikowe
        // "An error occurred while sending the request.".
        private static string BuildErrorMessage(Exception ex)
        {
            return ex.InnerException != null && ex.InnerException.Message != ex.Message
                ? $"{ex.Message} ({ex.InnerException.Message})"
                : ex.Message;
        }

        private static async Task<(bool, int?, string?)> CheckPingAsync(string ip, CancellationToken cancellationToken)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(ip, TimeoutMs).WaitAsync(cancellationToken);

                return reply.Status == IPStatus.Success
                    ? (true, (int)reply.RoundtripTime, null)
                    : (false, null, reply.Status.ToString());
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        private static async Task<(bool, int?, string?)> CheckTcpAsync(string ip, int port, CancellationToken cancellationToken)
        {
            using var timeoutCts = new CancellationTokenSource(TimeoutMs);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(ip, port, linkedCts.Token);
                stopwatch.Stop();

                return (true, (int)stopwatch.ElapsedMilliseconds, null);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                return (false, null, "Przekroczono czas oczekiwania na połączenie.");
            }
            catch (SocketException ex)
            {
                return (false, null, ex.Message);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
    }
}
