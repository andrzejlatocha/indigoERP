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
using Microsoft.AspNetCore.Authorization;

namespace indigoERP.Api.Controllers._Administrator
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GrupaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public GrupaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Grupa
        /// <summary>
        /// Zwraca listę grup o zadanym statusie
        /// </summary>
        /// <param name="groStatus"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Grupa>>> Select(string groStatus)
        {
            return await _context.Grupa.Where(w => w.groStatus == groStatus || groStatus == "*").ToListAsync();
        }

        // GET: api/Grupa
        /// <summary>
        /// zwraca listę aplikacji dla danej grupy
        /// </summary>
        /// <param name="appID">-1 wszystkie</param>
        /// <param name="appStatus">* wszystkie</param>
        /// <returns></returns>
        [HttpGet("{appID}/{groID}")]
        public async Task<ActionResult<IEnumerable<GrupaAplikacjaViewModel>>> SelectAplikacjaByGrupa(int appID, int groID, string appStatus)
        {
            return await _context.GrupaAplikacjaViewModel.Where(w => (w.appID == appID || appID == -1) && w.groID == groID && (w.appStatus == appStatus || appStatus == "*")).ToListAsync();
        }

        // GET: api/Grupa
        /// <summary>
        /// zwraca listę ról dla danej grupy
        /// </summary>
        /// <param name="appID">-1 wszystkie</param> 
        /// <param name="groID"></param>
        /// <param name="srlStatus">* wszystkie</param>
        /// <returns></returns>
        [HttpGet("{appID}/{groID}")]
        public async Task<ActionResult<IEnumerable<GrupaRolaViewModel>>> SelectRolaByGrupa(int appID, int groID, string srlStatus)
        {
            return await _context.GrupaRolaViewModel.Where(w => (w.appID == appID || appID == -1) && w.groID == groID && (w.srlStatus == srlStatus || srlStatus == "*")).ToListAsync();
        }

        // GET: api/Uzytkownik
        /// <summary>
        /// zwraca listę użytkownikó dla danej grupy
        /// </summary>
        /// <param name="appID">-1 wszystkie</param>
        /// <param name="groID"></param>
        /// <param name="usrStatus">* wszystkie</param>
        /// <returns></returns>
        [HttpGet("{appID}/{groID}")]
        public async Task<ActionResult<IEnumerable<UzytkownikGrupaViewModel>>> SelectUzytkownikByGrupa(int appID, int groID, string usrStatus)
        {
            return await _context.UzytkownikGrupaViewModel.Where(w => (w.appID == appID || appID == -1) && w.groID == groID && (w.usrStatus == usrStatus || usrStatus == "*")).ToListAsync();
        }

        // GET: api/Grupa/5
        /// <summary>
        /// Zwraca grupę o określonym identyfikatorze
        /// </summary>
        /// <param name="groID"></param>
        /// <returns></returns>
        [HttpGet("{groID}")]
        public async Task<ActionResult<Grupa>> Get(int groID)
        {
            var grupa = await _context.Grupa.FindAsync(groID);

            if (grupa == null)
            {
                return NotFound();
            }

            return grupa;
        }

        // PUT: api/Grupa/5
        /// <summary>
        /// Poprawia grupę o podanym identyfikatorze
        /// </summary>
        /// <param name="groID"></param>
        /// <param name="grupa"></param>
        /// <returns></returns>
        [HttpPut("{groID}")]
        public async Task<IActionResult> Update(int groID, Grupa grupa)
        {
            if (groID != grupa.groID)
            {
                return BadRequest();
            }

            _context.Entry(grupa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GrupaExists(groID))
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

        // POST: api/Grupa
        /// <summary>
        /// Wstawia nową grupę
        /// </summary>
        /// <param name="grupa"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Grupa>> Insert(Grupa grupa)
        {
            _context.Grupa.Add(grupa);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { grupa.groID }, grupa);
        }

        // DELETE: api/Grupa/5
        /// <summary>
        /// Usuwa grupę
        /// </summary>
        /// <param name="groID"></param>
        /// <returns></returns>
        [HttpDelete("{groID}")]
        public async Task<IActionResult> Delete(int groID)
        {
            var grupa = await _context.Grupa.FindAsync(groID);
            if (grupa == null)
            {
                return NotFound();
            }

            _context.Grupa.Remove(grupa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GrupaExists(int groID)
        {
            return _context.Grupa.Any(e => e.groID == groID);
        }
    }
}
