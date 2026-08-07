using indigoERP.Api.Data;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Entities._Wspolne;
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
