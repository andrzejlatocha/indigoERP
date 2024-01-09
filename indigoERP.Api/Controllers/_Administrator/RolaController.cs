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
    public class RolaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public RolaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Rola
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rola>>> Select(string srlStatus)
        {
            return await _context.Rola.Where(w => w.srlStatus == srlStatus || srlStatus == "*").ToListAsync();
        }

        // GET: api/Rola
        /// <summary>
        /// zwraca listę aplikacji dla roli
        /// </summary>
        /// <param name="appID">-1 wszystkie aplikacje</param>
        /// <param name="srlID"></param>
        /// <param name="appStatus">* wszystkie</param>
        /// <returns></returns>
        [HttpGet("{appID}/{srlID}")]
        public async Task<ActionResult<IEnumerable<RolaAplikacjaViewModel>>> SelectAplikacjaByRola(int appID, int srlID, string appStatus)
        {
            return await _context.RolaAplikacjaViewModel.Where(w => (w.appID == appID || appID == -1) && w.srlID == srlID && (w.appStatus == appStatus || appStatus == "*")).ToListAsync();
        }

        // GET: api/Rola
        /// <summary>
        /// zwraca listę grup dla danej roli
        /// </summary>
        /// <param name="appID">-1 wszystkie aplikacje</param>
        /// <param name="srlID"></param>
        /// <param name="groStatus">* wszystkie</param>
        /// <returns></returns>
        [HttpGet("{appID}/{srlID}")]
        public async Task<ActionResult<IEnumerable<GrupaRolaViewModel>>> SelectGrupaByRola(int appID, int srlID, string groStatus)
        {
            return await _context.GrupaRolaViewModel.Where(w => (w.appID == appID || appID == -1) && w.srlID == srlID && (w.groStatus == groStatus || groStatus == "*")).ToListAsync();
        }

        // GET: api/Rola
        /// <summary>
        /// zwraca listę użytkowników dla danej roli
        /// </summary>
        /// <param name="appID">-1 wszystkie aplikacje</param>
        /// <param name="usrID"></param>
        /// <param name="usrStatus"></param>
        /// <returns></returns>
        [HttpGet("{appID}/{srlID}")]
        public async Task<ActionResult<IEnumerable<UzytkownikRolaViewModel>>> SelectUzytkownikByRola(int appID, int srlID, string usrStatus)
        {
            return await _context.UzytkownikRolaViewModel.Where(w => (w.appID == appID || appID == -1) && w.srlID == srlID && (w.usrStatus == usrStatus || usrStatus == "*")).ToListAsync();
        }

        // GET: api/Rola/5
        [HttpGet("{srlID}")]
        public async Task<ActionResult<Rola>> Get(int srlID)
        {
            var rola = await _context.Rola.FindAsync(srlID);

            if (rola == null)
            {
                return NotFound();
            }

            return rola;
        }

        // PUT: api/Rola/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{srlID}")]
        public async Task<IActionResult> Update(int srlID, Rola rola)
        {
            if (srlID != rola.srlID)
            {
                return BadRequest();
            }

            _context.Entry(rola).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolaExists(srlID))
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

        // POST: api/Rola
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Rola>> Insert(Rola rola)
        {
            _context.Rola.Add(rola);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { rola.srlID }, rola);
        }

        // DELETE: api/Rola/5
        [HttpDelete("{srlID}")]
        public async Task<IActionResult> DeleteRola(int srlID)
        {
            var rola = await _context.Rola.FindAsync(srlID);
            if (rola == null)
            {
                return NotFound();
            }

            _context.Rola.Remove(rola);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RolaExists(int srlID)
        {
            return _context.Rola.Any(e => e.srlID == srlID);
        }
    }
}
