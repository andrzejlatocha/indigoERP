using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Slownik;
using indigoERP.Domain.Enumerators._Slownik;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UrzadSkarbowyController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public UrzadSkarbowyController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/UrzadSkarbowy
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UrzadSkarbowy>>> Select(string uskStatus)
        {
            return await _context.UrzadSkarbowy.Where(w => w.uskStatus == uskStatus).ToListAsync();
        }

        // GET: api/UrzadSkarbowy
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UrzadSkarbowyEnumerator>>> SelectLookup(string uskStatus)
        {
            return await _context.UrzadSkarbowyEnumerator.Where(w => w.uskStatus == uskStatus).ToListAsync();
        }

        // GET: api/UrzadSkarbowy/5
        [HttpGet("{uskKOD}")]
        public async Task<ActionResult<UrzadSkarbowy>> Get(string uskKOD)
        {
            var urzadSkarbowy = await _context.UrzadSkarbowy.FindAsync(uskKOD);

            if (urzadSkarbowy == null)
            {
                return NotFound();
            }

            return urzadSkarbowy;
        }

        // PUT: api/UrzadSkarbowy/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{uskKOD}")]
        public async Task<IActionResult> Update(string uskKOD, UrzadSkarbowy urzadSkarbowy)
        {
            if (uskKOD != urzadSkarbowy.uskKOD)
            {
                return BadRequest();
            }

            _context.Entry(urzadSkarbowy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UrzadSkarbowyExists(uskKOD))
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

        // POST: api/UrzadSkarbowy
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UrzadSkarbowy>> Insert(UrzadSkarbowy urzadSkarbowy)
        {
            _context.UrzadSkarbowy.Add(urzadSkarbowy);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UrzadSkarbowyExists(urzadSkarbowy.uskKOD))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { uskKOD = urzadSkarbowy.uskKOD }, urzadSkarbowy);
        }

        // DELETE: api/UrzadSkarbowy/5
        [HttpDelete("{uskKOD}")]
        public async Task<IActionResult> DeleteUrzadSkarbowy(string uskKOD)
        {
            var urzadSkarbowy = await _context.UrzadSkarbowy.FindAsync(uskKOD);
            if (urzadSkarbowy == null)
            {
                return NotFound();
            }

            _context.UrzadSkarbowy.Remove(urzadSkarbowy);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UrzadSkarbowyExists(string uskKOD)
        {
            return _context.UrzadSkarbowy.Any(e => e.uskKOD == uskKOD);
        }
    }
}
