using indigoERP.Api.Data;
using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.ViewModels._Administracja;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Administrator
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GrupaController : ControllerBase
    {
        private const string AdministratorRole = "HCZ-indigoERP-Administrator";
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;
        private readonly IConfiguration _configuration;

        public GrupaController(indigoERPContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GrupaViewModel>>> Select(string groStatus)
        {
            return await _context.GrupaViewModel
                .Where(w => w.groStatus == groStatus || groStatus == "*")
                .ToListAsync();
        }

        [HttpGet("{appID}/{groID}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GrupaAplikacjaViewModel>>> SelectAplikacjaByGrupa(int appID, int groID, string appStatus)
        {
            return await _context.GrupaAplikacjaViewModel
                .Where(w => (w.appID == appID || appID == -1) && w.groID == groID && (w.appStatus == appStatus || appStatus == "*"))
                .ToListAsync();
        }

        [HttpGet("{appID}/{groID}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GrupaRolaViewModel>>> SelectRolaByGrupa(int appID, int groID, string srlStatus)
        {
            return await _context.GrupaRolaViewModel
                .Where(w => (w.appID == appID || appID == -1) && w.groID == groID && (w.srlStatus == srlStatus || srlStatus == "*"))
                .ToListAsync();
        }

        [HttpGet("{appID}/{groID}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UzytkownikGrupaViewModel>>> SelectUzytkownikByGrupa(int appID, int groID, string usrStatus)
        {
            return await _context.UzytkownikGrupaViewModel
                .Where(w => (w.appID == appID || appID == -1) && w.groID == groID && (w.usrStatus == usrStatus || usrStatus == "*"))
                .ToListAsync();
        }

        [HttpGet("{groID}")]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<GrupaDto>> Get(int groID)
        {
            var grupa = await _context.Grupa.FindAsync(groID);

            if (grupa == null)
            {
                return NotFound();
            }

            return grupa.Adapt<GrupaDto>();
        }

        [HttpPut("{groID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Update(int groID, GrupaDto grupaDto)
        {
            if (groID != grupaDto.groID)
            {
                return BadRequest();
            }

            var grupa = await _context.Grupa.FindAsync(groID);

            if (grupa == null)
            {
                return NotFound();
            }

            grupaDto.Adapt(grupa);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GrupaExists(groID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<GrupaDto>> Insert(GrupaDto grupaDto)
        {
            grupaDto.groAplikacjaID ??= _configuration.GetSection("appSettings").GetValue<int>("AppID");

            var grupa = grupaDto.Adapt<Grupa>();

            _context.Grupa.Add(grupa);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { grupa.groID }, grupa.Adapt<GrupaDto>());
        }

        [HttpGet("{uwgID}")]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<GrupaCzlonkowieDto>> GetCzlonek(int uwgID)
        {
            var czlonek = await _context.GrupaCzlonkowie.FindAsync(uwgID);

            if (czlonek == null)
            {
                return NotFound();
            }

            return czlonek.Adapt<GrupaCzlonkowieDto>();
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<GrupaCzlonkowieDto>> InsertCzlonek(GrupaCzlonkowieDto czlonekDto)
        {
            czlonekDto.uwgStatus ??= "2";
            czlonekDto.uwgUzytkownikID = NormalizeIdentifier(czlonekDto.uwgUzytkownikID);
            czlonekDto.uwgGrupaID = NormalizeIdentifier(czlonekDto.uwgGrupaID);

            var exists = await _context.GrupaCzlonkowie.AnyAsync(w =>
                w.uwgGrupaNadrzednaID == czlonekDto.uwgGrupaNadrzednaID &&
                w.uwgGrupaID == czlonekDto.uwgGrupaID &&
                w.uwgUzytkownikID == czlonekDto.uwgUzytkownikID &&
                (w.uwgStatus == "2" || w.uwgStatus == null));

            if (exists)
            {
                return Conflict("Taki członek grupy już istnieje.");
            }

            var czlonek = czlonekDto.Adapt<GrupaCzlonkowie>();

            _context.GrupaCzlonkowie.Add(czlonek);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCzlonek", new { uwgID = czlonek.uwgID }, czlonek.Adapt<GrupaCzlonkowieDto>());
        }

        [HttpDelete("{uwgID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> DeleteCzlonek(int uwgID)
        {
            var czlonek = await _context.GrupaCzlonkowie.FindAsync(uwgID);

            if (czlonek == null)
            {
                return NotFound();
            }

            _context.GrupaCzlonkowie.Remove(czlonek);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{usrID:int}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> DeleteCzlonkowieByUzytkownik(int usrID)
        {
            if (usrID <= 0)
                return BadRequest("Nieprawidłowy identyfikator użytkownika.");

            var memberships = await _context.GrupaCzlonkowie.Where(w => w.uwgUzytkownikID == usrID).ToListAsync();
            _context.GrupaCzlonkowie.RemoveRange(memberships);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{groID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Delete(int groID)
        {
            var grupa = await _context.Grupa.FindAsync(groID);

            if (grupa == null)
            {
                return NotFound();
            }

            grupa.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GrupaExists(int groID)
        {
            return _context.Grupa.Any(e => e.groID == groID);
        }

        private static int? NormalizeIdentifier(int? value)
        {
            return value.GetValueOrDefault() > 0 ? value : null;
        }
    }
}
