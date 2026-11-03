using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace indigoERP.Blazor.OProgramie;

/// <summary>Źródło nowości dla strony O programie (anonimowy odczyt z API indigoERP).</summary>
public interface IOProgramieNowosci
{
    /// <summary>Aktywne nowości aplikacji, od najnowszej. Rzuca <see cref="HttpRequestException" />, gdy API jest niedostępne.</summary>
    Task<IReadOnlyList<OProgramieNowosc>> GetAsync(int appId, CancellationToken cancellationToken = default);
}

public sealed class OProgramieNowosciClient : IOProgramieNowosci
{
    public const string ClientName = "oProgramieNowosci";

    private readonly IHttpClientFactory factory;
    private readonly ILogger<OProgramieNowosciClient> logger;

    public OProgramieNowosciClient(IHttpClientFactory factory, ILogger<OProgramieNowosciClient> logger)
    {
        this.factory = factory;
        this.logger = logger;
    }

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IReadOnlyList<OProgramieNowosc>> GetAsync(int appId, CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(ClientName);

        if (client.BaseAddress is null)
        {
            throw new HttpRequestException("Brak konfiguracji adresu API indigoERP (appSettings:IndigoApiUrl).");
        }

        try
        {
            using var response = await client.GetAsync($"Nowosc/SelectByAplikacja/{appId}?nowStatus=2", cancellationToken);
            response.EnsureSuccessStatusCode();

            var list = await response.Content.ReadFromJsonAsync<List<OProgramieNowosc>>(JsonOptions, cancellationToken) ?? new List<OProgramieNowosc>();

            return list
                .OrderByDescending(n => n.nowData ?? DateTime.MinValue)
                .ThenByDescending(n => n.nowID)
                .ToList();
        }
        catch (Exception ex) when (ex is JsonException or TaskCanceledException)
        {
            logger.LogWarning(ex, "Nie udało się pobrać nowości aplikacji {AppID} z API indigoERP.", appId);
            throw new HttpRequestException("Nie udało się pobrać nowości z API indigoERP.", ex);
        }
    }
}

public static class OProgramieExtensions
{
    /// <summary>
    /// Rejestruje klienta nowości. Adres API indigoERP: appSettings:IndigoApiUrl; poza Development nadpisywany zmienną
    /// systemową indigoERP_Api_Url (Process, User, Machine). Brak adresu nie zatrzymuje aplikacji – strona pokaże komunikat.
    /// </summary>
    public static IServiceCollection AddOProgramie(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var address = ResolveAddress(configuration, environment);

        services.AddHttpClient(OProgramieNowosciClient.ClientName, client =>
        {
            if (address is not null)
            {
                client.BaseAddress = address;
            }

            client.Timeout = TimeSpan.FromSeconds(10);
        });
        services.AddScoped<IOProgramieNowosci, OProgramieNowosciClient>();

        return services;
    }

    internal static Uri? ResolveAddress(IConfiguration configuration, IHostEnvironment environment)
    {
        var value = configuration["appSettings:IndigoApiUrl"];

        if (!environment.IsDevelopment())
        {
            var fromVariable = ReadVariable("indigoERP_Api_Url");

            if (!string.IsNullOrWhiteSpace(fromVariable))
            {
                value = fromVariable;
            }
        }

        if (string.IsNullOrWhiteSpace(value) || !Uri.TryCreate(value.Trim().TrimEnd('/') + "/", UriKind.Absolute, out var uri))
        {
            return null;
        }

        return uri.Scheme is "http" or "https" ? uri : null;
    }

    private static string? ReadVariable(string name)
        => Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process)
           ?? Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User)
           ?? Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
}
