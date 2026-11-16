using indigoERP.Api.Data;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Wspolne;

/// <summary>Powiadomienia użytkownika (licznik na nagłówku). Dostęp: token Bearer.</summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class PowiadomienieController : ControllerBase
{
    private const string BearerScheme = "Bearer";

    private readonly indigoERPContext _context;

    public PowiadomienieController(indigoERPContext context)
    {
        _context = context;
    }

    // GET: api/Powiadomienie/SelectCountByUzytkownik/{appID}/{usrID} - liczba nieprzeczytanych powiadomień.
    [Authorize(AuthenticationSchemes = BearerScheme)]
    [HttpGet("{appID}/{usrID}")]
    public async Task<ActionResult<int>> SelectCountByUzytkownik(int appID, int usrID)
    {
        return await _context.PowiadomienieViewModel
            .CountAsync(w => w.powAplikacjaID == appID && w.powUzytkownikID == usrID && !w.powOdczyt);
    }

    // GET: api/Powiadomienie/SelectNieprzeczytane/{appID}/{usrID} - nieprzeczytane powiadomienia, najnowsze pierwsze.
    [Authorize(AuthenticationSchemes = BearerScheme)]
    [HttpGet("{appID}/{usrID}")]
    public async Task<ActionResult<IEnumerable<PowiadomienieViewModel>>> SelectNieprzeczytane(int appID, int usrID)
    {
        return await _context.PowiadomienieViewModel
            .AsNoTracking()
            .Where(w => w.powAplikacjaID == appID && w.powUzytkownikID == usrID && !w.powOdczyt)
            .OrderByDescending(w => w.powDataPowiadomienia)
            .Take(50)
            .ToListAsync();
    }
}
