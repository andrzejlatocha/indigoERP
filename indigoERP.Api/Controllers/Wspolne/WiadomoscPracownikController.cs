using indigoERP.Api.Data;
using indigoERP.Api.Services;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Entities._Wspolne;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace indigoERP.Api.Controllers._Wspolne;

/// <summary>Wysyłka wiadomości publicznej (np. Głos Hutnika, wzorzec z dbo.WiadomoscWzorzec) do pracowników zakresu, z załącznikami PDF. Dostęp: rola Gazeta lub Administrator.</summary>
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(Roles = GazetaRoles, AuthenticationSchemes = BearerScheme)]
public class WiadomoscPracownikController : ControllerBase
{
    private const string GazetaRoles = "HCZ-indigoERP-Wiadomość,HCZ-indigoERP-Administrator";
    private const string BearerScheme = "Bearer";

    // Numer roboczy: zapisany, niewidoczny publicznie. Numer wysłany: status "2" (widoczny na /wiadomosc).
    private const string StatusRobocza = "1";
    private const string StatusWyslana = "2";

    // Harmonogram wysyłki: szablon z odbiorcami (7); po anulowaniu status 6 (zgodnie z WiadomoscHarmonogramController).
    private const string StatusHarmonogram = "7";

    // Statusy odbiorcy (WiadomoscPracownik.wprStatus), zgodne z Workerem.
    private const string WprStatusNowa = "2";
    private const string WprStatusZakonczona = "4";
    private const string WprStatusBlad = "5";

    /// <summary>Skrzynka nadawcy wiadomości publicznych (Worker bierze nadawcę z Wiadomosc.wiaEmail).</summary>
    public const string NadawcaWiadomosciPublicznej = "indigoERP@huta-czestochowa.pl";

    /// <summary>Domyślny wzorzec szablonu wiadomości publicznej, w który Worker opakowuje treść z formularza (dbo.WiadomoscWzorzec.msgID, wzorzec Głos Hutnika).</summary>
    public const int WzorzecWiadomosciPublicznej = 9;

    // Każdy odbiorca to osobna wartość formularza (prcIDs); domyślny limit ASP.NET to 1024.
    private const int LimitOdbiorcow = 20000;

    private readonly indigoERPContext _context;
    private readonly IWiadomoscZakresPracownikSource _zakresy;
    private readonly IWiadomoscZalacznikStorage _zalaczniki;

    public WiadomoscPracownikController(indigoERPContext context)
        : this(context, new WiadomoscZakresPracownikSource(context), new WiadomoscZalacznikStorage(string.Empty))
    {
    }

