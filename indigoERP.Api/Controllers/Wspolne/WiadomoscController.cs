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
    public class WiadomoscController : ControllerBase
    {
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;

        public WiadomoscController(indigoERPContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WiadomoscViewModel>>> Select(int wiaAplikacjaID, string wiaStatus)
        {
            return await _context.WiadomoscViewModel
                .Where(w => w.wiaAplikacjaID == wiaAplikacjaID && (w.wiaStatus == wiaStatus || wiaStatus == "*"))
                .ToListAsync();
        }

        // Filtruje po aplikacji i odbiorcy wiadomości; wiaStatus="*" oznacza brak filtru statusu.
        [AllowAnonymous]
        [HttpGet("{appID}/{usrID}")]
        public async Task<ActionResult<IEnumerable<WiadomoscViewModel>>> SelectByUzytkownik(int appID, int usrID, string wiaStatus = "*")
        {
            return await _context.WiadomoscViewModel
                .Where(w => w.wiaAplikacjaID == appID && w.wiaUzytkownikID == usrID && (w.wiaStatus == wiaStatus || wiaStatus == "*"))
                .ToListAsync();
        }

        // Widok nie przechowuje stanu odczytania, dlatego licznik obejmuje wszystkie wiadomości odbiorcy w aplikacji.
        [AllowAnonymous]
        [HttpGet("{appID}/{usrID}")]
        public async Task<ActionResult<int>> SelectCountByUzytkownik(int appID, int usrID)
        {
            return await _context.WiadomoscViewModel
                .CountAsync(w => w.wiaAplikacjaID == appID && w.wiaUzytkownikID == usrID);
        }

        // Biuletyn publiczny (Głos Hutnika) — bez żadnej autoryzacji, serwowany osobnej,
        // nie-Blazorowej stronie Razor Pages (Pages/WiadomoscPubliczna.cshtml), żeby nie trzeba
        // było otwierać całego kręgu Blazor na anonimowy dostęp. Widok mvw.vWiadomoscPubliczna
        // już ogranicza wynik do wiaAplikacjaID=900 + wiaPubliczna=1 + wiaStatus='2'.
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WiadomoscPublicznaViewModel>>> SelectPubliczna()
        {
            return await _context.WiadomoscPublicznaViewModel.OrderByDescending(w => w.wiaData).ToListAsync();
        }

        // Historia wiadomości wysłanych z danej aplikacji (opcjonalnie przez konkretnego użytkownika) —
        // mvw.vWiadomoscHistoria łączy Wiadomosc + WiadomoscWysylka, bez zależności od WiadomoscUzytkownik
        // (ta ostatnia tabela w praktyce nie jest używana przez istniejące, systemowe wiadomości).
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WiadomoscHistoriaViewModel>>> SelectHistoria(
            int wiaAplikacjaID, int? wiaUzytkownikID, string wiaStatus = "*")
        {
            return await _context.WiadomoscHistoriaViewModel
                .Where(w => w.wiaAplikacjaID == wiaAplikacjaID
                    && (wiaUzytkownikID == null || w.wiaUzytkownikID == wiaUzytkownikID)
                    && (w.wiaStatus == wiaStatus || wiaStatus == "*"))
                .OrderByDescending(w => w.wiaData)
                .ToListAsync();
        }

        /// <summary>Liczby adresatów (wysłane, błędy, oczekujące) dla wiadomości aplikacji – jeden wiersz na wiadomość.</summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WiadomoscPodsumowanieDto>>> SelectPodsumowanie(int wiaAplikacjaID)
        {
            return await (
                from p in _context.WiadomoscPracownik.AsNoTracking()
                join w in _context.Wiadomosc.AsNoTracking() on p.wprWiadomoscID equals w.wiaID
                where w.wiaAplikacjaID == wiaAplikacjaID
                group p by p.wprWiadomoscID into g
                select new WiadomoscPodsumowanieDto
                {
                    wiaID = g.Key!.Value,
                    Razem = g.Count(),
                    Wyslane = g.Count(x => x.wprStatus == "4"),
                    Bledy = g.Count(x => x.wprStatus == "5"),
                    Oczekujace = g.Count(x => x.wprStatus == "2")
                })
                .ToListAsync();
        }

        // Widok zawiera odbiorcę bezpośrednio w dbo.Wiadomosc, dlatego rekord jest identyfikowany przez wiaID i usrID.
        [AllowAnonymous]
        [HttpGet("{wiaID}/{usrID}")]
        public async Task<ActionResult<WiadomoscViewModel>> Detail(int wiaID, int usrID)
        {
            var wiadomosc = await _context.WiadomoscViewModel
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.wiaID == wiaID && w.wiaUzytkownikID == usrID);

            if (wiadomosc == null)
            {
                return NotFound();
            }

            return wiadomosc;
        }

        [AllowAnonymous]
        [HttpGet("{wiaID}")]
        public async Task<ActionResult<WiadomoscDto>> Get(int wiaID)
        {
            var wiadomosc = await _context.Wiadomosc.FindAsync(wiaID);

            if (wiadomosc == null)
            {
                return NotFound();
            }

            return wiadomosc.Adapt<WiadomoscDto>();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPut("{wiaID}")]
        public async Task<IActionResult> Update(int wiaID, WiadomoscDto wiadomoscDto)
        {
            if (wiaID != wiadomoscDto.wiaID)
            {
                return BadRequest();
            }

            var wiadomosc = await _context.Wiadomosc.FindAsync(wiaID);

            if (wiadomosc == null)
            {
                return NotFound();
            }

            wiadomoscDto.Adapt(wiadomosc);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WiadomoscExists(wiaID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPost]
        public async Task<ActionResult<WiadomoscDto>> Insert(WiadomoscDto wiadomoscDto)
        {
            var wiadomosc = wiadomoscDto.Adapt<Wiadomosc>();

            _context.Wiadomosc.Add(wiadomosc);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { wiaID = wiadomosc.wiaID }, wiadomosc.Adapt<WiadomoscDto>());
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpDelete("{wiaID}")]
        public async Task<IActionResult> Delete(int wiaID)
        {
            var wiadomosc = await _context.Wiadomosc.FindAsync(wiaID);
            if (wiadomosc == null)
            {
                return NotFound();
            }

            wiadomosc.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WiadomoscExists(int wiaID)
        {
            return _context.Wiadomosc.Any(e => e.wiaID == wiaID);
        }
    }
}
