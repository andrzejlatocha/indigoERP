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
    public class WiadomoscController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public WiadomoscController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Wiadomosc
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WiadomoscViewModel>>> Select(int wiaAplikacjaID, string wiaStatus)
        {
            return await _context.WiadomoscViewModel.Where(w => w.wiaAplikacjaID == wiaAplikacjaID && (w.wiaStatus == wiaStatus || wiaStatus == "*")).ToListAsync();
        }

        // GET: api/Wiadomosc
        [HttpGet("{wiaAplikacjaID}/{wuzUzytkownikID}")]
        public async Task<ActionResult<IEnumerable<WiadomoscViewModel>>> SelectByUzytkownik(int wiaAplikacjaID, int wuzUzytkownikID, string wiaStatus)
        {
            return await _context.WiadomoscViewModel.Where(w => w.wiaAplikacjaID == wiaAplikacjaID && w.wuzUzytkownikID == wuzUzytkownikID && (w.wiaStatus == wiaStatus || wiaStatus == "*")).ToListAsync();
        }

        // GET: api/Wiadomosc
        [HttpGet("{wiaAplikacjaID}/{wuzUzytkownikID}")]
        public async Task<ActionResult<IEnumerable<WiadomoscViewModel>>> SelectOdczytByUzytkownik(int wiaAplikacjaID, int wuzUzytkownikID, string wiaStatus)
        {
            return await _context.WiadomoscViewModel.Where(w => w.wiaAplikacjaID == wiaAplikacjaID && w.wuzUzytkownikID == wuzUzytkownikID && w.wuzOdczyt == true && (w.wiaStatus == wiaStatus || wiaStatus == "*")).ToListAsync();
        }

        // GET: api/Wiadomosc
        [HttpGet("{wiaAplikacjaID}/{wuzUzytkownikID}")]
        public async Task<ActionResult<IEnumerable<WiadomoscViewModel>>> SelectBrakOdczytByUzytkownik(int wiaAplikacjaID, int wuzUzytkownikID, string wiaStatus)
        {
            return await _context.WiadomoscViewModel.Where(w => w.wiaAplikacjaID == wiaAplikacjaID && w.wuzUzytkownikID == wuzUzytkownikID && w.wuzOdczyt == false && (w.wiaStatus == wiaStatus || wiaStatus == "*")).ToListAsync();
        }

        // GET: api/Wiadomosc/5
        [HttpGet("{wiaID}")]
        public async Task<ActionResult<Wiadomosc>> Get(int wiaID)
        {
            var wiadomosc = await _context.Wiadomosc.FindAsync(wiaID);

            if (wiadomosc == null)
            {
                return NotFound();
            }

            return wiadomosc;
        }

        // PUT: api/Wiadomosc/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{wiaID}")]
        public async Task<IActionResult> Update(int wiaID, Wiadomosc wiadomosc)
        {
            if (wiaID != wiadomosc.wiaID)
            {
                return BadRequest();
            }

            _context.Entry(wiadomosc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WiadomoscExists(wiaID))
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

        // POST: api/Wiadomosc
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Wiadomosc>> Insert(Wiadomosc wiadomosc)
        {
            _context.Wiadomosc.Add(wiadomosc);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { wiaID = wiadomosc.wiaID }, wiadomosc);
        }

        // DELETE: api/Wiadomosc/5
        [HttpDelete("{wiaID}")]
        public async Task<IActionResult> Delete(int wiaID)
        {
            var wiadomosc = await _context.Wiadomosc.FindAsync(wiaID);
            if (wiadomosc == null)
            {
                return NotFound();
            }

            _context.Wiadomosc.Remove(wiadomosc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WiadomoscExists(int wiaID)
        {
            return _context.Wiadomosc.Any(e => e.wiaID == wiaID);
        }
    }
}
