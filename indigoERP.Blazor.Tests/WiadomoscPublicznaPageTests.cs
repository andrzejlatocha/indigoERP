using Bunit;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Enumerators._Wspolne;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using indigoERP.Service.Interfaces._Wspolne;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace indigoERP.Blazor.Tests;

public sealed class WiadomoscPublicznaPageTests : BunitContext
{
    private readonly WiadomoscPracownikStub pracownik = new();
    private readonly WiadomoscDokumentStub dokumenty = new();

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
        Services.AddSingleton<IWiadomoscPracownikService>(pracownik);
        Services.AddSingleton<IWiadomoscDokumentService>(dokumenty);
    }

    [Fact]
    public void Okno_wysylki_asks_for_scope_before_listing_recipients()
    {
        var cut = Render<Pages.WiadomoscPubliczna.WiadomoscWysylkaOkno>();

        cut.WaitForAssertion(() => Assert.Contains("Zapisz jako roboczy", cut.Markup));
        Assert.DoesNotContain("Wybierz zakres odbiorców", cut.Markup);
    }

    [Fact]
    public void Okno_wysylki_numeru_roboczego_pokazuje_zakres_i_przycisk_wyslij()
    {
        var cut = Render<Pages.WiadomoscPubliczna.WiadomoscWysylkaOkno>(parameters => parameters
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
        var cut = Render<Pages.WiadomoscPubliczna.WiadomoscZarzadzanie>();

        cut.WaitForAssertion(() => Assert.Contains("Nowa wiadomość", cut.Markup));
    }

    [Fact]
    public void Szczegoly_pokazuja_adresatow_status_tresc_i_zalaczniki()
    {
        dokumenty.Dokumenty = [new WiadomoscDokumentDto { wdoID = 3, wdoNazwa = "Plan.pdf" }];

        var cut = Render<Pages.WiadomoscPubliczna.WiadomoscDetail>(parameters => parameters
            .Add(p => p.Model, Wiadomosc(5, adresy: ["jan@hcz.pl", "ewa@hcz.pl"], status: "4", wyslane: 2, razem: 2)));

        cut.WaitForAssertion(() => Assert.Contains("Plan.pdf", cut.Markup));
        Assert.Contains("jan@hcz.pl, ewa@hcz.pl", cut.Markup);
        Assert.Contains("Wysłano 2 z 2", cut.Markup);
        Assert.Contains("Status 4", cut.Markup);
        Assert.Contains("Treść testowa", cut.Markup);
    }

    [Fact]
    public void Szczegoly_bez_adresow_pokazuja_zakres_pracownikow_i_brak_zalacznikow()
    {
        dokumenty.Dokumenty = [];

        var cut = Render<Pages.WiadomoscPubliczna.WiadomoscDetail>(parameters => parameters
            .Add(p => p.Model, Wiadomosc(6, adresy: [], status: "2", wyslane: 0, razem: 40)));

        cut.WaitForAssertion(() => Assert.Contains("Brak załączników.", cut.Markup));
        Assert.Contains("Zakres pracowników", cut.Markup);
    }

    [Fact]
    public void Lista_zglasza_wybrany_wiersz_po_kliknieciu()
    {
        WiadomoscPodsumowanieDto? wybrana = null;
        var lista = new List<WiadomoscPodsumowanieDto> { Wiadomosc(10, status: "2"), Wiadomosc(11, status: "4") };

        var cut = Render<Pages.WiadomoscPubliczna.ListaWiadomosci>(parameters => parameters
            .Add(p => p.Lista, lista)
            .Add(p => p.WybranaChanged, EventCallback.Factory.Create<WiadomoscPodsumowanieDto?>(this, v => wybrana = v)));

        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll("tr.rz-data-row").Count));
        cut.FindAll("tr.rz-data-row")[1].Click();

        Assert.NotNull(wybrana);
        Assert.Equal(11, wybrana!.wiaID);
    }

    [Fact]
    public void Lista_podswietla_wybrany_wiersz()
    {
        var lista = new List<WiadomoscPodsumowanieDto> { Wiadomosc(20, status: "2"), Wiadomosc(21, status: "4") };

        var cut = Render<Pages.WiadomoscPubliczna.ListaWiadomosci>(parameters => parameters
            .Add(p => p.Lista, lista)
            .Add(p => p.Wybrana, lista[1]));

        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll("tr.rz-data-row").Count));
        var zaznaczone = cut.FindAll("tr.rz-data-row.rz-state-highlight");
        Assert.Single(zaznaczone);
        Assert.Contains("Wiadomość 21", zaznaczone[0].TextContent);
    }

    [Fact]
    public void Panel_szczegoly_sa_nieaktywne_bez_wyboru_i_aktywne_po_zaznaczeniu()
    {
        pracownik.Podsumowanie =
        [
            Wiadomosc(30, status: "2", data: DateTime.Now.AddDays(-1)),
            Wiadomosc(31, status: "2", data: DateTime.Now.AddDays(-2))
        ];

        var cut = Render<Pages.WiadomoscPubliczna.WiadomosciPanel>();

        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll("tr.rz-data-row").Count));
        Assert.True(PrzyciskZTekstem(cut, "Szczegóły").HasAttribute("disabled"));
        Assert.Empty(PrzyciskiZTekstem(cut, "Popraw"));

        cut.FindAll("tr.rz-data-row")[0].Click();

        cut.WaitForAssertion(() => Assert.False(PrzyciskZTekstem(cut, "Szczegóły").HasAttribute("disabled")));
        Assert.Single(PrzyciskiZTekstem(cut, "Popraw"));
        Assert.Single(PrzyciskiZTekstem(cut, "Anuluj"));
    }

    [Fact]
    public void Panel_po_zaznaczeniu_wysylki_w_trakcie_nie_pokazuje_popraw_ani_usun()
    {
        // Status „3” (w trakcie wysyłania): poprawki i usunięcia już nie ma, anulowanie tak.
        pracownik.Podsumowanie = [Wiadomosc(40, status: "3", data: DateTime.Now.AddDays(-1))];

        var cut = Render<Pages.WiadomoscPubliczna.WiadomosciPanel>();

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll("tr.rz-data-row")));
        cut.FindAll("tr.rz-data-row")[0].Click();

        cut.WaitForAssertion(() => Assert.False(PrzyciskZTekstem(cut, "Szczegóły").HasAttribute("disabled")));
        Assert.Empty(PrzyciskiZTekstem(cut, "Popraw"));
        Assert.Empty(PrzyciskiZTekstem(cut, "Usuń"));
        Assert.Single(PrzyciskiZTekstem(cut, "Anuluj"));
    }

    [Fact]
    public void Panel_szczegoly_otwiera_okno_z_danymi_wybranej_wiadomosci()
    {
        pracownik.Podsumowanie = [Wiadomosc(50, status: "2", data: DateTime.Now.AddDays(-1))];
        dokumenty.Dokumenty = [];

        var cut = Render<Pages.WiadomoscPubliczna.WiadomosciPanel>();

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll("tr.rz-data-row")));
        cut.FindAll("tr.rz-data-row")[0].Click();
        cut.WaitForAssertion(() => Assert.False(PrzyciskZTekstem(cut, "Szczegóły").HasAttribute("disabled")));

        PrzyciskZTekstem(cut, "Szczegóły").Click();

        cut.WaitForAssertion(() => Assert.Contains("Zakres pracowników", cut.Markup));
        Assert.Contains("Treść testowa", cut.Markup);
    }

    // Ikona Material jest częścią TextContent przycisku, więc szukamy po fragmencie tekstu.
    private static AngleSharp.Dom.IElement PrzyciskZTekstem<T>(IRenderedComponent<T> cut, string tekst) where T : IComponent =>
        PrzyciskiZTekstem(cut, tekst).Single();

    private static IReadOnlyList<AngleSharp.Dom.IElement> PrzyciskiZTekstem<T>(IRenderedComponent<T> cut, string tekst) where T : IComponent =>
        cut.FindAll("button").Where(b => b.TextContent.Contains(tekst)).ToList();

    private static WiadomoscPodsumowanieDto Wiadomosc(
        int id,
        string status = "2",
        DateTime? data = null,
        IEnumerable<string>? adresy = null,
        int wyslane = 0,
        int razem = 1) => new()
    {
        wiaID = id,
        wiaTemat = $"Wiadomość {id}",
        wiaTresc = "<p>Treść testowa</p>",
        wiaAdresy = adresy?.ToList() ?? new List<string>(),
        wiaData = data ?? DateTime.Now.AddDays(-1),
        wiaStatus = status,
        Wyslane = wyslane,
        Razem = razem
    };

    private sealed class WiadomoscDokumentStub : IWiadomoscDokumentService
    {
        public IEnumerable<WiadomoscDokumentDto> Dokumenty { get; set; } = [];

        public Task<IEnumerable<WiadomoscDokumentDto>> SelectByWiadomosc(int wiaID) => Task.FromResult(Dokumenty);

        public Task<ResponseMessage> Upload(int wiaID, Microsoft.AspNetCore.Components.Forms.IBrowserFile file) =>
            Task.FromResult(new ResponseMessage(204, string.Empty, null));

        public Task<WiadomoscDokumentFile?> Download(int wdoID) => Task.FromResult<WiadomoscDokumentFile?>(null);

        public Task<ResponseMessage> Delete(int wdoID) => Task.FromResult(new ResponseMessage(204, string.Empty, null));
    }

    private sealed class WiadomoscPracownikStub : IWiadomoscPracownikService
    {
        public IEnumerable<WiadomoscPodsumowanieDto> Podsumowanie { get; set; } = [];

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

        public Task<IEnumerable<WiadomoscHarmonogramViewModel>> SelectHarmonogram(int appID) =>
            Task.FromResult(Enumerable.Empty<WiadomoscHarmonogramViewModel>());

        public Task<ResponseMessage> PoprawRobocza(int wiaID, WiadomoscPoprawaModel model) =>
            Task.FromResult(new ResponseMessage(204, string.Empty, null));

        public Task<ResponseMessage> UsunWiadomosc(int wiaID) =>
            Task.FromResult(new ResponseMessage(204, string.Empty, null));

        public Task<ResponseMessage> AnulujHarmonogram(int wiaID) =>
            Task.FromResult(new ResponseMessage(204, string.Empty, null));

        public Task<IEnumerable<WiadomoscPodsumowanieDto>> SelectPodsumowanie(int appID) =>
            Task.FromResult(Podsumowanie);
    }
}
