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
using indigoERP.Domain.Enumerators._Slownik;

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
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GminaViewModel>>> Select(string gmiStatus)
        {
            if (_context.GminaViewModel == null)
            {
                return NotFound();
            }

            return await _context.GminaViewModel.Where(w => w.gmiStatus == gmiStatus || gmiStatus == "*").ToListAsync();
        }

        // GET: api/Gmina
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GminaEnumerator>>> SelectLookup(string gmiStatus)
        {
            if (_context.GminaEnumerator == null)
            {
                return NotFound();
            }

            return await _context.GminaEnumerator.Where(w => w.gmiStatus == gmiStatus || gmiStatus == "*").ToListAsync();
        }

        // GET: api/Gmina
        [AllowAnonymous]
        [HttpGet("{kraID}")]
        public async Task<ActionResult<IEnumerable<GminaViewModel>>> SelectByKraj(int kraID, string gmiStatus)
        {
            if (_context.GminaViewModel == null)
            {
                return NotFound();
            }

            return await _context.GminaViewModel.Where(w => w.kraID == kraID && (w.gmiStatus == gmiStatus || gmiStatus == "*")).ToListAsync();
        }

        // GET: api/Gmina
        [AllowAnonymous]
        [HttpGet("{kraID}")]
        public async Task<ActionResult<IEnumerable<GminaEnumerator>>> SelectLookupByKraj(int kraID, string gmiStatus)
        {
            if (_context.GminaEnumerator == null)
            {
                return NotFound();
            }

            return await _context.GminaEnumerator.Where(w => w.kraID == kraID && (w.gmiStatus == gmiStatus || gmiStatus == "*")).ToListAsync();
        }

        // GET: api/Gmina
        [AllowAnonymous]
        [HttpGet("{wojID}")]
        public async Task<ActionResult<IEnumerable<GminaViewModel>>> SelectByWojewodztwo(int wojID, string gmiStatus)
        {
            if (_context.GminaViewModel == null)
            {
                return NotFound();
            }

            return await _context.GminaViewModel.Where(w => w.wojID == wojID && (w.gmiStatus == gmiStatus || gmiStatus == "*")).ToListAsync();
        }


        // GET: api/Gmina
        [AllowAnonymous]
        [HttpGet("{wojID}")]
        public async Task<ActionResult<IEnumerable<GminaEnumerator>>> SelectLookupByWojewodztwo(int wojID, string gmiStatus)
        {
            if (_context.GminaEnumerator == null)
            {
                return NotFound();
            }

            return await _context.GminaEnumerator.Where(w => w.wojID == wojID && (w.gmiStatus == gmiStatus || gmiStatus == "*")).ToListAsync();
        }

        // GET: api/Gmina
        [AllowAnonymous]
        [HttpGet("{powID}")]
        public async Task<ActionResult<IEnumerable<GminaViewModel>>> SelectByPowiat(int powID, string gmiStatus)
        {
            if (_context.GminaViewModel == null)
            {
                return NotFound();
            }

            return await _context.GminaViewModel.Where(w => w.powID == powID && (w.gmiStatus == gmiStatus || gmiStatus == "*")).ToListAsync();
        }

        // GET: api/Gmina
        [AllowAnonymous]
        [HttpGet("{powID}")]
        public async Task<ActionResult<IEnumerable<GminaEnumerator>>> SelectLookupByPowiat(int powID, string gmiStatus)
        {
            if (_context.GminaEnumerator == null)
            {
                return NotFound();
            }

            return await _context.GminaEnumerator.Where(w => w.powID == powID && (w.gmiStatus == gmiStatus || gmiStatus == "*")).ToListAsync();
        }

        // GET: api/Gmina/5
        [AllowAnonymous]
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
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
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
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
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
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
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
