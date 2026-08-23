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
    public class NowoscUzytkownikController : ControllerBase
    {
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;

        public NowoscUzytkownikController(indigoERPContext context)
        {
            _context = context;
        }

        // Co dany użytkownik już widział w danej aplikacji — z widoku mvw.vNowoscUzytkownik
        // (dbo.NowoscUzytkownik wzbogacony o nowAplikacjaID). Pusta lista = użytkownik jeszcze
        // nic nie widział w tej aplikacji (frontend wtedy wyświetla nowość i wywołuje Insert).
        [AllowAnonymous]
        [HttpGet("{appID}/{usrID}")]
        public async Task<ActionResult<IEnumerable<NowoscUzytkownikViewModel>>> SelectByUzytkownik(int appID, int usrID)
        {
            return await _context.NowoscUzytkownikViewModel
                .Where(w => w.nowAplikacjaID == appID && w.nusUzytkownikID == usrID)
                .ToListAsync();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpGet("{nusID}")]
        public async Task<ActionResult<NowoscUzytkownikDto>> Get(int nusID)
        {
            var nowoscUzytkownik = await _context.NowoscUzytkownik.FindAsync(nusID);

            if (nowoscUzytkownik == null)
            {
                return NotFound();
            }

            return nowoscUzytkownik.Adapt<NowoscUzytkownikDto>();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<NowoscUzytkownikDto>> Insert(NowoscUzytkownikDto nowoscUzytkownikDto)
        {
            var nowoscUzytkownik = nowoscUzytkownikDto.Adapt<NowoscUzytkownik>();

            _context.NowoscUzytkownik.Add(nowoscUzytkownik);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { nusID = nowoscUzytkownik.nusID }, nowoscUzytkownik.Adapt<NowoscUzytkownikDto>());
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPut("{nusID}")]
        public async Task<IActionResult> Update(int nusID, NowoscUzytkownikDto nowoscUzytkownikDto)
        {
            if (nusID != nowoscUzytkownikDto.nusID)
            {
                return BadRequest();
            }

            var nowoscUzytkownik = await _context.NowoscUzytkownik.FindAsync(nusID);

            if (nowoscUzytkownik == null)
            {
                return NotFound();
            }

            nowoscUzytkownikDto.Adapt(nowoscUzytkownik);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NowoscUzytkownikExists(nusID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpDelete("{nusID}")]
        public async Task<IActionResult> Delete(int nusID)
        {
            var nowoscUzytkownik = await _context.NowoscUzytkownik.FindAsync(nusID);
            if (nowoscUzytkownik == null)
            {
                return NotFound();
            }

            _context.NowoscUzytkownik.Remove(nowoscUzytkownik);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NowoscUzytkownikExists(int nusID)
        {
            return _context.NowoscUzytkownik.Any(e => e.nusID == nusID);
        }
    }
}
