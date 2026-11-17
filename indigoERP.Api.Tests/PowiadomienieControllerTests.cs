using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using indigoERP.Api.Controllers._Wspolne;
using indigoERP.Api.Data;
using indigoERP.Domain.Common;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Entities._Administrator;
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
        await SeedAplikacjiIUzytkownikaAsync(context);
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
    public async Task Insert_OdrzucaNieznanaAplikacje()
    {
        using var context = ApiTestContextFactory.Create(userId: 101);
        await SeedAplikacjiIUzytkownikaAsync(context);
        var controller = new PowiadomienieController(context);

        var wynik = await controller.Insert(DtoTestowe(powAplikacjaID: 999));

        Assert.IsType<BadRequestObjectResult>(wynik.Result);
        Assert.Equal(0, await context.Powiadomienie.CountAsync());
    }

    [Fact]
    public async Task Insert_OdrzucaNieznanegoUzytkownika()
    {
        using var context = ApiTestContextFactory.Create(userId: 101);
        await SeedAplikacjiIUzytkownikaAsync(context);
        var controller = new PowiadomienieController(context);

        var wynik = await controller.Insert(DtoTestowe(powUzytkownikID: 999));

        Assert.IsType<BadRequestObjectResult>(wynik.Result);
        Assert.Equal(0, await context.Powiadomienie.CountAsync());
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

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dto_OdrzucaZerowaIdentyfikatoryUzytkownikaIAplikacji(int wartosc)
    {
        var dto = DtoTestowe(powAplikacjaID: wartosc, powUzytkownikID: wartosc);

        var bledy = WalidujDto(dto);

        Assert.Contains(bledy, b => b.MemberNames.Contains(nameof(PowiadomienieDto.powAplikacjaID)));
        Assert.Contains(bledy, b => b.MemberNames.Contains(nameof(PowiadomienieDto.powUzytkownikID)));
    }

    // Sprawdza tylko, że atrybut [DozwolonyLink] jest podpięty do DTO. Pełne reguły są w testach BezpieczneLinki_*.
    [Fact]
    public void Dto_podpina_walidacje_linku_i_odrzuca_javascript()
    {
        var bledy = WalidujDto(DtoTestowe(powLink: "javascript:alert(1)"));

        Assert.Contains(bledy, b => b.MemberNames.Contains(nameof(PowiadomienieDto.powLink)));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("/podglad/zadanie", true)]
    [InlineData("http://localhost:8240/x", true)]
    [InlineData("https://app.hcz.com.pl/x?a=1", true)]
    [InlineData("javascript:alert(1)", false)]
    [InlineData("//evil.example.com", false)]
    [InlineData("data:text/plain,abc", false)]
    [InlineData("mailto:ktos@example.com", false)]
    public void BezpieczneLinki_RozpoznajeDozwoloneAdresy(string? link, bool oczekiwany)
    {
        Assert.Equal(oczekiwany, BezpieczneLinki.Dozwolony(link));
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

    // Brak testu "OdczytajWszystkie_OznaczaTylkoNieprzeczytaneZalogowanego": ExecuteUpdate nie działa w EF InMemory.
    // Zachowanie masowej aktualizacji trzeba sprawdzić na bazie testowej (luka opisana w Changes.md).
    [Fact]
    public async Task Odczytaj_nieistniejacePowiadomienie_zwraca_NotFound()
    {
        using var context = ApiTestContextFactory.Create(userId: 101);
        var controller = Kontroler(context, usrID: 7);

        var wynik = await controller.Odczytaj(999);

        Assert.IsType<NotFoundResult>(wynik);
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

    private static PowiadomienieDto DtoTestowe(
        int powAplikacjaID = 132,
        int powUzytkownikID = 7,
        string? powLink = "/podglad/wiadomosc") => new()
    {
        powAplikacjaID = powAplikacjaID,
        powUzytkownikID = powUzytkownikID,
        powKodRodzaj = "TEST",
        powTytul = "Tytuł testowy",
        powTresc = "Treść testowa",
        powLink = powLink
    };

    private static List<ValidationResult> WalidujDto(PowiadomienieDto dto)
    {
        var bledy = new List<ValidationResult>();
        Validator.TryValidateObject(dto, new ValidationContext(dto), bledy, validateAllProperties: true);
        return bledy;
    }

    private static async Task SeedAplikacjiIUzytkownikaAsync(indigoERPContext context)
    {
        context.Aplikacja.Add(new Aplikacja { appID = 132 });
        context.Uzytkownik.Add(new Uzytkownik { usrID = 7 });
        await context.SaveChangesAsync();
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
