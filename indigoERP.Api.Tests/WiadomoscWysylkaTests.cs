using System.Reflection;
using System.Security.Claims;
using indigoERP.Api.Controllers._Wspolne;
using indigoERP.Api.Data;
using indigoERP.Api.Services;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Entities._Wspolne;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Tests;

public sealed class WiadomoscWysylkaTests
{
    private static WiadomoscZakresPracownikViewModel Odbiorca(int prcID, string nazwisko, string? email) => new()
    {
        prcID = prcID,
        prcNazwisko = nazwisko,
        prcImie = "Jan",
        prcKod = "K" + prcID,
        kntID = prcID,
        kntKontakt = email
    };

    private static WiadomoscPracownikController Kontroler(
        indigoERPContext context, IWiadomoscZakresPracownikSource zakresy, IWiadomoscZalacznikStorage? magazyn = null)
    {
        var http = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("usrID", "101")], "Test"))
        };

        return new WiadomoscPracownikController(context, zakresy, magazyn ?? new WiadomoscZalacznikStorage(string.Empty))
        {
            ControllerContext = new ControllerContext { HttpContext = http }
        };
    }

    private static WiadomoscWysylkaZakresModel Model(params int[] prcIDs) => new()
    {
        wiaAplikacjaID = 900,
        wiaTemat = "Temat",
        wiaTresc = "<p>Treść</p>",
        wzaID = 1,
        prcIDs = prcIDs.ToList()
    };

    [Fact]
    public async Task Wysylka_sends_only_to_zakres_members_with_email_and_reports_the_rest()
    {
        await using var context = ApiTestContextFactory.Create();
        var zakres = new FakeZakres(
            Odbiorca(1, "Kowalski", "a@huta-czestochowa.pl"),
            Odbiorca(2, "Nowak", null),
            Odbiorca(3, "Zieliński", "c@huta-czestochowa.pl"));
        var kontroler = Kontroler(context, zakres);

        var result = await kontroler.InsertWysylka(Model(1, 2, 3, 99), null, CancellationToken.None);

        var wynik = Assert.IsType<WiadomoscWysylkaZakresWynikModel>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal(2, wynik.dodano);
        Assert.Equal(2, wynik.pominieto);
        Assert.Contains(wynik.komunikaty, k => k.Contains("spoza"));
        Assert.Contains(wynik.komunikaty, k => k.Contains("Nowak") && k.Contains("e-mail"));

        var wiadomosc = await context.Wiadomosc.SingleAsync();
        Assert.Equal(wiadomosc.wiaID, wynik.wiaID);
        Assert.Equal(900, wiadomosc.wiaAplikacjaID);
        Assert.Equal(101, wiadomosc.wiaUzytkownikID);
        Assert.Equal("Temat", wiadomosc.wiaTemat);

        var odbiorcy = await context.WiadomoscPracownik.OrderBy(w => w.wprPracownikID).ToListAsync();
        Assert.Equal(new int?[] { 1, 3 }, odbiorcy.Select(o => o.wprPracownikID));
        Assert.All(odbiorcy, o => Assert.Equal(wiadomosc.wiaID, o.wprWiadomoscID));
        Assert.Equal("a@huta-czestochowa.pl", odbiorcy[0].wprEmail);
    }

    [Fact]
    public async Task Wysylka_rejects_an_empty_recipient_list()
    {
        await using var context = ApiTestContextFactory.Create();
        var kontroler = Kontroler(context, new FakeZakres(Odbiorca(1, "Kowalski", "a@huta-czestochowa.pl")));

        var result = await kontroler.InsertWysylka(Model(), null, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(0, await context.Wiadomosc.CountAsync());
    }

    [Fact]
    public async Task Wysylka_rejects_an_unknown_zakres()
    {
        await using var context = ApiTestContextFactory.Create();
        var kontroler = Kontroler(context, new FakeZakres(null));

        var result = await kontroler.InsertWysylka(Model(1), null, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(0, await context.Wiadomosc.CountAsync());
    }

    [Fact]
    public async Task Wysylka_rejects_when_no_selected_recipient_has_an_email()
    {
        await using var context = ApiTestContextFactory.Create();
        var kontroler = Kontroler(context, new FakeZakres(Odbiorca(1, "Kowalski", null)));

        var result = await kontroler.InsertWysylka(Model(1), null, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(0, await context.WiadomoscPracownik.CountAsync());
    }

    [Fact]
    public void Message_endpoints_are_limited_to_gazeta_and_administrator_roles()
    {
        var expected = "HCZ-indigoERP-Gazeta,HCZ-indigoERP-Administrator";

        foreach (var typ in new[] { typeof(WiadomoscPracownikController), typeof(WiadomoscZakresController) })
        {
            var atrybut = Assert.IsType<AuthorizeAttribute>(typ.GetCustomAttribute<AuthorizeAttribute>());
            Assert.Equal(expected, atrybut.Roles);
        }
    }

    [Fact]
    public async Task Wysylka_rejects_a_file_that_is_not_a_pdf_and_creates_nothing()
    {
        await using var context = ApiTestContextFactory.Create();
        var kontroler = Kontroler(context, new FakeZakres(Odbiorca(1, "Kowalski", "a@huta-czestochowa.pl")));
        var plik = Plik("gazeta.docx", "PK-dokument");

        var result = await kontroler.InsertWysylka(Model(1), new FormFileCollection { plik }, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(0, await context.Wiadomosc.CountAsync());
    }

    [Fact]
    public async Task Wysylka_rejects_a_pdf_name_with_a_wrong_header()
    {
        await using var context = ApiTestContextFactory.Create();
        var kontroler = Kontroler(context, new FakeZakres(Odbiorca(1, "Kowalski", "a@huta-czestochowa.pl")));
        var plik = Plik("gazeta.pdf", "to nie jest pdf");

        var result = await kontroler.InsertWysylka(Model(1), new FormFileCollection { plik }, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(0, await context.Wiadomosc.CountAsync());
    }

    [Fact]
    public async Task Wysylka_stores_the_pdf_and_links_it_to_the_message()
    {
        await using var context = ApiTestContextFactory.Create();
        var katalog = Path.Combine(Path.GetTempPath(), "glos-test-" + Guid.NewGuid().ToString("N"));
        try
        {
            var kontroler = Kontroler(context, new FakeZakres(Odbiorca(1, "Kowalski", "a@huta-czestochowa.pl")),
                new WiadomoscZalacznikStorage(katalog));
            var plik = Plik("Gazeta nr 5.pdf", "%PDF-1.4 test");

            var result = await kontroler.InsertWysylka(Model(1), new FormFileCollection { plik }, CancellationToken.None);

            var wynik = Assert.IsType<WiadomoscWysylkaZakresWynikModel>(Assert.IsType<OkObjectResult>(result.Result).Value);
            var dokument = await context.WiadomoscDokument.SingleAsync();
            Assert.Equal(wynik.wiaID, dokument.wdoWiadomoscID);
            Assert.Equal("Gazeta nr 5.pdf", dokument.wdoNazwa);
            Assert.Equal("pdf", dokument.wdoTyp);
            Assert.True(File.Exists(Path.Combine(katalog, dokument.wdoKatalog, dokument.wdoPlik)));
        }
        finally
        {
            if (Directory.Exists(katalog))
            {
                Directory.Delete(katalog, recursive: true);
            }
        }
    }

    [Fact]
    public async Task Robocza_is_saved_without_recipients_and_listed_as_draft()
    {
        await using var context = ApiTestContextFactory.Create();
        var kontroler = Kontroler(context, new FakeZakres(Odbiorca(1, "Kowalski", "a@huta-czestochowa.pl")));

        var result = await kontroler.InsertRobocza(
            new WiadomoscRoboczaModel { wiaAplikacjaID = 900, wiaTemat = "Gazeta nr 6", wiaTresc = "<p>Treść</p>" },
            null, CancellationToken.None);

        var wiadomosc = Assert.IsType<WiadomoscDto>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal("1", (await context.Wiadomosc.SingleAsync()).wiaStatus);
        Assert.Equal(0, await context.WiadomoscPracownik.CountAsync());

        var robocze = await kontroler.SelectRobocze(900);
        var lista = Assert.IsAssignableFrom<IEnumerable<WiadomoscDto>>(robocze.Value);
        Assert.Equal(new[] { wiadomosc.wiaID }, lista.Select(w => w.wiaID));
    }

    [Fact]
    public async Task Robocza_send_adds_recipients_and_publishes_the_message()
    {
        await using var context = ApiTestContextFactory.Create();
        var kontroler = Kontroler(context, new FakeZakres(
            Odbiorca(1, "Kowalski", "a@huta-czestochowa.pl"),
            Odbiorca(2, "Nowak", null)));
        var zapis = await kontroler.InsertRobocza(
            new WiadomoscRoboczaModel { wiaAplikacjaID = 900, wiaTemat = "Gazeta nr 6", wiaTresc = "<p>Treść</p>" },
            null, CancellationToken.None);
        var wiaID = Assert.IsType<WiadomoscDto>(Assert.IsType<OkObjectResult>(zapis.Result).Value).wiaID;

        var result = await kontroler.WyslijRobocza(wiaID, new WiadomoscWyslijRoboczaModel { wzaID = 1, prcIDs = [1, 2] }, CancellationToken.None);

        var wynik = Assert.IsType<WiadomoscWysylkaZakresWynikModel>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal(1, wynik.dodano);
        Assert.Equal(1, wynik.pominieto);
        var wiadomosc = await context.Wiadomosc.SingleAsync();
        Assert.Equal("2", wiadomosc.wiaStatus);
        Assert.True(wiadomosc.wiaPubliczna);
        Assert.Equal(new int?[] { 1 }, (await context.WiadomoscPracownik.ToListAsync()).Select(o => o.wprPracownikID));
    }

    [Fact]
    public async Task Robocza_cannot_be_sent_twice()
    {
        await using var context = ApiTestContextFactory.Create();
        var kontroler = Kontroler(context, new FakeZakres(Odbiorca(1, "Kowalski", "a@huta-czestochowa.pl")));
        var zapis = await kontroler.InsertRobocza(
            new WiadomoscRoboczaModel { wiaAplikacjaID = 900, wiaTemat = "Gazeta nr 6", wiaTresc = "<p>Treść</p>" },
            null, CancellationToken.None);
        var wiaID = Assert.IsType<WiadomoscDto>(Assert.IsType<OkObjectResult>(zapis.Result).Value).wiaID;
        await kontroler.WyslijRobocza(wiaID, new WiadomoscWyslijRoboczaModel { wzaID = 1, prcIDs = [1] }, CancellationToken.None);

        var ponownie = await kontroler.WyslijRobocza(wiaID, new WiadomoscWyslijRoboczaModel { wzaID = 1, prcIDs = [1] }, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(ponownie.Result);
        Assert.Equal(1, await context.WiadomoscPracownik.CountAsync());
    }

    [Fact]
    public async Task Robocza_send_returns_not_found_for_unknown_message()
    {
        await using var context = ApiTestContextFactory.Create();
        var kontroler = Kontroler(context, new FakeZakres(Odbiorca(1, "Kowalski", "a@huta-czestochowa.pl")));

        var result = await kontroler.WyslijRobocza(999, new WiadomoscWyslijRoboczaModel { wzaID = 1, prcIDs = [1] }, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Wysylka_stores_the_pdf_in_app_and_user_folders_and_links_a_plik_record()
    {
        await using var context = ApiTestContextFactory.Create();
        var katalog = Path.Combine(Path.GetTempPath(), "glos-plik-" + Guid.NewGuid().ToString("N"));
        try
        {
            var kontroler = Kontroler(context, new FakeZakres(Odbiorca(1, "Kowalski", "a@huta-czestochowa.pl")),
                new WiadomoscZalacznikStorage(katalog));
            var plik = Plik("gazeta.pdf", "%PDF-1.4 test");

            await kontroler.InsertWysylka(Model(1), new FormFileCollection { plik }, CancellationToken.None);

            var dokument = await context.WiadomoscDokument.SingleAsync();
            Assert.NotNull(dokument.wdoPlikID);
            var rekord = await context.Plik.FindAsync(dokument.wdoPlikID);
            Assert.NotNull(rekord);
            Assert.Equal(900, rekord.plkAplikacjaID);
            Assert.Equal(dokument.wdoKatalog, rekord.plkFolder);
            Assert.Equal(dokument.wdoPlik, rekord.plkPlik);
            Assert.Equal("pdf", rekord.plkRozszerzenie);
            Assert.StartsWith("CORE0900" + Path.DirectorySeparatorChar, dokument.wdoKatalog);
            Assert.Contains("000000000101" + Path.DirectorySeparatorChar, dokument.wdoKatalog);
            Assert.True(File.Exists(Path.Combine(katalog, rekord.plkFolder, rekord.plkPlik)));
        }
        finally
        {
            if (Directory.Exists(katalog))
            {
                Directory.Delete(katalog, recursive: true);
            }
        }
    }

    [Fact]
    public async Task Robocza_is_saved_with_the_glos_sender_so_the_worker_can_send_it()
    {
        await using var context = ApiTestContextFactory.Create();
        var kontroler = Kontroler(context, new FakeZakres(Odbiorca(1, "Kowalski", "a@huta-czestochowa.pl")));

        await kontroler.InsertRobocza(
            new WiadomoscRoboczaModel { wiaAplikacjaID = 900, wiaTemat = "Gazeta nr 7", wiaTresc = "<p>Treść</p>" },
            null, CancellationToken.None);

        Assert.Equal("indigoERP@huta-czestochowa.pl", (await context.Wiadomosc.SingleAsync()).wiaEmail);
    }

    [Fact]
    public async Task Send_fills_the_glos_sender_when_a_draft_has_none()
    {
        await using var context = ApiTestContextFactory.Create();
        var kontroler = Kontroler(context, new FakeZakres(Odbiorca(1, "Kowalski", "a@huta-czestochowa.pl")));
        var zapis = await kontroler.InsertRobocza(
            new WiadomoscRoboczaModel { wiaAplikacjaID = 900, wiaTemat = "Gazeta nr 7", wiaTresc = "<p>Treść</p>" },
            null, CancellationToken.None);
        var wiaID = Assert.IsType<WiadomoscDto>(Assert.IsType<OkObjectResult>(zapis.Result).Value).wiaID;
        var wiadomosc = await context.Wiadomosc.SingleAsync();
        wiadomosc.wiaEmail = null;
        await context.SaveChangesAsync();

        await kontroler.WyslijRobocza(wiaID, new WiadomoscWyslijRoboczaModel { wzaID = 1, prcIDs = [1] }, CancellationToken.None);

        Assert.Equal("indigoERP@huta-czestochowa.pl", (await context.Wiadomosc.SingleAsync()).wiaEmail);
    }

    [Fact]
    public async Task Wysylka_uses_one_row_per_employee_when_the_zakres_lists_the_same_person_twice()
    {
        await using var context = ApiTestContextFactory.Create();
        var zakres = new FakeZakres(
            Odbiorca(1, "Kowalski", null),
            Odbiorca(1, "Kowalski", "a@huta-czestochowa.pl"));
        var kontroler = Kontroler(context, zakres);

        var result = await kontroler.InsertWysylka(Model(1), null, CancellationToken.None);

        var wynik = Assert.IsType<WiadomoscWysylkaZakresWynikModel>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal(1, wynik.dodano);
        var odbiorca = await context.WiadomoscPracownik.SingleAsync();
        Assert.Equal("a@huta-czestochowa.pl", odbiorca.wprEmail);
    }
    private static FormFile Plik(string nazwa, string tresc)
    {
        var bajty = System.Text.Encoding.ASCII.GetBytes(tresc);
        return new FormFile(new MemoryStream(bajty), 0, bajty.Length, "pliki", nazwa);
    }
    private sealed class FakeZakres(params WiadomoscZakresPracownikViewModel[]? odbiorcy) : IWiadomoscZakresPracownikSource
    {
        public Task<IReadOnlyList<WiadomoscZakresPracownikViewModel>?> Pobierz(int wzaID, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<WiadomoscZakresPracownikViewModel>?>(odbiorcy);
    }
}