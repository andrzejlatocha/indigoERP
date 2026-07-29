using indigoERP.Api.Data;
using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.Enumerators._Administrator;
using indigoERP.Domain.ViewModels;
using indigoERP.Domain.ViewModels._Administracja;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Administrator
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "HCZ-indigoERP-Administrator", AuthenticationSchemes = "Bearer")]
    public class AplikacjaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public AplikacjaController(indigoERPContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AplikacjaViewModel>>> Select(string appStatus)
        {
            return await _context.AplikacjaViewModel.Where(w => w.appStatus == appStatus || appStatus == "*").ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AplikacjaEnumerator>>> SelectLookup(string appStatus = "2")
        {
            return await _context.AplikacjaEnumerator.Where(w => w.appStatus == appStatus || appStatus == "*").ToListAsync();
        }

        [HttpGet("{appID}")]
        public async Task<ActionResult<IEnumerable<RolaAplikacjaViewModel>>> SelectRolaByAplikacja(int appID, string srlStatus)
        {
            return await _context.RolaAplikacjaViewModel.Where(w => w.appID == appID && (w.srlStatus == srlStatus || srlStatus == "*")).ToListAsync();
        }

        [HttpGet("{appID}")]
        public async Task<ActionResult<IEnumerable<GrupaAplikacjaViewModel>>> SelectGrupaByAplikacja(int appID, string groStatus)
        {
            return await _context.GrupaAplikacjaViewModel.Where(w => w.appID == appID && (w.groStatus == groStatus || groStatus == "*")).ToListAsync();
        }

        [HttpGet("{appID}")]
        public async Task<ActionResult<IEnumerable<UzytkownikAplikacjaViewModel>>> SelectUzytkownikByAplikacja(int appID, string usrStatus)
        {
            return await _context.UzytkownikAplikacjaViewModel.Where(w => w.appID == appID && (w.usrStatus == usrStatus || usrStatus == "*")).ToListAsync();
        }

        [HttpGet("{appID}")]
        public async Task<ActionResult<AplikacjaDto>> Get(int appID)
        {
            var aplikacja = await _context.Aplikacja.FindAsync(appID);

            if (aplikacja == null)
            {
                return NotFound();
            }

            return aplikacja.Adapt<AplikacjaDto>();
        }

        [HttpPut("{appID}")]
        public async Task<IActionResult> Update(int appID, AplikacjaDto aplikacjaDto)
        {
            if (appID != aplikacjaDto.appID)
            {
                return BadRequest("Nie można zmienić symbolu istniejącej aplikacji.");
            }

            var aplikacja = await _context.Aplikacja.FindAsync(appID);

            if (aplikacja == null)
            {
                return NotFound();
            }

            aplikacjaDto.Adapt(aplikacja);

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

                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<AplikacjaDto>> Insert(AplikacjaDto aplikacjaDto)
        {
            if (await _context.Aplikacja.AnyAsync(w => w.appID == aplikacjaDto.appID))
            {
                return BadRequest($"Aplikacja o symbolu {aplikacjaDto.appID} już istnieje.");
            }

            var aplikacja = aplikacjaDto.Adapt<Aplikacja>();

            _context.Aplikacja.Add(aplikacja);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { appID = aplikacja.appID }, aplikacja.Adapt<AplikacjaDto>());
        }

        [HttpDelete("{appID}")]
        public async Task<IActionResult> Delete(int appID)
        {
            var aplikacja = await _context.Aplikacja.FindAsync(appID);

            if (aplikacja == null)
            {
                return NotFound();
            }

            aplikacja.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AplikacjaExists(int appID)
        {
            return _context.Aplikacja.Any(e => e.appID == appID);
        }
    }
}