    [ActivatorUtilitiesConstructor]
    public WiadomoscPracownikController(
        indigoERPContext context, IWiadomoscZakresPracownikSource zakresy, IWiadomoscZalacznikStorage zalaczniki)
    {
        _context = context;
        _zakresy = zakresy;
        _zalaczniki = zalaczniki;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WiadomoscPracownikViewModel>>> Select(string wprStatus = "2")
    {
        var pracownicy = await _context.WiadomoscPracownikViewModel
            .AsNoTracking()
            .Where(w => w.wprStatus == wprStatus || wprStatus == "*")
            .OrderBy(w => w.wprID)
            .ToListAsync();

        return pracownicy;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WiadomoscPracownikViewModel>>> SelectWyslane(int appID)
    {
        return await _context.WiadomoscPracownikViewModel
            .AsNoTracking()
            .Where(w => w.wiaAplikacjaID == appID)
            .OrderByDescending(w => w.wiaData)
            .ThenBy(w => w.wprID)
            .ToListAsync();
    }

    [HttpGet("{wzaID}")]
    public async Task<ActionResult<IEnumerable<WiadomoscZakresPracownikViewModel>>> SelectByWiadomoscZakres(int wzaID)
    {
        var pracownicy = await _zakresy.Pobierz(wzaID, CancellationToken.None);

        if (pracownicy == null)
        {
            return NotFound();
        }

        var osoby = pracownicy.Where(p => p.prcID.HasValue).GroupBy(p => p.prcID).Select(g =>
        {
            var osoba = g.First();
            return new WiadomoscZakresPracownikViewModel
            {
                prcID = osoba.prcID, prcKod = osoba.prcKod, prcImie = osoba.prcImie, prcNazwisko = osoba.prcNazwisko,
                kntID = osoba.kntID,
                kntKontakt = string.Join("; ", g.Select(p => p.kntKontakt?.Trim()).Where(e => !string.IsNullOrWhiteSpace(e))
                    .Distinct(StringComparer.OrdinalIgnoreCase))
            };
        }).ToList();

        return Ok(osoby);
    }

    /// <summary>Numery robocze aplikacji (zapisane, jeszcze nie wysłane), najnowsze pierwsze.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WiadomoscDto>>> SelectRobocze(int appID)
    {
        return await _context.Wiadomosc
            .AsNoTracking()
            .Where(w => w.wiaAplikacjaID == appID && w.wiaStatus == StatusRobocza && w.Usuniety != true)
            .OrderByDescending(w => w.wiaData)
            .ProjectToType<WiadomoscDto>()
            .ToListAsync();
    }

    /// <summary>Zapisuje numer roboczy wraz z załącznikami PDF; odbiorców dobiera się przy wysyłce.</summary>
    [HttpPost]
    [RequestSizeLimit(WiadomoscZalacznikStorage.MaxLacznyRozmiar)]
    [RequestFormLimits(MultipartBodyLengthLimit = WiadomoscZalacznikStorage.MaxLacznyRozmiar)]
    public async Task<ActionResult<WiadomoscDto>> InsertRobocza(
        [FromForm] WiadomoscRoboczaModel model, IFormFileCollection? pliki, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.wiaTemat) || string.IsNullOrWhiteSpace(model.wiaTresc))
        {
            return BadRequest("Temat i treść wiadomości są wymagane.");
        }

        if (model.wiaWzorzecID == null)
        {
            return BadRequest("Wybierz wzorzec wiadomości.");
        }

        var zalaczniki = pliki?.ToList() ?? new List<IFormFile>();
        var blad = WalidujZalaczniki(zalaczniki);

        if (blad != null)
        {
            return BadRequest(blad);
        }

        int.TryParse(User.FindFirst("usrID")?.Value, out var usrID);

        await using var transakcja = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync(cancellationToken)
            : null;

        var wiadomosc = new Wiadomosc
        {
            wiaAplikacjaID = model.wiaAplikacjaID,
            wiaUzytkownikID = usrID,
            wiaTemat = model.wiaTemat.Trim(),
            wiaTresc = model.wiaTresc,
            wiaData = DateTime.Now,
            wiaStatus = StatusRobocza,
            wiaEmail = NadawcaWiadomosciPublicznej,
            wiaWzorzecID = model.wiaWzorzecID.Value,
            wiaPubliczna = true
        };

        _context.Wiadomosc.Add(wiadomosc);
        await _context.SaveChangesAsync(cancellationToken);

        await ZapiszZalaczniki(wiadomosc.wiaID, model.wiaAplikacjaID, zalaczniki, usrID, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        if (transakcja != null)
        {
            await transakcja.CommitAsync(cancellationToken);
        }

        return Ok(wiadomosc.Adapt<WiadomoscDto>());
    }

    /// <summary>
    /// Wysyła numer roboczy do odbiorców zakresu: dopisuje odbiorców, publikuje numer (status "2") i ustawia datę wysyłki.
    /// Odbiorcy są sprawdzani po stronie serwera względem zakresu.
    /// </summary>
    [HttpPost("{wiaID}")]
    [RequestFormLimits(ValueCountLimit = LimitOdbiorcow)]
    public async Task<ActionResult<WiadomoscWysylkaZakresWynikModel>> WyslijRobocza(
        int wiaID, [FromForm] WiadomoscWyslijRoboczaModel model, CancellationToken cancellationToken)
    {
        var wiadomosc = await _context.Wiadomosc.FirstOrDefaultAsync(
            w => w.wiaID == wiaID && w.Usuniety != true, cancellationToken);

        if (wiadomosc == null)
        {
            return NotFound();
        }

        if (wiadomosc.wiaStatus != StatusRobocza)
        {
            return BadRequest("Ta wiadomość nie jest numerem roboczym i została już wysłana.");
        }

        // Worker wysyła z adresu zapisanego w wiaEmail; numery zapisane przed ustawieniem nadawcy dostają go przy wysyłce.
        if (string.IsNullOrWhiteSpace(wiadomosc.wiaEmail))
        {
            wiadomosc.wiaEmail = NadawcaWiadomosciPublicznej;
        }

        // Szkice zapisane przed ustawieniem wzorca dostają go przy wysyłce.
        wiadomosc.wiaWzorzecID ??= WzorzecWiadomosciPublicznej;

        if (model.prcIDs.Count == 0)
        {
            return BadRequest("Wybierz co najmniej jednego odbiorcę.");
        }

        var zakres = await _zakresy.Pobierz(model.wzaID, cancellationToken);

        if (zakres == null)
        {
            return BadRequest("Nieprawidłowy zakres odbiorców.");
        }

        var wynik = new WiadomoscWysylkaZakresWynikModel { wiaID = wiaID };
        var doWyslania = PrzygotujOdbiorcow(zakres, model.prcIDs, wynik, model.wzaID == 7);

        if (doWyslania.Count == 0)
        {
            return BadRequest("Żaden z wybranych odbiorców nie ma adresu e-mail w zakresie.");
        }

        var bladHarmonogramu = string.IsNullOrWhiteSpace(model.kodHarmonogram) ? null : WalidujHarmonogram(model);

        if (bladHarmonogramu != null)
        {
            return BadRequest(bladHarmonogramu);
        }

        int.TryParse(User.FindFirst("usrID")?.Value, out var usrID);

        await using var transakcja = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync(cancellationToken)
            : null;

        if (string.IsNullOrWhiteSpace(model.kodHarmonogram))
        {
            DodajOdbiorcow(wiaID, doWyslania);
            wiadomosc.wiaStatus = StatusWyslana;
        }
        else
        {
            // Harmonogram: szablon (status 7) z odbiorcami; Worker kopiuje go na wysyłkę w terminie.
            DodajOdbiorcow(wiaID, doWyslania, StatusHarmonogram);
            wiadomosc.wiaStatus = StatusHarmonogram;
            wiadomosc.wiaKodHarmonogram = model.kodHarmonogram;
            wiadomosc.wiaDataHarmonogramStart = model.dataHarmonogramStart;
            wiadomosc.wiaDataHarmonogramKoniec = model.kodHarmonogram == "J" ? null : model.dataHarmonogramKoniec;
            wiadomosc.wiaDataNastepnejWysylki = model.dataHarmonogramStart;
            wiadomosc.wiaHarmonogramAktywny = true;
        }

        wiadomosc.wiaData = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);

        if (transakcja != null)
        {
            await transakcja.CommitAsync(cancellationToken);
        }

        wynik.dodano = doWyslania.Count;

        return Ok(wynik);
    }

