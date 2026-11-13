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
    public class RolaController : ControllerBase
    {
        private const string AdministratorRole = "HCZ-indigoERP-Administrator";
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;
        private readonly IConfiguration _configuration;

        public RolaController(indigoERPContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RolaViewModel>>> Select(string srlStatus)
        {
            return await _context.RolaViewModel
                .Where(w => w.srlStatus == srlStatus || srlStatus == "*")
                .ToListAsync();
        }

        // Role przypisane do aplikacji (lista do combo w formularzach: wartość srlID, tekst srlNazwa).
        [HttpGet("{appID}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RolaAplikacjaViewModel>>> SelectLookupByAplikacja(int appID, string srlStatus)
        {
            return await _context.RolaAplikacjaViewModel
                .Where(w => w.appID == appID && (w.srlStatus == srlStatus || srlStatus == "*"))
                .ToListAsync();
        }

        [HttpGet("{appID}/{srlID}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RolaAplikacjaViewModel>>> SelectAplikacjaByRola(int appID, int srlID, string appStatus)
        {
            return await _context.RolaAplikacjaViewModel
                .Where(w => (w.appID == appID || appID == -1) && w.srlID == srlID && (w.appStatus == appStatus || appStatus == "*"))
                .ToListAsync();
        }

        [HttpGet("{appID}/{srlID}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GrupaRolaViewModel>>> SelectGrupaByRola(int appID, int srlID, string groStatus)
        {
            return await _context.GrupaRolaViewModel
                .Where(w => (w.appID == appID || appID == -1) && w.srlID == srlID && (w.groStatus == groStatus || groStatus == "*"))
                .ToListAsync();
        }

        [HttpGet("{appID}/{srlID}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UzytkownikRolaViewModel>>> SelectUzytkownikByRola(int appID, int srlID, string usrStatus)
        {
            // Widok mvw.vUzytkownikRolaEfektywna trzyma już całą logikę (bezpośrednie + dziedziczone
            // członkostwo przez grupę, deduplikacja z priorytetem dla wiersza z widoku) — patrz analogiczne
            // użycie w UzytkownikController.SelectRolaByUzytkownik (ten sam widok, filtrowany z drugiej strony).
            return await _context.UzytkownikRolaEfektywnaViewModel
                .Where(w => w.srlID == srlID
                    && (w.appID == appID || appID == -1)
                    && (w.usrStatus == usrStatus || usrStatus == "*"))
                .Select(w => new UzytkownikRolaViewModel
                {
                    cwrID = w.cwrID,
                    appID = w.appID,
                    usrID = w.usrID,
                    usrNazwa = w.usrNazwa,
                    usrNazwaAD = w.usrNazwaAD,
                    usrStatus = w.usrStatus,
                    srlID = w.srlID,
                    srlNazwa = w.srlNazwa,
                    srlOpis = w.srlOpis,
                    srlStatus = w.srlStatus,
                    uwgPoziom = w.uwgPoziom,
                    uwgDziedziczy = w.uwgDziedziczy
                })
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpGet("{srlID}")]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<RolaDto>> Get(int srlID)
        {
            var rola = await _context.Rola.FindAsync(srlID);

            if (rola == null)
            {
                return NotFound();
            }

            return rola.Adapt<RolaDto>();
        }

        [HttpPut("{srlID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Update(int srlID, RolaDto rolaDto)
        {
            if (srlID != rolaDto.srlID)
            {
                return BadRequest();
            }

            var rola = await _context.Rola.FindAsync(srlID);

            if (rola == null)
            {
                return NotFound();
            }

            rolaDto.Adapt(rola);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolaExists(srlID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<RolaDto>> Insert(RolaDto rolaDto)
        {
            rolaDto.srlAplikacjaID ??= _configuration.GetSection("appSettings").GetValue<int>("AppID");

            var rola = rolaDto.Adapt<Rola>();

            _context.Rola.Add(rola);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { rola.srlID }, rola.Adapt<RolaDto>());
        }

        [HttpGet("{cwrID}")]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<RolaCzlonkowieDto>> GetCzlonek(int cwrID)
        {
            var czlonek = await _context.RolaCzlonkowie.FindAsync(cwrID);

            if (czlonek == null)
            {
                return NotFound();
            }

            return czlonek.Adapt<RolaCzlonkowieDto>();
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<RolaCzlonkowieDto>> InsertCzlonek(RolaCzlonkowieDto czlonekDto)
        {
            czlonekDto.cwrStatus ??= "2";
            czlonekDto.cwrUzytkownikID = NormalizeIdentifier(czlonekDto.cwrUzytkownikID);
            czlonekDto.cwrGrupaID = NormalizeIdentifier(czlonekDto.cwrGrupaID);

            var exists = await _context.RolaCzlonkowie.AnyAsync(w =>
                w.cwrRolaID == czlonekDto.cwrRolaID &&
                w.cwrGrupaID == czlonekDto.cwrGrupaID &&
                w.cwrUzytkownikID == czlonekDto.cwrUzytkownikID &&
                (w.cwrStatus == "2" || w.cwrStatus == null));

            if (exists)
            {
                return Conflict("Taki członek roli już istnieje.");
            }

            var czlonek = czlonekDto.Adapt<RolaCzlonkowie>();

            _context.RolaCzlonkowie.Add(czlonek);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCzlonek", new { cwrID = czlonek.cwrID }, czlonek.Adapt<RolaCzlonkowieDto>());
        }

        [HttpDelete("{cwrID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> DeleteCzlonek(int cwrID)
        {
            var czlonek = await _context.RolaCzlonkowie.FindAsync(cwrID);

            if (czlonek == null)
            {
                return NotFound();
            }

            _context.RolaCzlonkowie.Remove(czlonek);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{srlID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Delete(int srlID)
        {
            var rola = await _context.Rola.FindAsync(srlID);

            if (rola == null)
            {
                return NotFound();
            }

            rola.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RolaExists(int srlID)
        {
            return _context.Rola.Any(e => e.srlID == srlID);
        }

        private static int? NormalizeIdentifier(int? value)
        {
            return value.GetValueOrDefault() > 0 ? value : null;
        }
    }
}
