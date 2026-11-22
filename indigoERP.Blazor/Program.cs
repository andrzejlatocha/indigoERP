using indigoERP.Domain.Logging;
using indigoERP.Blazor.Services;
using indigoERP.Blazor.Utils;
using indigoERP.Service.Interfaces;
using indigoERP.Service.Interfaces._Administrator;
using indigoERP.Service.Interfaces._Autoryzacja;
using indigoERP.Service.Interfaces._Podpis;
using indigoERP.Service.Interfaces._Slownik;
using indigoERP.Service.Interfaces._Wspolne;
using indigoERP.Service.Services;
using indigoERP.Service.Services._Administrator;
using indigoERP.Service.Services._Autoryzacja;
using indigoERP.Service.Services._Podpis;
using indigoERP.Service.Services._Slownik;
using indigoERP.Service.Services._Wspolne;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Radzen;
using System.Globalization;
using indigoERP.Blazor.OProgramie;

var builder = WebApplication.CreateBuilder(args);

// Logi do plikow dziennych (jak w namibPMS); katalog z Logging:FilePath (produkcja: C:\inetpub\logs\indigoERP).
string logPath = DailyFileLoggerExtensions.ResolveLogDirectory(builder.Configuration["Logging:FilePath"], builder.Environment.ContentRootPath);
builder.Logging.AddDailyFile(logPath, "indigoERP.Blazor");
// Blad startu (np. brak konfiguracji) konczy proces, zanim ruszy logowanie hosta - zapisujemy go do tego samego pliku.
AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    new DailyFileLoggerProvider(logPath, "indigoERP.Blazor", LogLevel.Error).CreateLogger("Startup")
        .LogCritical(e.ExceptionObject as Exception, "Nieobsluzony blad - aplikacja zatrzymana.");
// Logowanie odpowiedzi API z bledem dla kazdego HttpClient z fabryki.
builder.Services.AddTransient<ApiErrorHandler>();
builder.Services.ConfigureHttpClientDefaults(b => b.AddHttpMessageHandler<ApiErrorHandler>());
builder.Services.AddOProgramie(builder.Configuration, builder.Environment);

// Katalog kluczy Data Protection (cookies logowania i antyfalsyfikacja Blazora) zapisany na dysku: po restarcie procesu nie gina sesje.
// Zmienna systemowa DataProtection_KeysPath (wspolna dla serwera, Process, potem Machine) nadpisuje DataProtection:KeysPath z appsettings.
// Konfiguracja jest tylko rezerwa. Poza Development brak obu zatrzymuje start czytelnym bledem.
var dataProtectionKeysPathFromEnv = Environment.GetEnvironmentVariable("DataProtection_KeysPath")
    ?? Environment.GetEnvironmentVariable("DataProtection_KeysPath", EnvironmentVariableTarget.Machine);

if (!string.IsNullOrWhiteSpace(dataProtectionKeysPathFromEnv))
{
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?> { ["DataProtection:KeysPath"] = dataProtectionKeysPathFromEnv });
}

var dataProtectionKeysPath = builder.Configuration["DataProtection:KeysPath"];

if (string.IsNullOrWhiteSpace(dataProtectionKeysPath))
{
    if (!builder.Environment.IsDevelopment())
    {
        throw new InvalidOperationException(
            "Brak katalogu kluczy Data Protection. Ustaw zmienna systemowa DataProtection_KeysPath (wspolna dla serwera) albo DataProtection:KeysPath w konfiguracji.");
    }

    dataProtectionKeysPath = Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys");
}

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath))
    .SetApplicationName("indigoERP.Blazor");

// Zmienna środowiskowa InternalSso nadpisuje InternalSso:Key z appsettings.json — jedna, wspólna
// zmienna systemowa dla wszystkich aplikacji na tym serwerze (nie per-appka), patrz też indigoERP.Api\Program.cs.
var internalSsoKeyFromEnv = Environment.GetEnvironmentVariable("InternalSso");

if (!string.IsNullOrEmpty(internalSsoKeyFromEnv))
{
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?> { ["InternalSso:Key"] = internalSsoKeyFromEnv });
}

