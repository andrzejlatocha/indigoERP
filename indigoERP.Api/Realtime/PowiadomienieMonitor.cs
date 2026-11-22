using indigoERP.Api.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Realtime;

// Obejmuje również wiadomości zapisywane przez indigoERP.Worker i inne aplikacje.
public sealed class PowiadomienieMonitor(IServiceScopeFactory scopes, PowiadomienieSubskrypcje subscriptions,
    IHubContext<PowiadomienieHub> hub, ILogger<PowiadomienieMonitor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var previous = new Dictionary<(int AppID, int UserID), (int Notifications, int Messages, int Tasks, int Events)>();
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var active = subscriptions.Snapshot();
            foreach (var key in previous.Keys.Except(active).ToArray()) previous.Remove(key);
            foreach (var key in active)
            {
                try
                {
                    using var scope = scopes.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<indigoERPContext>();
                    var counts = (
                        await db.PowiadomienieViewModel.CountAsync(x => x.powAplikacjaID == key.AppID
                            && x.powUzytkownikID == key.UserID && !x.powOdczyt, stoppingToken),
                        await db.WiadomoscUzytkownikViewModel.CountAsync(x => x.wiaAplikacjaID == key.AppID
                            && x.wuzUzytkownikID == key.UserID && x.wuzOdczyt == false, stoppingToken),
                        await db.ZadanieViewModel.CountAsync(x => x.tskAplikacjaID == key.AppID
                            && x.tusUzytkownikID == key.UserID && x.tusOdczyt == false, stoppingToken),
                        await db.ZdarzenieViewModel.CountAsync(x => x.zdaAplikacjaID == key.AppID
                            && x.zurUzytkownikID == key.UserID && x.zurOdczyt == false, stoppingToken));
                    if (!previous.TryGetValue(key, out var before) || before != counts)
                    {
                        await hub.Clients.Group(PowiadomienieSubskrypcje.Group(key.AppID, key.UserID))
                            .SendAsync("PowiadomieniaZmienione", stoppingToken);
                        previous[key] = counts;
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
                catch (Exception ex) { logger.LogWarning(ex, "Nie udało się sprawdzić zmian powiadomień."); }
            }
        }
    }
}
