using System.Security.Claims;
using Bunit;
using indigoERP.Service.Interfaces._Wspolne;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace indigoERP.Blazor.Tests;

public sealed class PowiadomienieStronaTests : BunitContext
{
    private readonly PodgladServiceStub stub = new();

    public PowiadomienieStronaTests()
    {
        Services.AddLogging();
        Services.AddScoped<DialogService>();
        Services.AddScoped<NotificationService>();
        Services.AddScoped<ContextMenuService>();
        Services.AddScoped<TooltipService>();
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["appSettings:AppID"] = "132" })
            .Build());
        Services.AddSingleton<AuthenticationStateProvider>(new TestAuthenticationStateProvider([new Claim("usrID", "7")]));
        Services.AddSingleton<IPodgladService>(stub);
    }

    [Fact]
    public void Strona_renderuje_link_dla_adresu_aplikacji()
    {
        stub.Powiadomienia = [PodgladServiceStub.Powiadomienie(1, "Test", link: "/podglad/wiadomosc")];

        var cut = Render<Pages.Powiadomienie.PowiadomienieStrona>();

        cut.WaitForAssertion(() => Assert.Contains("href=\"/podglad/wiadomosc\"", cut.Markup));
    }

    [Fact]
    public void Strona_nie_renderuje_linku_javascript_tylko_tekst()
    {
        stub.Powiadomienia = [PodgladServiceStub.Powiadomienie(1, "Niebezpieczny", link: "javascript:alert(1)")];

        var cut = Render<Pages.Powiadomienie.PowiadomienieStrona>();

        cut.WaitForAssertion(() => Assert.Contains("Niebezpieczny", cut.Markup));
        Assert.DoesNotContain("javascript:", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<a", cut.Markup);
    }

    [Fact]
    public void Strona_oznacza_wszystkie_i_przeladowuje_liste_po_sukcesie()
    {
        stub.Powiadomienia = [PodgladServiceStub.Powiadomienie(1, "Nieprzeczytane")];
        stub.WynikOznaczWszystkich = 1;
        var cut = Render<Pages.Powiadomienie.PowiadomienieStrona>();
        cut.WaitForAssertion(() => Assert.Contains("Nieprzeczytane", cut.Markup));

        ClickButton(cut, "Oznacz wszystkie jako przeczytane");

        cut.WaitForAssertion(() => Assert.Equal(2, stub.PobraniaListy));
    }

    [Fact]
    public void Strona_nie_przeladowuje_listy_gdy_oznaczenie_wszystkich_sie_nie_udalo()
    {
        stub.Powiadomienia = [PodgladServiceStub.Powiadomienie(1, "Nieprzeczytane")];
        stub.WynikOznaczWszystkich = null;
        var cut = Render<Pages.Powiadomienie.PowiadomienieStrona>();
        cut.WaitForAssertion(() => Assert.Contains("Nieprzeczytane", cut.Markup));

        ClickButton(cut, "Oznacz wszystkie jako przeczytane");

        // Po błędzie lista nie jest pobierana ponownie: nadal tylko pierwsze wczytanie.
        cut.WaitForAssertion(() => Assert.Equal(1, stub.PobraniaListy));
    }

    [Fact]
    public void Okno_powiadomien_pokazuje_przycisk_Zobacz_wszystkie()
    {
        stub.Powiadomienia = [PodgladServiceStub.Powiadomienie(1, "Test w oknie")];

        var cut = Render<Pages.Podglad.PodgladLista>(parameters => parameters.Add(p => p.Typ, "powiadomienia"));

        cut.WaitForAssertion(() => Assert.Contains("Test w oknie", cut.Markup));
        Assert.Contains("Zobacz wszystkie", cut.Markup);
    }

    private static void ClickButton<T>(IRenderedComponent<T> cut, string tekst) where T : IComponent
    {
        cut.FindAll("button").First(b => b.TextContent.Contains(tekst, StringComparison.Ordinal)).Click();
    }
}
