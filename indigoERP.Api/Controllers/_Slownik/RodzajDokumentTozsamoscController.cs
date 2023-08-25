using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Slownik;
using Microsoft.AspNetCore.Authorization;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RodzajDokumentTozsamoscController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public RodzajDokumentTozsamoscController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/RodzajDokumentTozsamosc
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RodzajDokumentTozsamosc>>> Select(string dtyStatus)
        {
          if (_context.RodzajDokumentTozsamosc == null)
          {
              return NotFound();
          }
            return await _context.RodzajDokumentTozsamosc.Where(w => w.dtyStatus == dtyStatus || dtyStatus == "*").ToListAsync();
        }

        // GET: api/RodzajDokumentTozsamosc/5
        [AllowAnonymous]
        [HttpGet("{dtyID}")]
        public async Task<ActionResult<RodzajDokumentTozsamosc>> Get(int dtyID)
        {
          if (_context.RodzajDokumentTozsamosc == null)
          {
              return NotFound();
          }
            var rodzajDokumentTozsamosc = await _context.RodzajDokumentTozsamosc.FindAsync(dtyID);

            if (rodzajDokumentTozsamosc == null)
            {
                return NotFound();
            }

            return rodzajDokumentTozsamosc;
        }

        // PUT: api/RodzajDokumentTozsamosc/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPut("{dtyID}")]
        public async Task<IActionResult> Update(int dtyID, RodzajDokumentTozsamosc rodzajDokumentTozsamosc)
        {
            if (dtyID != rodzajDokumentTozsamosc.dtyID)
            {
                return BadRequest();
            }

            _context.Entry(rodzajDokumentTozsamosc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RodzajDokumentTozsamoscExists(dtyID))
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

        // POST: api/RodzajDokumentTozsamosc
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPost]
        public async Task<ActionResult<RodzajDokumentTozsamosc>> Insert(RodzajDokumentTozsamosc rodzajDokumentTozsamosc)
        {
          if (_context.RodzajDokumentTozsamosc == null)
          {
              return Problem("Entity set 'indigoERPContext.RodzajDokumentTozsamosc' is null.");
          }
            _context.RodzajDokumentTozsamosc.Add(rodzajDokumentTozsamosc);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { dtyID = rodzajDokumentTozsamosc.dtyID }, rodzajDokumentTozsamosc);
        }

        // DELETE: api/RodzajDokumentTozsamosc/5
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.RodzajDokumentTozsamosc == null)
            {
                return NotFound();
            }
            var rodzajDokumentTozsamosc = await _context.RodzajDokumentTozsamosc.FindAsync(id);
            if (rodzajDokumentTozsamosc == null)
            {
                return NotFound();
            }

            _context.RodzajDokumentTozsamosc.Remove(rodzajDokumentTozsamosc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RodzajDokumentTozsamoscExists(int dtyID)
        {
            return (_context.RodzajDokumentTozsamosc?.Any(e => e.dtyID == dtyID)).GetValueOrDefault();
        }
    }
}
