using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace indigoERP.WebUI.Pages
{
    [AllowAnonymous]
    public class DeniedModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
