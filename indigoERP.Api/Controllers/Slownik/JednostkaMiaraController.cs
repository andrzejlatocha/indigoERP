using indigoERP.Api.Data;
using indigoERP.Domain.Dtos._Slownik;
using indigoERP.Domain.Entities._Slownik;
using indigoERP.Domain.Enumerators._Slownik;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JednostkaMiaraController : ControllerBase
    {
        private const string SlownikRoles = "HCZ-indigoERP-Administrator,PL-SCX-APP-indigoER-Administrator,PL-SCX-APP-indigoER-Słownik";

        private readonly indigoERPContext _context;

        public JednostkaMiaraController(indigoERPContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JednostkaMiara>>> Select(string jmiStatus)
        {
            if (_context.JednostkaMiara == null)
            {
                return NotFound();
            }

            return await _context.JednostkaMiara.Where(w => w.jmiStatus == jmiStatus || jmiStatus == "*").OrderBy(w => w.jmiOpis).ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JednostkaMiaraEnumerator>>> SelectLookup(string jmiStatus)
        {
            if (_context.JednostkaMiaraEnumerator == null)
            {
                return NotFound();
            }

            return await _context.JednostkaMiaraEnumerator.Where(w => w.jmiStatus == jmiStatus || jmiStatus == "*").OrderBy(w => w.jmiOpis).ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("{jmiKOD}")]
        public async Task<ActionResult<JednostkaMiaraDto>> Get(string jmiKOD)
        {
            if (_context.JednostkaMiara == null)
            {
                return NotFound();
            }

            var jednostkaMiara = await _context.JednostkaMiara.FindAsync(jmiKOD);

            if (jednostkaMiara == null)
            {
                return NotFound();
            }

            return jednostkaMiara.Adapt<JednostkaMiaraDto>();
        }

        [Authorize(Roles = SlownikRoles, AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPut("{jmiKOD}")]
        public async Task<IActionResult> Update(string jmiKOD, JednostkaMiaraDto jednostkaMiaraDto)
        {
            if (jmiKOD != jednostkaMiaraDto.jmiKOD)
            {
                return BadRequest("Nie można zmienić kodu istniejącej jednostki miary.");
            }

            if (_context.JednostkaMiara == null)
            {
                return NotFound();
            }

            var jednostkaMiara = await _context.JednostkaMiara.FindAsync(jmiKOD);

            if (jednostkaMiara == null)
            {
                return NotFound();
            }

            jednostkaMiaraDto.Adapt(jednostkaMiara);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JednostkaMiaraExists(jmiKOD))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [Authorize(Roles = SlownikRoles, AuthenticationSchemes = "Bearer,Cookie")]
        [HttpPost]
        public async Task<ActionResult<JednostkaMiaraDto>> Insert(JednostkaMiaraDto jednostkaMiaraDto)
        {
            if (_context.JednostkaMiara == null)
            {
                return Problem("Entity set 'indigoERPContext.JednostkaMiara' is null.");
            }

            if (await _context.JednostkaMiara.AnyAsync(w => w.jmiKOD == jednostkaMiaraDto.jmiKOD))
            {
                return Conflict($"Jednostka miary o kodzie {jednostkaMiaraDto.jmiKOD} już istnieje.");
            }

            var jednostkaMiara = jednostkaMiaraDto.Adapt<JednostkaMiara>();

            _context.JednostkaMiara.Add(jednostkaMiara);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { jmiKOD = jednostkaMiara.jmiKOD }, jednostkaMiara.Adapt<JednostkaMiaraDto>());
        }

        [Authorize(Roles = SlownikRoles, AuthenticationSchemes = "Bearer,Cookie")]
        [HttpDelete("{jmiKOD}")]
        public async Task<IActionResult> Delete(string jmiKOD)
        {
            if (_context.JednostkaMiara == null)
            {
                return NotFound();
            }

            var jednostkaMiara = await _context.JednostkaMiara.FindAsync(jmiKOD);

            if (jednostkaMiara == null)
            {
                return NotFound();
            }

            _context.JednostkaMiara.Remove(jednostkaMiara);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JednostkaMiaraExists(string jmiKOD)
        {
            return (_context.JednostkaMiara?.Any(e => e.jmiKOD == jmiKOD)).GetValueOrDefault();
        }
    }
}
