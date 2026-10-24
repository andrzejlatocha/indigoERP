using indigoERP.Api.Controllers._Administrator;
using indigoERP.Api.Services;
using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.Models._Administrator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Tests;

public sealed class UzytkownikEmailGraphTests
{
    private static GraphUserInfo Graph(string id, string sam, string? mail, string? upn = null) =>
        new(id, "Nazwa " + sam, mail, upn ?? sam + "@huta-czestochowa.pl", sam, true);

    private static Uzytkownik Uzytkownik(int id, string nazwa, string? email, string status = "2", string? ad = null) =>
        new() { usrID = id, usrNazwa = nazwa, usrNazwaAD = ad ?? "HCZ\\" + nazwa, usrEmail = email, usrStatus = status, Usuniety = false };

    private static UzytkownikEmailGraphRoznice Roznice(params GraphUserInfo[] users) => new(users);

    [Fact]
    public void Empty_address_with_a_matching_microsoft_account_is_marked_as_add()
    {
        var roznice = Roznice(Graph("1", "jan.kowalski", "jan.kowalski@huta-czestochowa.pl"));

        var wynik = roznice.Klasyfikuj(Uzytkownik(1, "jan.kowalski", ""));

        Assert.Equal(UzytkownikEmailGraphAkcja.Dodaj, wynik.akcja);
        Assert.Equal("Dopasowano", wynik.wynik);
        Assert.Equal("jan.kowalski@huta-czestochowa.pl", wynik.graphEmail);
    }

    [Fact]
    public void Different_address_is_marked_as_correct_and_the_same_address_is_ignored_case_insensitively()
    {
        var roznice = Roznice(Graph("1", "jan.kowalski", "Jan.Kowalski@huta-czestochowa.pl"));

        var inny = roznice.Klasyfikuj(Uzytkownik(1, "jan.kowalski", "stary@huta-czestochowa.pl"));
        var taki = roznice.Klasyfikuj(Uzytkownik(1, "jan.kowalski", "jan.kowalski@huta-czestochowa.pl"));

        Assert.Equal(UzytkownikEmailGraphAkcja.Popraw, inny.akcja);
        Assert.Equal(string.Empty, taki.akcja);
    }

    [Fact]
    public void Inactive_account_with_an_address_is_marked_as_remove_and_without_address_has_no_action()
    {
        var roznice = Roznice(Graph("1", "jan.kowalski", "jan.kowalski@huta-czestochowa.pl"));

        var zAdresem = roznice.Klasyfikuj(Uzytkownik(1, "jan.kowalski", "jan.kowalski@huta-czestochowa.pl", status: "1"));
        var bez = roznice.Klasyfikuj(Uzytkownik(2, "anna.nowak", "", status: "1"));

        Assert.Equal(UzytkownikEmailGraphAkcja.Usun, zAdresem.akcja);
        Assert.Equal(string.Empty, bez.akcja);
        Assert.False(UzytkownikEmailGraphRoznice.DoPokazania(bez));
    }

    [Fact]
    public void User_principal_name_is_used_only_to_add_and_never_to_overwrite_an_existing_address()
    {
        var roznice = Roznice(Graph("1", "jan.kowalski", mail: null, upn: "jan.kowalski@huta-czestochowa.pl"));

        var dodaj = roznice.Klasyfikuj(Uzytkownik(1, "jan.kowalski", ""));
        var popraw = roznice.Klasyfikuj(Uzytkownik(1, "jan.kowalski", "inny@huta-czestochowa.pl"));

        Assert.Equal(UzytkownikEmailGraphAkcja.Dodaj, dodaj.akcja);
        Assert.Equal("jan.kowalski@huta-czestochowa.pl", dodaj.graphEmail);
        Assert.Equal(string.Empty, popraw.akcja);
    }

    [Fact]
    public void Ambiguous_and_missing_accounts_get_no_action_but_unresolved_empty_ones_are_shown()
    {
        var roznice = Roznice(
            Graph("1", "jan.kowalski", "a@huta-czestochowa.pl"),
            Graph("2", "jan.kowalski", "b@huta-czestochowa.pl"));

        var niejednoznaczny = roznice.Klasyfikuj(Uzytkownik(1, "jan.kowalski", ""));
        var brak = roznice.Klasyfikuj(Uzytkownik(2, "ktos.inny", ""));

        Assert.Equal("Niejednoznaczne", niejednoznaczny.wynik);
        Assert.Equal(string.Empty, niejednoznaczny.akcja);
        Assert.Equal("Brak", brak.wynik);
        Assert.True(UzytkownikEmailGraphRoznice.DoPokazania(niejednoznaczny));
        Assert.True(UzytkownikEmailGraphRoznice.DoPokazania(brak));
    }

