using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Slownik;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GminaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public GminaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Gmina
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gmina>>> Select(string gmiStatus)
        {
          if (_context.Gmina == null)
          {
              return NotFound();
          }
            return await _context.Gmina.Where(w => w.gmiStatus == gmiStatus || gmiStatus == "*").ToListAsync();
        }

        // GET: api/Gmina/5
        [HttpGet("{gmiID}")]
        public async Task<ActionResult<Gmina>> Get(int gmiID)
        {
          if (_context.Gmina == null)
          {
              return NotFound();
          }
            var gmina = await _context.Gmina.FindAsync(gmiID);

            if (gmina == null)
            {
                return NotFound();
            }

            return gmina;
        }

        // PUT: api/Gmina/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{gmiID}")]
        public async Task<IActionResult> PutGmina(int gmiID, Gmina gmina)
        {
            if (gmiID != gmina.gmiID)
            {
                return BadRequest();
            }

            _context.Entry(gmina).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GminaExists(gmiID))
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

        // POST: api/Gmina
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Gmina>> PostGmina(Gmina gmina)
        {
          if (_context.Gmina == null)
          {
              return Problem("Entity set 'indigoERPContext.Gmina' is null.");
          }
            _context.Gmina.Add(gmina);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { gmiID = gmina.gmiID }, gmina);
        }

        // DELETE: api/Gmina/5
        [HttpDelete("{gmiID}")]
        public async Task<IActionResult> DeleteGmina(int gmiID)
        {
            if (_context.Gmina == null)
            {
                return NotFound();
            }
            var gmina = await _context.Gmina.FindAsync(gmiID);
            if (gmina == null)
            {
                return NotFound();
            }

            _context.Gmina.Remove(gmina);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GminaExists(int gmiID)
        {
            return (_context.Gmina?.Any(e => e.gmiID == gmiID)).GetValueOrDefault();
        }
    }
}
