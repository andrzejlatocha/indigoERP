using indigoERP.Api.Data;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Entities._Wspolne;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace indigoERP.Api.Controllers._Wspolne
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class WiadomoscWysylkaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public WiadomoscWysylkaController(indigoERPContext context)
        {
            _context = context;
        }

        [HttpGet("{wwyID}")]
        public async Task<ActionResult<WiadomoscWysylkaDto>> Get(int wwyID)
        {
            var wysylka = await _context.WiadomoscWysylka.FindAsync(wwyID);

            if (wysylka == null)
            {
                return NotFound();
            }

            return wysylka.Adapt<WiadomoscWysylkaDto>();
        }

        [HttpPost]
        public async Task<ActionResult<WiadomoscWysylkaDto>> Insert(WiadomoscWysylkaDto wysylkaDto)
        {
            var wysylka = wysylkaDto.Adapt<WiadomoscWysylka>();

            _context.WiadomoscWysylka.Add(wysylka);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { wysylka.wwyID }, wysylka.Adapt<WiadomoscWysylkaDto>());
        }
    }
}
