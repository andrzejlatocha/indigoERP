using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Autoryzacja;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Administrator
{
    // Podgląd i ręczne zdejmowanie blokad logowania nałożonych przez ban manager authorizationSRV
    // (app.LoginBlokada). Blokady są zakładane automatycznie (5 nieudanych prób w 15 min), więc ten
    // kontroler NIE ma Insert - tylko Select (podgląd) i Odblokuj (zdjęcie blokady przed jej wygaśnięciem).
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "HCZ-indigoERP-Administrator", AuthenticationSchemes = "Bearer")]
    public class LoginBlokadaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public LoginBlokadaController(indigoERPContext context)
        {
            _context = context;
        }

        // lgbStatus: "2" = aktywne (nieodblokowane i jeszcze w oknie czasowym), "1" = nieaktywne
        // (odblokowane ręcznie lub wygasłe), "*" = wszystkie.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoginBlokada>>> Select(string lgbStatus)
        {
            var teraz = DateTime.Now;

            var query = _context.LoginBlokada.AsNoTracking().AsQueryable();

            query = lgbStatus switch
            {
                "2" => query.Where(w => w.lgbDataOdblokowania == null && w.lgbDataOd <= teraz && w.lgbDataDo >= teraz),
                "1" => query.Where(w => w.lgbDataOdblokowania != null || w.lgbDataDo < teraz),
                _ => query
            };

            return await query.OrderByDescending(w => w.lgbDataOd).ToListAsync();
        }

        [HttpPost("{lgbID}")]
        public async Task<IActionResult> Odblokuj(int lgbID)
        {
            var blokada = await _context.LoginBlokada.FindAsync(lgbID);

            if (blokada == null)
            {
                return NotFound();
            }

            if (blokada.lgbDataOdblokowania != null)
            {
                return NoContent();
            }

            var value = User.FindFirst("usrID")?.Value;
            blokada.lgbOdblokowanoUsrID = int.TryParse(value, out var usrID) ? usrID : null;
            blokada.lgbDataOdblokowania = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
