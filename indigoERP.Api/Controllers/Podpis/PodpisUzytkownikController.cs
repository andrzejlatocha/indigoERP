using indigoERP.Api.Data;
using indigoERP.Domain.Dtos._Podpis;
using indigoERP.Domain.Entities._Podpis;
using indigoERP.Domain.ViewModels._Podpis;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Podpis
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PodpisUzytkownikController : ControllerBase
    {
        private const string AdministratorRole = "HCZ-indigoERP-Podpisy";
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;

        public PodpisUzytkownikController(indigoERPContext context)
        {
            _context = context;
        }

        [HttpGet("{pdpID}")]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<IEnumerable<PodpisUzytkownikViewModel>>> SelectByPodpis(int pdpID, string pduStatus)
        {
            return await _context.PodpisUzytkownikViewModel
                .Where(w => w.pduPodpisID == pdpID && (w.pduStatus == pduStatus || pduStatus == "*"))
                .ToListAsync();
        }

        [HttpGet("{pduID}")]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<PodpisUzytkownikDto>> Get(int pduID)
        {
            var pozycja = await _context.PodpisUzytkownik.FindAsync(pduID);

            if (pozycja == null)
            {
                return NotFound();
            }

            return pozycja.Adapt<PodpisUzytkownikDto>();
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<PodpisUzytkownikDto>> Insert(PodpisUzytkownikDto pozycjaDto)
        {
            var exists = await _context.PodpisUzytkownik.AnyAsync(w =>
                w.pduPodpisID == pozycjaDto.pduPodpisID &&
                w.pduUzytkownikID == pozycjaDto.pduUzytkownikID &&
                (w.pduStatus == "2" || w.pduStatus == null));

            if (exists)
            {
                return Conflict("Ten użytkownik ma już przypisany ten szablon podpisu.");
            }

            var pozycja = pozycjaDto.Adapt<PodpisUzytkownik>();

            _context.PodpisUzytkownik.Add(pozycja);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { pduID = pozycja.pduID }, pozycja.Adapt<PodpisUzytkownikDto>());
        }

        [HttpPut("{pduID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Update(int pduID, PodpisUzytkownikDto pozycjaDto)
        {
            if (pduID != pozycjaDto.pduID)
            {
                return BadRequest();
            }

            var pozycja = await _context.PodpisUzytkownik.FindAsync(pduID);

            if (pozycja == null)
            {
                return NotFound();
            }

            pozycjaDto.Adapt(pozycja);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PodpisUzytkownikExists(pduID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{pduID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Delete(int pduID)
        {
            var pozycja = await _context.PodpisUzytkownik.FindAsync(pduID);

            if (pozycja == null)
            {
                return NotFound();
            }

            pozycja.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PodpisUzytkownikExists(int pduID)
        {
            return _context.PodpisUzytkownik.Any(e => e.pduID == pduID);
        }
    }
}
