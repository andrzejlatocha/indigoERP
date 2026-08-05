using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Administrator;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Services
{
    public class ServerMonitoringHostedService : BackgroundService
    {
        private const int MaxParallelChecks = 8;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServerCheckService _checkService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServerMonitoringHostedService> _logger;

        public ServerMonitoringHostedService(
            IServiceScopeFactory scopeFactory,
            IServerCheckService checkService,
            IConfiguration configuration,
            ILogger<ServerMonitoringHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _checkService = checkService;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var intervalMinutes = _configuration.GetSection("appSettings").GetValue<double?>("MonitoringIntervalMinutes") ?? 5;

            _logger.LogInformation("ServerMonitoringHostedService uruchomiony, interwał={IntervalMinutes} min.", intervalMinutes);

            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(intervalMinutes));

            do
            {
                try
                {
                    await RunCheckCycleAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Błąd podczas cyklu sprawdzania serwerów.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task RunCheckCycleAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<indigoERPContext>();

            var serwery = await context.Serwer
                .Where(w => w.srvStatus == "2" && (w.Usuniety == false || w.Usuniety == null))
                .ToListAsync(stoppingToken);

            if (serwery.Count == 0)
            {
                return;
            }

            using var semaphore = new SemaphoreSlim(MaxParallelChecks);

            var logi = await Task.WhenAll(serwery.Select(async serwer =>
            {
                await semaphore.WaitAsync(stoppingToken);

                try
                {
                    var (isAlive, responseMs, error) = await _checkService.CheckAsync(serwer.srvSerwerTypKOD, serwer.srvAdres, serwer.srvPort, stoppingToken);

                    return new SerwerLog
                    {
                        slgSerwerID = serwer.srvID,
                        slgDataSprawdzenie = DateTime.Now,
                        slgDostepny = isAlive,
                        slgCzasOdpowiedzMs = responseMs,
                        slgBlad = error
                    };
                }
                finally
                {
                    semaphore.Release();
                }
            }));

            context.SerwerLog.AddRange(logi);
            await context.SaveChangesAsync(stoppingToken);
        }
    }
}
