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

        // GET: api/MiejscowoscViewModel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MiejscowoscViewModel>>> Select(string mscStatus)
        {
          if (_context.MiejscowoscViewModel == null)
          {
              return NotFound();
          }
            return await _context.MiejscowoscViewModel.Where(w => w.mscStatus == mscStatus || mscStatus == "*").ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MiejscowoscViewModel>>> SelectByKraj(int kraID, string mscStatus)
        {
            if (_context.MiejscowoscViewModel == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscViewModel.Where(w => w.kraID == kraID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MiejscowoscViewModel>>> SelectByWojewodztwo(int wojID, string mscStatus)
        {
            if (_context.MiejscowoscViewModel == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscViewModel.Where(w => w.wojID == wojID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MiejscowoscViewModel>>> SelectByPowiat(int powID, string mscStatus)
        {
            if (_context.MiejscowoscViewModel == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscViewModel.Where(w => w.powID == powID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MiejscowoscViewModel>>> SelectByGmina(int gmiID, string mscStatus)
        {
            if (_context.MiejscowoscViewModel == null)
            {
                return NotFound();
            }
            return await _context.MiejscowoscViewModel.Where(w => w.gmiID == gmiID && (w.mscStatus == mscStatus || mscStatus == "*")).ToListAsync();
        }
		
        // GET: api/Miejscowosc/5
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
