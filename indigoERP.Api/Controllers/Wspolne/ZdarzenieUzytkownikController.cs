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
    public class ZdarzenieUzytkownikController : ControllerBase
    {
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;

        public ZdarzenieUzytkownikController(indigoERPContext context)
        {
            _context = context;
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpGet("{zurID}")]
        public async Task<ActionResult<ZdarzenieUzytkownikDto>> Get(int zurID)
        {
            var zdarzenieUzytkownik = await _context.ZdarzenieUzytkownik.FindAsync(zurID);

            if (zdarzenieUzytkownik == null)
            {
                return NotFound();
            }

            return zdarzenieUzytkownik.Adapt<ZdarzenieUzytkownikDto>();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPost]
        public async Task<ActionResult<ZdarzenieUzytkownikDto>> Insert(ZdarzenieUzytkownikDto zdarzenieUzytkownikDto)
        {
            var zdarzenieUzytkownik = zdarzenieUzytkownikDto.Adapt<ZdarzenieUzytkownik>();

            _context.ZdarzenieUzytkownik.Add(zdarzenieUzytkownik);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { zurID = zdarzenieUzytkownik.zurID }, zdarzenieUzytkownik.Adapt<ZdarzenieUzytkownikDto>());
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPut("{zurID}")]
        public async Task<IActionResult> Update(int zurID, ZdarzenieUzytkownikDto zdarzenieUzytkownikDto)
        {
            if (zurID != zdarzenieUzytkownikDto.zurID)
            {
                return BadRequest();
            }

            var zdarzenieUzytkownik = await _context.ZdarzenieUzytkownik.FindAsync(zurID);

            if (zdarzenieUzytkownik == null)
            {
                return NotFound();
            }

            zdarzenieUzytkownikDto.Adapt(zdarzenieUzytkownik);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZdarzenieUzytkownikExists(zurID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpDelete("{zurID}")]
        public async Task<IActionResult> Delete(int zurID)
        {
            var zdarzenieUzytkownik = await _context.ZdarzenieUzytkownik.FindAsync(zurID);
            if (zdarzenieUzytkownik == null)
            {
                return NotFound();
            }

            _context.ZdarzenieUzytkownik.Remove(zdarzenieUzytkownik);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ZdarzenieUzytkownikExists(int zurID)
        {
            return _context.ZdarzenieUzytkownik.Any(e => e.zurID == zurID);
        }
    }
}
