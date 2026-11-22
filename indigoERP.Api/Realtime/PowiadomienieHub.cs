using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace indigoERP.Api.Realtime;

public sealed class PowiadomienieSubskrypcje
{
    private readonly ConcurrentDictionary<string, (int AppID, int UserID)> connections = new();
    public void Add(string connection, int appID, int userID) => connections[connection] = (appID, userID);
    public void Remove(string connection) => connections.TryRemove(connection, out _);
    public (int AppID, int UserID)[] Snapshot() => connections.Values.Distinct().ToArray();
    public static string Group(int appID, int userID) => $"powiadomienie:{appID}:{userID}";
}

[Authorize(AuthenticationSchemes = "Bearer")]
public sealed class PowiadomienieHub(PowiadomienieSubskrypcje subscriptions) : Hub
{
    public async Task Subscribe(int appID)
    {
        if (appID <= 0 || !int.TryParse(Context.User?.FindFirst("usrID")?.Value, out int userID) || userID <= 0)
            throw new HubException("Brak identyfikatora użytkownika lub aplikacji.");
        if (Context.Items.TryGetValue("group", out var oldGroup) && oldGroup is string group)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        Context.Items["group"] = PowiadomienieSubskrypcje.Group(appID, userID);
        subscriptions.Add(Context.ConnectionId, appID, userID);
        await Groups.AddToGroupAsync(Context.ConnectionId, PowiadomienieSubskrypcje.Group(appID, userID));
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        subscriptions.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
