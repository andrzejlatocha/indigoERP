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
using Microsoft.AspNetCore.Authorization;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DaneFirmaAdresController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public DaneFirmaAdresController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/DaneFirmaAdres
        [AllowAnonymous]
        [HttpGet("{stfID}")]
        public async Task<ActionResult<IEnumerable<DaneFirmaAdresViewModel>>> Select(int stfID)
        {
            if (_context.DaneFirmaAdresViewModel == null)
            {
                return NotFound();
            }
            return await _context.DaneFirmaAdresViewModel.Where(w => w.fadDaneFirmaID == stfID).ToListAsync();
        }

        // GET: api/DaneFirmaAdres/5
        [AllowAnonymous]
        [HttpGet("{fadID}")]
        public async Task<ActionResult<DaneFirmaAdres>> Get(int fadID)
        {
            if (_context.DaneFirmaAdres == null)
            {
                return NotFound();
            }
            var daneFirmaAdres = await _context.DaneFirmaAdres.FindAsync(fadID);

            if (daneFirmaAdres == null)
            {
                return NotFound();
            }

            return daneFirmaAdres;
        }

        // PUT: api/DaneFirmaAdres/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPut("{fadID}")]
        public async Task<IActionResult> Update(int fadID, DaneFirmaAdres daneFirmaAdres)
        {
            if (fadID != daneFirmaAdres.fadID)
            {
                return BadRequest();
            }

            _context.Entry(daneFirmaAdres).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DaneFirmaAdresExists(fadID))
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

        // POST: api/DaneFirmaAdres
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPost]
        public async Task<ActionResult<DaneFirmaAdres>> Insert(DaneFirmaAdres daneFirmaAdres)
        {
            if (_context.DaneFirmaAdres == null)
            {
                return Problem("Entity set 'indigoERPContext.DaneFirmaAdres'  is null.");
            }
            _context.DaneFirmaAdres.Add(daneFirmaAdres);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { daneFirmaAdres.fadID }, daneFirmaAdres);
        }

        // DELETE: api/DaneFirmaAdres/5
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpDelete("{fadID}")]
        public async Task<IActionResult> Delete(int fadID)
        {
            if (_context.DaneFirmaAdres == null)
            {
                return NotFound();
            }
            var daneFirmaAdres = await _context.DaneFirmaAdres.FindAsync(fadID);
            if (daneFirmaAdres == null)
            {
                return NotFound();
            }

            _context.DaneFirmaAdres.Remove(daneFirmaAdres);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DaneFirmaAdresExists(int id)
        {
            return (_context.DaneFirmaAdres?.Any(e => e.fadID == id)).GetValueOrDefault();
        }
    }
}
