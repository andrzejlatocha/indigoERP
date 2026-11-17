using System.Reflection;
using System.Security.Claims;
using indigoERP.Api.Controllers._Wspolne;
using indigoERP.Api.Data;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Entities._Wspolne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace indigoERP.Api.Tests;

public class PowiadomienieControllerTests
{
    private const string RolaZapisu = "HCZ-indigoERP-Administrator,HCZ-indigoERP-Integracja-epicaHR";

    [Fact]
    public async Task Insert_ZapisujePowiadomienieNieprzeczytane()
    {
        using var context = ApiTestContextFactory.Create(userId: 101);
        var controller = new PowiadomienieController(context);
        var dto = new PowiadomienieDto
        {
            powAplikacjaID = 132,
            powUzytkownikID = 7,
            powKodRodzaj = "TEST",
            powTytul = "Tytuł testowy",
            powTresc = "Treść testowa",
            powLink = "/podglad/wiadomosc"
        };

        var wynik = await controller.Insert(dto);

        var ok = Assert.IsType<OkObjectResult>(wynik.Result);
        var id = Assert.IsType<int>(ok.Value);
        Assert.True(id > 0);

        var zapisane = await context.Powiadomienie.AsNoTracking().SingleAsync(w => w.powID == id);
        Assert.Equal(132, zapisane.powAplikacjaID);
        Assert.Equal(7, zapisane.powUzytkownikID);
        Assert.Equal("Tytuł testowy", zapisane.powTytul);
        Assert.Equal("/podglad/wiadomosc", zapisane.powLink);
        Assert.False(zapisane.powOdczyt);
        Assert.Null(zapisane.powDataOdczytu);
        Assert.True(zapisane.powDataPowiadomienia > DateTime.Now.AddMinutes(-1));
        Assert.Equal(101, zapisane.UtworzenieID);
    }

    [Fact]
    public void Insert_WymagaRoliAdministratoraLubIntegracjiEpicaHR()
    {
        var metoda = typeof(PowiadomienieController).GetMethod(nameof(PowiadomienieController.Insert));
        Assert.NotNull(metoda);

        var atrybut = metoda!.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(atrybut);
        Assert.Equal(RolaZapisu, atrybut!.Roles);
        Assert.Equal("Bearer", atrybut.AuthenticationSchemes);
    }

    [Fact]
    public void Insert_NieMaAtrybutuAnonimowego()
    {
        var metoda = typeof(PowiadomienieController).GetMethod(nameof(PowiadomienieController.Insert));
        Assert.NotNull(metoda);
        Assert.Null(metoda!.GetCustomAttribute<AllowAnonymousAttribute>());
    }

    [Fact]
    public async Task Odczytaj_OznaczaWlasnePowiadomienie()
    {
        using var context = ApiTestContextFactory.Create(userId: 101);
        var powID = await DodajAsync(context, uzytkownikID: 7, tytul: "A");
        var controller = Kontroler(context, usrID: 7);

        var wynik = await controller.Odczytaj(powID);

        Assert.IsType<NoContentResult>(wynik);
        var zapisane = await context.Powiadomienie.AsNoTracking().SingleAsync(w => w.powID == powID);
        Assert.True(zapisane.powOdczyt);
        Assert.NotNull(zapisane.powDataOdczytu);
    }

    [Fact]
    public async Task Odczytaj_CudzePowiadomienieJestZabronione()
    {
        using var context = ApiTestContextFactory.Create(userId: 101);
        var powID = await DodajAsync(context, uzytkownikID: 7, tytul: "A");
        var controller = Kontroler(context, usrID: 8);

        var wynik = await controller.Odczytaj(powID);

        Assert.IsType<ForbidResult>(wynik);
        var zapisane = await context.Powiadomienie.AsNoTracking().SingleAsync(w => w.powID == powID);
        Assert.False(zapisane.powOdczyt);
    }

    [Fact]
    public async Task OdczytajWszystkie_OznaczaTylkoNieprzeczytaneZalogowanego()
    {
        using var context = ApiTestContextFactory.Create(userId: 101);
        await DodajAsync(context, uzytkownikID: 7, tytul: "A");
        await DodajAsync(context, uzytkownikID: 7, tytul: "B");
        await DodajAsync(context, uzytkownikID: 7, tytul: "C", odczytane: true);
        var obcyPowID = await DodajAsync(context, uzytkownikID: 8, tytul: "D");
        var controller = Kontroler(context, usrID: 7);

        var wynik = await controller.OdczytajWszystkie(appID: 132, usrID: 7);

        Assert.Equal(2, Assert.IsType<int>(wynik.Value));
        Assert.Equal(0, await context.Powiadomienie.CountAsync(w => w.powUzytkownikID == 7 && !w.powOdczyt));
        Assert.False((await context.Powiadomienie.AsNoTracking().SingleAsync(w => w.powID == obcyPowID)).powOdczyt);
    }

    [Fact]
    public async Task OdczytajWszystkie_InnegoUzytkownikaJestZabronione()
    {
        using var context = ApiTestContextFactory.Create(userId: 101);
        await DodajAsync(context, uzytkownikID: 7, tytul: "A");
        var controller = Kontroler(context, usrID: 8);

        var wynik = await controller.OdczytajWszystkie(appID: 132, usrID: 7);

        Assert.IsType<ForbidResult>(wynik.Result);
        Assert.Equal(1, await context.Powiadomienie.CountAsync(w => w.powUzytkownikID == 7 && !w.powOdczyt));
    }

    private static PowiadomienieController Kontroler(indigoERPContext context, int usrID)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("usrID", usrID.ToString())], "Test"));
        return new PowiadomienieController(context)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } }
        };
    }

    private static async Task<int> DodajAsync(indigoERPContext context, int uzytkownikID, string tytul, bool odczytane = false, int dniWstecz = 0)
    {
        var powiadomienie = new Powiadomienie
        {
            powAplikacjaID = 132,
            powUzytkownikID = uzytkownikID,
            powKodRodzaj = "TEST",
            powTytul = tytul,
            powDataPowiadomienia = DateTime.Now.AddDays(-dniWstecz),
            powOdczyt = odczytane,
            powDataOdczytu = odczytane ? DateTime.Now : null
        };
        context.Powiadomienie.Add(powiadomienie);
        await context.SaveChangesAsync();
        return powiadomienie.powID;
    }
}
