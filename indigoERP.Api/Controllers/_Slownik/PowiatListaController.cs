using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using indigoERP.Api.Data;
using indigoERP.Domain.ViewModels._Slownik;
using Microsoft.AspNetCore.Authorization;

namespace indigoERP.Api.Controllers._Slownik
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PowiatListaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public PowiatListaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/PowiatViewModel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PowiatViewModel>>> Select(string powStatus)
        {
          if (_context.PowiatViewModel == null)
          {
              return NotFound();
          }
            return await _context.PowiatViewModel.Where(w => w.powStatus == powStatus || powStatus == "*").ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PowiatViewModel>>> SelectByKraj(int kraID, string powStatus)
        {
            if (_context.PowiatViewModel == null)
            {
                return NotFound();
            }
            return await _context.PowiatViewModel.Where(w => w.kraID == kraID && (w.powStatus == powStatus || powStatus == "*")).ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PowiatViewModel>>> SelectByWojewodztwo(int wojID, string powStatus)
        {
            if (_context.PowiatViewModel == null)
            {
                return NotFound();
            }
            return await _context.PowiatViewModel.Where(w => w.wojID == wojID && (w.powStatus == powStatus || powStatus == "*")).ToListAsync();
        }

        // GET: api/PowiatViewModel/5
        [HttpGet("{powID}")]
        public async Task<ActionResult<PowiatViewModel>> GetPowiatViewModel(int powID)
        {
          if (_context.PowiatViewModel == null)
          {
              return NotFound();
          }
            var powiatViewModel = await _context.PowiatViewModel.FindAsync(powID);

            if (powiatViewModel == null)
            {
                return NotFound();
            }

            return powiatViewModel;
        }
    }
}
