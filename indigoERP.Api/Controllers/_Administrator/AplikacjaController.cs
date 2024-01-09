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
    public class AplikacjaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public AplikacjaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Aplikacja
        /// <summary>
        /// Zwraca listę aplikacji o zadanym statusie
        /// </summary>
        /// <param name="appStatus"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Aplikacja>>> Select(string appStatus)
        {
            return await _context.Aplikacja.Where(w => w.appStatus == appStatus || appStatus == "*").ToListAsync();
        }

        // GET: api/Grupa
        /// <summary>
        /// zwraca listę grup dla danej aplikacji
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="srlStatus">* wszystkie</param>
        /// <returns></returns>
        [HttpGet("{appID}")]
        public async Task<ActionResult<IEnumerable<RolaAplikacjaViewModel>>> SelectRolaByAplikacja(int appID, string srlStatus)
        {
            return await _context.RolaAplikacjaViewModel.Where(w => w.appID == appID && (w.srlStatus == srlStatus || srlStatus == "*")).ToListAsync();
        }

        // GET: api/Grupa
        /// <summary>
        /// zwraca listę grup dla danej aplikacji
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="groStatus">* wszystkie</param>
        /// <returns></returns>
        [HttpGet("{appID}")]
        public async Task<ActionResult<IEnumerable<GrupaAplikacjaViewModel>>> SelectGrupaByAplikacja(int appID, string groStatus)
        {
            return await _context.GrupaAplikacjaViewModel.Where(w => w.appID == appID && (w.groStatus == groStatus || groStatus == "*")).ToListAsync();
        }

        // GET: api/Grupa
        /// <summary>
        /// zwraca listę użytkowników mających dostęp do aplikacji
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="usrStatus">* wszystkie</param>
        /// <returns></returns>
        [HttpGet("{appID}")]
        public async Task<ActionResult<IEnumerable<UzytkownikAplikacjaViewModel>>> SelectUzytkownikByAplikacja(int appID, string usrStatus)
        {
            return await _context.UzytkownikAplikacjaViewModel.Where(w => w.appID == appID && (w.usrStatus == usrStatus || usrStatus == "*")).ToListAsync();
        }

        // GET: api/Aplikacja/5
        /// <summary>
        /// Zwraca aplikację o określonym identyfikatorze
        /// </summary>
        /// <param name="appID"></param>
        /// <returns></returns>
        [HttpGet("{appID}")]
        public async Task<ActionResult<Aplikacja>> Get(int appID)
        {
            var aplikacja = await _context.Aplikacja.FindAsync(appID);

            if (aplikacja == null)
            {
                return NotFound();
            }

            return aplikacja;
        }

        // PUT: api/Aplikacja/5
        /// <summary>
        /// Poprawia aplikację o podanym identyfikatorze
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="aplikacja"></param>
        /// <returns></returns>
        [HttpPut("{appID}")]
        public async Task<IActionResult> Update(int appID, Aplikacja aplikacja)
        {
            if (appID != aplikacja.appID)
            {
                return BadRequest();
            }

            _context.Entry(aplikacja).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AplikacjaExists(appID))
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

        // POST: api/Aplikacja
        /// <summary>
        /// Wstawia nową aplikację
        /// </summary>
        /// <param name="aplikacja"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Aplikacja>> Insert(Aplikacja aplikacja)
        {
            _context.Aplikacja.Add(aplikacja);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { appID = aplikacja.appID }, aplikacja);
        }

        // DELETE: api/Aplikacja/5
        /// <summary>
        /// Usuwa aplikację
        /// </summary>
        /// <param name="appID"></param>
        /// <returns></returns>
        [HttpDelete("{appID}")]
        public async Task<IActionResult> Delete(int appID)
        {
            var aplikacja = await _context.Aplikacja.FindAsync(appID);
            if (aplikacja == null)
            {
                return NotFound();
            }

            _context.Aplikacja.Remove(aplikacja);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AplikacjaExists(int appID)
        {
            return _context.Aplikacja.Any(e => e.appID == appID);
        }
    }
}
