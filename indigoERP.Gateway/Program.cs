using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Jedyny cel proxowania — wewnętrzny (intranetowy) indigoERP.Api. Ta bramka jest jedynym
// komponentem modułu Podpisy wystawianym publicznie; indigoERP.Api zostaje wyłącznie wewnętrzny.
var internalApiUri = GetConfiguredApiUri(builder.Configuration, "IndigoApiUrl");

var routes = new[]
{
    new RouteConfig
    {
        RouteId = "podpis-preview-by-email",
        ClusterId = "indigo-api",
        Match = new RouteMatch { Path = "/Api/Podpis/PreviewByEmail" },
        RateLimiterPolicy = "PodpisPreviewByEmail"
    }
};

var clusters = new[]
{
    new ClusterConfig
    {
        ClusterId = "indigo-api",
        Destinations = new Dictionary<string, DestinationConfig>
        {
            ["indigo-api-1"] = new DestinationConfig { Address = internalApiUri.ToString() }
        }
    }
};

// Świadomie skonfigurowane w kodzie (nie z appsettings) — dokładnie jedna trasa, żeby bramka
// fizycznie nie mogła przepuścić niczego więcej niż PreviewByEmail, nawet przez pomyłkę w configu.
builder.Services.AddReverseProxy().LoadFromMemory(routes, clusters);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Pierwsza linia obrony przed enumeracją adresów email, teraz gdy endpoint jest osiągalny
    // z całego internetu (poza VPN, z komórek), a nie tylko z sieci firmowej.
    options.AddPolicy("PodpisPreviewByEmail", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1)
            }));
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseStaticFiles();
app.UseRateLimiter();

app.MapReverseProxy();

app.Run();

static Uri GetConfiguredApiUri(IConfiguration configuration, string key)
{
    var value = configuration[$"appSettings:{key}"];

    if (TryCreateHttpUri(value, out var uri))
    {
        return uri;
    }

    var configuredValue = string.IsNullOrWhiteSpace(value) ? "<puste>" : value.Trim();
    throw new InvalidOperationException($"Nieprawidłowa konfiguracja appSettings:{key}: '{configuredValue}'.");
}

static bool TryCreateHttpUri(string? value, out Uri uri)
{
    uri = null!;

    if (string.IsNullOrWhiteSpace(value))
    {
        return false;
    }

    var normalizedValue = value.Trim();

    if (!normalizedValue.Contains("://", StringComparison.Ordinal))
    {
        normalizedValue = $"https://{normalizedValue}";
    }

    if (!Uri.TryCreate(normalizedValue, UriKind.Absolute, out var parsedUri))
    {
        return false;
    }

    if (string.IsNullOrWhiteSpace(parsedUri.Host))
    {
        return false;
    }

    if (parsedUri.Scheme != Uri.UriSchemeHttp && parsedUri.Scheme != Uri.UriSchemeHttps)
    {
        return false;
    }

    uri = parsedUri;
    return true;
}
