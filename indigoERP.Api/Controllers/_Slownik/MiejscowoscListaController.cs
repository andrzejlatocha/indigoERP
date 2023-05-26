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
    public class MiejscowoscListaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public MiejscowoscListaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/MiejscowoscViewModel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MiejscowoscViewModel>>> Select()
        {
          if (_context.MiejscowoscViewModel == null)
          {
              return NotFound();
          }
            return await _context.MiejscowoscViewModel.ToListAsync();
        }

        // GET: api/MiejscowoscViewModel/5
        [HttpGet("{mscID}")]
        public async Task<ActionResult<MiejscowoscViewModel>> Get(int mscID)
        {
          if (_context.MiejscowoscViewModel == null)
          {
              return NotFound();
          }
            var miejscowoscViewModel = await _context.MiejscowoscViewModel.FindAsync(mscID);

            if (miejscowoscViewModel == null)
            {
                return NotFound();
            }

            return miejscowoscViewModel;
        }
    }
}
