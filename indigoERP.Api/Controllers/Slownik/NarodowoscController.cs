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
using indigoERP.Domain.Enumerators._Slownik;

namespace indigoERP.Api.Controllers._Slownik
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NarodowoscController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public NarodowoscController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Narodowosc
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Narodowosc>>> Select(string narStatus)
        {
            if (_context.Narodowosc == null)
            {
                return NotFound();
            }

            return await _context.Narodowosc.Where(w => w.narStatus == narStatus || narStatus == "*").ToListAsync();
        }

        // GET: api/Narodowosc
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NarodowoscEnumerator>>> SelectLookup(string narStatus)
        {
            if (_context.NarodowoscEnumerator == null)
            {
                return NotFound();
            }

            return await _context.NarodowoscEnumerator.Where(w => w.narStatus == narStatus || narStatus == "*").ToListAsync();
        }

        // GET: api/Narodowosc/5
        [AllowAnonymous]
        [HttpGet("{narID}")]
        public async Task<ActionResult<Narodowosc>> Get(int narID)
        {
            if (_context.Narodowosc == null)
            {
                return NotFound();
            }
            var narodowosc = await _context.Narodowosc.FindAsync(narID);

            if (narodowosc == null)
            {
                return NotFound();
            }

            return narodowosc;
        }

        // PUT: api/Narodowosc/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPut("{narID}")]
        public async Task<IActionResult> Update(int narID, Narodowosc narodowosc)
        {
            if (narID != narodowosc.narID)
            {
                return BadRequest();
            }

            _context.Entry(narodowosc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NarodowoscExists(narID))
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

        // POST: api/Narodowosc
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPost]
        public async Task<ActionResult<Narodowosc>> Insert(Narodowosc narodowosc)
        {
            if (_context.Narodowosc == null)
            {
                return Problem("Entity set 'indigoERPContext.Narodowosc'  is null.");
            }
            _context.Narodowosc.Add(narodowosc);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { narodowosc.narID }, narodowosc);
        }

        // DELETE: api/Narodowosc/5
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpDelete("{narID}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Narodowosc == null)
            {
                return NotFound();
            }
            var narodowosc = await _context.Narodowosc.FindAsync(id);
            if (narodowosc == null)
            {
                return NotFound();
            }

            narodowosc.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NarodowoscExists(int narID)
        {
            return (_context.Narodowosc?.Any(e => e.narID == narID)).GetValueOrDefault();
        }
    }
}
