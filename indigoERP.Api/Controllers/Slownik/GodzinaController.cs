using indigoERP.Api.Data;
using indigoERP.Domain.Enumerators._Slownik;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GodzinaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public GodzinaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Plec
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GodzinaEnumerator>>> SelectLookup(string czaStatus)
        {
            if (_context.GodzinaEnumerator == null)
            {
                return NotFound();
            }

            return await _context.GodzinaEnumerator.Where(w => w.czaStatus == czaStatus).ToListAsync();
        }
    }
}
