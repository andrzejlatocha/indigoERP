using indigoERP.Api.Data;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Wspolne;

/// <summary>Harmonogram wysyłki Głosu Hutnika: lista aktywnych cykli i anulowanie. Dostęp: rola Gazeta lub Administrator.</summary>
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(Roles = "HCZ-indigoERP-Wiadomość,HCZ-indigoERP-Administrator", AuthenticationSchemes = "Bearer")]
public class WiadomoscHarmonogramController : ControllerBase
{
    private const string StatusHarmonogram = "7";
    private const string StatusHarmonogramAnulowany = "6";

    private readonly indigoERPContext _context;

    public WiadomoscHarmonogramController(indigoERPContext context)
    {
        _context = context;
    }

    // GET: api/WiadomoscHarmonogram/SelectAktywne?appID=900
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WiadomoscHarmonogramViewModel>>> SelectAktywne(int appID, CancellationToken cancellationToken)
    {
        return await _context.Wiadomosc
            .Where(w => w.wiaAplikacjaID == appID && w.wiaStatus == StatusHarmonogram && w.wiaHarmonogramAktywny && w.Usuniety != true)
            .OrderBy(w => w.wiaDataNastepnejWysylki)
            .Select(w => new WiadomoscHarmonogramViewModel
            {
                wiaID = w.wiaID,
                wiaTemat = w.wiaTemat,
                wiaKodHarmonogram = w.wiaKodHarmonogram,
                wiaDataHarmonogramStart = w.wiaDataHarmonogramStart,
                wiaDataHarmonogramKoniec = w.wiaDataHarmonogramKoniec,
                wiaDataNastepnejWysylki = w.wiaDataNastepnejWysylki,
            })
            .ToListAsync(cancellationToken);
    }

    // POST: api/WiadomoscHarmonogram/Anuluj/5 — wyłącza cykl; wysyłki już skopiowane (status 2) idą dalej.
    [HttpPost("{wiaID}")]
    public async Task<IActionResult> Anuluj(int wiaID, CancellationToken cancellationToken)
    {
        var szablon = await _context.Wiadomosc.FirstOrDefaultAsync(
            w => w.wiaID == wiaID && w.Usuniety != true, cancellationToken);

        if (szablon == null)
        {
            return NotFound();
        }

        if (szablon.wiaStatus != StatusHarmonogram || !szablon.wiaHarmonogramAktywny)
        {
            return BadRequest("Ta wiadomość nie ma aktywnego harmonogramu.");
        }

        szablon.wiaHarmonogramAktywny = false;
        szablon.wiaStatus = StatusHarmonogramAnulowany;
        szablon.wiaDataNastepnejWysylki = null;
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