    /// <summary>
    /// Zapisuje wiadomość, załączniki PDF i odbiorców w jednej transakcji. Odbiorcy są sprawdzani po stronie serwera
    /// względem zakresu; osoby spoza zakresu lub bez adresu e-mail są pomijane i zgłaszane w wyniku.
    /// </summary>
    [HttpPost]
    [RequestSizeLimit(WiadomoscZalacznikStorage.MaxLacznyRozmiar)]
    [RequestFormLimits(MultipartBodyLengthLimit = WiadomoscZalacznikStorage.MaxLacznyRozmiar, ValueCountLimit = LimitOdbiorcow)]
    public async Task<ActionResult<WiadomoscWysylkaZakresWynikModel>> InsertWysylka(
        [FromForm] WiadomoscWysylkaZakresModel model, IFormFileCollection? pliki, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.wiaTemat) || string.IsNullOrWhiteSpace(model.wiaTresc))
        {
            return BadRequest("Temat i treść wiadomości są wymagane.");
        }

        if (model.wiaWzorzecID == null)
        {
            return BadRequest("Wybierz wzorzec wiadomości.");
        }

        if (model.prcIDs.Count == 0)
        {
            return BadRequest("Wybierz co najmniej jednego odbiorcę.");
        }

        var zalaczniki = pliki?.ToList() ?? new List<IFormFile>();
        var blad = WalidujZalaczniki(zalaczniki);

        if (blad != null)
        {
            return BadRequest(blad);
        }

        var zakres = await _zakresy.Pobierz(model.wzaID, cancellationToken);

        if (zakres == null)
        {
            return BadRequest("Nieprawidłowy zakres odbiorców.");
        }

        var wynik = new WiadomoscWysylkaZakresWynikModel();
        var doWyslania = PrzygotujOdbiorcow(zakres, model.prcIDs, wynik, model.wzaID == 7);

        if (doWyslania.Count == 0)
        {
            return BadRequest("Żaden z wybranych odbiorców nie ma adresu e-mail w zakresie.");
        }

        int.TryParse(User.FindFirst("usrID")?.Value, out var usrID);

        await using var transakcja = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync(cancellationToken)
            : null;

        var wiadomosc = new Wiadomosc
        {
            wiaAplikacjaID = model.wiaAplikacjaID,
            wiaUzytkownikID = usrID,
            wiaTemat = model.wiaTemat.Trim(),
            wiaTresc = model.wiaTresc,
            wiaData = DateTime.Now,
            wiaStatus = StatusWyslana,
            wiaEmail = NadawcaWiadomosciPublicznej,
            wiaWzorzecID = model.wiaWzorzecID.Value,
            // Wysyłka z zakładki wiadomości publicznych to numer publiczny — trafia na stronę /wiadomosc.
            wiaPubliczna = true
        };

        _context.Wiadomosc.Add(wiadomosc);
        await _context.SaveChangesAsync(cancellationToken);

        await ZapiszZalaczniki(wiadomosc.wiaID, model.wiaAplikacjaID, zalaczniki, usrID, cancellationToken);
        DodajOdbiorcow(wiadomosc.wiaID, doWyslania);
        await _context.SaveChangesAsync(cancellationToken);

        if (transakcja != null)
        {
            await transakcja.CommitAsync(cancellationToken);
        }

        wynik.wiaID = wiadomosc.wiaID;
        wynik.dodano = doWyslania.Count;

        return Ok(wynik);
    }

    /// <summary>Podsumowanie wysyłki każdej wiadomości aplikacji: odbiorcy razem, wysłani, z błędem i oczekujący.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WiadomoscPodsumowanieDto>>> SelectPodsumowanie(int appID)
    {
        var wiadomosci = await _context.Wiadomosc
            .AsNoTracking()
            .Where(w => w.wiaAplikacjaID == appID && w.Usuniety != true)
            .OrderByDescending(w => w.wiaData)
            .Select(w => new { w.wiaID, w.wiaTemat, w.wiaData, w.wiaStatus })
            .ToListAsync();

        var ids = wiadomosci.Select(w => w.wiaID).ToList();
        var liczby = await _context.WiadomoscPracownik
            .AsNoTracking()
            .Where(p => p.wprWiadomoscID != null && ids.Contains(p.wprWiadomoscID.Value))
            .GroupBy(p => p.wprWiadomoscID!.Value)
            .Select(g => new
            {
                WiadomoscID = g.Key,
                Razem = g.Count(),
                Wyslane = g.Count(p => p.wprStatus == WprStatusZakonczona),
                Bledy = g.Count(p => p.wprStatus == WprStatusBlad),
                Oczekujace = g.Count(p => p.wprStatus == WprStatusNowa)
            })
            .ToListAsync();
        var slownik = liczby.ToDictionary(x => x.WiadomoscID);

        return wiadomosci.Select(w =>
        {
            slownik.TryGetValue(w.wiaID, out var l);
            return new WiadomoscPodsumowanieDto
            {
                wiaID = w.wiaID,
                wiaTemat = w.wiaTemat,
                wiaData = w.wiaData,
                wiaStatus = w.wiaStatus,
                Razem = l?.Razem ?? 0,
                Wyslane = l?.Wyslane ?? 0,
                Bledy = l?.Bledy ?? 0,
                Oczekujace = l?.Oczekujace ?? 0
            };
        }).ToList();
    }
    [HttpGet("{wprWiadomoscID}")]
    public async Task<ActionResult<IEnumerable<WiadomoscPracownikDto>>> SelectByWiadomosc(int wprWiadomoscID, string wprStatus = "2")
    {
        var pracownicy = await _context.WiadomoscPracownik
            .Where(w => w.wprWiadomoscID == wprWiadomoscID && (w.wprStatus == wprStatus || wprStatus == "*"))
            .OrderBy(w => w.wprID)
            .ProjectToType<WiadomoscPracownikDto>()
            .ToListAsync();

        return pracownicy;
    }

    [HttpGet("{wprID}")]
    public async Task<ActionResult<WiadomoscPracownikDto>> Get(int wprID)
    {
        var pracownik = await _context.WiadomoscPracownik.FindAsync(wprID);

        return pracownik == null ? NotFound() : pracownik.Adapt<WiadomoscPracownikDto>();
    }

    [HttpPost]
    public async Task<ActionResult<WiadomoscPracownikDto>> Insert(WiadomoscPracownikDto pracownikDto)
    {
        var pracownik = pracownikDto.Adapt<WiadomoscPracownik>();
        _context.WiadomoscPracownik.Add(pracownik);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { wprID = pracownik.wprID }, pracownik.Adapt<WiadomoscPracownikDto>());
    }

    [HttpPut("{wprID}")]
    public async Task<IActionResult> Update(int wprID, WiadomoscPracownikDto pracownikDto)
    {
        if (wprID != pracownikDto.wprID)
        {
            return BadRequest();
        }

        var pracownik = await _context.WiadomoscPracownik.FindAsync(wprID);

        if (pracownik == null)
        {
            return NotFound();
        }

        pracownikDto.Adapt(pracownik);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Popraw wiadomość przed rozpoczęciem wysyłki (robocza, w kolejce albo harmonogram przed kolejnym cyklem).
    [HttpPut("{wiaID}")]
    public async Task<IActionResult> Popraw(int wiaID, [FromBody] WiadomoscPoprawaModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.wiaTemat))
        {
            return BadRequest("Podaj temat wiadomości.");
        }

        var wiadomosc = await _context.Wiadomosc.FirstOrDefaultAsync(
            w => w.wiaID == wiaID && w.Usuniety != true, cancellationToken);

        if (wiadomosc == null)
        {
            return NotFound();
        }

        if (!MozeBycZmieniona(wiadomosc.wiaStatus))
        {
            return BadRequest("Wysyłka tej wiadomości została już rozpoczęta.");
        }

        wiadomosc.wiaTemat = model.wiaTemat.Trim();
        wiadomosc.wiaTresc = model.wiaTresc;
        wiadomosc.DataModyfikacja = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    // Usuń wiadomość przed rozpoczęciem wysyłki (logicznie; Worker i listy pomijają usunięte).
    [HttpDelete("{wiaID}")]
    public async Task<IActionResult> Usun(int wiaID, CancellationToken cancellationToken)
    {
        var wiadomosc = await _context.Wiadomosc.FirstOrDefaultAsync(
            w => w.wiaID == wiaID && w.Usuniety != true, cancellationToken);

        if (wiadomosc == null)
        {
            return NotFound();
        }

        if (!MozeBycZmieniona(wiadomosc.wiaStatus))
        {
            return BadRequest("Wysyłka tej wiadomości została już rozpoczęta.");
        }

        wiadomosc.Usuniety = true;
        wiadomosc.DataModyfikacja = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    // Poprawa i usunięcie tylko przed startem wysyłki: robocza (1), w kolejce (2) albo harmonogram (7).
    private static bool MozeBycZmieniona(string? status) => status is StatusRobocza or StatusWyslana or StatusHarmonogram;

    [HttpDelete("{wprID}")]
    public async Task<IActionResult> Delete(int wprID)
    {
        var pracownik = await _context.WiadomoscPracownik.FindAsync(wprID);

        if (pracownik == null)
        {
            return NotFound();
        }

        _context.WiadomoscPracownik.Remove(pracownik);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static string? WalidujZalaczniki(IReadOnlyCollection<IFormFile> zalaczniki)
    {
        if (zalaczniki.Count > WiadomoscZalacznikStorage.MaxPlikow)
        {
            return $"Można dołączyć najwyżej {WiadomoscZalacznikStorage.MaxPlikow} plików.";
        }

        foreach (var plik in zalaczniki)
        {
            var blad = WiadomoscZalacznikStorage.Waliduj(plik);

            if (blad != null)
            {
                return blad;
            }
        }

        return null;
    }

    /// <summary>
    /// Zapisuje PDF w {appFolder}\{usrFolder}\{czas}\ (wzorzec epicaHR), tworzy rekord dbo.Plik
    /// i wiąże go z załącznikiem wiadomości przez wdoPlikID. Worker wysyła plik właśnie przez ten rekord.
    /// </summary>
    private async Task ZapiszZalaczniki(
        int wiaID, int appID, IEnumerable<IFormFile> zalaczniki, int usrID, CancellationToken cancellationToken)
    {
        var appFolder = WiadomoscZalacznikStorage.Segment(User.FindFirstValue("appFolder") ?? $"CORE{appID:D4}");
        var usrFolder = WiadomoscZalacznikStorage.Segment(User.FindFirstValue("usrFolder") ?? usrID.ToString("D12"));

        foreach (var plik in zalaczniki)
        {
            var (katalog, nazwaPliku) = await _zalaczniki.ZapiszPlik(plik, appFolder, usrFolder, cancellationToken);
            var nazwa = Path.GetFileName(plik.FileName);

            var rekordPliku = new Plik
            {
                plkAplikacjaID = appID,
                plkRozszerzenie = "pdf",
                plkNazwa = nazwa,
                plkPlik = nazwaPliku,
                plkFolder = katalog,
                plkLink = Path.Combine(katalog, nazwaPliku),
                plkWersja = 1
            };

            _context.Plik.Add(rekordPliku);
            await _context.SaveChangesAsync(cancellationToken);

            _context.WiadomoscDokument.Add(new WiadomoscDokument
            {
                wdoWiadomoscID = wiaID,
                wdoTyp = "pdf",
                wdoNazwa = nazwa,
                wdoPlik = nazwaPliku,
                wdoKatalog = katalog,
                wdoPlikID = rekordPliku.plkID,
                wdoStatus = "2"
            });
        }
    }

    /// <summary>Wybiera odbiorców z zakresu z adresem e-mail; pozostałych zgłasza w wyniku.</summary>
    private static List<WiadomoscZakresPracownikViewModel> PrzygotujOdbiorcow(
        IReadOnlyList<WiadomoscZakresPracownikViewModel> zakres, IEnumerable<int> prcIDs,
        WiadomoscWysylkaZakresWynikModel wynik, bool wszystkieAdresy)
    {
        var odbiorcy = zakres
            .Where(z => z.prcID.HasValue)
            .GroupBy(z => z.prcID!.Value)
            .ToDictionary(g => g.Key, g => g.OrderBy(z => string.IsNullOrWhiteSpace(z.kntKontakt) ? 1 : 0).ToList());
        var wybrani = prcIDs.Distinct().ToList();

        var spozaZakresu = wybrani.Count(id => !odbiorcy.ContainsKey(id));

        if (spozaZakresu > 0)
        {
            wynik.pominieto += spozaZakresu;
            wynik.komunikaty.Add($"Pominięto {spozaZakresu} osób spoza wybranego zakresu.");
        }

        var doWyslania = new List<WiadomoscZakresPracownikViewModel>();

        foreach (var id in wybrani.Where(odbiorcy.ContainsKey))
        {
            var adresy = odbiorcy[id].Where(o => !string.IsNullOrWhiteSpace(o.kntKontakt)).ToList();
            var odbiorca = odbiorcy[id][0];

            if (adresy.Count == 0)
            {
                wynik.pominieto++;
                wynik.komunikaty.Add($"{odbiorca.prcNazwisko} {odbiorca.prcImie}: brak adresu e-mail.");
                continue;
            }

            doWyslania.AddRange(wszystkieAdresy ? adresy : adresy.Take(1));
        }

        return doWyslania.DistinctBy(o => o.kntKontakt!.Trim(), StringComparer.OrdinalIgnoreCase).ToList();
    }

    // Cykl: J = jednorazowo, D = codziennie, T = tygodniowo, M = miesięcznie. Termin musi być w przyszłości.
    private static string? WalidujHarmonogram(WiadomoscWyslijRoboczaModel model)
    {
        if (model.kodHarmonogram is not ("J" or "D" or "T" or "M"))
        {
            return "Nieprawidłowy rodzaj harmonogramu (J, D, T albo M).";
        }

        if (model.dataHarmonogramStart == null)
        {
            return "Podaj termin pierwszej wysyłki.";
        }

        if (model.dataHarmonogramStart <= DateTime.Now)
        {
            return "Termin pierwszej wysyłki musi być w przyszłości.";
        }

        if (model.kodHarmonogram != "J" && model.dataHarmonogramKoniec is DateTime koniec && koniec < model.dataHarmonogramStart)
        {
            return "Data końcowa cyklu jest wcześniejsza niż pierwszy termin.";
        }

        return null;
    }

    private void DodajOdbiorcow(int wiaID, IEnumerable<WiadomoscZakresPracownikViewModel> odbiorcy, string status = WprStatusNowa)
    {
        foreach (var odbiorca in odbiorcy)
        {
            _context.WiadomoscPracownik.Add(new WiadomoscPracownik
            {
                wprWiadomoscID = wiaID,
                wprPracownikID = odbiorca.prcID,
                wprEmail = odbiorca.kntKontakt!.Trim(),
                wprStatus = status
            });
        }
    }
}
