using indigoERP.Blazor.Utils;
using indigoERP.Service.Interfaces;
using indigoERP.Service.Interfaces._Administrator;
using indigoERP.Service.Interfaces._Autoryzacja;
using indigoERP.Service.Interfaces._Slownik;
using indigoERP.Service.Services;
using indigoERP.Service.Services._Administrator;
using indigoERP.Service.Services._Autoryzacja;
using indigoERP.Service.Services._Slownik;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Radzen;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
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
services.AddScoped<IGrupaService, GrupaService>();
services.AddScoped<IPracownikService, PracownikService>();
services.AddScoped<IRolaService, RolaService>();
services.AddScoped<IUzytkownikService, UzytkownikService>();
services.AddScoped<ISerwerService, SerwerService>();
services.AddScoped<ISerwerTypService, SerwerTypService>();

// Home
services.AddScoped<IHomeService, HomeService>();

//Aplikacja


services.AddHttpContextAccessor();
services.AddLocalization();

var cultureInfo = new CultureInfo("pl-PL");

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

services.AddMemoryCache();

var app = builder.Build();

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
