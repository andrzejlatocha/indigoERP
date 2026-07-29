using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Slownik;
using Microsoft.AspNetCore.Authorization;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DaneFirmaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public DaneFirmaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/DaneFirma/5
        [AllowAnonymous]
        [HttpGet("{stfID}")]
        public async Task<ActionResult<DaneFirma>> Get()
        {
          if (_context.DaneFirma == null)
          {
              return NotFound();
          }
            var daneFirma = await _context.DaneFirma.FirstAsync();

            if (daneFirma == null)
            {
                return NotFound();
            }

            return daneFirma;
        }

        // PUT: api/DaneFirma/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPut("{stfID}")]
        public async Task<IActionResult> Update(int stfID, DaneFirma daneFirma)
        {
            if (stfID != daneFirma.stfID)
            {
                return BadRequest();
            }

            _context.Entry(daneFirma).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DaneFirmaExists(stfID))
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

        // POST: api/DaneFirma
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPost]
        public async Task<ActionResult<DaneFirma>> Insert(DaneFirma daneFirma)
        {
          if (_context.DaneFirma == null)
          {
              return Problem("Entity set 'indigoERPContext.DaneFirma'  is null.");
          }
            _context.DaneFirma.Add(daneFirma);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { stfID = daneFirma.stfID }, daneFirma);
        }

        // DELETE: api/DaneFirma/5
        [Authorize(Roles = "PL-SCX-APP-indigoER-Administrator,", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpDelete("{stfID}")]
        public async Task<IActionResult> Delete(int stfID)
        {
            if (_context.DaneFirma == null)
            {
                return NotFound();
            }
            var daneFirma = await _context.DaneFirma.FindAsync(stfID);
            if (daneFirma == null)
            {
                return NotFound();
            }

            daneFirma.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DaneFirmaExists(int stfID)
        {
            return (_context.DaneFirma?.Any(e => e.stfID == stfID)).GetValueOrDefault();
        }
    }
}
