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
using System.Security.Cryptography;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ObywatelstwoController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public ObywatelstwoController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Obywatelstwo
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Obywatelstwo>>> Select(string obyStatus)
        {
           if (_context.Obywatelstwo == null)
           {
              return NotFound();
           }

           return await _context.Obywatelstwo.Where(w => w.obyStatus == obyStatus || obyStatus == "*").ToListAsync();
        }

        // GET: api/Obywatelstwo/5
        [AllowAnonymous]
        [HttpGet("{obyID}")]
        public async Task<ActionResult<Obywatelstwo>> Get(int obyID)
        {
          if (_context.Obywatelstwo == null)
          {
              return NotFound();
          }
            var obywatelstwo = await _context.Obywatelstwo.FindAsync(obyID);

            if (obywatelstwo == null)
            {
                return NotFound();
            }

            return obywatelstwo;
        }

        // PUT: api/Obywatelstwo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "indigoERP-Administrator,indigoERP-Slownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPut("{obyID}")]
        public async Task<IActionResult> Update(int obyID, Obywatelstwo obywatelstwo)
        {
            if (obyID != obywatelstwo.obyID)
            {
                return BadRequest();
            }

            _context.Entry(obywatelstwo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ObywatelstwoExists(obyID))
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

        // POST: api/Obywatelstwo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "indigoERP-Administrator,indigoERP-Slownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPost]
        public async Task<ActionResult<Obywatelstwo>> Insert(Obywatelstwo obywatelstwo)
        {
          if (_context.Obywatelstwo == null)
          {
              return Problem("Entity set 'indigoERPContext.Obywatelstwo' is null.");
          }
            _context.Obywatelstwo.Add(obywatelstwo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { obyID = obywatelstwo.obyID }, obywatelstwo);
        }

        // DELETE: api/Obywatelstwo/5
        [Authorize(Roles = "indigoERP-Administrator,indigoERP-Slownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpDelete("{obyID}")]
        public async Task<IActionResult> Delete(int obyID)
        {
            if (_context.Obywatelstwo == null)
            {
                return NotFound();
            }
            var obywatelstwo = await _context.Obywatelstwo.FindAsync(obyID);
            if (obywatelstwo == null)
            {
                return NotFound();
            }

            _context.Obywatelstwo.Remove(obywatelstwo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ObywatelstwoExists(int obyID)
        {
            return (_context.Obywatelstwo?.Any(e => e.obyID == obyID)).GetValueOrDefault();
        }
    }
}
