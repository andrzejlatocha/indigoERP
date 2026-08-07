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
    public class ZadanieUzytkownikController : ControllerBase
    {
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;

        public ZadanieUzytkownikController(indigoERPContext context)
        {
            _context = context;
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpGet("{tusID}")]
        public async Task<ActionResult<ZadanieUzytkownikDto>> Get(int tusID)
        {
            var zadanieUzytkownik = await _context.ZadanieUzytkownik.FindAsync(tusID);

            if (zadanieUzytkownik == null)
            {
                return NotFound();
            }

            return zadanieUzytkownik.Adapt<ZadanieUzytkownikDto>();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPost]
        public async Task<ActionResult<ZadanieUzytkownikDto>> Insert(ZadanieUzytkownikDto zadanieUzytkownikDto)
        {
            var zadanieUzytkownik = zadanieUzytkownikDto.Adapt<ZadanieUzytkownik>();

            _context.ZadanieUzytkownik.Add(zadanieUzytkownik);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { tusID = zadanieUzytkownik.tusID }, zadanieUzytkownik.Adapt<ZadanieUzytkownikDto>());
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPut("{tusID}")]
        public async Task<IActionResult> Update(int tusID, ZadanieUzytkownikDto zadanieUzytkownikDto)
        {
            if (tusID != zadanieUzytkownikDto.tusID)
            {
                return BadRequest();
            }

            var zadanieUzytkownik = await _context.ZadanieUzytkownik.FindAsync(tusID);

            if (zadanieUzytkownik == null)
            {
                return NotFound();
            }

            zadanieUzytkownikDto.Adapt(zadanieUzytkownik);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZadanieUzytkownikExists(tusID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpDelete("{tusID}")]
        public async Task<IActionResult> Delete(int tusID)
        {
            var zadanieUzytkownik = await _context.ZadanieUzytkownik.FindAsync(tusID);
            if (zadanieUzytkownik == null)
            {
                return NotFound();
            }

            _context.ZadanieUzytkownik.Remove(zadanieUzytkownik);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ZadanieUzytkownikExists(int tusID)
        {
            return _context.ZadanieUzytkownik.Any(e => e.tusID == tusID);
        }
    }
}
