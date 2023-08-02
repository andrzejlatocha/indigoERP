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
    public class PowiatController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public PowiatController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Powiat
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PowiatViewModel>>> Select(string powStatus)
        {
          if (_context.PowiatViewModel == null)
          {
              return NotFound();
          }
            return await _context.PowiatViewModel.Where(w => w.powStatus == powStatus || powStatus == "*").ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PowiatViewModel>>> SelectByKraj(int kraID, string powStatus)
        {
            if (_context.PowiatViewModel == null)
            {
                return NotFound();
            }
            return await _context.PowiatViewModel.Where(w => w.kraID == kraID && (w.powStatus == powStatus || powStatus == "*")).ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PowiatViewModel>>> SelectByWojewodztwo(int wojID, string powStatus)
        {
            if (_context.PowiatViewModel == null)
            {
                return NotFound();
            }
            return await _context.PowiatViewModel.Where(w => w.wojID == wojID && (w.powStatus == powStatus || powStatus == "*")).ToListAsync();
        }

        // GET: api/Powiat/5
        [HttpGet("{powID}")]
        public async Task<ActionResult<Powiat>> Get(int powID)
        {
          if (_context.Powiat == null)
          {
              return NotFound();
          }
            var powiat = await _context.Powiat.FindAsync(powID);

            if (powiat == null)
            {
                return NotFound();
            }

            return powiat;
        }

        // PUT: api/Powiat/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{powID}")]
        public async Task<IActionResult> Update(int powID, Powiat powiat)
        {
            if (powID != powiat.powID)
            {
                return BadRequest();
            }

            _context.Entry(powiat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PowiatExists(powID))
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

        // POST: api/Powiat
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Powiat>> Insert(Powiat powiat)
        {
          if (_context.Powiat == null)
          {
              return Problem("Entity set 'indigoERPContext.Powiat' is null.");
          }
            _context.Powiat.Add(powiat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { powID = powiat.powID }, powiat);
        }

        // DELETE: api/Powiat/5
        [HttpDelete("{powID}")]
        public async Task<IActionResult> Delete(int powID)
        {
            if (_context.Powiat == null)
            {
                return NotFound();
            }
            var powiat = await _context.Powiat.FindAsync(powID);
            if (powiat == null)
            {
                return NotFound();
            }

            _context.Powiat.Remove(powiat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PowiatExists(int powID)
        {
            return (_context.Powiat?.Any(e => e.powID == powID)).GetValueOrDefault();
        }
    }
}
