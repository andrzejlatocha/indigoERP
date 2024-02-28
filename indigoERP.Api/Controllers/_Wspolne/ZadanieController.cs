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
    public class ZadanieController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public ZadanieController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Zadanie
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ZadanieViewModel>>> Select(int tskAplikacjaID, string tskStatus)
        {
            return await _context.ZadanieViewModel.Where(w => w.tskAplikacjaID == tskAplikacjaID && (w.tskStatus == tskStatus || tskStatus == "*")).ToListAsync();
        }

        // GET: api/Zadanie        
        [HttpGet("{tskAplikacjaID}/{tskUzytkownikID}")]
        public async Task<ActionResult<IEnumerable<ZadanieViewModel>>> SelectByUzytkownik(int tskAplikacjaID, int tusUzytkownikID, string tusStatus)
        {
            return await _context.ZadanieViewModel.Where(w => w.tskAplikacjaID == tskAplikacjaID && w.tusUzytkownikID == tusUzytkownikID && (w.tusStatus == tusStatus || tusStatus == "*")).ToListAsync();
        }

        // GET: api/Zadanie/5
        [HttpGet("{tskID}")]
        public async Task<ActionResult<Zadanie>> Get(int tskID)
        {
            var zadanie = await _context.Zadanie.FindAsync(tskID);

            if (zadanie == null)
            {
                return NotFound();
            }

            return zadanie;
        }

        // PUT: api/Zadanie/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{tskID}")]
        public async Task<IActionResult> Update(int tskID, Zadanie zadanie)
        {
            if (tskID != zadanie.tskID)
            {
                return BadRequest();
            }

            _context.Entry(zadanie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZadanieExists(tskID))
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

        // POST: api/Zadanie
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Zadanie>> Insert(Zadanie zadanie)
        {
            _context.Zadanie.Add(zadanie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { tskID = zadanie.tskID }, zadanie);
        }

        // DELETE: api/Zadanie/5
        [HttpDelete("{tskID}")]
        public async Task<IActionResult> Delete(int tskID)
        {
            var zadanie = await _context.Zadanie.FindAsync(tskID);
            if (zadanie == null)
            {
                return NotFound();
            }

            _context.Zadanie.Remove(zadanie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ZadanieExists(int tskID)
        {
            return _context.Zadanie.Any(e => e.tskID == tskID);
        }
    }
}
