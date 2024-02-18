using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Slownik;
using indigoERP.Domain.Enumerators._Slownik;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Slownik
{    
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TypPlikController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public TypPlikController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/TypPlik
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TypPlik>>> Select(string tpkStatus)
        {
            if (_context.TypPlik == null)
            {
                return NotFound();
            }

            return await _context.TypPlik.Where(w => w.tpkStatus == tpkStatus || tpkStatus == "*").ToListAsync();
        }

        // GET: api/TypPlik
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TypPlikEnumerator>>> SelectLookup(string tpkStatus)
        {
            if (_context.TypPlikEnumerator == null)
            {
                return NotFound();
            }

            return await _context.TypPlikEnumerator.Where(w => w.tpkStatus == tpkStatus || tpkStatus == "*").ToListAsync();
        }

        // GET: api/TypPlik
        [AllowAnonymous]
        [HttpGet("{tpkAplikacjaID}")]
        public async Task<ActionResult<IEnumerable<TypPlik>>> SelectByAplikacja(int tpkAplikacjaID, string tpkStatus)
        {
            if (_context.TypPlik == null)
            {
                return NotFound();
            }

            return await _context.TypPlik.Where(w => (w.tpkAplikacjaID == tpkAplikacjaID || w.tpkAplikacjaID == 0) && (w.tpkStatus == tpkStatus || tpkStatus == "*")).ToListAsync();
        }

        // GET: api/TypPlik
        [AllowAnonymous]
        [HttpGet("{tpkAplikacjaID}")]
        public async Task<ActionResult<IEnumerable<TypPlikEnumerator>>> SelectLookupByAplikacja(int tpkAplikacjaID, string tpkStatus)
        {
            if (_context.TypPlikEnumerator == null)
            {
                return NotFound();
            }

            return await _context.TypPlikEnumerator.Where(w => (w.tpkAplikacjaID == tpkAplikacjaID || w.tpkAplikacjaID == 0) && (w.tpkStatus == tpkStatus || tpkStatus == "*")).ToListAsync();
        }

        // GET: api/TypPlik/5
        [AllowAnonymous]
        [HttpGet("{tpkID}")]
        public async Task<ActionResult<TypPlik>> Get(int tpkID)
        {
          if (_context.TypPlik == null)
          {
              return NotFound();
          }
            var typPlik = await _context.TypPlik.FindAsync(tpkID);

            if (typPlik == null)
            {
                return NotFound();
            }

            return typPlik;
        }

        // PUT: api/TypPlik/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPut("{tpkID}")]
        public async Task<IActionResult> Update(int tpkID, TypPlik typPlik)
        {
            if (tpkID != typPlik.tpkID)
            {
                return BadRequest();
            }

            _context.Entry(typPlik).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypPlikExists(tpkID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TypPlik
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPost]
        public async Task<ActionResult<TypPlik>> Insert(TypPlik typPlik)
        {
          if (_context.TypPlik == null)
          {
              return Problem("Entity set 'indigoERPContext.TypPlik' is null.");
          }
            _context.TypPlik.Add(typPlik);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { tpkID = typPlik.tpkID }, typPlik);
        }

        // DELETE: api/TypPlik/5
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpDelete("{tpkID}")]
        public async Task<IActionResult> Delete(int tpkID)
        {
            if (_context.TypPlik == null)
            {
                return NotFound();
            }
            var typPlik = await _context.TypPlik.FindAsync(tpkID);
            if (typPlik == null)
            {
                return NotFound();
            }

            _context.TypPlik.Remove(typPlik);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TypPlikExists(int tpkID)
        {
            return (_context.TypPlik?.Any(e => e.tpkID == tpkID)).GetValueOrDefault();
        }
    }
}
