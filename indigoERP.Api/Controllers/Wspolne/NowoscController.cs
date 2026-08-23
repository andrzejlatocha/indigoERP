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
    public class NowoscController : ControllerBase
    {
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;

        public NowoscController(indigoERPContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NowoscViewModel>>> Select(string nowStatus)
        {
            return await _context.NowoscViewModel.Where(w => w.nowStatus == nowStatus || nowStatus == "*").ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("{appID}")]
        public async Task<ActionResult<IEnumerable<NowoscViewModel>>> SelectByAplikacja(int appID, string nowStatus)
        {
            return await _context.NowoscViewModel.Where(w => w.nowAplikacjaID == appID && (w.nowStatus == nowStatus || nowStatus == "*")).ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("{nowID}")]
        public async Task<ActionResult<NowoscDto>> Get(int nowID)
        {
            var nowosc = await _context.Nowosc.FindAsync(nowID);

            if (nowosc == null)
            {
                return NotFound();
            }

            return nowosc.Adapt<NowoscDto>();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPut("{nowID}")]
        public async Task<IActionResult> Update(int nowID, NowoscDto nowoscDto)
        {
            if (nowID != nowoscDto.nowID)
            {
                return BadRequest();
            }

            var nowosc = await _context.Nowosc.FindAsync(nowID);

            if (nowosc == null)
            {
                return NotFound();
            }

            nowoscDto.Adapt(nowosc);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NowoscExists(nowID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpPost]
        public async Task<ActionResult<NowoscDto>> Insert(NowoscDto nowoscDto)
        {
            var nowosc = nowoscDto.Adapt<Nowosc>();

            _context.Nowosc.Add(nowosc);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { nowID = nowosc.nowID }, nowosc.Adapt<NowoscDto>());
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpDelete("{nowID}")]
        public async Task<IActionResult> Delete(int nowID)
        {
            var nowosc = await _context.Nowosc.FindAsync(nowID);
            if (nowosc == null)
            {
                return NotFound();
            }

            _context.Nowosc.Remove(nowosc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NowoscExists(int nowID)
        {
            return _context.Nowosc.Any(e => e.nowID == nowID);
        }
    }
}
