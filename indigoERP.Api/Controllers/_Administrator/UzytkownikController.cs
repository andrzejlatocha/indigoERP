using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.ViewModels._Administracja;

namespace indigoERP.Api.Controllers._Administrator
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UzytkownikController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public UzytkownikController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Uzytkownik
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Uzytkownik>>> Select(string usrStatus)
        {
            return await _context.Uzytkownik.Where(w => w.usrStatus == usrStatus || usrStatus == "*").ToListAsync();
        }

        // GET: api/Uzytkownik
        /// <summary>
        /// zwraca listę aplikacji do jakich dostęp posiada użytkownik
        /// </summary>
        /// <param name="usrID"></param>
        /// <param name="appStatus">* wszystkie</param>
        /// <returns></returns>
        [HttpGet("{usrID}")]
        public async Task<ActionResult<IEnumerable<UzytkownikAplikacjaViewModel>>> SelectAplikacjaByUzytkownik(int usrID, string appStatus)
        {
            return await _context.UzytkownikAplikacjaViewModel.Where(w => w.usrID == usrID && (w.appStatus == appStatus || appStatus == "*")).ToListAsync();
        }

        // GET: api/Uzytkownik
        /// <summary>
        /// zwraca listę grup do jakich należy użytkownik 
        /// </summary>
        /// <param name="appID"> -1 wsystkie</param>
        /// <param name="usrID"></param>
        /// <param name="srlStatus">* wszystkie</param>
        /// <returns></returns>
        [HttpGet("{appID}/{usrID}")]
        public async Task<ActionResult<IEnumerable<UzytkownikRolaViewModel>>> SelectRolaByUzytkownik(int appID, int usrID, string srlStatus)
        {
            return await _context.UzytkownikRolaViewModel.Where(w => (w.appID == appID || appID == -1) && w.usrID == usrID && (w.srlStatus == srlStatus || srlStatus == "*")).ToListAsync();
        }

        // GET: api/Uzytkownik
        /// <summary>
        /// zwraca listę grup do jakich należy użytkownik 
        /// </summary>
        /// <param name="appID"> -1 wsystkie </param>
        /// <param name="usrID"></param>
        /// <param name="groStatus">* wszystkie</param>
        /// <returns></returns>
        [HttpGet("{appID}/{usrID}")]
        public async Task<ActionResult<IEnumerable<UzytkownikGrupaViewModel>>> SelectGrupaByUzytkownik(int appID, int usrID, string groStatus)
        {
            return await _context.UzytkownikGrupaViewModel.Where(w => (w.appID == appID || appID == -1) && w.usrID == usrID && (w.groStatus == groStatus || groStatus == "*")).ToListAsync();
        }

        // GET: api/Uzytkownik/5
        [HttpGet("{usrID}")]
        public async Task<ActionResult<Uzytkownik>> Get(int usrID)
        {
            var uzytkownik = await _context.Uzytkownik.FindAsync(usrID);

            if (uzytkownik == null)
            {
                return NotFound();
            }

            return uzytkownik;
        }

        // PUT: api/Uzytkownik/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{usrID}")]
        public async Task<IActionResult> Update(int usrID, Uzytkownik uzytkownik)
        {
            if (usrID != uzytkownik.usrID)
            {
                return BadRequest();
            }

            _context.Entry(uzytkownik).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UzytkownikExists(usrID))
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

        // POST: api/Uzytkownik
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Uzytkownik>> Insert(Uzytkownik uzytkownik)
        {
            _context.Uzytkownik.Add(uzytkownik);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { uzytkownik.usrID }, uzytkownik);
        }

        // DELETE: api/Uzytkownik/5
        [HttpDelete("{usrID}")]
        public async Task<IActionResult> Delete(int usrID)
        {
            var uzytkownik = await _context.Uzytkownik.FindAsync(usrID);
            if (uzytkownik == null)
            {
                return NotFound();
            }

            _context.Uzytkownik.Remove(uzytkownik);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UzytkownikExists(int usrID)
        {
            return _context.Uzytkownik.Any(e => e.usrID == usrID);
        }
    }
}
