using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using indigoERP.Api.Data;
using indigoERP.Domain.ViewModels._Autoryzacja;

namespace indigoERP.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UzytkownikAplikacjaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public UzytkownikAplikacjaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/UzytkownikAplikacja
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UzytkownikAplikacjaViewModel>>> Select(int usrID)
        {
           if (_context.UzytkownikAplikacjaViewModel == null)
           {
              return NotFound();
           }

           return await _context.UzytkownikAplikacjaViewModel.Where(w => w.usrID == usrID).ToListAsync(); ;
        }
    }
}
