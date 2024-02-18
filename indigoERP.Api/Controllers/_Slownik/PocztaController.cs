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
using Microsoft.AspNetCore.Authorization;
using indigoERP.Domain.Enumerators._Slownik;

namespace indigoERP.Api.Controllers._Slownik
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PocztaController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public PocztaController(indigoERPContext context)
        {
            _context = context;
        }

        // GET: api/Poczta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PocztaViewModel>>> Select(string pczStatus)
        {
            if (_context.PocztaViewModel == null)
            {
                return NotFound();
            }

            return await _context.PocztaViewModel.Where(w => w.pczStatus == pczStatus).ToListAsync();
        }

        // GET: api/Poczta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PocztaEnumerator>>> SelectLookup(string pczStatus)
        {
            if (_context.PocztaEnumerator == null)
            {
                return NotFound();
            }

            return await _context.PocztaEnumerator.Where(w => w.pczStatus == pczStatus).ToListAsync();
        }

        // GET: api/Poczta
        [HttpGet("{kraID}")]
        public async Task<ActionResult<IEnumerable<PocztaViewModel>>> SelectByKraj(int kraID, string pczStatus)
        {
            if (_context.PocztaViewModel == null)
            {
                return NotFound();
            }

            return await _context.PocztaViewModel.Where(w => w.kraID == kraID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
        }

        // GET: api/Poczta
        [HttpGet("{kraID}")]
        public async Task<ActionResult<IEnumerable<PocztaEnumerator>>> SelectLookupByKraj(int kraID, string pczStatus)
        {
            if (_context.PocztaEnumerator == null)
            {
                return NotFound();
            }

            return await _context.PocztaEnumerator.Where(w => w.kraID == kraID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
        }

        // GET: api/Poczta
        [HttpGet("{wojID}")]
        public async Task<ActionResult<IEnumerable<PocztaViewModel>>> SelectByWojewodztwo(int wojID, string pczStatus)
        {
            if (_context.PocztaViewModel == null)
            {
                return NotFound();
            }

            return await _context.PocztaViewModel.Where(w => w.wojID == wojID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
        }

        // GET: api/Poczta
        [HttpGet("{wojID}")]
        public async Task<ActionResult<IEnumerable<PocztaEnumerator>>> SelectLookupByWojewodztwo(int wojID, string pczStatus)
        {
            if (_context.PocztaEnumerator == null)
            {
                return NotFound();
            }

            return await _context.PocztaEnumerator.Where(w => w.wojID == wojID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
        }

        // GET: api/Poczta
        [HttpGet("{powID}")]
        public async Task<ActionResult<IEnumerable<PocztaViewModel>>> SelectByPowiat(int powID, string pczStatus)
        {
            if (_context.PocztaViewModel == null)
            {
                return NotFound();
            }

            return await _context.PocztaViewModel.Where(w => w.powID == powID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
        }

        // GET: api/Poczta
        [HttpGet("{powID}")]
        public async Task<ActionResult<IEnumerable<PocztaEnumerator>>> SelectLookupByPowiat(int powID, string pczStatus)
        {
            if (_context.PocztaEnumerator == null)
            {
                return NotFound();
            }

            return await _context.PocztaEnumerator.Where(w => w.powID == powID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
        }

        // GET: api/Poczta
        [HttpGet("{gmiID}")]
        public async Task<ActionResult<IEnumerable<PocztaViewModel>>> SelectByGmina(int gmiID, string pczStatus)
        {
            if (_context.PocztaViewModel == null)
            {
                return NotFound();
            }

            return await _context.PocztaViewModel.Where(w => w.gmiID == gmiID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
        }

        // GET: api/Poczta
        [HttpGet("{gmiID}")]
        public async Task<ActionResult<IEnumerable<PocztaEnumerator>>> SelectLookupByGmina(int gmiID, string pczStatus)
        {
            if (_context.PocztaEnumerator == null)
            {
                return NotFound();
            }

            return await _context.PocztaEnumerator.Where(w => w.gmiID == gmiID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet("{pczID}")]
        public async Task<ActionResult<IEnumerable<PocztaViewModel>>> SelectByPoczta(int pczID, string pczStatus)
        {
            if (_context.PocztaViewModel == null)
            {
                return NotFound();
            }
            return await _context.PocztaViewModel.Where(w => w.pczID == pczID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
        }

        // GET: api/Miejscowosc
        [AllowAnonymous]
        [HttpGet("{pczID}")]
        public async Task<ActionResult<IEnumerable<PocztaEnumerator>>> SelectLookupByPoczta(int pczID, string pczStatus)
        {
            if (_context.PocztaEnumerator == null)
            {
                return NotFound();
            }
            return await _context.PocztaEnumerator.Where(w => w.pczID == pczID && (w.pczStatus == pczStatus || pczStatus == "*")).ToListAsync();
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
