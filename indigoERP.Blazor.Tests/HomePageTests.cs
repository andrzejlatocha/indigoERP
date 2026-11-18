using System.Security.Claims;
using Bunit;
using indigoERP.Domain.ViewModels;
using indigoERP.Service.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace indigoERP.Blazor.Tests;

public sealed class HomePageTests : BunitContext
{
    public HomePageTests()
    {
        Services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor());
        Services.AddSingleton<AuthenticationStateProvider>(new TestAuthenticationStateProvider([new Claim("usrID", "77")]));
        Services.AddSingleton<IHomeService>(new HomeServiceStub());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Home_renders_shortcuts_and_applications_returned_for_the_current_user()
    {
        var cut = Render<Pages.Home>();

        cut.WaitForAssertion(() => Assert.Contains("home-loading", cut.Markup));
        WaitForHomeContent(cut);
        Assert.DoesNotContain("Stary Intranet", cut.Markup);
        Assert.Contains("Formularz zgłoszenia", cut.Markup);
        Assert.Contains("Testowa aplikacja", cut.Markup);
        Assert.Contains("https://test.local/aplikacja", cut.Markup);
    }

    [Fact]
    public void Home_renders_legacy_application_buttons_with_their_own_addresses()
    {
        var cut = Render<Pages.Home>();

        WaitForHomeContent(cut);
        var buttons = cut.FindAll("a.legacy-button");

        Assert.Equal(8, buttons.Count);
        Assert.All(buttons, button => Assert.Contains("btn-warning", button.ClassName));
        Assert.Contains(buttons, b => b.TextContent.Contains("hydraRCP") && b.GetAttribute("href") == "http://wenus.hcz.com.pl/hydra/");
        Assert.Contains(buttons, b => b.TextContent.Contains("cerberTMS") && b.GetAttribute("href") == "https://intranet.hcz.com.pl/cerber/");
        Assert.Equal(8, buttons.Select(b => b.GetAttribute("href")).Distinct().Count());
    }

    private static void WaitForHomeContent(IRenderedComponent<Pages.Home> cut)
    {
        cut.WaitForAssertion(() => Assert.Contains("IT Support", cut.Markup), TimeSpan.FromSeconds(4));
    }

    private sealed class HomeServiceStub : IHomeService
    {
        public Task<IEnumerable<AplikacjaViewModel>> SelectAplikacja(string appStatus) => Task.FromResult<IEnumerable<AplikacjaViewModel>>([]);

        public Task<IEnumerable<AplikacjaViewModel>> SelectAplikacjaByUzytkownik(int usrID, string appStatus)
        {
            Assert.Equal(77, usrID);
            Assert.Equal("2", appStatus);

            return Task.FromResult<IEnumerable<AplikacjaViewModel>>(
            [
                new AplikacjaViewModel
                {
                    appID = 132,
                    appNazwa = "Testowa aplikacja",
                    appLink = "https://test.local/aplikacja",
                    appIkona = "images/test.png",
                    appStatus = "2"
                }
            ]);
        }
    }
}
