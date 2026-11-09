using indigoERP.Domain.Logging;
using indigoERP.Api.Middleware;
using EntityFrameworkCore.UseRowNumberForPaging;
using indigoERP.Api.Data;
using indigoERP.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Logi do plikow dziennych (jak w namibPMS); katalog z Logging:FilePath (produkcja: C:\inetpub\logs\indigoERP.Api).
string logPath = DailyFileLoggerExtensions.ResolveLogDirectory(builder.Configuration["Logging:FilePath"], builder.Environment.ContentRootPath);
builder.Logging.AddDailyFile(logPath, "indigoERP.Api");
// Blad startu (np. brak konfiguracji) konczy proces, zanim ruszy logowanie hosta - zapisujemy go do tego samego pliku.
AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    new DailyFileLoggerProvider(logPath, "indigoERP.Api", LogLevel.Error).CreateLogger("Startup")
        .LogCritical(e.ExceptionObject as Exception, "Nieobsluzony blad - aplikacja zatrzymana.");

builder.AddServiceDefaults();

// DocumentPath (zmienna wspólna dla API, Blazora i Workera, bez prefiksu) wskazuje katalog załączników i nadpisuje FileStorage:Path.
var documentPath = Environment.GetEnvironmentVariable("DocumentPath");

if (!string.IsNullOrWhiteSpace(documentPath))
{
    builder.Configuration["FileStorage:Path"] = documentPath;
}

// Zmienne środowiskowe indigoERP_MonitoringIntervalMinutes / indigoERP_MonitoringSqlLogin /
// indigoERP_MonitoringSqlPassword nadpisują appSettings:MonitoringIntervalMinutes/MonitoringSqlLogin/
// MonitoringSqlPassword z appsettings.json. Prefiks indigoERP_ (nie goły appSettings_) — kilka aplikacji
// na tym samym serwerze może mieć sekcję "appSettings" w swoim appsettings.json, więc bez prefiksu
// zmienna środowiskowa ustawiona na poziomie systemu nadpisywałaby appSettings:X we wszystkich z nich.
// Wbudowany dostawca zmiennych środowiskowych ASP.NET Core wymaga podwójnego podkreślenia jako
// separatora sekcji, więc przy pojedynczym podkreśleniu mapujemy je ręcznie zamiast AddEnvironmentVariables.
var monitoringEnvOverrides = new Dictionary<string, string?>();
var missingMonitoringEnvVars = new List<string>();

void MapMonitoringEnvVar(string envVarName, string configKey)
{
    var value = Environment.GetEnvironmentVariable(envVarName);

    if (!string.IsNullOrEmpty(value))
    {
        monitoringEnvOverrides[configKey] = value;
    }
    else
    {
        missingMonitoringEnvVars.Add(envVarName);
    }
}

MapMonitoringEnvVar("indigoERP_MonitoringIntervalMinutes", "appSettings:MonitoringIntervalMinutes");
MapMonitoringEnvVar("indigoERP_MonitoringSqlLogin", "appSettings:MonitoringSqlLogin");
MapMonitoringEnvVar("indigoERP_MonitoringSqlPassword", "appSettings:MonitoringSqlPassword");
MapMonitoringEnvVar("indigoERP_JwtKey", "Jwt:Key");
MapMonitoringEnvVar("indigoERP_ApiKey", "appSettings:ApiKey");
// InternalSso to jedyny wyjątek od konwencji "indigoERP_" powyżej — to celowo JEDNA, wspólna
// zmienna systemowa, z której korzystają wszystkie aplikacje na tym serwerze (nie per-appka).
MapMonitoringEnvVar("InternalSso", "InternalSso:Key");
// Microsoft Graph (pobieranie adresów e-mail użytkowników z M365) - zmienne systemowe wspólne dla aplikacji, bez prefiksu indigoERP_.
MapMonitoringEnvVar("MicrosoftGraph_TenantId", "MicrosoftGraph:TenantId");
MapMonitoringEnvVar("MicrosoftGraph_ClientId", "MicrosoftGraph:ClientId");
MapMonitoringEnvVar("MicrosoftGraph_ClientSecret", "MicrosoftGraph:ClientSecret");

if (monitoringEnvOverrides.Count > 0)
{
    builder.Configuration.AddInMemoryCollection(monitoringEnvOverrides);
}

var configuration = builder.Configuration;
var services = builder.Services;
var connectionString = builder.Configuration.GetConnectionString("indigoERPConnection");

// Add services to the container.

