using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace indigoERP.Api.Controllers._Wspolne
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ZdarzenieController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public ZdarzenieController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Zdarzenie
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ZdarzenieViewModel>>> Select(int zdaAplikacjaID, string zdaStatus)
        {
            return await _context.ZdarzenieViewModel.Where(w => w.zdaAplikacjaID == zdaAplikacjaID && (w.zdaStatus == zdaStatus || zdaStatus == "*")).ToListAsync();
        }

        // GET: api/Zdarzenie
        [HttpGet("{zdaAplikacjaID}/{zurUzytkownikID}")]
        public async Task<ActionResult<IEnumerable<ZdarzenieViewModel>>> SelectByUzytkownik(int zdaAplikacjaID, int zurUzytkownikID, string zdaStatus)
        {
            return await _context.ZdarzenieViewModel.Where(w => w.zdaAplikacjaID == zdaAplikacjaID && w.zurUzytkownikID == zurUzytkownikID && (w.zdaStatus == zdaStatus || zdaStatus == "*")).ToListAsync();
        }

        // GET: api/Zdarzenie/5
        [HttpGet("{zdaID}")]
        public async Task<ActionResult<Zdarzenie>> Get(int zdaID)
        {
            var zdarzenie = await _context.Zdarzenie.FindAsync(zdaID);

            if (zdarzenie == null)
            {
                return NotFound();
            }

            return zdarzenie;
        }

        // PUT: api/Zdarzenie/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{zdaID}")]
        public async Task<IActionResult> Update(int zdaID, Zdarzenie zdarzenie)
        {
            if (zdaID != zdarzenie.zdaID)
            {
                return BadRequest();
            }

            _context.Entry(zdarzenie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZdarzenieExists(zdaID))
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

        // POST: api/Zdarzenie
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Zdarzenie>> Insert(Zdarzenie zdarzenie)
        {
            _context.Zdarzenie.Add(zdarzenie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { zdaID = zdarzenie.zdaID }, zdarzenie);
        }

        // DELETE: api/Zdarzenie/5
        [HttpDelete("{zdaID}")]
        public async Task<IActionResult> Delete(int zdaID)
        {
            var zdarzenie = await _context.Zdarzenie.FindAsync(zdaID);
            if (zdarzenie == null)
            {
                return NotFound();
            }

            _context.Zdarzenie.Remove(zdarzenie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ZdarzenieExists(int zdaID)
        {
            return _context.Zdarzenie.Any(e => e.zdaID == zdaID);
        }
    }
}
