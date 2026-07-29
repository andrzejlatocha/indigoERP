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
    [Route("api/[controller]/[action]/[action]")]
    [ApiController]
    public class MarkaPojazdController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public MarkaPojazdController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/MarkaPojazd
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarkaPojazdViewModel>>> Select(string mpjStatus)
        {
            if (_context.MarkaPojazdViewModel == null)
            {
                return NotFound();
            }

            return await _context.MarkaPojazdViewModel.Where(w => w.mpjStatus == mpjStatus || mpjStatus == "*").ToListAsync();
        }

        // GET: api/MarkaPojazd
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarkaPojazdEnumerator>>> SelectLookup(string mpjStatus)
        {
            if (_context.MarkaPojazdEnumerator == null)
            {
                return NotFound();
            }

            return await _context.MarkaPojazdEnumerator.Where(w => w.mpjStatus == mpjStatus || mpjStatus == "*").ToListAsync();
        }

        // GET: api/MarkaPojazd
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarkaPojazd>>> SelectMarka(string mpjStatus)
        {
            if (_context.MarkaPojazd == null)
            {
                return NotFound();
            }

            return await _context.MarkaPojazd.Where(w => w.mpjMarkaPojazdID == null && (w.mpjStatus == mpjStatus || mpjStatus == "*")).ToListAsync();
        }

        // GET: api/MarkaPojazd
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarkaPojazdEnumerator>>> SelectLookpMarka(string mpjStatus)
        {
            if (_context.MarkaPojazdEnumerator == null)
            {
                return NotFound();
            }

            return await _context.MarkaPojazdEnumerator.Where(w => w.mpjMarkaPojazdID == null && (w.mpjStatus == mpjStatus || mpjStatus == "*")).ToListAsync();
        }

        // GET: api/MarkaPojazd
        [AllowAnonymous]
        [HttpGet("{mpjID}")]
        public async Task<ActionResult<IEnumerable<MarkaPojazd>>> SelectModelByMarka(int mpjID)
        {
            if (_context.MarkaPojazd == null)
            {
                return NotFound();
            }

            return await _context.MarkaPojazd.Where(w => w.mpjMarkaPojazdID == mpjID && w.mpjStatus == "2").ToListAsync();
        }

        // GET: api/MarkaPojazd
        [AllowAnonymous]
        [HttpGet("{mpjID}")]
        public async Task<ActionResult<IEnumerable<MarkaPojazdEnumerator>>> SelectLookupModelByMarka(int mpjID)
        {
            if (_context.MarkaPojazdEnumerator == null)
            {
                return NotFound();
            }

            return await _context.MarkaPojazdEnumerator.Where(w => w.mpjMarkaPojazdID == mpjID && w.mpjStatus == "2").ToListAsync();
        }

        // GET: api/MarkaPojazd/5
        [HttpGet("{mpjID}")]
        public async Task<ActionResult<MarkaPojazd>> Get(int mpjID)
        {
          if (_context.MarkaPojazd == null)
          {
              return NotFound();
          }
            var markaPojazd = await _context.MarkaPojazd.FindAsync(mpjID);

            if (markaPojazd == null)
            {
                return NotFound();
            }

            return markaPojazd;
        }

        // PUT: api/MarkaPojazd/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{mpjID}")]
        public async Task<IActionResult> Update(int mpjID, MarkaPojazd markaPojazd)
        {
            if (mpjID != markaPojazd.mpjID)
            {
                return BadRequest();
            }

            _context.Entry(markaPojazd).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarkaPojazdExists(mpjID))
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

        // POST: api/MarkaPojazd
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<MarkaPojazd>> Insert(MarkaPojazd markaPojazd)
        {
          if (_context.MarkaPojazd == null)
          {
              return Problem("Entity set 'indigoERPContext.MarkaPojazd'  is null.");
          }
            _context.MarkaPojazd.Add(markaPojazd);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { mpjID = markaPojazd.mpjID }, markaPojazd);
        }

        // DELETE: api/MarkaPojazd/5
        [Authorize]
        [HttpDelete("{mpjID}")]
        public async Task<IActionResult> Delete(int mpjID)
        {
            if (_context.MarkaPojazd == null)
            {
                return NotFound();
            }
            var markaPojazd = await _context.MarkaPojazd.FindAsync(mpjID);
            if (markaPojazd == null)
            {
                return NotFound();
            }

            markaPojazd.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MarkaPojazdExists(int mpjID)
        {
            return (_context.MarkaPojazd?.Any(e => e.mpjID == mpjID)).GetValueOrDefault();
        }
    }
}
