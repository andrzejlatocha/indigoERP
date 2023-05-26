using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using indigoERP.Service.Interfaces;
using indigoERP.Domain.ViewModels._Autoryzacja;

namespace indigoERP.WebUI.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        //private readonly IUzytkownikAplikacjaService _uzytkownikAplikacjaService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            //_uzytkownikAplikacjaService = uzytkownikAplikacjaService;
        }

        public void OnGet()
        {
        }

        //public IEnumerable<UzytkownikAplikacjaViewModel> uzytkownikAplikacja { get; set; }

        //public async Task<IActionResult> OnGetAsync()
        //{
        //    int usrID = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "usrID")?.Value);

        //    uzytkownikAplikacja = await _uzytkownikAplikacjaService.Select(usrID);

        //    return Page();
        //}
    }
}