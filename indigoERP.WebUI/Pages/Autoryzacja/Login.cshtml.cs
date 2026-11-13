using indigoERP.Domain.Models._Autoryzacja;
using indigoERP.Service.Interfaces._Autoryzacja;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace epicaHR.Desktop.WebUI.Pages._Autoryzacja
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly ILoginService _loginService;
        public static string? _returnUrl;

        public LoginModel(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [BindProperty]
        public UzytkownikLoginModel uzytkownikModel { get; set; }

        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            _returnUrl = returnUrl;

            string? user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            if (!string.IsNullOrEmpty(user))
            {
                var uzytkownikModel = new UzytkownikLoginModel();
                uzytkownikModel.usrNazwa = user;
                uzytkownikModel.usrHaslo = "masterKEY1";

                if (await GetUser(uzytkownikModel))
                {
                    return Redirect(_returnUrl);
                }
                else
                {
                    return Redirect("/Denied");
                }
            }
            else
            {
                return Redirect("/Denied");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (await GetUser(uzytkownikModel))
            {
                return Redirect(_returnUrl);
            }

            return NotFound("B��dne has�o lub nazwa u�ytkownika.");
        }

        public async Task<bool> GetUser(UzytkownikLoginModel uzytkownikModel)
        {
            if (uzytkownikModel != null)
            {
                var uzytkownik = await _loginService.Get(uzytkownikModel);

                if (uzytkownik != null)
                {
                    var claims = new List<Claim>();

                    claims.Add(new Claim("usrID", uzytkownik.usrID.ToString()));
                    claims.Add(new Claim("usrImie", uzytkownik.usrImie));
                    claims.Add(new Claim("usrNazwisko", uzytkownik.usrNazwisko));
                    claims.Add(new Claim("appFolder", uzytkownik.appFolder));
                    claims.Add(new Claim("usrFolder", uzytkownik.usrFolder));
                    claims.Add(new Claim("usrZdjecie", uzytkownik.usrZdjecie));
                    claims.Add(new Claim("wdzKod", uzytkownik.wdzKod));
                    claims.Add(new Claim("wdzSymbol", uzytkownik.wdzSymbol));
                    claims.Add(new Claim("wdzNazwa", uzytkownik.wdzNazwa));
                    claims.Add(new Claim("stnKod", uzytkownik.stnKod));
                    claims.Add(new Claim("stnNazwa", uzytkownik.stnNazwa));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, uzytkownik.usrNazwa));
                    claims.Add(new Claim(ClaimTypes.Email, uzytkownik.usrEmail));
                    claims.Add(new Claim("accessToken", uzytkownik.usrToken));

                    string[] roles = uzytkownik.srlNazwa.Split('|');

                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    HttpContext.SignInAsync(claimsPrincipal);

                    return true;
                }
            }

            return false;
        }
    }
}
