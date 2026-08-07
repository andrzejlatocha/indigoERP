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
    public class ZadanieController : ControllerBase
    {
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;

        public ZadanieController(indigoERPContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ZadanieViewModel>>> Select(int tskAplikacjaID, string tskStatus)
        {
            return await _context.ZadanieViewModel.Where(w => w.tskAplikacjaID == tskAplikacjaID && (w.tskStatus == tskStatus || tskStatus == "*")).ToListAsync();
        }

        // Filtruje po appID + usrID (porównanie do tusUzytkownikID), tskStatus opcjonalny (domyślnie "*" = bez filtra).
        [AllowAnonymous]
        [HttpGet("{appID}/{usrID}")]
        public async Task<ActionResult<IEnumerable<ZadanieViewModel>>> SelectByUzytkownik(int appID, int usrID, string tskStatus = "*")
        {
            return await _context.ZadanieViewModel.Where(w => w.tskAplikacjaID == appID && w.tusUzytkownikID == usrID && (w.tskStatus == tskStatus || tskStatus == "*")).ToListAsync();
        }

        // Licznik nieprzeczytanych (tusOdczyt = false) dla danego użytkownika w danej aplikacji — pod badge w UI,
        // żeby nie trzeba było ściągać całej listy tylko po liczbę.
        [AllowAnonymous]
        [HttpGet("{appID}/{usrID}")]
        public async Task<ActionResult<int>> SelectCountByUzytkownik(int appID, int usrID)
        {
            return await _context.ZadanieViewModel.CountAsync(w => w.tskAplikacjaID == appID && w.tusUzytkownikID == usrID && w.tusOdczyt == false);
        }

        // Jeden wiersz z widoku (zadanie + konkretny przypisany użytkownik) — mvw.vZadanie ma jeden wiersz
        // na parę zadanie/użytkownik (INNER JOIN do ZadanieUzytkownik), stąd klucz złożony.
        [AllowAnonymous]
        [HttpGet("{tskID}/{usrID}")]
        public async Task<ActionResult<ZadanieViewModel>> Detail(int tskID, int usrID)
        {
            var zadanie = await _context.ZadanieViewModel
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.tskID == tskID && w.tusUzytkownikID == usrID);

            if (zadanie == null)
            {
                return NotFound();
            }

            return zadanie;
        }

        [AllowAnonymous]
        [HttpGet("{tskID}")]
        public async Task<ActionResult<ZadanieDto>> Get(int tskID)
        {
            var zadanie = await _context.Zadanie.FindAsync(tskID);

            if (zadanie == null)
            {
                return NotFound();
            }

            return zadanie.Adapt<ZadanieDto>();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPut("{tskID}")]
        public async Task<IActionResult> Update(int tskID, ZadanieDto zadanieDto)
        {
            if (tskID != zadanieDto.tskID)
            {
                return BadRequest();
            }

            var zadanie = await _context.Zadanie.FindAsync(tskID);

            if (zadanie == null)
            {
                return NotFound();
            }

            zadanieDto.Adapt(zadanie);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZadanieExists(tskID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPost]
        public async Task<ActionResult<ZadanieDto>> Insert(ZadanieDto zadanieDto)
        {
            var zadanie = zadanieDto.Adapt<Zadanie>();

            _context.Zadanie.Add(zadanie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { tskID = zadanie.tskID }, zadanie.Adapt<ZadanieDto>());
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpDelete("{tskID}")]
        public async Task<IActionResult> Delete(int tskID)
        {
            var zadanie = await _context.Zadanie.FindAsync(tskID);
            if (zadanie == null)
            {
                return NotFound();
            }

            zadanie.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ZadanieExists(int tskID)
        {
            return _context.Zadanie.Any(e => e.tskID == tskID);
        }
    }
}
