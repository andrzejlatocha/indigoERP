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
    public class ZdarzenieController : ControllerBase
    {
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;

        public ZdarzenieController(indigoERPContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ZdarzenieViewModel>>> Select(int zdaAplikacjaID, string zdaStatus)
        {
            return await _context.ZdarzenieViewModel.Where(w => w.zdaAplikacjaID == zdaAplikacjaID && (w.zdaStatus == zdaStatus || zdaStatus == "*")).ToListAsync();
        }

        // Filtruje po appID + usrID (porównanie do zurUzytkownikID), zdaStatus opcjonalny (domyślnie "*" = bez filtra).
        [AllowAnonymous]
        [HttpGet("{appID}/{usrID}")]
        public async Task<ActionResult<IEnumerable<ZdarzenieViewModel>>> SelectByUzytkownik(int appID, int usrID, string zdaStatus = "*")
        {
            return await _context.ZdarzenieViewModel.Where(w => w.zdaAplikacjaID == appID && w.zurUzytkownikID == usrID && (w.zdaStatus == zdaStatus || zdaStatus == "*")).ToListAsync();
        }

        // Licznik nieprzeczytanych (zurOdczyt = false) dla danego użytkownika w danej aplikacji — pod badge w UI,
        // żeby nie trzeba było ściągać całej listy tylko po liczbę.
        [AllowAnonymous]
        [HttpGet("{appID}/{usrID}")]
        public async Task<ActionResult<int>> SelectCountByUzytkownik(int appID, int usrID)
        {
            return await _context.ZdarzenieViewModel.CountAsync(w => w.zdaAplikacjaID == appID && w.zurUzytkownikID == usrID && w.zurOdczyt == false);
        }

        [AllowAnonymous]
        [HttpGet("{zdaAplikacjaID}/{zurUzytkownikID}")]
        public async Task<ActionResult<IEnumerable<ZdarzenieViewModel>>> SelectOdczytByUzytkownik(int zdaAplikacjaID, int zurUzytkownikID, string zdaStatus)
        {
            return await _context.ZdarzenieViewModel.Where(w => w.zdaAplikacjaID == zdaAplikacjaID && w.zurUzytkownikID == zurUzytkownikID && w.zurOdczyt == true && (w.zdaStatus == zdaStatus || zdaStatus == "*")).ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("{zdaAplikacjaID}/{zurUzytkownikID}")]
        public async Task<ActionResult<IEnumerable<ZdarzenieViewModel>>> SelectBrakOdczytByUzytkownik(int zdaAplikacjaID, int zurUzytkownikID, string zdaStatus)
        {
            return await _context.ZdarzenieViewModel.Where(w => w.zdaAplikacjaID == zdaAplikacjaID && w.zurUzytkownikID == zurUzytkownikID && w.zurOdczyt == false && (w.zdaStatus == zdaStatus || zdaStatus == "*")).ToListAsync();
        }

        // Jeden wiersz z widoku (zdarzenie + konkretny odbiorca) — mvw.vZdarzenie ma jeden wiersz
        // na parę zdarzenie/odbiorca (INNER JOIN do ZdarzenieUzytkownik), stąd klucz złożony.
        [AllowAnonymous]
        [HttpGet("{zdaID}/{usrID}")]
        public async Task<ActionResult<ZdarzenieViewModel>> Detail(int zdaID, int usrID)
        {
            var zdarzenie = await _context.ZdarzenieViewModel
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.zdaID == zdaID && w.zurUzytkownikID == usrID);

            if (zdarzenie == null)
            {
                return NotFound();
            }

            return zdarzenie;
        }

        [AllowAnonymous]
        [HttpGet("{zdaID}")]
        public async Task<ActionResult<ZdarzenieDto>> Get(int zdaID)
        {
            var zdarzenie = await _context.Zdarzenie.FindAsync(zdaID);

            if (zdarzenie == null)
            {
                return NotFound();
            }

            return zdarzenie.Adapt<ZdarzenieDto>();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPut("{zdaID}")]
        public async Task<IActionResult> Update(int zdaID, ZdarzenieDto zdarzenieDto)
        {
            if (zdaID != zdarzenieDto.zdaID)
            {
                return BadRequest();
            }

            var zdarzenie = await _context.Zdarzenie.FindAsync(zdaID);

            if (zdarzenie == null)
            {
                return NotFound();
            }

            zdarzenieDto.Adapt(zdarzenie);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZdarzenieExists(zdaID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPost]
        public async Task<ActionResult<ZdarzenieDto>> Insert(ZdarzenieDto zdarzenieDto)
        {
            var zdarzenie = zdarzenieDto.Adapt<Zdarzenie>();

            _context.Zdarzenie.Add(zdarzenie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { zdaID = zdarzenie.zdaID }, zdarzenie.Adapt<ZdarzenieDto>());
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpDelete("{zdaID}")]
        public async Task<IActionResult> Delete(int zdaID)
        {
            var zdarzenie = await _context.Zdarzenie.FindAsync(zdaID);
            if (zdarzenie == null)
            {
                return NotFound();
            }

            zdarzenie.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ZdarzenieExists(int zdaID)
        {
            return _context.Zdarzenie.Any(e => e.zdaID == zdaID);
        }
    }
}
