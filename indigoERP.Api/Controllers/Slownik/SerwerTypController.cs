using indigoERP.Api.Data;
using indigoERP.Domain.Entities._Slownik;
using indigoERP.Domain.Enumerators._Slownik;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "HCZ-indigoERP-Administrator", AuthenticationSchemes = "Bearer")]
    public class SerwerTypController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public SerwerTypController(indigoERPContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SerwerTyp>>> Select(string styStatus)
        {
            return await _context.SerwerTyp
                .Where(w => w.styStatus == styStatus || styStatus == "*")
                .ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SerwerTypEnumerator>>> SelectLookup(string styStatus)
        {
            return await _context.SerwerTypEnumerator
                .Where(w => w.styStatus == styStatus || styStatus == "*")
                .ToListAsync();
        }

        [HttpGet("{styKOD}")]
        public async Task<ActionResult<SerwerTyp>> Get(string styKOD)
        {
            var serwerTyp = await _context.SerwerTyp.FindAsync(styKOD);

            if (serwerTyp == null)
            {
                return NotFound();
            }

            return serwerTyp;
        }

        [HttpPut("{styKOD}")]
        public async Task<IActionResult> Update(string styKOD, SerwerTyp serwerTyp)
        {
            if (styKOD != serwerTyp.styKOD)
            {
                return BadRequest();
            }

            _context.Entry(serwerTyp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SerwerTypExists(styKOD))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<SerwerTyp>> Insert(SerwerTyp serwerTyp)
        {
            _context.SerwerTyp.Add(serwerTyp);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SerwerTypExists(serwerTyp.styKOD))
                {
                    return Conflict();
                }

                throw;
            }

            return CreatedAtAction("Get", new { styKOD = serwerTyp.styKOD }, serwerTyp);
        }

        [HttpDelete("{styKOD}")]
        public async Task<IActionResult> Delete(string styKOD)
        {
            var serwerTyp = await _context.SerwerTyp.FindAsync(styKOD);

            if (serwerTyp == null)
            {
                return NotFound();
            }

            serwerTyp.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SerwerTypExists(string styKOD)
        {
            return _context.SerwerTyp.Any(e => e.styKOD == styKOD);
        }
    }
}
