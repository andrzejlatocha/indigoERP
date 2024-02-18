using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Slownik;
using indigoERP.Domain.Enumerators._Slownik;
using indigoERP.Domain.ViewModels._Slownik;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MiejscowoscController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public MiejscowoscController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MiejscowoscViewModel>>> Select(string mscStatus)
        {
            if (_context.MiejscowoscViewModel == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscViewModel.Where(w => w.mscStatus == mscStatus || mscStatus == "*").ToListAsync();
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MiejscowoscEnumerator>>> SelectLookup(string mscStatus)
        {
            if (_context.MiejscowoscEnumerator == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscEnumerator.Where(w => w.mscStatus == mscStatus || mscStatus == "*").ToListAsync();
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet("{kraID}")]
        public async Task<ActionResult<IEnumerable<MiejscowoscViewModel>>> SelectByKraj(int kraID, string mscStatus)
        {
            if (_context.MiejscowoscViewModel == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscViewModel.Where(w => w.kraID == kraID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet("{kraID}")]
        public async Task<ActionResult<IEnumerable<MiejscowoscEnumerator>>> SelectLookupByKraj(int kraID, string mscStatus)
        {
            if (_context.MiejscowoscEnumerator == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscEnumerator.Where(w => w.kraID == kraID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet("{wojID}")]
        public async Task<ActionResult<IEnumerable<MiejscowoscViewModel>>> SelectByWojewodztwo(int wojID, string mscStatus)
        {
            if (_context.MiejscowoscViewModel == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscViewModel.Where(w => w.wojID == wojID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet("{wojID}")]
        public async Task<ActionResult<IEnumerable<MiejscowoscEnumerator>>> SelectLookupByWojewodztwo(int wojID, string mscStatus)
        {
            if (_context.MiejscowoscEnumerator == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscEnumerator.Where(w => w.wojID == wojID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet("{powID}")]
        public async Task<ActionResult<IEnumerable<MiejscowoscViewModel>>> SelectByPowiat(int powID, string mscStatus)
        {
            if (_context.MiejscowoscViewModel == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscViewModel.Where(w => w.powID == powID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet("{powID}")]
        public async Task<ActionResult<IEnumerable<MiejscowoscEnumerator>>> SelectLookupByPowiat(int powID, string mscStatus)
        {
            if (_context.MiejscowoscEnumerator == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscEnumerator.Where(w => w.powID == powID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet("{gmiID}")]
        public async Task<ActionResult<IEnumerable<MiejscowoscViewModel>>> SelectByGmina(int gmiID, string mscStatus)
        {
            if (_context.MiejscowoscViewModel == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscViewModel.Where(w => w.gmiID == gmiID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet("{gmiID}")]
        public async Task<ActionResult<IEnumerable<MiejscowoscEnumerator>>> SelectLookupByGmina(int gmiID, string mscStatus)
        {
            if (_context.MiejscowoscEnumerator == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscEnumerator.Where(w => w.gmiID == gmiID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet("{mscID}")]
        public async Task<ActionResult<IEnumerable<MiejscowoscViewModel>>> SelectByMiejscowosc(int mscID, string mscStatus)
        {
            if (_context.MiejscowoscViewModel == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscViewModel.Where(w => w.mscID == mscID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet("{mscID}")]
        public async Task<ActionResult<IEnumerable<MiejscowoscEnumerator>>> SelectLookupByMiejscowosc(int mscID, string mscStatus)
        {
            if (_context.MiejscowoscEnumerator == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscEnumerator.Where(w => w.mscID == mscID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }

        // GET: api/Miejscowosc/5
        [AllowAnonymous]
        [HttpGet("{mscID}")]
        public async Task<ActionResult<MiejscowoscEnumerator>> GetMyMiejscowosc(int mscID)
        {
            if (_context.MiejscowoscEnumerator == null)
            {
                return NotFound();
            }
            var miejscowosc = await _context.MiejscowoscEnumerator.FindAsync(mscID);

            if (miejscowosc == null)
            {
                return NotFound();
            }

            return miejscowosc;
        }

        // GET: api/Miejscowosc/5
        [AllowAnonymous]
        [HttpGet("{mscID}")]
        public async Task<ActionResult<Miejscowosc>> Get(int mscID)
        {
            if (_context.Miejscowosc == null)
            {
                return NotFound();
            }
            var miejscowosc = await _context.Miejscowosc.FindAsync(mscID);

            if (miejscowosc == null)
            {
                return NotFound();
            }

            return miejscowosc;
        }

        // PUT: api/Miejscowosc/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPut("{mscID}")]
        public async Task<IActionResult> Update(int mscID, Miejscowosc miejscowosc)
        {
            if (mscID != miejscowosc.mscID)
            {
                return BadRequest();
            }

            _context.Entry(miejscowosc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MiejscowoscExists(mscID))
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

        // POST: api/Miejscowosc
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPost]
        public async Task<ActionResult<Miejscowosc>> Insert(Miejscowosc miejscowosc)
        {
            if (_context.Miejscowosc == null)
            {
                return Problem("Entity set 'indigoERPContext.Miejscowosc'  is null.");
            }
            _context.Miejscowosc.Add(miejscowosc);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { mscID = miejscowosc.mscID }, miejscowosc);
        }

        // DELETE: api/Miejscowosc/5
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpDelete("{mscID}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Miejscowosc == null)
            {
                return NotFound();
            }
            var miejscowosc = await _context.Miejscowosc.FindAsync(id);
            if (miejscowosc == null)
            {
                return NotFound();
            }

            _context.Miejscowosc.Remove(miejscowosc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MiejscowoscExists(int mscID)
        {
            return (_context.Miejscowosc?.Any(e => e.mscID == mscID)).GetValueOrDefault();
        }
    }
}
