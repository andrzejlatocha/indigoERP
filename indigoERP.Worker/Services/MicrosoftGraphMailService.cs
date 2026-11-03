using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.StaticFiles;

namespace indigoERP.Worker.Services;

public interface IMicrosoftGraphMailService
{
    bool IsConfigured { get; }
    Task SendAsync(GraphMailMessage message, CancellationToken cancellationToken);
    Task<IReadOnlyList<UndeliveryNotice>> GetUndeliveryNoticesAsync(string sender, DateTimeOffset since, CancellationToken cancellationToken);
}

public sealed class MicrosoftGraphMailService : IMicrosoftGraphMailService
{
    private const string GraphScope = "https://graph.microsoft.com/.default";
    private const int SimpleAttachmentLimitBytes = 3 * 1024 * 1024;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private string? _accessToken;
    private DateTimeOffset _accessTokenExpiresAt;

    public MicrosoftGraphMailService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(TenantId) && !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret);

    private string? TenantId => _configuration["MicrosoftGraph:TenantId"];
    private string? ClientId => _configuration["MicrosoftGraph:ClientId"];
    private string? ClientSecret => _configuration["MicrosoftGraph:ClientSecret"];

    public async Task SendAsync(GraphMailMessage message, CancellationToken cancellationToken)
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException(
                "Brak konfiguracji Graph. Ustaw MicrosoftGraph_TenantId, MicrosoftGraph_ClientId i MicrosoftGraph_ClientSecret.");
        }

        ValidateEmail(message.Nadawca, "nadawcy");

        if (message.Odbiorcy.Count == 0)
        {
            throw new InvalidOperationException("Wiadomość nie ma odbiorców.");
        }

        foreach (var odbiorca in message.Odbiorcy)
        {
            ValidateEmail(odbiorca, "odbiorcy");
        }

        var client = _httpClientFactory.CreateClient();
        var token = await GetAccessTokenAsync(client, cancellationToken);
        var sender = Uri.EscapeDataString(message.Nadawca);
        var draftId = await CreateDraftAsync(client, token, sender, message, cancellationToken);

        foreach (var attachment in message.Zalaczniki)
        {
            if (attachment.Zawartosc.Length <= SimpleAttachmentLimitBytes)
            {
                await AddSimpleAttachmentAsync(client, token, sender, draftId, attachment, cancellationToken);
            }
            else
            {
                await AddLargeAttachmentAsync(client, token, sender, draftId, attachment, cancellationToken);
            }
        }

        await SendDraftAsync(client, token, sender, draftId, cancellationToken);
    }

    private async Task<string> CreateDraftAsync(
        HttpClient client,
        string token,
        string sender,
        GraphMailMessage message,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            subject = message.Temat,
            body = new { contentType = "HTML", content = message.Tresc },
            toRecipients = message.Odbiorcy.Select(email => new { emailAddress = new { address = email } })
        };

        using var request = CreateGraphRequest(HttpMethod.Post, $"https://graph.microsoft.com/v1.0/users/{sender}/messages", token);
        request.Content = CreateJsonContent(payload);

        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        EnsureSuccess(response, responseBody, "utworzenie wiadomości roboczej");

        using var document = JsonDocument.Parse(responseBody);
        return document.RootElement.GetProperty("id").GetString()
            ?? throw new InvalidOperationException("Microsoft Graph nie zwrócił identyfikatora wiadomości.");
    }

    private async Task AddSimpleAttachmentAsync(
        HttpClient client,
        string token,
        string sender,
        string draftId,
        GraphMailAttachment attachment,
        CancellationToken cancellationToken)
    {
        var payload = new Dictionary<string, object?>
        {
            ["@odata.type"] = "#microsoft.graph.fileAttachment",
            ["name"] = attachment.Nazwa,
            ["contentType"] = attachment.ContentType,
            ["contentBytes"] = Convert.ToBase64String(attachment.Zawartosc)
        };

        var url = $"https://graph.microsoft.com/v1.0/users/{sender}/messages/{Uri.EscapeDataString(draftId)}/attachments";
        using var request = CreateGraphRequest(HttpMethod.Post, url, token);
        request.Content = CreateJsonContent(payload);

        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        EnsureSuccess(response, responseBody, $"dodanie załącznika {attachment.Nazwa}");
    }

    private async Task AddLargeAttachmentAsync(
        HttpClient client,
        string token,
        string sender,
        string draftId,
        GraphMailAttachment attachment,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            AttachmentItem = new
            {
                attachmentType = "file",
                name = attachment.Nazwa,
                size = attachment.Zawartosc.LongLength,
                contentType = attachment.ContentType
            }
        };

        var url = $"https://graph.microsoft.com/v1.0/users/{sender}/messages/{Uri.EscapeDataString(draftId)}/attachments/createUploadSession";
        using var request = CreateGraphRequest(HttpMethod.Post, url, token);
        request.Content = CreateJsonContent(payload);

        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        EnsureSuccess(response, responseBody, $"utworzenie sesji załącznika {attachment.Nazwa}");

        using var document = JsonDocument.Parse(responseBody);
        var uploadUrl = document.RootElement.GetProperty("uploadUrl").GetString()
            ?? throw new InvalidOperationException("Microsoft Graph nie zwrócił adresu sesji załącznika.");

        const int chunkSize = 320 * 1024;

        for (var offset = 0; offset < attachment.Zawartosc.Length; offset += chunkSize)
        {
            var count = Math.Min(chunkSize, attachment.Zawartosc.Length - offset);
            using var uploadRequest = new HttpRequestMessage(HttpMethod.Put, uploadUrl)
            {
                Content = new ByteArrayContent(attachment.Zawartosc, offset, count)
            };
            uploadRequest.Content.Headers.ContentLength = count;
            uploadRequest.Content.Headers.ContentRange = new System.Net.Http.Headers.ContentRangeHeaderValue(
                offset,
                offset + count - 1,
                attachment.Zawartosc.Length);

            using var uploadResponse = await client.SendAsync(uploadRequest, cancellationToken);
            var uploadBody = await uploadResponse.Content.ReadAsStringAsync(cancellationToken);

            if (!uploadResponse.IsSuccessStatusCode && uploadResponse.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                EnsureSuccess(uploadResponse, uploadBody, $"wysłanie załącznika {attachment.Nazwa}");
            }
        }
    }

    public async Task<IReadOnlyList<UndeliveryNotice>> GetUndeliveryNoticesAsync(
        string sender,
        DateTimeOffset since,
        CancellationToken cancellationToken)
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException(
                "Brak konfiguracji Graph. Ustaw MicrosoftGraph_TenantId, MicrosoftGraph_ClientId i MicrosoftGraph_ClientSecret.");
        }

        ValidateEmail(sender, "nadawcy");

        var client = _httpClientFactory.CreateClient();
        var token = await GetAccessTokenAsync(client, cancellationToken);
        var od = since.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", System.Globalization.CultureInfo.InvariantCulture);
        var filter = Uri.EscapeDataString($"receivedDateTime ge {od}");
        var url = $"https://graph.microsoft.com/v1.0/users/{Uri.EscapeDataString(sender)}/mailFolders/inbox/messages"
            + $"?$filter={filter}&$select=subject,from,body,receivedDateTime&$top=100";

        using var request = CreateGraphRequest(HttpMethod.Get, url, token);
        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        EnsureSuccess(response, responseBody, "odczyt skrzynki nadawcy");

        using var document = JsonDocument.Parse(responseBody);
        var powiadomienia = new List<UndeliveryNotice>();

        foreach (var item in document.RootElement.GetProperty("value").EnumerateArray())
        {
            var temat = item.TryGetProperty("subject", out var subjectValue) ? subjectValue.GetString() ?? string.Empty : string.Empty;
            var nadawca = item.TryGetProperty("from", out var fromValue)
                && fromValue.TryGetProperty("emailAddress", out var emailValue)
                && emailValue.TryGetProperty("address", out var addressValue)
                    ? addressValue.GetString() ?? string.Empty
                    : string.Empty;

            if (!NdrDopasowanie.CzyPowiadomienie(nadawca, temat))
            {
                continue;
            }

            var tresc = item.TryGetProperty("body", out var bodyValue) && bodyValue.TryGetProperty("content", out var contentValue)
                ? NdrDopasowanie.OczyscHtml(contentValue.GetString() ?? string.Empty)
                : string.Empty;
            var received = item.TryGetProperty("receivedDateTime", out var receivedValue)
                && DateTimeOffset.TryParse(receivedValue.GetString(), out var receivedParsed)
                    ? receivedParsed
                    : DateTimeOffset.MinValue;

            powiadomienia.Add(new UndeliveryNotice(received, tresc));
        }

        return powiadomienia;
    }

    private async Task SendDraftAsync(HttpClient client, string token, string sender, string draftId, CancellationToken cancellationToken)
    {
        var url = $"https://graph.microsoft.com/v1.0/users/{sender}/messages/{Uri.EscapeDataString(draftId)}/send";
        using var request = CreateGraphRequest(HttpMethod.Post, url, token);
        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        EnsureSuccess(response, responseBody, "wysłanie wiadomości");
    }

    private async Task<string> GetAccessTokenAsync(HttpClient client, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_accessToken) && _accessTokenExpiresAt > DateTimeOffset.UtcNow.AddMinutes(2))
        {
            return _accessToken;
        }

        await _tokenLock.WaitAsync(cancellationToken);

        try
        {
            if (!string.IsNullOrWhiteSpace(_accessToken) && _accessTokenExpiresAt > DateTimeOffset.UtcNow.AddMinutes(2))
            {
                return _accessToken;
            }

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
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            EnsureSuccess(response, responseBody, "uwierzytelnienie Microsoft Graph");

            using var document = JsonDocument.Parse(responseBody);
            _accessToken = document.RootElement.GetProperty("access_token").GetString()
                ?? throw new InvalidOperationException("Microsoft Graph nie zwrócił access_token.");
            var expiresIn = document.RootElement.TryGetProperty("expires_in", out var expiresValue) && expiresValue.TryGetInt32(out var seconds)
                ? seconds
                : 300;
            _accessTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn);

            return _accessToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private static HttpRequestMessage CreateGraphRequest(HttpMethod method, string url, string token)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private static StringContent CreateJsonContent<T>(T payload) =>
        new(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

    private static void ValidateEmail(string email, string typ)
    {
        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            throw new InvalidOperationException($"Nieprawidłowy adres e-mail {typ}: {email}.");
        }
    }

    private static void EnsureSuccess(HttpResponseMessage response, string body, string operation)
    {
        if (!response.IsSuccessStatusCode)
        {
            var summary = body.Length <= 500 ? body : body[..500];
            throw new InvalidOperationException($"Microsoft Graph: {operation} nie powiodło się ({(int)response.StatusCode}): {summary}");
        }
    }
}

public sealed record GraphMailMessage(
    string Nadawca,
    IReadOnlyCollection<string> Odbiorcy,
    string Temat,
    string Tresc,
    IReadOnlyCollection<GraphMailAttachment> Zalaczniki);

public sealed record GraphMailAttachment(string Nazwa, string ContentType, byte[] Zawartosc)
{
    public static GraphMailAttachment Create(string name, byte[] content)
    {
        var provider = new FileExtensionContentTypeProvider();
        return new GraphMailAttachment(name, provider.TryGetContentType(name, out var type) ? type : "application/octet-stream", content);
    }
}
