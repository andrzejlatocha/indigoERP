using Bunit;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Enumerators._Wspolne;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using indigoERP.Service.Interfaces._Wspolne;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace indigoERP.Blazor.Tests;

public sealed class WiadomoscPublicznaPageTests : BunitContext
{
    public WiadomoscPublicznaPageTests()
    {
        Services.AddLogging();
        Services.AddScoped<DialogService>();
        Services.AddScoped<NotificationService>();
        Services.AddScoped<ContextMenuService>();
        Services.AddScoped<TooltipService>();
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["appSettings:AppID"] = "900" })
            .Build());
        Services.AddSingleton<IWiadomoscPracownikService>(new WiadomoscPracownikStub());
    }

    [Fact]
    public void Okno_wysylki_asks_for_scope_before_listing_recipients()
    {
        var cut = Render<Pages.WiadomoscPubliczna.GlosWysylkaOkno>();

        cut.WaitForAssertion(() => Assert.Contains("Zapisz jako roboczy", cut.Markup));
        Assert.DoesNotContain("Wybierz zakres odbiorców", cut.Markup);
    }

    [Fact]
    public void Okno_wysylki_numeru_roboczego_pokazuje_zakres_i_przycisk_wyslij()
    {
        var cut = Render<Pages.WiadomoscPubliczna.GlosWysylkaOkno>(parameters => parameters
            .Add(p => p.WiadomoscID, 7)
            .Add(p => p.Tytul, "Gazeta nr 6"));

        cut.WaitForAssertion(() => Assert.Contains("Wybierz zakres odbiorców", cut.Markup));
        Assert.Contains("Gazeta nr 6", cut.Markup);
        Assert.Contains("Wyślij (0)", cut.Markup);
    }

    [Fact]
    public void SkrocOpis_usuwa_znaczniki_html_i_skraca_tekst()
    {
        var wynik = Pages.WiadomoscPublicznaModel.SkrocOpis("<p>Ala&nbsp;ma <b>kota</b></p>", 10);

        Assert.Equal("Ala ma kot…", wynik);
    }

    [Fact]
    public void Zarzadzanie_renderuje_przycisk_nowego_numeru()
    {
        var cut = Render<Pages.WiadomoscPubliczna.GlosZarzadzanie>();

        cut.WaitForAssertion(() => Assert.Contains("Nowy numer", cut.Markup));
    }

    private sealed class WiadomoscPracownikStub : IWiadomoscPracownikService
    {
        public Task<IEnumerable<WiadomoscZakresEnumerator>> SelectZakresy(int appID) =>
            Task.FromResult<IEnumerable<WiadomoscZakresEnumerator>>(
            [
                new WiadomoscZakresEnumerator { wzaID = 1, wzaNazwa = "Wydział testowy", wzaStatus = "2" }
            ]);

        public Task<IEnumerable<WiadomoscZakresPracownikViewModel>> SelectPracownicyZakres(int wzaID) =>
            Task.FromResult<IEnumerable<WiadomoscZakresPracownikViewModel>>([]);

        public Task<(ResponseMessage Response, WiadomoscWysylkaZakresWynikModel? Wynik)> InsertWysylka(
            WiadomoscWysylkaZakresModel model, IReadOnlyList<WiadomoscPlikWysylki> pliki) =>
            Task.FromResult<(ResponseMessage, WiadomoscWysylkaZakresWynikModel?)>(
                (new ResponseMessage(200, string.Empty, 1), new WiadomoscWysylkaZakresWynikModel { wiaID = 1 }));

        public Task<(ResponseMessage Response, WiadomoscDto? Wiadomosc)> InsertRobocza(
            WiadomoscRoboczaModel model, IReadOnlyList<WiadomoscPlikWysylki> pliki) =>
            Task.FromResult<(ResponseMessage, WiadomoscDto?)>((new ResponseMessage(200, string.Empty, 1), new WiadomoscDto { wiaID = 1 }));

        public Task<IEnumerable<WiadomoscDto>> SelectRobocze(int appID) =>
            Task.FromResult<IEnumerable<WiadomoscDto>>([]);

        public Task<(ResponseMessage Response, WiadomoscWysylkaZakresWynikModel? Wynik)> WyslijRobocza(
            int wiaID, WiadomoscWyslijRoboczaModel model) =>
            Task.FromResult<(ResponseMessage, WiadomoscWysylkaZakresWynikModel?)>(
                (new ResponseMessage(200, string.Empty, wiaID), new WiadomoscWysylkaZakresWynikModel { wiaID = wiaID }));

        public Task<IEnumerable<WiadomoscPracownikViewModel>> SelectWyslane(int appID) =>
            Task.FromResult<IEnumerable<WiadomoscPracownikViewModel>>([]);

        public Task<IEnumerable<WiadomoscWzorzecEnumerator>> SelectWzorce(int appID) =>
            Task.FromResult<IEnumerable<WiadomoscWzorzecEnumerator>>(
            [
                new WiadomoscWzorzecEnumerator { msgID = 9, msgTemat = "Głos Hutnika", msgStatus = "2" }
            ]);

        public Task<IEnumerable<WiadomoscPodsumowanieDto>> SelectPodsumowanie(int appID) =>
            Task.FromResult<IEnumerable<WiadomoscPodsumowanieDto>>([]);
    }
}
