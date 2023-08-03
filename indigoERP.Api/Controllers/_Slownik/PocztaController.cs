using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using epicaHR.Domain.Entities._Slownik;
using indigoERP.Api.Data;
using indigoERP.Domain.ViewModels._Slownik;

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
        public async Task<ActionResult<IEnumerable<PocztaViewModel>>> Select(string pczStatus)
        {
          if (_context.Poczta == null)
          {
              return NotFound();
          }
            return await _context.PocztaViewModel.Where(w => w.pczStatus == pczStatus ).ToListAsync();
        }

        [HttpGet("{kraID}")]
        public async Task<ActionResult<IEnumerable<PocztaViewModel>>> SelectByKraj(int kraID, string pczStatus)
        {
            if (_context.PocztaViewModel == null)
            {
                return NotFound();
            }
            return await _context.PocztaViewModel.Where(w => w.kraID == kraID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
        }

        [HttpGet("{wojID}")]
        public async Task<ActionResult<IEnumerable<PocztaViewModel>>> SelectByWojewodztwo(int wojID, string pczStatus)
        {
            if (_context.Poczta == null)
            {
                return NotFound();
            }
            return await _context.PocztaViewModel.Where(w => w.wojID == wojID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
        }

        [HttpGet("{powID}")]
        public async Task<ActionResult<IEnumerable<PocztaViewModel>>> SelectByPowiat(int powID, string pczStatus)
        {
            if (_context.PocztaViewModel == null)
            {
                return NotFound();
            }
            return await _context.PocztaViewModel.Where(w => w.powID == powID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
        }

        [HttpGet("{gmiID}")]
        public async Task<ActionResult<IEnumerable<PocztaViewModel>>> SelectByGmina(int gmiID, string pczStatus)
        {
            if (_context.PocztaViewModel == null)
            {
                return NotFound();
            }
            return await _context.PocztaViewModel.Where(w => w.gmiID == gmiID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
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
