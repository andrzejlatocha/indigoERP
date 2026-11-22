using indigoERP.Api.Realtime;
using Microsoft.AspNetCore.Authorization;

namespace indigoERP.Api.Tests;

public sealed class PowiadomienieHubTests
{
    [Fact]
    public void HubWymagaTokenuBearer()
    {
        var attribute = Assert.Single(typeof(PowiadomienieHub).GetCustomAttributes(typeof(AuthorizeAttribute), true));
        Assert.Equal("Bearer", ((AuthorizeAttribute)attribute).AuthenticationSchemes);
    }

    [Fact]
    public void SubskrypcjeSaIzolowaneWedlugAplikacjiIUzytkownika()
    {
        var subscriptions = new PowiadomienieSubskrypcje();
        subscriptions.Add("first", 360, 5);
        subscriptions.Add("second", 360, 5);
        subscriptions.Add("third", 361, 5);
        Assert.Equal(2, subscriptions.Snapshot().Length);
        Assert.NotEqual(PowiadomienieSubskrypcje.Group(360, 5), PowiadomienieSubskrypcje.Group(360, 6));
        Assert.NotEqual(PowiadomienieSubskrypcje.Group(360, 5), PowiadomienieSubskrypcje.Group(361, 5));
        subscriptions.Remove("first");
        Assert.Equal(2, subscriptions.Snapshot().Length);
        subscriptions.Remove("second");
        Assert.Single(subscriptions.Snapshot());
    }
}
