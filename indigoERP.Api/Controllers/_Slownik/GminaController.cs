using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Slownik;
using indigoERP.Domain.ViewModels._Slownik;

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
        public async Task<ActionResult<IEnumerable<GminaViewModel>>> Select(string gmiStatus)
        {
          if (_context.GminaViewModel == null)
          {
              return NotFound();
          }
            return await _context.GminaViewModel.Where(w => w.gmiStatus == gmiStatus || gmiStatus == "*").ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GminaViewModel>>> SelectByKraj(int kraID)
        {
            if (_context.GminaViewModel == null)
            {
                return NotFound();
            }
            return await _context.GminaViewModel.Where(w => w.kraID == kraID).ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GminaViewModel>>> SelectByWojewodztwo(int wojID)
        {
            if (_context.GminaViewModel == null)
            {
                return NotFound();
            }
            return await _context.GminaViewModel.Where(w => w.wojID == wojID).ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GminaViewModel>>> SelectByPowiat(int powID)
        {
            if (_context.GminaViewModel == null)
            {
                return NotFound();
            }
            return await _context.GminaViewModel.Where(w => w.powID == powID).ToListAsync();
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
        public async Task<IActionResult> Update(int gmiID, Gmina gmina)
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
        public async Task<ActionResult<Gmina>> Insert(Gmina gmina)
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
        public async Task<IActionResult> Delete(int gmiID)
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
