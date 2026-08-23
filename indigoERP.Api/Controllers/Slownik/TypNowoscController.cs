using indigoERP.Api.Data;
using indigoERP.Domain.Enumerators._Slownik;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TypNowoscController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public TypNowoscController(indigoERPContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TypNowoscEnumerator>>> SelectLookup(string tnoStatus)
        {
            return await _context.TypNowoscEnumerator
                .Where(w => w.tnoStatus == tnoStatus || tnoStatus == "*")
                .ToListAsync();
        }
    }
}
