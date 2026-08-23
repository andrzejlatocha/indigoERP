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
    public class PodpisGrupaController : ControllerBase
    {
        private const string AdministratorRole = "HCZ-indigoERP-Podpisy";
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;

        public PodpisGrupaController(indigoERPContext context)
        {
            _context = context;
        }

        [HttpGet("{pdpID}")]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<IEnumerable<PodpisGrupaViewModel>>> SelectByPodpis(int pdpID, string pdgStatus)
        {
            return await _context.PodpisGrupaViewModel
                .Where(w => w.pdgPodpisID == pdpID && (w.pdgStatus == pdgStatus || pdgStatus == "*"))
                .ToListAsync();
        }

        [HttpGet("{pdgID}")]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<PodpisGrupaDto>> Get(int pdgID)
        {
            var pozycja = await _context.PodpisGrupa.FindAsync(pdgID);

            if (pozycja == null)
            {
                return NotFound();
            }

            return pozycja.Adapt<PodpisGrupaDto>();
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<PodpisGrupaDto>> Insert(PodpisGrupaDto pozycjaDto)
        {
            var exists = await _context.PodpisGrupa.AnyAsync(w =>
                w.pdgPodpisID == pozycjaDto.pdgPodpisID &&
                w.pdgGrupaID == pozycjaDto.pdgGrupaID &&
                (w.pdgStatus == "2" || w.pdgStatus == null));

            if (exists)
            {
                return Conflict("Ta grupa ma już przypisany ten szablon podpisu.");
            }

            var pozycja = pozycjaDto.Adapt<PodpisGrupa>();

            _context.PodpisGrupa.Add(pozycja);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { pdgID = pozycja.pdgID }, pozycja.Adapt<PodpisGrupaDto>());
        }

        [HttpPut("{pdgID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Update(int pdgID, PodpisGrupaDto pozycjaDto)
        {
            if (pdgID != pozycjaDto.pdgID)
            {
                return BadRequest();
            }

            var pozycja = await _context.PodpisGrupa.FindAsync(pdgID);

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
                if (!PodpisGrupaExists(pdgID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{pdgID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Delete(int pdgID)
        {
            var pozycja = await _context.PodpisGrupa.FindAsync(pdgID);

            if (pozycja == null)
            {
                return NotFound();
            }

            pozycja.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PodpisGrupaExists(int pdgID)
        {
            return _context.PodpisGrupa.Any(e => e.pdgID == pdgID);
        }
    }
}
