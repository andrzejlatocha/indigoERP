using indigoERP.Api.Data;
using indigoERP.Domain.Enumerators._Slownik;
using indigoERP.Domain.ViewModels._Slownik;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WalutaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public WalutaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Waluta
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WalutaViewModel>>> Select(string walStatus)
        {
            if (_context.WalutaViewModel == null)
            {
                return NotFound();
            }
            return await _context.WalutaViewModel.Where(w => w.walStatus == walStatus || walStatus == "*").ToListAsync();
        }

        // GET: api/Waluta
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WalutaEnumerator>>> SelectLookup(string walStatus)
        {
            if (_context.WalutaEnumerator == null)
            {
                return NotFound();
            }
            return await _context.WalutaEnumerator.Where(w => w.walStatus == walStatus || walStatus == "*").ToListAsync();
        }
    }
}
