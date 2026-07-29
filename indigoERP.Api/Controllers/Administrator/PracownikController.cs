using indigoERP.Api.Data;
using indigoERP.Domain.Enumerators._Administrator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Administrator
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "HCZ-indigoERP-Administrator", AuthenticationSchemes = "Bearer")]
    public class PracownikController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public PracownikController(indigoERPContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PracownikEnumerator>>> SelectLookup()
        {
            var pracownicy = await _context.PracownikEnumerator
                .AsNoTracking()
                .OrderBy(o => o.prcOpis)
                .ToListAsync();

            return pracownicy
                .GroupBy(g => g.prcID)
                .Select(s => s.First())
                .ToList();
        }
    }
}
