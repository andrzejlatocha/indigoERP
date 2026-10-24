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

namespace indigoERP.Api.Controllers._Wspolne;

/// <summary>Wysyłka „Głos Hutnika” do pracowników zakresu (z załącznikami PDF). Dostęp: rola Gazeta lub Administrator.</summary>
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(Roles = GazetaRoles, AuthenticationSchemes = BearerScheme)]
public class WiadomoscPracownikController : ControllerBase
{
    private const string GazetaRoles = "HCZ-indigoERP-Gazeta,HCZ-indigoERP-Administrator";
    private const string BearerScheme = "Bearer";

    // Numer roboczy: zapisany, niewidoczny publicznie. Numer wysłany: status "2" (widoczny na /gloshutnika).
    private const string StatusRobocza = "1";
    private const string StatusWyslana = "2";

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

        return pracownicy == null ? NotFound() : Ok(pracownicy);
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
            wiaPubliczna = true
        };

        _context.Wiadomosc.Add(wiadomosc);
        await _context.SaveChangesAsync(cancellationToken);

        await ZapiszZalaczniki(wiadomosc.wiaID, zalaczniki, usrID, cancellationToken);
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
        var doWyslania = PrzygotujOdbiorcow(zakres, model.prcIDs, wynik);

        if (doWyslania.Count == 0)
        {
            return BadRequest("Żaden z wybranych odbiorców nie ma adresu e-mail w zakresie.");
        }

        int.TryParse(User.FindFirst("usrID")?.Value, out var usrID);

        await using var transakcja = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync(cancellationToken)
            : null;

        DodajOdbiorcow(wiaID, doWyslania);
        wiadomosc.wiaStatus = StatusWyslana;
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
    [RequestFormLimits(MultipartBodyLengthLimit = WiadomoscZalacznikStorage.MaxLacznyRozmiar)]
    public async Task<ActionResult<WiadomoscWysylkaZakresWynikModel>> InsertWysylka(
        [FromForm] WiadomoscWysylkaZakresModel model, IFormFileCollection? pliki, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.wiaTemat) || string.IsNullOrWhiteSpace(model.wiaTresc))
        {
            return BadRequest("Temat i treść wiadomości są wymagane.");
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
        var doWyslania = PrzygotujOdbiorcow(zakres, model.prcIDs, wynik);

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
            // Wysyłka z zakładki Głos Hutnika to numer publiczny — trafia na stronę /gloshutnika.
            wiaPubliczna = true
        };

        _context.Wiadomosc.Add(wiadomosc);
        await _context.SaveChangesAsync(cancellationToken);

        await ZapiszZalaczniki(wiadomosc.wiaID, zalaczniki, usrID, cancellationToken);
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

    private async Task ZapiszZalaczniki(int wiaID, IEnumerable<IFormFile> zalaczniki, int usrID, CancellationToken cancellationToken)
    {
        foreach (var plik in zalaczniki)
        {
            var (katalog, nazwaPliku) = await _zalaczniki.ZapiszPlik(plik, usrID, cancellationToken);

            _context.WiadomoscDokument.Add(new WiadomoscDokument
            {
                wdoWiadomoscID = wiaID,
                wdoTyp = "pdf",
                wdoNazwa = Path.GetFileName(plik.FileName),
                wdoPlik = nazwaPliku,
                wdoKatalog = katalog,
                wdoStatus = "2"
            });
        }
    }

    /// <summary>Wybiera odbiorców z zakresu z adresem e-mail; pozostałych zgłasza w wyniku.</summary>
    private static List<WiadomoscZakresPracownikViewModel> PrzygotujOdbiorcow(
        IReadOnlyList<WiadomoscZakresPracownikViewModel> zakres, IEnumerable<int> prcIDs, WiadomoscWysylkaZakresWynikModel wynik)
    {
        var odbiorcy = zakres.Where(z => z.prcID.HasValue).ToDictionary(z => z.prcID!.Value);
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
            var odbiorca = odbiorcy[id];

            if (string.IsNullOrWhiteSpace(odbiorca.kntKontakt))
            {
                wynik.pominieto++;
                wynik.komunikaty.Add($"{odbiorca.prcNazwisko} {odbiorca.prcImie}: brak adresu e-mail.");
                continue;
            }

            doWyslania.Add(odbiorca);
        }

        return doWyslania;
    }

    private void DodajOdbiorcow(int wiaID, IEnumerable<WiadomoscZakresPracownikViewModel> odbiorcy)
    {
        foreach (var odbiorca in odbiorcy)
        {
            _context.WiadomoscPracownik.Add(new WiadomoscPracownik
            {
                wprWiadomoscID = wiaID,
                wprPracownikID = odbiorca.prcID,
                wprEmail = odbiorca.kntKontakt!.Trim(),
                wprStatus = "2"
            });
        }
    }
}
