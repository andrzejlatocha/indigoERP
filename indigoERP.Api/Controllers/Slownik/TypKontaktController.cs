using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Slownik;
using indigoERP.Domain.Enumerators._Slownik;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TypKontaktController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public TypKontaktController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/TypKontakt
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TypKontakt>>> Select(string tkoStatus)
        {
            return await _context.TypKontakt.Where(w => w.tkoStatus == tkoStatus || tkoStatus == "*").ToListAsync();
        }

        // GET: api/TypKontakt
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TypKontaktEnumerator>>> SelectLookup(string tkoStatus)
        {
            return await _context.TypKontaktEnumerator.Where(w => w.tkoStatus == tkoStatus || tkoStatus == "*").ToListAsync();
        }

        // GET: api/TypKontakt
        [HttpGet("{tkoAplikacjaID}")]
        public async Task<ActionResult<IEnumerable<TypKontakt>>> SelectByAplikacja(int tkoAplikacjaID, string tkoStatus)
        {
            return await _context.TypKontakt.Where(w => (w.tkoAplikacjaID == tkoAplikacjaID || w.tkoAplikacjaID == 0) && (w.tkoStatus == tkoStatus || tkoStatus == "*")).ToListAsync();
        }

        // GET: api/TypKontakt
        [HttpGet("{tkoAplikacjaID}")]
        public async Task<ActionResult<IEnumerable<TypKontaktEnumerator>>> SelectLookupByAplikacja(int tkoAplikacjaID, string tkoStatus)
        {
            return await _context.TypKontaktEnumerator.Where(w => (w.tkoAplikacjaID == tkoAplikacjaID || w.tkoAplikacjaID == 0) && (w.tkoStatus == tkoStatus || tkoStatus == "*")).ToListAsync();
        }

        // GET: api/TypKontakt/5
        [HttpGet("{tkoID}")]
        public async Task<ActionResult<TypKontakt>> Get(int tkoID)
        {
            var typKontakt = await _context.TypKontakt.FindAsync(tkoID);

            if (typKontakt == null)
            {
                return NotFound();
            }

            return typKontakt;
        }

        // PUT: api/TypKontakt/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{tkoID}")]
        public async Task<IActionResult> Update(int tkoID, TypKontakt typKontakt)
        {
            if (tkoID != typKontakt.tkoID)
            {
                return BadRequest();
            }

            _context.Entry(typKontakt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypKontaktExists(tkoID))
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

        // POST: api/TypKontakt
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TypKontakt>> Insert(TypKontakt typKontakt)
        {
            _context.TypKontakt.Add(typKontakt);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypKontakt", new { tkoID = typKontakt.tkoID }, typKontakt);
        }

        // DELETE: api/TypKontakt/5
        [HttpDelete("{tkoID}")]
        public async Task<IActionResult> DeleteTypKontakt(int tkoID)
        {
            var typKontakt = await _context.TypKontakt.FindAsync(tkoID);
            if (typKontakt == null)
            {
                return NotFound();
            }

            typKontakt.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TypKontaktExists(int tkoID)
        {
            return _context.TypKontakt.Any(e => e.tkoID == tkoID);
        }
    }
}
