using indigoERP.Api.Data;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Entities._Wspolne;
using indigoERP.Domain.Enumerators._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Wspolne
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WiadomoscWzorzecController : ControllerBase
    {
        private const string AdministratorRole = "HCZ-indigoERP-Wiadomość";
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;

        public WiadomoscWzorzecController(indigoERPContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<IEnumerable<WiadomoscWzorzecViewModel>>> Select(int msgAplikacjaID, string msgStatus)
        {
            return await _context.WiadomoscWzorzecViewModel
                .Where(w => w.msgAplikacjaID == msgAplikacjaID && (w.msgStatus == msgStatus || msgStatus == "*"))
                .ToListAsync();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<IEnumerable<WiadomoscWzorzecEnumerator>>> SelectLookup(int msgAplikacjaID, string msgStatus = "2")
        {
            return await _context.WiadomoscWzorzec
                .Where(w => w.msgAplikacjaID == msgAplikacjaID && (w.msgStatus == msgStatus || msgStatus == "*"))
                .Select(w => new WiadomoscWzorzecEnumerator { msgID = w.msgID, msgTemat = w.msgTemat, msgStatus = w.msgStatus })
                .ToListAsync();
        }

        [HttpGet("{msgID}")]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<WiadomoscWzorzecDto>> Get(int msgID)
        {
            var wzorzec = await _context.WiadomoscWzorzec.FindAsync(msgID);

            if (wzorzec == null)
            {
                return NotFound();
            }

            return wzorzec.Adapt<WiadomoscWzorzecDto>();
        }

        [HttpPut("{msgID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Update(int msgID, WiadomoscWzorzecDto wzorzecDto)
        {
            if (msgID != wzorzecDto.msgID)
            {
                return BadRequest();
            }

            var wzorzec = await _context.WiadomoscWzorzec.FindAsync(msgID);

            if (wzorzec == null)
            {
                return NotFound();
            }

            wzorzecDto.Adapt(wzorzec);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WiadomoscWzorzecExists(msgID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<WiadomoscWzorzecDto>> Insert(WiadomoscWzorzecDto wzorzecDto)
        {
            var wzorzec = wzorzecDto.Adapt<WiadomoscWzorzec>();

            _context.WiadomoscWzorzec.Add(wzorzec);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { wzorzec.msgID }, wzorzec.Adapt<WiadomoscWzorzecDto>());
        }

        [HttpDelete("{msgID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Delete(int msgID)
        {
            var wzorzec = await _context.WiadomoscWzorzec.FindAsync(msgID);

            if (wzorzec == null)
            {
                return NotFound();
            }

            wzorzec.msgStatus = "1";
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WiadomoscWzorzecExists(int msgID)
        {
            return _context.WiadomoscWzorzec.Any(e => e.msgID == msgID);
        }
    }
}
