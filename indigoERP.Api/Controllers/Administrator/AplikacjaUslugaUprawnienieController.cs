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
    // Nadania service-to-service (authorizationSRV Service/GetToken) — w odróżnieniu od
    // Rola/Grupa/Aplikacja celowo BEZ [AllowAnonymous] na odczycie: to tabela kontrolująca
    // zaufanie między aplikacjami, nie słownik do cudzych comboboxów.
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "HCZ-indigoERP-Administrator", AuthenticationSchemes = "Bearer")]
    public class AplikacjaUslugaUprawnienieController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public AplikacjaUslugaUprawnienieController(indigoERPContext context)
        {
            _context = context;
        }

        // usgStatus: "2" = aktywne (usgAktywny=1), "1" = nieaktywne (usgAktywny=0), "*" = wszystkie —
        // ta sama konwencja co Select(xxxStatus) w Rola/Grupa/Aplikacja, dla spójności filtra w gridzie.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AplikacjaUslugaUprawnienieViewModel>>> Select(string usgStatus)
        {
            return await _context.AplikacjaUslugaUprawnienieViewModel
                .Where(w => usgStatus == "*" || (usgStatus == "2" ? w.usgAktywny : !w.usgAktywny))
                .ToListAsync();
        }

        [HttpGet("{usgID}")]
        public async Task<ActionResult<AplikacjaUslugaUprawnienieDto>> Get(int usgID)
        {
            var usluga = await _context.AplikacjaUslugaUprawnienie.FindAsync(usgID);

            if (usluga == null)
            {
                return NotFound();
            }

            return usluga.Adapt<AplikacjaUslugaUprawnienieDto>();
        }

        [HttpPut("{usgID}")]
        public async Task<IActionResult> Update(int usgID, AplikacjaUslugaUprawnienieDto uslugaDto)
        {
            if (usgID != uslugaDto.usgID)
            {
                return BadRequest();
            }

            var usluga = await _context.AplikacjaUslugaUprawnienie.FindAsync(usgID);

            if (usluga == null)
            {
                return NotFound();
            }

            uslugaDto.Adapt(usluga);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AplikacjaUslugaUprawnienieExists(usgID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<AplikacjaUslugaUprawnienieDto>> Insert(AplikacjaUslugaUprawnienieDto uslugaDto)
        {
            var usluga = uslugaDto.Adapt<AplikacjaUslugaUprawnienie>();

            _context.AplikacjaUslugaUprawnienie.Add(usluga);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { usluga.usgID }, usluga.Adapt<AplikacjaUslugaUprawnienieDto>());
        }

        [HttpDelete("{usgID}")]
        public async Task<IActionResult> Delete(int usgID)
        {
            var usluga = await _context.AplikacjaUslugaUprawnienie.FindAsync(usgID);

            if (usluga == null)
            {
                return NotFound();
            }

            // Tabela nie ma kolumny Usuniety (nie dziedziczy AuditableEntity) - w odróżnieniu
            // od Rola/Grupa/Aplikacja usuwamy twardo, zgodnie z rzeczywistym schematem tabeli.
            _context.AplikacjaUslugaUprawnienie.Remove(usluga);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AplikacjaUslugaUprawnienieExists(int usgID)
        {
            return _context.AplikacjaUslugaUprawnienie.Any(e => e.usgID == usgID);
        }
    }
}
