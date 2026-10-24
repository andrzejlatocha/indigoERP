using indigoERP.Api.Data;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Entities._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Wspolne
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WiadomoscUzytkownikController : ControllerBase
    {
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;

        public WiadomoscUzytkownikController(indigoERPContext context)
        {
            _context = context;
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WiadomoscUzytkownikViewModel>>> Select(string wiaStatus = "2")
        {
            return await _context.WiadomoscUzytkownikViewModel
                .AsNoTracking()
                .Where(w => w.wiaStatus == wiaStatus || wiaStatus == "*")
                .OrderByDescending(w => w.wiaData)
                .ThenBy(w => w.wuzID)
                .ToListAsync();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpGet("{appID}/{usrID}")]
        public async Task<ActionResult<int>> SelectCountByUzytkownik(int appID, int usrID)
        {
            return await _context.WiadomoscUzytkownikViewModel
                .CountAsync(w => w.wiaAplikacjaID == appID && w.wuzUzytkownikID == usrID && w.wuzOdczyt == false);
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpGet("{appID}/{usrID}")]
        public async Task<ActionResult<IEnumerable<WiadomoscUzytkownikViewModel>>> SelectBrakOdczytByUzytkownik(
            int appID, int usrID, string wiaStatus = "*")
        {
            return await _context.WiadomoscUzytkownikViewModel
                .AsNoTracking()
                .Where(w => w.wiaAplikacjaID == appID
                    && w.wuzUzytkownikID == usrID
                    && w.wuzOdczyt == false
                    && (w.wiaStatus == wiaStatus || wiaStatus == "*"))
                .OrderByDescending(w => w.wiaData)
                .ThenBy(w => w.wuzID)
                .ToListAsync();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpGet("{wuzID}")]
        public async Task<ActionResult<WiadomoscUzytkownikDto>> Get(int wuzID)
        {
            var wiadomoscUzytkownik = await _context.WiadomoscUzytkownik.FindAsync(wuzID);

            if (wiadomoscUzytkownik == null)
            {
                return NotFound();
            }

            return wiadomoscUzytkownik.Adapt<WiadomoscUzytkownikDto>();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPost]
        public async Task<ActionResult<WiadomoscUzytkownikDto>> Insert(WiadomoscUzytkownikDto wiadomoscUzytkownikDto)
        {
            var wiadomoscUzytkownik = wiadomoscUzytkownikDto.Adapt<WiadomoscUzytkownik>();

            _context.WiadomoscUzytkownik.Add(wiadomoscUzytkownik);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { wuzID = wiadomoscUzytkownik.wuzID }, wiadomoscUzytkownik.Adapt<WiadomoscUzytkownikDto>());
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPut("{wuzID}")]
        public async Task<IActionResult> Update(int wuzID, WiadomoscUzytkownikDto wiadomoscUzytkownikDto)
        {
            if (wuzID != wiadomoscUzytkownikDto.wuzID)
            {
                return BadRequest();
            }

            var wiadomoscUzytkownik = await _context.WiadomoscUzytkownik.FindAsync(wuzID);

            if (wiadomoscUzytkownik == null)
            {
                return NotFound();
            }

            wiadomoscUzytkownikDto.Adapt(wiadomoscUzytkownik);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WiadomoscUzytkownikExists(wuzID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpDelete("{wuzID}")]
        public async Task<IActionResult> Delete(int wuzID)
        {
            var wiadomoscUzytkownik = await _context.WiadomoscUzytkownik.FindAsync(wuzID);
            if (wiadomoscUzytkownik == null)
            {
                return NotFound();
            }

            _context.WiadomoscUzytkownik.Remove(wiadomoscUzytkownik);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WiadomoscUzytkownikExists(int wuzID)
        {
            return _context.WiadomoscUzytkownik.Any(e => e.wuzID == wuzID);
        }
    }
}
