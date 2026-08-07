using indigoERP.Api.Data;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Entities._Wspolne;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Wspolne
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PlikController : ControllerBase
    {
        private readonly indigoERPContext _context;
        private readonly ILogger<PlikController> _logger;

        public PlikController(indigoERPContext context, ILogger<PlikController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{plkID}")]
        public async Task<ActionResult<PlikDto>> Get(int plkID)
        {
            var plik = await _context.Plik.FindAsync(plkID);

            if (plik == null)
            {
                return NotFound();
            }

            return plik.Adapt<PlikDto>();
        }

        [HttpPut("{plkID}")]
        public async Task<IActionResult> Update(int plkID, PlikDto plikDto)
        {
            if (plkID != plikDto.plkID)
            {
                return BadRequest();
            }

            var plik = await _context.Plik.FindAsync(plkID);

            if (plik == null)
            {
                return NotFound();
            }

            plikDto.Adapt(plik);
            PreparePlikData(plik, false);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlikExists(plkID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<PlikDto>> Insert(PlikDto plikDto)
        {
            var plik = plikDto.Adapt<Plik>();
            PreparePlikData(plik, true);

            _context.Plik.Add(plik);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                const string message = "Błąd bazy danych podczas zapisu pliku.";
                _logger.LogError(ex, "{Message} Aplikacja {ApplicationId}, użytkownik {UserId}.", message, plik.plkAplikacjaID,
                    plik.UtworzenieID);
                indigoERP.Api.Services.ApplicationFileLogger.LogError("File insert", message, ex);

                return Problem(statusCode: StatusCodes.Status500InternalServerError, title: message);
            }

            return CreatedAtAction("Get", new { plkID = plik.plkID }, plik.Adapt<PlikDto>());
        }

        [HttpDelete("{plkID}")]
        public async Task<IActionResult> Delete(int plkID)
        {
            var plik = await _context.Plik.FindAsync(plkID);

            if (plik == null)
            {
                return NotFound();
            }

            plik.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlikExists(int plkID)
        {
            return _context.Plik.Any(e => e.plkID == plkID);
        }

        private static void PreparePlikData(Plik plik, bool isNew)
        {
            if (isNew)
            {
                plik.plkID = 0;
            }
        }
    }
}
