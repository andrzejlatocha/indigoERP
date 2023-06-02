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
    public class WojewodztwoController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public WojewodztwoController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Wojewodztwo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wojewodztwo>>> GetWojewodztwo()
        {
          if (_context.Wojewodztwo == null)
          {
              return NotFound();
          }
            return await _context.Wojewodztwo.ToListAsync();
        }

        // GET: api/Wojewodztwo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Wojewodztwo>> GetWojewodztwo(int id)
        {
          if (_context.Wojewodztwo == null)
          {
              return NotFound();
          }
            var wojewodztwo = await _context.Wojewodztwo.FindAsync(id);

            if (wojewodztwo == null)
            {
                return NotFound();
            }

            return wojewodztwo;
        }

        // PUT: api/Wojewodztwo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWojewodztwo(int id, Wojewodztwo wojewodztwo)
        {
            if (id != wojewodztwo.wojID)
            {
                return BadRequest();
            }

            _context.Entry(wojewodztwo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WojewodztwoExists(id))
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

        // POST: api/Wojewodztwo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Wojewodztwo>> PostWojewodztwo(Wojewodztwo wojewodztwo)
        {
          if (_context.Wojewodztwo == null)
          {
              return Problem("Entity set 'indigoERPContext.Wojewodztwo'  is null.");
          }
            _context.Wojewodztwo.Add(wojewodztwo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWojewodztwo", new { id = wojewodztwo.wojID }, wojewodztwo);
        }

        // DELETE: api/Wojewodztwo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWojewodztwo(int id)
        {
            if (_context.Wojewodztwo == null)
            {
                return NotFound();
            }
            var wojewodztwo = await _context.Wojewodztwo.FindAsync(id);
            if (wojewodztwo == null)
            {
                return NotFound();
            }

            _context.Wojewodztwo.Remove(wojewodztwo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WojewodztwoExists(int id)
        {
            return (_context.Wojewodztwo?.Any(e => e.wojID == id)).GetValueOrDefault();
        }
    }
}
