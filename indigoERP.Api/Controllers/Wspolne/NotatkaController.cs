using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Wspolne
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotatkaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public NotatkaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Notatka
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotatkaViewModel>>> Select(int notAplikacjaID, string notStatus)
        {
            return await _context.NotatkaViewModel.Where(w => w.notAplikacjaID == notAplikacjaID && (w.notStatus == notStatus || notStatus == "*")).ToListAsync();
        }

        // GET: api/Notatka/5
        [HttpGet("{notID}")]
        public async Task<ActionResult<Notatka>> Get(int notID)
        {
            var notatka = await _context.Notatka.FindAsync(notID);

            if (notatka == null)
            {
                return NotFound();
            }

            return notatka;
        }

        // PUT: api/Notatka/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{notID}")]
        public async Task<IActionResult> Update(int notID, Notatka notatka)
        {
            if (notID != notatka.notID)
            {
                return BadRequest();
            }

            _context.Entry(notatka).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotatkaExists(notID))
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

        // POST: api/Notatka
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Notatka>> Insert(Notatka notatka)
        {
            _context.Notatka.Add(notatka);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { notID = notatka.notID }, notatka);
        }

        // DELETE: api/Notatka/5
        [HttpDelete("{notID}")]
        public async Task<IActionResult> Delete(int notID)
        {
            var notatka = await _context.Notatka.FindAsync(notID);
            if (notatka == null)
            {
                return NotFound();
            }

            notatka.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NotatkaExists(int notID)
        {
            return _context.Notatka.Any(e => e.notID == notID);
        }
    }
}
