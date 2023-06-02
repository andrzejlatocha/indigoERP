using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using epicaHR.Domain.Entities._Slownik;
using indigoERP.Api.Data;

namespace indigoERP.Api.Controllers._Slownik
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PocztaListaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public PocztaListaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/PocztaLista
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Poczta>>> Select()
        {
          if (_context.Poczta == null)
          {
              return NotFound();
          }
            return await _context.Poczta.ToListAsync();
        }

        // GET: api/PocztaLista/5
        [HttpGet("{pczID}")]
        public async Task<ActionResult<Poczta>> Get(int pczID)
        {
          if (_context.Poczta == null)
          {
              return NotFound();
          }
            var poczta = await _context.Poczta.FindAsync(pczID);

            if (poczta == null)
            {
                return NotFound();
            }

            return poczta;
        }
    }
}
