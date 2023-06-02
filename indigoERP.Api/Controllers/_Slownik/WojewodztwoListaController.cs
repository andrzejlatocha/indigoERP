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
    public class WojewodztwoListaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public WojewodztwoListaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/WojewodztwoViewModel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WojewodztwoViewModel>>> Select()
        {
          if (_context.WojewodztwoViewModel == null)
          {
              return NotFound();
          }
            return await _context.WojewodztwoViewModel.ToListAsync();
        }

        // GET: api/WojewodztwoViewModel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WojewodztwoViewModel>>> SelectByKraj(int kraID)
        {
            if (_context.WojewodztwoViewModel == null)
            {
                return NotFound();
            }
            return await _context.WojewodztwoViewModel.Where(w => w.kraID == kraID).ToListAsync();
        }

        // GET: api/WojewodztwoViewModel/5
        [HttpGet("{wojID}")]
        public async Task<ActionResult<WojewodztwoViewModel>> Get(int wojID)
        {
          if (_context.WojewodztwoViewModel == null)
          {
              return NotFound();
          }
            var wojewodztwoViewModel = await _context.WojewodztwoViewModel.FindAsync(wojID);

            if (wojewodztwoViewModel == null)
            {
                return NotFound();
            }

            return wojewodztwoViewModel;
        }   
    }
}
