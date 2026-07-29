using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Wspolne;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Wspolne
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KontaktController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public KontaktController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Kontakt/5
        [HttpGet("{kntID}")]
        public async Task<ActionResult<Kontakt>> Get(int kntID)
        {
            if (_context.Kontakt == null)
            {
                return NotFound();
            }
            var kontakt = await _context.Kontakt.FindAsync(kntID);

            if (kontakt == null)
            {
                return NotFound();
            }

            return kontakt;
        }

        // PUT: api/Kontakt/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{kntID}")]
        public async Task<IActionResult> Update(int kntID, Kontakt kontakt)
        {
            if (kntID != kontakt.kntID)
            {
                return BadRequest();
            }

            _context.Entry(kontakt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KontaktExists(kntID))
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

        // POST: api/Kontakt
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Kontakt>> Insert(Kontakt kontakt)
        {
            if (_context.Kontakt == null)
            {
                return Problem("Entity set 'indigoERPContext.Kontakt' is null.");
            }
            _context.Kontakt.Add(kontakt);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { kntID = kontakt.kntID }, kontakt);
        }

        // DELETE: api/Kontakt/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int kntID)
        {
            if (_context.Kontakt == null)
            {
                return NotFound();
            }
            var kontakt = await _context.Kontakt.FindAsync(kntID);
            if (kontakt == null)
            {
                return NotFound();
            }

            kontakt.Usuniety = true;
            await _context.SaveChangesAsync();

            return Ok(200);
        }

        private bool KontaktExists(int kntID)
        {
            return (_context.Kontakt?.Any(e => e.kntID == kntID)).GetValueOrDefault();
        }
    }
}
