using indigoERP.Domain.Models._Autoryzacja;
using indigoERP.Domain.ViewModels._Autoryzacja;
using indigoERP.Service.Interfaces._Autoryzacja;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace indigoERP.Blazor.Utils
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly ILoginService _loginService;

        public ClaimsTransformation(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity == null || string.IsNullOrEmpty(principal.Identity.Name))
                return principal;

            UzytkownikLoginModel uzytkownikModel = new UzytkownikLoginModel(principal.Identity.Name, "masterKEY1");

            UzytkownikViewModel uzytkownik = await _loginService.Get(uzytkownikModel);

            if (uzytkownik == null)
                return principal;

            var clone = principal.Clone();

            Claim[] claims = new Claim[]
            {
                new Claim("usrID", uzytkownik.usrID.ToString()),
                new Claim("usrPracownikID", uzytkownik.usrPracownikID.ToString()),
                new Claim("usrImie", uzytkownik.usrImie.ToString()),
                new Claim("usrNazwisko", uzytkownik.usrNazwisko.ToString()),
                new Claim("usrNazwa", uzytkownik.usrImie.ToString() + " " + uzytkownik.usrNazwisko.ToString()),
                new Claim("appFolder", uzytkownik.appFolder.ToString()),
                new Claim("usrFolder", uzytkownik.usrFolder.ToString()),
                new Claim("usrZdjecie", uzytkownik.usrZdjecie.ToString()),
                new Claim("wdzKod", uzytkownik.wdzKod.ToString()),
                new Claim("wdzSymbol", uzytkownik.wdzSymbol.ToString()),
                new Claim("wdzNazwa", uzytkownik.wdzNazwa.ToString()),
                new Claim("stnKod", uzytkownik.stnKod.ToString()),
                new Claim("stnNazwa", uzytkownik.stnNazwa.ToString()),
                new Claim(ClaimTypes.NameIdentifier, uzytkownik.usrNazwa.ToString()),
                new Claim(ClaimTypes.Email, uzytkownik.usrEmail.ToString()),
                new Claim("accessToken", uzytkownik.usrToken.ToString())
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims);
            clone.AddIdentity(identity);

            return clone;
        }
    }
}
