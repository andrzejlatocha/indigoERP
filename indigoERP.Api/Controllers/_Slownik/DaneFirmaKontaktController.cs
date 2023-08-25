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
    public class DaneFirmaKontaktController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public DaneFirmaKontaktController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/DaneFirmaKontakt
        [AllowAnonymous]
        [HttpGet("{stfID}")]
        public async Task<ActionResult<IEnumerable<DaneFirmaKontaktViewModel>>> Select(int stfID)
        {
            if (_context.DaneFirmaKontaktViewModel == null)
            {
                return NotFound();
            }
            return await _context.DaneFirmaKontaktViewModel.Where(w => w.fkoDaneFirmaID == stfID).ToListAsync();
        }

        // GET: api/DaneFirmaKontakt/5
        [AllowAnonymous]
        [HttpGet("{fkoID}")]
        public async Task<ActionResult<DaneFirmaKontakt>> Get(int fkoID)
        {
            if (_context.DaneFirmaKontakt == null)
            {
                return NotFound();
            }
            var daneFirmaKontakt = await _context.DaneFirmaKontakt.FindAsync(fkoID);

            if (daneFirmaKontakt == null)
            {
                return NotFound();
            }

            return daneFirmaKontakt;
        }

        // PUT: api/DaneFirmaKontakt/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPut("{fkoID}")]
        public async Task<IActionResult> Update(int fkoID, DaneFirmaKontakt daneFirmaKontakt)
        {
            if (fkoID != daneFirmaKontakt.fkoID)
            {
                return BadRequest();
            }

            _context.Entry(daneFirmaKontakt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DaneFirmaKontaktExists(fkoID))
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

        // POST: api/DaneFirmaKontakt
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPost]
        public async Task<ActionResult<DaneFirmaKontakt>> Insert(DaneFirmaKontakt daneFirmaKontakt)
        {
            if (_context.DaneFirmaKontakt == null)
            {
                return Problem("Entity set 'indigoERPContext.DaneFirmaKontakt'  is null.");
            }
            _context.DaneFirmaKontakt.Add(daneFirmaKontakt);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { daneFirmaKontakt.fkoID }, daneFirmaKontakt);
        }

        // DELETE: api/DaneFirmaKontakt/5
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpDelete("{fkoID}")]
        public async Task<IActionResult> Delete(int fkoID)
        {
            if (_context.DaneFirmaKontakt == null)
            {
                return NotFound();
            }
            var daneFirmaKontakt = await _context.DaneFirmaKontakt.FindAsync(fkoID);
            if (daneFirmaKontakt == null)
            {
                return NotFound();
            }

            _context.DaneFirmaKontakt.Remove(daneFirmaKontakt);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DaneFirmaKontaktExists(int fkoID)
        {
            return (_context.DaneFirmaKontakt?.Any(e => e.fkoID == fkoID)).GetValueOrDefault();
        }
    }
}