    [Fact]
    public async Task Update_applies_only_changes_matching_the_computed_action_and_takes_addresses_from_microsoft()
    {
        await using var context = ApiTestContextFactory.Create();
        context.Uzytkownik.AddRange(
            Uzytkownik(1, "jan.kowalski", ""),
            Uzytkownik(2, "anna.nowak", "anna.nowak@huta-czestochowa.pl", status: "1"),
            Uzytkownik(3, "piotr.zielinski", "stary@huta-czestochowa.pl"));
        await context.SaveChangesAsync();
        var graph = new FakeGraph(
            Graph("1", "jan.kowalski", "jan.kowalski@huta-czestochowa.pl"),
            Graph("3", "piotr.zielinski", "piotr.zielinski@huta-czestochowa.pl"));
        var controller = new UzytkownikController(context, ApiTestContextFactory.Configuration(), graph);

        var result = await controller.UpdateDaneGraph(new UzytkownikEmailGraphZatwierdzModel
        {
            zmiany =
            [
                new UzytkownikEmailGraphZmianaModel { usrID = 1, akcja = UzytkownikEmailGraphAkcja.Dodaj },
                new UzytkownikEmailGraphZmianaModel { usrID = 2, akcja = UzytkownikEmailGraphAkcja.Usun },
                new UzytkownikEmailGraphZmianaModel { usrID = 3, akcja = UzytkownikEmailGraphAkcja.Dodaj }
            ]
        }, CancellationToken.None);

        var wynik = Assert.IsType<UzytkownikEmailGraphWynikModel>(result.Value);
        Assert.Equal(1, wynik.dodano);
        Assert.Equal(1, wynik.usunieto);
        Assert.Equal(0, wynik.poprawiono);
        Assert.Equal(1, wynik.pominieto);
        Assert.Equal("jan.kowalski@huta-czestochowa.pl", (await context.Uzytkownik.FindAsync(1))!.usrEmail);
        Assert.Equal(string.Empty, (await context.Uzytkownik.FindAsync(2))!.usrEmail);
        Assert.Equal("stary@huta-czestochowa.pl", (await context.Uzytkownik.FindAsync(3))!.usrEmail);
    }

    [Fact]
    public async Task Select_returns_service_unavailable_when_microsoft_graph_is_not_configured()
    {
        await using var context = ApiTestContextFactory.Create();
        var controller = new UzytkownikController(context, ApiTestContextFactory.Configuration(), new FakeGraph { Configured = false });

        var result = await controller.SelectDaneGraph(CancellationToken.None);

        var problem = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(503, problem.StatusCode);
    }

    [Fact]
    public async Task Select_lists_actions_first_and_skips_users_without_differences()
    {
        await using var context = ApiTestContextFactory.Create();
        context.Uzytkownik.AddRange(
            Uzytkownik(1, "zbigniew.zielinski", ""),
            Uzytkownik(2, "adam.nowak", "adam.nowak@huta-czestochowa.pl"),
            Uzytkownik(3, "ewa.stara", "ewa@huta-czestochowa.pl", status: "1"),
            Uzytkownik(4, "nieznany.ktos", ""));
        await context.SaveChangesAsync();
        var graph = new FakeGraph(
            Graph("1", "zbigniew.zielinski", "zbigniew.zielinski@huta-czestochowa.pl"),
            Graph("2", "adam.nowak", "adam.nowak@huta-czestochowa.pl"));
        var controller = new UzytkownikController(context, ApiTestContextFactory.Configuration(), graph);

        var result = await controller.SelectDaneGraph(CancellationToken.None);

        var lista = Assert.IsAssignableFrom<IEnumerable<UzytkownikEmailGraphModel>>(result.Value).ToList();
        Assert.Equal(["ewa.stara", "zbigniew.zielinski", "nieznany.ktos"], lista.Select(p => p.usrNazwa));
        Assert.DoesNotContain(lista, p => p.usrNazwa == "adam.nowak");
    }

    private sealed class FakeGraph(params GraphUserInfo[] users) : IMicrosoftGraphUserService
    {
        public bool Configured { get; init; } = true;

        public bool IsConfigured => Configured;

        public Task<IReadOnlyList<GraphUserInfo>> GetUsersAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<GraphUserInfo>>(users);
    }
}
