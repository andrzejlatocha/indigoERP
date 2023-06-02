using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using epicaHR.Domain.Entities._Slownik;
using indigoERP.Api.Data;
using Microsoft.AspNetCore.Authorization;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PlecController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public PlecController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Plec
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plec>>> Select(string pleStatus)
        {
          if (_context.Plec == null)
          {
              return NotFound();
          }
            return await _context.Plec.Where(w => w.pleStatus == pleStatus).ToListAsync();
        }

        // GET: api/Plec/5
        [AllowAnonymous]
        [HttpGet("{pleKOD}")]
        public async Task<ActionResult<Plec>> GetPlec(string pleKOD)
        {
          if (_context.Plec == null)
          {
              return NotFound();
          }
            var plec = await _context.Plec.FindAsync(pleKOD);

            if (plec == null)
            {
                return NotFound();
            }

            return plec;
        }

        // PUT: api/Plec/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "indigoERP-Administrator,indigoERP-Slownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPut("{pleKOD}")]
        public async Task<IActionResult> Update(string pleKOD, Plec plec)
        {
            if (pleKOD != plec.pleKOD)
            {
                return BadRequest();
            }

            _context.Entry(plec).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlecExists(pleKOD))
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

        // POST: api/Plec
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "indigoERP-Administrator,indigoERP-Slownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPost]
        public async Task<ActionResult<Plec>> Insert(Plec plec)
        {
          if (_context.Plec == null)
          {
              return Problem("Entity set 'indigoERPContext.Plec' is null.");
          }
            _context.Plec.Add(plec);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PlecExists(plec.pleKOD))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { pleKOD = plec.pleKOD }, plec);
        }

        // DELETE: api/Plec/5
        [Authorize(Roles = "indigoERP-Administrator,indigoERP-Slownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpDelete("{pleKOD}")]
        public async Task<IActionResult> Delete(string pleKOD)
        {
            if (_context.Plec == null)
            {
                return NotFound();
            }
            var plec = await _context.Plec.FindAsync(pleKOD);
            if (plec == null)
            {
                return NotFound();
            }

            _context.Plec.Remove(plec);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlecExists(string pleKOD)
        {
            return (_context.Plec?.Any(e => e.pleKOD == pleKOD)).GetValueOrDefault();
        }
    }
}
