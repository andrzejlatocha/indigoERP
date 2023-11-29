using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Wspolne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Wspolne
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdresController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public AdresController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Adres/5
        [HttpGet("{adrID}")]
        public async Task<ActionResult<Adres>> Get(int adrID)
        {
          if (_context.Adres == null)
          {
              return NotFound();
          }
            var adres = await _context.Adres.FindAsync(adrID);

            if (adres == null)
            {
                return NotFound();
            }

            return adres;
        }

        // PUT: api/Adres/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{adrID}")]
        public async Task<IActionResult> Update(int adrID, Adres adres)
        {
            if (adrID != adres.adrID)
            {
                return BadRequest();
            }

            _context.Entry(adres).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdresExists(adrID))
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

        // POST: api/Adres
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Adres>> Insert(Adres adres)
        {
          if (_context.Adres == null)
          {
              return Problem("Entity set 'indigoERPContext.Adres' is null.");
          }
            _context.Adres.Add(adres);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = adres.adrID }, adres);
        }

        // DELETE: api/Adres/5
        [Authorize]
        [HttpDelete("{adrID}")]
        public async Task<IActionResult> Delete(int adrID)
        {
            if (_context.Adres == null)
            {
                return NotFound();
            }
            var adres = await _context.Adres.FindAsync(adrID);
            if (adres == null)
            {
                return NotFound();
            }

            _context.Adres.Remove(adres);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdresExists(int adrID)
        {
            return (_context.Adres?.Any(e => e.adrID == adrID)).GetValueOrDefault();
        }
    }
}
