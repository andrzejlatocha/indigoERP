using indigoERP.Worker.Services;
using Microsoft.Extensions.Options;

namespace indigoERP.Worker;

public sealed class WiadomoscWorker : BackgroundService
{
    private readonly WiadomoscKolejkaService _kolejkaService;
    private readonly IOptions<WorkerOptions> _options;
    private readonly ILogger<WiadomoscWorker> _logger;

    public WiadomoscWorker(
        WiadomoscKolejkaService kolejkaService,
        IOptions<WorkerOptions> options,
        ILogger<WiadomoscWorker> logger)
    {
        _kolejkaService = kolejkaService;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Uruchomiono kolejkę wiadomości indigoERP.Worker.");

        var nastepneSprawdzenieOdpowiedzi = DateTimeOffset.MinValue;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _kolejkaService.ProcessAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Nie udało się przetworzyć kolejki wiadomości.");
            }

            if (DateTimeOffset.UtcNow >= nastepneSprawdzenieOdpowiedzi)
            {
                try
                {
                    await _kolejkaService.OznaczNiedostarczoneAsync(stoppingToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Nie udało się sprawdzić odpowiedzi o niedostarczeniu.");
                }

                nastepneSprawdzenieOdpowiedzi = DateTimeOffset.UtcNow.AddMinutes(15);
            }

            var seconds = Math.Max(5, _options.Value.PollingIntervalSeconds);
            await Task.Delay(TimeSpan.FromSeconds(seconds), stoppingToken);
        }
    }
}
