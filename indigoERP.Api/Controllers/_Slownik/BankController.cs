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
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public BankController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Bank
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bank>>> Select(string lbaStatus)
        {
          if (_context.Bank == null)
          {
              return NotFound();
          }
            return await _context.Bank.Where(w => w.lbaStatus == lbaStatus || lbaStatus == "*").ToListAsync();
        }

        // GET: api/Bank/5
        [HttpGet("{lbaID}")]
        public async Task<ActionResult<Bank>> Get(int lbaID)
        {
          if (_context.Bank == null)
          {
              return NotFound();
          }
            var bank = await _context.Bank.FindAsync(lbaID);

            if (bank == null)
            {
                return NotFound();
            }

            return bank;
        }

        // PUT: api/Bank/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{lbaID}")]
        public async Task<IActionResult> Update(int lbaID, Bank bank)
        {
            if (lbaID != bank.lbaID)
            {
                return BadRequest();
            }

            _context.Entry(bank).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankExists(lbaID))
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

        // POST: api/Bank
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bank>> Insert(Bank bank)
        {
          if (_context.Bank == null)
          {
              return Problem("Entity set 'indigoERPContext.Bank' is null.");
          }
            _context.Bank.Add(bank);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { lbaID = bank.lbaID }, bank);
        }

        // DELETE: api/Bank/5
        [HttpDelete("{lbaID}")]
        public async Task<IActionResult> Delete(int lbaID)
        {
            if (_context.Bank == null)
            {
                return NotFound();
            }
            var bank = await _context.Bank.FindAsync(lbaID);
            if (bank == null)
            {
                return NotFound();
            }

            _context.Bank.Remove(bank);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BankExists(int lbaID)
        {
            return (_context.Bank?.Any(e => e.lbaID == lbaID)).GetValueOrDefault();
        }
    }
}
