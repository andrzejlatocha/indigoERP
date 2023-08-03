using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Wspolne;
using Microsoft.AspNetCore.Authorization;

namespace indigoERP.Api.Controllers._Wspolne
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PlikController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public PlikController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Plik/5
        [HttpGet("{plkID}")]
        public async Task<ActionResult<Plik>> Get(int plkID)
        {
          if (_context.Plik == null)
          {
              return NotFound();
          }
            var plik = await _context.Plik.FindAsync(plkID);

            if (plik == null)
            {
                return NotFound();
            }            

            return plik;
        }

        // PUT: api/Plik/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{plkID}")]
        public async Task<IActionResult> Update(int plkID, Plik plik)
        {
            if (plkID != plik.plkID)
            {
                return BadRequest();
            }

            _context.Entry(plik).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlikExists(plkID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(200);
        }

        // POST: api/Plik
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Plik>> Insert(Plik plik)
        {
          if (_context.Plik == null)
          {
              return Problem("Entity set 'indigoERPContext.Plik' is null.");
          }
            _context.Plik.Add(plik);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { plkID = plik.plkID }, plik);
        }

        // DELETE: api/Plik/5
        [HttpDelete("{plkID}")]
        public async Task<IActionResult> Delete(int plkID)
        {
            if (_context.Plik == null)
            {
                return NotFound();
            }
            var plik = await _context.Plik.FindAsync(plkID);
            if (plik == null)
            {
                return NotFound();
            }

            _context.Plik.Remove(plik);
            await _context.SaveChangesAsync();

            return Ok(200);
        }

        private bool PlikExists(int plkID)
        {
            return (_context.Plik?.Any(e => e.plkID == plkID)).GetValueOrDefault();
        }
    }
}
