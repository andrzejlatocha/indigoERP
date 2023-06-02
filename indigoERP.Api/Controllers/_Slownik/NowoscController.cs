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

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NowoscController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public NowoscController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Nowosc
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Nowosc>>> Select(int appID)
        {
          if (_context.Nowosc == null)
          {
              return NotFound();
          }
            return await _context.Nowosc.Where(w => w.newAplikacjaID == appID).ToListAsync();
        }

        // GET: api/Nowosc/5
        [AllowAnonymous]
        [HttpGet("{newID}")]
        public async Task<ActionResult<Nowosc>> Get(int newID)
        {
          if (_context.Nowosc == null)
          {
              return NotFound();
          }
            var nowosc = await _context.Nowosc.FindAsync(newID);

            if (nowosc == null)
            {
                return NotFound();
            }

            return nowosc;
        }

        // PUT: api/Nowosc/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "indigoERP-Administrator,indigoERP-Slownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPut("{newID}")]
        public async Task<IActionResult> Update(int newID, Nowosc nowosc)
        {
            if (newID != nowosc.newID)
            {
                return BadRequest();
            }

            _context.Entry(nowosc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NowoscExists(newID))
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

        // POST: api/Nowosc
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "indigoERP-Administrator,indigoERP-Slownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPost]
        public async Task<ActionResult<Nowosc>> Insert(Nowosc nowosc)
        {
          if (_context.Nowosc == null)
          {
              return Problem("Entity set 'indigoERPContext.Nowosc' is null.");
          }
            _context.Nowosc.Add(nowosc);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { newID = nowosc.newID }, nowosc);
        }

        // DELETE: api/Nowosc/5
        [Authorize(Roles = "indigoERP-Administrator,indigoERP-Slownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpDelete("{newID}")]
        public async Task<IActionResult> Delete(int newID)
        {
            if (_context.Nowosc == null)
            {
                return NotFound();
            }
            var nowosc = await _context.Nowosc.FindAsync(newID);
            if (nowosc == null)
            {
                return NotFound();
            }

            _context.Nowosc.Remove(nowosc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NowoscExists(int newID)
        {
            return (_context.Nowosc?.Any(e => e.newID == newID)).GetValueOrDefault();
        }
    }
}
