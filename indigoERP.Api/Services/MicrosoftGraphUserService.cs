using System.Net.Http.Headers;
using System.Text.Json;

namespace indigoERP.Api.Services
{
    public sealed record GraphUserInfo(
        string? Id,
        string? DisplayName,
        string? Mail,
        string? UserPrincipalName,
        string? OnPremisesSamAccountName,
        bool? AccountEnabled);

    public interface IMicrosoftGraphUserService
    {
        /// <summary>Czy ustawiono MicrosoftGraph:TenantId, ClientId i ClientSecret.</summary>
        bool IsConfigured { get; }

        /// <summary>Pobiera wszystkich użytkowników tenanta (z paginacją @odata.nextLink).</summary>
        Task<IReadOnlyList<GraphUserInfo>> GetUsersAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// Klient Microsoft Graph (client credentials, uprawnienie aplikacyjne User.Read.All).
    /// Dane połączenia: MicrosoftGraph:TenantId / ClientId / ClientSecret - na serwerze ze zmiennych systemowych
    /// MicrosoftGraph_TenantId / MicrosoftGraph_ClientId / MicrosoftGraph_ClientSecret (mapowanie w Program.cs),
    /// lokalnie z appsettings.Development.json.
    /// </summary>
    public sealed class MicrosoftGraphUserService : IMicrosoftGraphUserService
    {
        private const string GraphScope = "https://graph.microsoft.com/.default";
        private const string UsersUrl = "https://graph.microsoft.com/v1.0/users"
            + "?$select=id,displayName,mail,userPrincipalName,onPremisesSamAccountName,accountEnabled&$top=999";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MicrosoftGraphUserService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string? TenantId => _configuration["MicrosoftGraph:TenantId"];
        private string? ClientId => _configuration["MicrosoftGraph:ClientId"];
        private string? ClientSecret => _configuration["MicrosoftGraph:ClientSecret"];

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(TenantId) && !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret);

        public async Task<IReadOnlyList<GraphUserInfo>> GetUsersAsync(CancellationToken cancellationToken)
        {
            if (!IsConfigured)
            {
                throw new InvalidOperationException(
                    "Brak konfiguracji Microsoft Graph: ustaw zmienne systemowe MicrosoftGraph_TenantId, MicrosoftGraph_ClientId i "
                    + "MicrosoftGraph_ClientSecret (lokalnie sekcja MicrosoftGraph w appsettings.Development.json).");
            }

            var client = _httpClientFactory.CreateClient();
            var token = await GetAccessTokenAsync(client, cancellationToken);
            var users = new List<GraphUserInfo>();
            string? url = UsersUrl;

            while (!string.IsNullOrEmpty(url))
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var response = await client.SendAsync(request, cancellationToken);
                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Microsoft Graph zwrócił {(int)response.StatusCode}: {Truncate(body)}");
                }

                using var document = JsonDocument.Parse(body);
                var root = document.RootElement;

                foreach (var item in root.GetProperty("value").EnumerateArray())
                {
                    users.Add(new GraphUserInfo(
                        GetString(item, "id"),
                        GetString(item, "displayName"),
                        GetString(item, "mail"),
                        GetString(item, "userPrincipalName"),
                        GetString(item, "onPremisesSamAccountName"),
                        item.TryGetProperty("accountEnabled", out var enabled) && enabled.ValueKind is JsonValueKind.True or JsonValueKind.False
                            ? enabled.GetBoolean()
                            : null));
                }

                url = root.TryGetProperty("@odata.nextLink", out var next) ? next.GetString() : null;
            }

            return users;
        }

        private async Task<string> GetAccessTokenAsync(HttpClient client, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = ClientId!,
                    ["client_secret"] = ClientSecret!,
                    ["scope"] = GraphScope,
                    ["grant_type"] = "client_credentials"
                })
            };

            using var response = await client.SendAsync(request, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Logowanie do Microsoft Graph nie powiodło się ({(int)response.StatusCode}): {Truncate(body)}");
            }

            using var document = JsonDocument.Parse(body);

            return document.RootElement.GetProperty("access_token").GetString()
                ?? throw new InvalidOperationException("Microsoft Graph nie zwrócił access_token.");
        }

        private static string? GetString(JsonElement item, string name) =>
            item.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

        private static string Truncate(string text) => text.Length <= 300 ? text : text[..300];
    }
}
