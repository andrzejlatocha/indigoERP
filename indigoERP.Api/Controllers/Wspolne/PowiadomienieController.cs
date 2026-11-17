using indigoERP.Api.Data;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Entities._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Wspolne;

/// <summary>Powiadomienia użytkownika (licznik i lista w nagłówku, oznaczanie odczytu). Dostęp: token Bearer.</summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class PowiadomienieController : ControllerBase
{
    private const string BearerScheme = "Bearer";

    // Zapis: administrator indigoERP oraz integracja epicaHR (wiadomości, zadania i zdarzenia). Kolejne integracje dopisywać tutaj.
    private const string ZapisRole = "HCZ-indigoERP-Administrator,HCZ-indigoERP-Integracja-epicaHR";

    private readonly indigoERPContext _context;

    public PowiadomienieController(indigoERPContext context)
    {
        _context = context;
    }

    // POST: api/Powiadomienie/Insert - nowe powiadomienie dla użytkownika.
    [Authorize(Roles = ZapisRole, AuthenticationSchemes = BearerScheme)]
    [HttpPost]
    public async Task<ActionResult<int>> Insert(PowiadomienieDto powiadomienieDto)
    {
        if (!await _context.Aplikacja.AnyAsync(w => w.appID == powiadomienieDto.powAplikacjaID))
        {
            return BadRequest("Nieznana aplikacja.");
        }

        if (!await _context.Uzytkownik.AnyAsync(w => w.usrID == powiadomienieDto.powUzytkownikID))
        {
            return BadRequest("Nieznany użytkownik.");
        }

        var powiadomienie = powiadomienieDto.Adapt<Powiadomienie>();
        powiadomienie.powDataPowiadomienia = DateTime.Now;
        powiadomienie.powOdczyt = false;

        _context.Powiadomienie.Add(powiadomienie);
        await _context.SaveChangesAsync();

        return Ok(powiadomienie.powID);
    }

    // GET: api/Powiadomienie/SelectCountByUzytkownik/{appID}/{usrID} - liczba nieprzeczytanych powiadomień.
    [Authorize(AuthenticationSchemes = BearerScheme)]
    [HttpGet("{appID}/{usrID}")]
    public async Task<ActionResult<int>> SelectCountByUzytkownik(int appID, int usrID)
    {
        return await _context.PowiadomienieViewModel
            .CountAsync(w => w.powAplikacjaID == appID && w.powUzytkownikID == usrID && !w.powOdczyt);
    }

    // GET: api/Powiadomienie/SelectOstatnie/{appID}/{usrID}?take=50 - najnowsze powiadomienia (przeczytane i nieprzeczytane).
    [Authorize(AuthenticationSchemes = BearerScheme)]
    [HttpGet("{appID}/{usrID}")]
    public async Task<ActionResult<IEnumerable<PowiadomienieViewModel>>> SelectOstatnie(int appID, int usrID, int take = 50)
    {
        var limit = Math.Clamp(take, 1, 500);

        return await _context.PowiadomienieViewModel
            .AsNoTracking()
            .Where(w => w.powAplikacjaID == appID && w.powUzytkownikID == usrID)
            .OrderByDescending(w => w.powDataPowiadomienia)
            .ThenByDescending(w => w.powID)
            .Take(limit)
            .ToListAsync();
    }

    // POST: api/Powiadomienie/Odczytaj/{powID} - oznacza jedno powiadomienie zalogowanego użytkownika jako przeczytane.
    [Authorize(AuthenticationSchemes = BearerScheme)]
    [HttpPost("{powID}")]
    public async Task<IActionResult> Odczytaj(int powID)
    {
        var usrID = BiezacyUzytkownikID();
        if (usrID is null)
        {
            return Forbid();
        }

        var powiadomienie = await _context.Powiadomienie.FirstOrDefaultAsync(w => w.powID == powID);
        if (powiadomienie is null)
        {
            return NotFound();
        }

        if (powiadomienie.powUzytkownikID != usrID.Value)
        {
            return Forbid();
        }

        if (!powiadomienie.powOdczyt)
        {
            powiadomienie.powOdczyt = true;
            powiadomienie.powDataOdczytu = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        return NoContent();
    }

    // POST: api/Powiadomienie/OdczytajWszystkie/{appID}/{usrID} - oznacza wszystkie nieprzeczytane powiadomienia użytkownika.
    [Authorize(AuthenticationSchemes = BearerScheme)]
    [HttpPost("{appID}/{usrID}")]
    public async Task<ActionResult<int>> OdczytajWszystkie(int appID, int usrID)
    {
        var zalogowany = BiezacyUzytkownikID();
        if (zalogowany is null || zalogowany.Value != usrID)
        {
            return Forbid();
        }

        // Jedna instrukcja UPDATE w bazie zamiast wczytywania wszystkich nieprzeczytanych rekordów do pamięci.
        var teraz = DateTime.Now;
        var zmienione = await _context.Powiadomienie
            .Where(w => w.powAplikacjaID == appID && w.powUzytkownikID == usrID && !w.powOdczyt && w.Usuniety != true)
            .ExecuteUpdateAsync(s => s
                .SetProperty(w => w.powOdczyt, true)
                .SetProperty(w => w.powDataOdczytu, teraz)
                .SetProperty(w => w.DataModyfikacja, teraz)
                .SetProperty(w => w.ModyfikacjaID, usrID));

        return zmienione;
    }

    private int? BiezacyUzytkownikID() =>
        int.TryParse(User.FindFirst("usrID")?.Value, out var usrID) ? usrID : null;
}
