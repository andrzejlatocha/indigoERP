using indigoERP.Worker;
using indigoERP.Worker.Data;
using indigoERP.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
var retentionDays = Math.Max(1, builder.Configuration.GetValue<int?>("FileLogging:RetentionDays") ?? 30);
var fileLogLevel = Enum.TryParse<LogLevel>(builder.Configuration["FileLogging:LogLevel"], true, out var configuredLogLevel)
    ? configuredLogLevel
    : LogLevel.Information;
builder.Logging.AddProvider(new DailyFileLoggerProvider(logDirectory, fileLogLevel, retentionDays));

var environmentOverrides = new Dictionary<string, string?>();
MapEnvironmentVariable("MicrosoftGraph_TenantId", "MicrosoftGraph:TenantId");
MapEnvironmentVariable("MicrosoftGraph_ClientId", "MicrosoftGraph:ClientId");
MapEnvironmentVariable("MicrosoftGraph_ClientSecret", "MicrosoftGraph:ClientSecret");
MapEnvironmentVariable("indigoERP_FileStorage_Path", "FileStorage:Path");

if (environmentOverrides.Count > 0)
{
    builder.Configuration.AddInMemoryCollection(environmentOverrides);
}

builder.AddServiceDefaults();
builder.Services.AddWindowsService(options => options.ServiceName = "indigoERP.Worker");
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.SectionName));
builder.Services.AddSingleton<WiadomoscWorkerContextFactory>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IMicrosoftGraphMailService, MicrosoftGraphMailService>();
builder.Services.AddSingleton<WiadomoscKolejkaService>();
builder.Services.AddHostedService<WiadomoscWorker>();

await builder.Build().RunAsync();

void MapEnvironmentVariable(string environmentVariableName, string configurationKey)
{
    var value = Environment.GetEnvironmentVariable(environmentVariableName);

    if (!string.IsNullOrWhiteSpace(value))
    {
        environmentOverrides[configurationKey] = value;
    }
}