var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

services.AddRazorPages().AddRazorRuntimeCompilation();

if (builder.Environment.IsDevelopment())
{
    services.AddServerSideBlazor().AddCircuitOptions(x => x.DetailedErrors = true);
}
else
{
    services.AddServerSideBlazor();
}

var baseApiUri = GetConfiguredApiUri(configuration, "BaseApiUrl", "IndigoApiUrl");
services.AddHttpClient("baseApiClient", client =>
{
    client.BaseAddress = baseApiUri;
    client.DefaultRequestHeaders.Clear();
});

var indigoApiUri = GetConfiguredApiUri(configuration, "IndigoApiUrl");
services.AddHttpClient("indigoApiClient", client =>
{
    client.BaseAddress = indigoApiUri;
    client.DefaultRequestHeaders.Clear();
});

// Radzen
services.AddScoped<DialogService>();
services.AddScoped<NotificationService>();
services.AddScoped<ContextMenuService>();
services.AddScoped<TooltipService>();

// Autoryzacja
services.AddSingleton<ILoginService, LoginService>();
services.AddSingleton<IClaimsTransformation, ClaimsTransformation>();

// Adminstrator
services.AddScoped<IAplikacjaService, AplikacjaService>();
services.AddScoped<IAplikacjaUslugaUprawnienieService, AplikacjaUslugaUprawnienieService>();
services.AddScoped<IGrupaService, GrupaService>();
services.AddScoped<ILicznikNaglowkaService, LicznikNaglowkaService>();
services.AddScoped<IPodgladService, PodgladService>();
services.AddScoped<ILoginBlokadaService, LoginBlokadaService>();
services.AddScoped<IPracownikService, PracownikService>();
services.AddScoped<IRolaService, RolaService>();
services.AddScoped<IUzytkownikService, UzytkownikService>();
services.AddScoped<ISerwerService, SerwerService>();
services.AddScoped<ISerwerTypService, SerwerTypService>();

// Home
services.AddScoped<IHomeService, HomeService>();

// Podpisy
services.AddScoped<IPodpisService, PodpisService>();
services.AddScoped<IPodpisUzytkownikService, PodpisUzytkownikService>();
services.AddScoped<IPodpisGrupaService, PodpisGrupaService>();

// Wiadomości
services.AddScoped<IWiadomoscService, WiadomoscService>();
services.AddScoped<IWiadomoscUzytkownikService, WiadomoscUzytkownikService>();
services.AddScoped<IWiadomoscWzorzecService, WiadomoscWzorzecService>();
services.AddScoped<IWiadomoscDokumentService, WiadomoscDokumentService>();
services.AddScoped<IWiadomoscWysylkaService, WiadomoscWysylkaService>();
services.AddScoped<IWiadomoscPracownikService, WiadomoscPracownikService>();
services.AddScoped<IPolecenieSluzboweService, PolecenieSluzboweService>();

//Aplikacja


services.AddHttpContextAccessor();
services.AddLocalization();

var cultureInfo = new CultureInfo("pl-PL");

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

services.AddMemoryCache();

var app = builder.Build();

// Diagnostyka konfiguracji — nie ujawnia wartości sekretu, tylko potwierdza że jest ustawiony
// (ten sam log co w indigoERP.Api\Program.cs, żeby od razu było widać brak klucza na obu stronach).
app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup")
    .LogInformation("Konfiguracja bezpieczeństwa: InternalSso:Key ustawiony={InternalSsoKeySet}.", !string.IsNullOrWhiteSpace(configuration["InternalSso:Key"]));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapRazorPages();
app.MapFallbackToPage("/_Host");

app.Run();

static Uri GetConfiguredApiUri(IConfiguration configuration, string key, string? fallbackKey = null)
{
    var value = configuration[$"appSettings:{key}"];

    if (TryCreateHttpUri(value, out var uri))
    {
        return uri;
    }

    if (!string.IsNullOrWhiteSpace(fallbackKey))
    {
        var fallbackValue = configuration[$"appSettings:{fallbackKey}"];

        if (TryCreateHttpUri(fallbackValue, out uri))
        {
            return uri;
        }
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
