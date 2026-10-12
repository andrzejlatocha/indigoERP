using indigoERP.Api.Data;
using indigoERP.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly indigoERPContext _context;

        public HomeController(indigoERPContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AplikacjaViewModel>>> SelectAplikacja(string appStatus)
        {
            return await _context.AplikacjaViewModel
                .Where(w => w.appWyswietl && (w.appStatus == appStatus || appStatus == "*"))
                .ToListAsync();
        }

        /// <summary>
        /// Aplikacje do wyświetlenia na stronie głównej, w których użytkownik ma co najmniej jedną rolę
        /// (bezpośrednio albo przez grupę - mvw.vUzytkownikAplikacjaDostep).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AplikacjaViewModel>>> SelectAplikacjaByUzytkownik(int usrID, string appStatus)
        {
            var aplikacje = _context.UzytkownikAplikacjaDostepViewModel.Where(d => d.usrID == usrID).Select(d => d.appID);

            return await _context.AplikacjaViewModel
                .Where(w => w.appWyswietl && (w.appStatus == appStatus || appStatus == "*") && aplikacje.Contains(w.appID))
                .ToListAsync();
        }
    }
}
