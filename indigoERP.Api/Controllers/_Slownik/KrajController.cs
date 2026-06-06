using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Slownik;
using indigoERP.Domain.Enumerators._Slownik;
using indigoERP.Domain.ViewModels._Slownik;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KrajController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public KrajController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Kraj
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KrajViewModel>>> Select(string kraStatus)
        {
            if (_context.KrajViewModel == null)
            {
                return NotFound();
            }
            return await _context.KrajViewModel.Where(w => w.kraStatus == kraStatus || kraStatus == "*").ToListAsync();
        }

        // GET: api/Kraj
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KrajEnumerator>>> SelectLookup(string kraStatus)
        {
            if (_context.KrajEnumerator == null)
            {
                return NotFound();
            }
            return await _context.KrajEnumerator.Where(w => w.kraStatus == kraStatus || kraStatus == "*").ToListAsync();
        }


        // GET: api/Kraj/5
        [AllowAnonymous]
        [HttpGet("{kraID}")]
        public async Task<ActionResult<Kraj>> Get(int kraID)
        {
          if (_context.Kraj == null)
          {
              return NotFound();
          }
            var kraj = await _context.Kraj.FindAsync(kraID);

            if (kraj == null)
            {
                return NotFound();
            }

            return kraj;
        }

        // PUT: api/Kraj/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPut("{kraID}")]
        public async Task<IActionResult> Update(int kraID, Kraj kraj)
        {
            if (kraID != kraj.kraID)
            {
                return BadRequest();
            }

            _context.Entry(kraj).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KrajExists(kraID))
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

        // POST: api/Kraj
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPost]
        public async Task<ActionResult<Kraj>> Insert(Kraj kraj)
        {
          if (_context.Kraj == null)
          {
              return Problem("Entity set 'indigoERPContext.Kraj' is null.");
          }
            _context.Kraj.Add(kraj);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { kraID = kraj.kraID }, kraj);
        }

        // DELETE: api/Kraj/5
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpDelete("{kraID}")]
        public async Task<IActionResult> Delete(int kraID)
        {
            if (_context.Kraj == null)
            {
                return NotFound();
            }
            var kraj = await _context.Kraj.FindAsync(kraID);
            if (kraj == null)
            {
                return NotFound();
            }

            _context.Kraj.Remove(kraj);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KrajExists(int kraID)
        {
            return (_context.Kraj?.Any(e => e.kraID == kraID)).GetValueOrDefault();
        }
    }
}
