using indigoERP.Api.Data;
using indigoERP.Domain.Dtos._Podpis;
using indigoERP.Domain.Entities._Podpis;
using indigoERP.Domain.Enumerators._Podpis;
using indigoERP.Domain.ViewModels._Autoryzacja;
using indigoERP.Domain.ViewModels._Podpis;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Podpis
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PodpisController : ControllerBase
    {
        private const string AdministratorRole = "HCZ-indigoERP-Podpisy";
        private const string BearerScheme = "Bearer";
        private const string ApiKeyHeader = "X-Api-Key";

        private readonly indigoERPContext _context;
        private readonly IConfiguration _configuration;

        public PodpisController(indigoERPContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<IEnumerable<PodpisViewModel>>> Select(string pdpStatus)
        {
            return await _context.PodpisViewModel.Where(w => w.pdpStatus == pdpStatus || pdpStatus == "*").ToListAsync();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<IEnumerable<PodpisEnumerator>>> SelectLookup(string pdpStatus = "2")
        {
            return await _context.PodpisEnumerator.Where(w => w.pdpStatus == pdpStatus || pdpStatus == "*").ToListAsync();
        }

        [HttpGet("{pdpID}")]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<PodpisDto>> Get(int pdpID)
        {
            var podpis = await _context.Podpis.FindAsync(pdpID);

            if (podpis == null)
            {
                return NotFound();
            }

            return podpis.Adapt<PodpisDto>();
        }

        [HttpPut("{pdpID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Update(int pdpID, PodpisDto podpisDto)
        {
            if (pdpID != podpisDto.pdpID)
            {
                return BadRequest();
            }

            var podpis = await _context.Podpis.FindAsync(pdpID);

            if (podpis == null)
            {
                return NotFound();
            }

            podpisDto.Adapt(podpis);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PodpisExists(pdpID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<PodpisDto>> Insert(PodpisDto podpisDto)
        {
            var podpis = podpisDto.Adapt<Podpis>();

            _context.Podpis.Add(podpis);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { podpis.pdpID }, podpis.Adapt<PodpisDto>());
        }

        [HttpDelete("{pdpID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Delete(int pdpID)
        {
            var podpis = await _context.Podpis.FindAsync(pdpID);

            if (podpis == null)
            {
                return NotFound();
            }

            podpis.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Wyznacza gotowy HTML podpisu dla użytkownika wg reguły pierwszeństwa:
        // 1) przypisanie bezpośrednie (PodpisUzytkownik), 2) przypisanie do jednej z jego grup
        // (PodpisGrupa, przy kolizji wygrywa najpóźniej zmodyfikowane), 3) szablon domyślny
        // (pdpDomyslny), 4) brak. Tokeny w treści podstawiane danymi z mvw.vUzytkownik.
        [HttpGet("{usrID}")]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<string>> Preview(int usrID)
        {
            var uzytkownik = await _context.UzytkownikViewModel.FirstOrDefaultAsync(w => w.usrID == usrID);

            if (uzytkownik == null)
            {
                return NotFound();
            }

            return Ok(await BuildSignatureHtmlAsync(uzytkownik));
        }

        // Odpowiednik Preview, ale dla dodatku Outlook: nie ma sesji indigoERP (Bearer), więc
        // użytkownik jest identyfikowany po adresie email z profilu M365, a dostęp zabezpiecza
        // nagłówek X-Api-Key zamiast logowania. Klucz jest widoczny w kodzie JS dodatku (client-side),
        // więc to tylko odstraszacz przed masowym scrapowaniem, nie prawdziwy sekret.
        [HttpGet]
        public async Task<ActionResult<string>> PreviewByEmail(string email)
        {
            var expectedApiKey = _configuration.GetSection("appSettings").GetValue<string>("ApiKey");

            if (string.IsNullOrEmpty(expectedApiKey) || Request.Headers[ApiKeyHeader] != expectedApiKey)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest();
            }

            var uzytkownik = await _context.UzytkownikViewModel.FirstOrDefaultAsync(w => w.usrEmail == email);

            if (uzytkownik == null)
            {
                return NotFound();
            }

            return Ok(await BuildSignatureHtmlAsync(uzytkownik));
        }

        private async Task<string> BuildSignatureHtmlAsync(UzytkownikViewModel uzytkownik)
        {
            var pdpID = await _context.PodpisUzytkownik
                .Where(w => w.pduUzytkownikID == uzytkownik.usrID && w.pduStatus == "2" && (w.Usuniety == false || w.Usuniety == null))
                .Select(w => (int?)w.pduPodpisID)
                .FirstOrDefaultAsync();

            if (pdpID == null)
            {
                var grupyUzytkownika = await _context.UzytkownikGrupaViewModel
                    .Where(w => w.usrID == uzytkownik.usrID)
                    .Select(w => w.groID)
                    .ToListAsync();

                pdpID = await _context.PodpisGrupa
                    .Where(w => grupyUzytkownika.Contains(w.pdgGrupaID) && w.pdgStatus == "2" && (w.Usuniety == false || w.Usuniety == null))
                    .OrderByDescending(w => w.DataModyfikacja)
                    .Select(w => (int?)w.pdgPodpisID)
                    .FirstOrDefaultAsync();
            }

            if (pdpID == null)
            {
                pdpID = await _context.Podpis
                    .Where(w => w.pdpDomyslny && w.pdpStatus == "2" && (w.Usuniety == false || w.Usuniety == null))
                    .Select(w => (int?)w.pdpID)
                    .FirstOrDefaultAsync();
            }

            if (pdpID == null)
            {
                return string.Empty;
            }

            var podpis = await _context.Podpis.FindAsync(pdpID.Value);

            if (podpis == null)
            {
                return string.Empty;
            }

            return podpis.pdpTresc
                .Replace("{Imie}", uzytkownik.usrImie)
                .Replace("{Nazwisko}", uzytkownik.usrNazwisko)
                .Replace("{ImieNazwisko}", string.IsNullOrEmpty(uzytkownik.usrImieNazwisko) ? $"{uzytkownik.usrImie} {uzytkownik.usrNazwisko}" : uzytkownik.usrImieNazwisko)
                .Replace("{Stanowisko}", uzytkownik.stnNazwa)
                .Replace("{Dzial}", uzytkownik.wdzNazwa)
                .Replace("{Telefon}", uzytkownik.usrTelefon)
                .Replace("{Komorka}", uzytkownik.usrKomorka)
                .Replace("{Email}", uzytkownik.usrEmail);
        }

        private bool PodpisExists(int pdpID)
        {
            return _context.Podpis.Any(e => e.pdpID == pdpID);
        }
    }
}
