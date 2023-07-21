using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using indigoERP.Api.Data;
using indigoERP.Domain.ViewModels._Slownik;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KrajListaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public KrajListaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/KrajViewModel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KrajViewModel>>> Select(string kraStatus)
        {
          if (_context.KrajViewModel == null)
          {
              return NotFound();
          }
            return await _context.KrajViewModel.Where(w => w.kraStatus == kraStatus || kraStatus == "*").ToListAsync();
        }

        // GET: api/KrajViewModel/5
        [HttpGet("{kraID}")]
        public async Task<ActionResult<KrajViewModel>> Get(int kraID)
        {
          if (_context.KrajViewModel == null)
          {
              return NotFound();
          }
            var krajViewModel = await _context.KrajViewModel.FindAsync(kraID);

            if (krajViewModel == null)
            {
                return NotFound();
            }

            return krajViewModel;
        }
    }
}
