using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using indigoERP.Domain.Data;
using indigoERP.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace indigoERP.WebUI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UzytkownikAplikacjaController : Controller
    {
        private readonly IUzytkownikAplikacjaService _uzytkownikAplikacjaService;

        public UzytkownikAplikacjaController(IUzytkownikAplikacjaService uzytkownikAplikacjaService)
        {
            _uzytkownikAplikacjaService = uzytkownikAplikacjaService;
        }

        [HttpGet]
        public async Task<IActionResult> Select(DataSourceLoadOptions loadOptions)
        {
            int usrID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "usrID")?.Value);

            var uzytkownikAplikacja = await _uzytkownikAplikacjaService.Select(usrID);

            loadOptions.PrimaryKey = new[] { "appID" };
            loadOptions.PaginateViaPrimaryKey = true;

            return Json(DataSourceLoader.Load(uzytkownikAplikacja, loadOptions));
        }
    }
}
