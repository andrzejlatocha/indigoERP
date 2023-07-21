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
    public class GminaListaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public GminaListaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/GminaViewModel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GminaViewModel>>> Select(string gmiStatus)
        {
          if (_context.GminaViewModel == null)
          {
              return NotFound();
          }
            return await _context.GminaViewModel.Where(w => w.gmiStatus == gmiStatus || gmiStatus == "*").ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GminaViewModel>>> SelectByKraj(int kraID)
        {
            if (_context.GminaViewModel == null)
            {
                return NotFound();
            }
            return await _context.GminaViewModel.Where(w => w.kraID == kraID).ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GminaViewModel>>> SelectByWojewodztwo(int wojID)
        {
            if (_context.GminaViewModel == null)
            {
                return NotFound();
            }
            return await _context.GminaViewModel.Where(w => w.wojID == wojID).ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GminaViewModel>>> SelectByPowiat(int powID)
        {
            if (_context.GminaViewModel == null)
            {
                return NotFound();
            }
            return await _context.GminaViewModel.Where(w => w.powID == powID).ToListAsync();
        }
        // GET: api/GminaViewModel/5
        [HttpGet("{gmiID}")]
        public async Task<ActionResult<GminaViewModel>> GetGminaViewModel(int gmiID)
        {
          if (_context.GminaViewModel == null)
          {
              return NotFound();
          }
            var gminaViewModel = await _context.GminaViewModel.FindAsync(gmiID);

            if (gminaViewModel == null)
            {
                return NotFound();
            }

            return gminaViewModel;
        }
    }
}
