using indigoERP.Api.Controllers._Wspolne;
using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Wspolne;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace indigoERP.Api.Tests;

public sealed class WiadomoscHarmonogramControllerTests
{
    private const int Aplikacja = 900;
    private static readonly DateTime Termin = new(2027, 3, 11, 8, 0, 0);

    private static Wiadomosc Szablon(int id, string status, bool aktywny, DateTime? nastepna,
        int aplikacja = Aplikacja, string? email = WiadomoscPracownikController.NadawcaWiadomosciPublicznej) => new()
    {
        wiaID = id,
        wiaAplikacjaID = aplikacja,
        wiaEmail = email,
        wiaTemat = "Temat " + id,
        wiaTresc = "<p>Treść</p>",
        wiaStatus = status,
        wiaHarmonogramAktywny = aktywny,
        wiaDataNastepnejWysylki = nastepna,
    };

    private static WiadomoscHarmonogramController Kontroler(indigoERPContext context) => new(context);

    [Fact]
    public async Task Anuluj_zatrzymuje_cykl_harmonogramu_i_zeruje_nastepny_termin()
    {
        await using var context = ApiTestContextFactory.Create();
        context.Wiadomosc.Add(Szablon(1, "7", aktywny: true, nastepna: Termin));
        await context.SaveChangesAsync();

        var wynik = await Kontroler(context).Anuluj(1, CancellationToken.None);

        Assert.IsType<NoContentResult>(wynik);
        var szablon = await context.Wiadomosc.SingleAsync(w => w.wiaID == 1);
        Assert.Equal("6", szablon.wiaStatus);
        Assert.False(szablon.wiaHarmonogramAktywny);
        Assert.Null(szablon.wiaDataNastepnejWysylki);
    }

    [Fact]
    public async Task Anuluj_zatrzymuje_wysylke_juz_skopiowana_do_kolejki()
    {
        await using var context = ApiTestContextFactory.Create();
        context.Wiadomosc.Add(Szablon(1, "2", aktywny: false, nastepna: null));
        await context.SaveChangesAsync();

        var wynik = await Kontroler(context).Anuluj(1, CancellationToken.None);

        Assert.IsType<NoContentResult>(wynik);
        Assert.Equal("6", (await context.Wiadomosc.SingleAsync(w => w.wiaID == 1)).wiaStatus);
    }

    [Theory]
    [InlineData("4")]
    [InlineData("6")]
    [InlineData("8")]
    public async Task Anuluj_odrzuca_wysylke_zakonczona_albo_juz_anulowana(string status)
    {
        await using var context = ApiTestContextFactory.Create();
        context.Wiadomosc.Add(Szablon(1, status, aktywny: false, nastepna: null));
        await context.SaveChangesAsync();

        var wynik = await Kontroler(context).Anuluj(1, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(wynik);
        Assert.Equal(status, (await context.Wiadomosc.SingleAsync(w => w.wiaID == 1)).wiaStatus);
    }

    [Fact]
    public async Task Anuluj_zwraca_404_dla_nieistniejacej_wiadomosci()
    {
        await using var context = ApiTestContextFactory.Create();

        var wynik = await Kontroler(context).Anuluj(404, CancellationToken.None);

        Assert.IsType<NotFoundResult>(wynik);
    }

    [Fact]
    public async Task Anuluj_nie_zwraca_usunietej_wiadomosci()
    {
        await using var context = ApiTestContextFactory.Create();
        var szablon = Szablon(1, "7", aktywny: true, nastepna: Termin);
        szablon.Usuniety = true;
        context.Wiadomosc.Add(szablon);
        await context.SaveChangesAsync();

        var wynik = await Kontroler(context).Anuluj(1, CancellationToken.None);

        Assert.IsType<NotFoundResult>(wynik);
    }

    [Fact]
    public async Task SelectAktywne_pokazuje_aktywne_cykle_i_kolejke_tylko_dla_nadawcy_i_aplikacji()
    {
        await using var context = ApiTestContextFactory.Create();
        context.Wiadomosc.AddRange(
            Szablon(1, "7", aktywny: true, nastepna: Termin.AddDays(7)),
            Szablon(2, "2", aktywny: false, nastepna: Termin),
            Szablon(3, "7", aktywny: false, nastepna: null),
            Szablon(4, "7", aktywny: true, nastepna: Termin, aplikacja: 123),
            Szablon(5, "7", aktywny: true, nastepna: Termin, email: "inny@huta-czestochowa.pl"),
            Szablon(6, "4", aktywny: false, nastepna: null));
        await context.SaveChangesAsync();

        var wynik = await Kontroler(context).SelectAktywne(Aplikacja, CancellationToken.None);

        var lista = wynik.Value!.ToList();
        Assert.Equal(new[] { 2, 1 }, lista.Select(w => w.wiaID));
        Assert.Equal("7", lista.Single(w => w.wiaID == 1).wiaStatus);
    }
}
