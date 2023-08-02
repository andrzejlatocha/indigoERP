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
    public class WojewodztwoController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public WojewodztwoController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/WojewodztwoViewModel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WojewodztwoViewModel>>> Select(string wojStatus)
        {
          if (_context.WojewodztwoViewModel == null)
          {
              return NotFound();
          }
            return await _context.WojewodztwoViewModel.Where(w => w.wojStatus == wojStatus || wojStatus == "*").ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WojewodztwoViewModel>>> SelectByKraj(int kraID, string wojStatus)
        {
            if (_context.WojewodztwoViewModel == null)
            {
                return NotFound();
            }
            return await _context.WojewodztwoViewModel.Where(w => w.kraID == kraID && (w.wojStatus == wojStatus || wojStatus == "*")).ToListAsync();
        }

        // GET: api/Wojewodztwo/5
        [HttpGet("{wojID}")]
        public async Task<ActionResult<Wojewodztwo>> GetWojewodztwo(int wojID)
        {
          if (_context.Wojewodztwo == null)
          {
              return NotFound();
          }
            var wojewodztwo = await _context.Wojewodztwo.FindAsync(wojID);

            if (wojewodztwo == null)
            {
                return NotFound();
            }

            return wojewodztwo;
        }

        // PUT: api/Wojewodztwo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{wojID}")]
        public async Task<IActionResult> Update(int wojID, Wojewodztwo wojewodztwo)
        {
            if (wojID != wojewodztwo.wojID)
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
                if (!WojewodztwoExists(wojID))
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
        public async Task<ActionResult<Wojewodztwo>> Insert(Wojewodztwo wojewodztwo)
        {
          if (_context.Wojewodztwo == null)
          {
              return Problem("Entity set 'indigoERPContext.Wojewodztwo'  is null.");
          }
            _context.Wojewodztwo.Add(wojewodztwo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { wojID = wojewodztwo.wojID }, wojewodztwo);
        }

        // DELETE: api/Wojewodztwo/5
        [HttpDelete("{wojID}")]
        public async Task<IActionResult> Delete(int wojID)
        {
            if (_context.Wojewodztwo == null)
            {
                return NotFound();
            }
            var wojewodztwo = await _context.Wojewodztwo.FindAsync(wojID);
            if (wojewodztwo == null)
            {
                return NotFound();
            }

            _context.Wojewodztwo.Remove(wojewodztwo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WojewodztwoExists(int wojID)
        {
            return (_context.Wojewodztwo?.Any(e => e.wojID == wojID)).GetValueOrDefault();
        }
    }
}
