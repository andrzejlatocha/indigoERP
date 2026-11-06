using indigoERP.Domain.Logging;
using indigoERP.WebUI.Services;
using indigoERP.Service.Interfaces._Autoryzacja;
using indigoERP.Service.Services._Autoryzacja;
using Microsoft.AspNetCore.Authentication.Negotiate;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using indigoERP.Service.Interfaces;
using indigoERP.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Logi do plikow dziennych (jak w namibPMS); katalog z Logging:FilePath (produkcja: C:\inetpub\logs\indigoERP.WebUI).
string logPath = DailyFileLoggerExtensions.ResolveLogDirectory(builder.Configuration["Logging:FilePath"], builder.Environment.ContentRootPath);
builder.Logging.AddDailyFile(logPath, "indigoERP.WebUI");
// Blad startu (np. brak konfiguracji) konczy proces, zanim ruszy logowanie hosta - zapisujemy go do tego samego pliku.
AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    new DailyFileLoggerProvider(logPath, "indigoERP.WebUI", LogLevel.Error).CreateLogger("Startup")
        .LogCritical(e.ExceptionObject as Exception, "Nieobsluzony blad - aplikacja zatrzymana.");
// Logowanie odpowiedzi API z bledem dla kazdego HttpClient z fabryki.
builder.Services.AddTransient<ApiErrorHandler>();
builder.Services.ConfigureHttpClientDefaults(b => b.AddHttpMessageHandler<ApiErrorHandler>());
var configuration = builder.Configuration;
var services = builder.Services;
var connectionString = builder.Configuration.GetConnectionString("indigoERPConnection");

// Add services to the container.
services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});
services.AddRazorPages().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null).AddRazorRuntimeCompilation(); 

string? baseApiUrl = configuration.GetSection("appSettings").GetValue<string>(key: "BaseApiUrl");
services.AddHttpClient("indigoApiClient", client =>
{
    client.BaseAddress = new Uri(baseApiUrl);
    client.DefaultRequestHeaders.Clear();
});

//Autoryzacja
services.AddSingleton<ILoginService, LoginService>();

//Aplikacja
services.AddSingleton<IUzytkownikAplikacjaService, UzytkownikAplikacjaService>();

services.AddHttpContextAccessor();
services.AddLocalization();

var cultureInfo = new CultureInfo("pl-PL");

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookie";
    options.DefaultChallengeScheme = "Cookie";
})
     .AddCookie("Cookie", options =>
     {
         options.LoginPath = "/Autoryzacja/Login";
         options.AccessDeniedPath = "/Denied";
         options.ExpireTimeSpan = TimeSpan.FromMinutes(configuration.GetSection("appSettings").GetValue<int>(key: "timeOut"));
     });

services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error/");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
