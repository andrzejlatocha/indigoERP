using indigoERP.Worker.Services;

namespace indigoERP.Worker.Tests;

public class BledyWysylkiTests
{
    [Fact]
    public void Przejsciowy_Timeout_IsTransient()
    {
        Assert.True(BledyWysylki.Przejsciowy(new TimeoutException()));
        Assert.True(BledyWysylki.Przejsciowy(new TaskCanceledException()));
    }

    [Fact]
    public void Przejsciowy_GraphThrottling_IsTransient()
    {
        var wyjatek = new InvalidOperationException("Microsoft Graph: wysłanie wiadomości nie powiodło się (429): Too many requests");

        Assert.True(BledyWysylki.Przejsciowy(wyjatek));
    }

    [Fact]
    public void Przejsciowy_GraphServiceUnavailable_IsTransient()
    {
        var wyjatek = new InvalidOperationException("Microsoft Graph: wysłanie wiadomości nie powiodło się (503)");

        Assert.True(BledyWysylki.Przejsciowy(wyjatek));
    }

    [Fact]
    public void Przejsciowy_InvalidSender_IsPermanent()
    {
        var wyjatek = new InvalidOperationException("Microsoft Graph: utworzenie wiadomości roboczej nie powiodło się (404): ErrorInvalidUser");

        Assert.False(BledyWysylki.Przejsciowy(wyjatek));
    }

    [Fact]
    public void Przejsciowy_TransientInnerException_IsTransient()
    {
        var wyjatek = new InvalidOperationException("opakowany", new TimeoutException());

        Assert.True(BledyWysylki.Przejsciowy(wyjatek));
    }
}
