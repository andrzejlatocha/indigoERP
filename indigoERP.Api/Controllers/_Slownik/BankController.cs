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
    /// <summary>
    /// Pobieranie i aktualizacja listy banków, domyślnie tabela zawieraa wszystkie polskie banki 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public BankController(indigoERPContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Pobieranie listy banków
        /// </summary>
        /// <param name="lbaStatus">
        /// </param>
        /// <returns>
        /// Lista banków
        /// </returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bank>>> Select(string lbaStatus)
        {
          if (_context.Bank == null)
          {
              return NotFound();
          }
            return await _context.Bank.Where(w => w.lbaStatus == lbaStatus || lbaStatus == "*").ToListAsync();
        }

        /// <summary>
        /// Pobranie danych banku
        /// </summary>
        /// <param name="lbaID"></param>
        /// <returns>
        /// Bank
        /// </returns>
        [AllowAnonymous]
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

        /// <summary>
        /// Aktualizacja danych banku
        /// </summary>
        /// <param name="lbaKOD"></param>
        /// <param name="bank"></param>
        /// <returns>
        /// </returns>
        [Authorize(Roles = "indigoERP-Administrator,indigoERP-Slownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPut("{lbaKOD}")]
        public async Task<IActionResult> Update(string lbaKOD, Bank bank)
        {
            if (lbaKOD != bank.lbaKOD)
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
                if (!BankExists(lbaKOD))
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

        /// <summary>
        /// Dodanie banku
        /// </summary>
        /// <param name="bank"></param>
        /// <returns></returns>
        [Authorize(Roles = "indigoERP-Administrator,indigoERP-Slownik", AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPost]
        public async Task<ActionResult<Bank>> Insert(Bank bank)
        {
          if (_context.Bank == null)
          {
              return Problem("Entity set 'indigoERPContext.Bank' is null.");
          }
            _context.Bank.Add(bank);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { lbaID = bank.lbaKOD }, bank);
        }

        /// <summary>
        /// Usunięcie banku o oknkretny ID
        /// </summary>
        /// <param name="lbaID"></param>
        /// <returns></returns>
        [Authorize(Roles = "indigoERP-Administrator,indigoERP-Slownik", AuthenticationSchemes = "Bearer,Cookie")]
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

        private bool BankExists(string lbaKOD)
        {
            return (_context.Bank?.Any(e => e.lbaKOD == lbaKOD)).GetValueOrDefault();
        }
    }
}
