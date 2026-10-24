using indigoERP.Api.Data;
using indigoERP.Domain.Enumerators._Wspolne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Wspolne;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(Roles = GazetaRoles, AuthenticationSchemes = "Bearer")]
public sealed class WiadomoscZakresController : ControllerBase
{
    private const string GazetaRoles = "HCZ-indigoERP-Gazeta,HCZ-indigoERP-Administrator";

    private readonly indigoERPContext _context;

    public WiadomoscZakresController(indigoERPContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WiadomoscZakresEnumerator>>> Select(int appID)
    {
        return await _context.WiadomoscZakresEnumerator
            .AsNoTracking()
            .Where(w => (w.wzaAplikacjaID == appID || w.wzaAplikacjaID == 0) && w.wzaStatus == "2")
            .OrderBy(w => w.wzaID)
            .ToListAsync();
    }
}