services.AddHttpContextAccessor();
services.AddControllers();
int serverVersion = configuration.GetSection("appSettings").GetValue<int>(key: "serverVer");

if (serverVersion > 2008)
{
    services.AddDbContext<indigoERPContext>(options => options.UseSqlServer(connectionString));
}
else
{
    services.AddDbContext<indigoERPContext>(options => options.UseSqlServer(connectionString, o => o.UseRowNumberForPaging()));
}

services.AddHttpClient();
services.AddSingleton<IMicrosoftGraphUserService, MicrosoftGraphUserService>();
services.AddSingleton<IEpicaHrServiceTokenProvider, EpicaHrServiceTokenProvider>();
services.AddScoped<IEpicaHrPracownikEmailClient, EpicaHrPracownikEmailClient>();
services.AddSingleton<IAtacamaServiceTokenProvider, AtacamaServiceTokenProvider>();
services.AddHttpClient("atacamaDMSPolecenia", client => client.Timeout = TimeSpan.FromSeconds(60));
services.AddScoped<IAtacamaPolecenieClient, AtacamaPolecenieClient>();
services.AddScoped<IWiadomoscZakresPracownikSource, WiadomoscZakresPracownikSource>();
services.AddSingleton<IWiadomoscMiniaturaService, WiadomoscMiniaturaService>();
services.AddScoped<IWiadomoscZalacznikStorage>(sp =>
    new WiadomoscZalacznikStorage(WiadomoscZalacznikStorage.Root(sp.GetRequiredService<IConfiguration>(), sp.GetRequiredService<IWebHostEnvironment>())));
services.AddSingleton<IServerCheckService, ServerCheckService>();
services.AddHostedService<ServerMonitoringHostedService>();

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer("Bearer", options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidIssuer = configuration.GetSection("Jwt").GetValue<string>("Issuer"),
            ValidateAudience = false,
            ValidAudience = configuration.GetSection("Jwt").GetValue<string>("Audience"),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt").GetValue<string>(key: "Key")))
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.ResolveConflictingActions(descriptions => descriptions.First());
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "indigoERP.API",
        Description = "API dla aplikacji indigoERP"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "Jwt",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });

    //options.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        },
    //        new List<string>()
    //    }
    //});
});

var app = builder.Build();
app.UseApiErrorHandling();

// Diagnostyka konfiguracji — wrażliwe wartości mogą pochodzić z appsettings.json (dev) albo ze zmiennych
// środowiskowych indigoERP_MonitoringSqlLogin / indigoERP_MonitoringSqlPassword / indigoERP_JwtKey /
// indigoERP_ApiKey (produkcja). Log nie ujawnia haseł/kluczy, tylko potwierdza że są ustawione.
var startupLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
startupLogger.LogInformation(
    "Konfiguracja monitoringu: MonitoringSqlLogin={Login}, MonitoringSqlPassword ustawione={PasswordSet}, MonitoringIntervalMinutes={IntervalMinutes}.",
    configuration["appSettings:MonitoringSqlLogin"],
    !string.IsNullOrWhiteSpace(configuration["appSettings:MonitoringSqlPassword"]),
    configuration["appSettings:MonitoringIntervalMinutes"]);
startupLogger.LogInformation(
    "Konfiguracja bezpieczeństwa: Jwt:Key ustawiony={JwtKeySet}, appSettings:ApiKey ustawiony={ApiKeySet}, InternalSso:Key ustawiony={InternalSsoKeySet}.",
    !string.IsNullOrWhiteSpace(configuration["Jwt:Key"]),
    !string.IsNullOrWhiteSpace(configuration["appSettings:ApiKey"]),
    !string.IsNullOrWhiteSpace(configuration["InternalSso:Key"]));
startupLogger.LogInformation(
    "Konfiguracja Microsoft Graph: TenantId ustawiony={TenantSet}, ClientId ustawiony={ClientSet}, ClientSecret ustawiony={SecretSet}.",
    !string.IsNullOrWhiteSpace(configuration["MicrosoftGraph:TenantId"]),
    !string.IsNullOrWhiteSpace(configuration["MicrosoftGraph:ClientId"]),
    !string.IsNullOrWhiteSpace(configuration["MicrosoftGraph:ClientSecret"]));

if (missingMonitoringEnvVars.Count > 0)
{
    startupLogger.LogError(
        "Zmienne środowiskowe nieustawione, użyto wartości z appsettings.json: {MissingEnvVars}.",
        string.Join(", ", missingMonitoringEnvVars));
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
