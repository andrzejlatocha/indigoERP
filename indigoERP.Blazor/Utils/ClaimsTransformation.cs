using indigoERP.Domain.Models._Autoryzacja;
using indigoERP.Domain.ViewModels._Autoryzacja;
using indigoERP.Service.Interfaces._Autoryzacja;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace indigoERP.Blazor.Utils
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly IConfiguration _configuration;
        private readonly ILoginService _loginService;

        public ClaimsTransformation(IConfiguration configuration, ILoginService loginService)
        {
            _configuration = configuration;
            _loginService = loginService;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity == null || string.IsNullOrEmpty(principal.Identity.Name))
                return principal;

            UzytkownikLoginModel uzytkownikModel = new UzytkownikLoginModel(principal.Identity.Name, "masterKEY1");
            uzytkownikModel.appID = _configuration["appSettings:appID"];

            UzytkownikViewModel uzytkownik = await _loginService.Get(uzytkownikModel);

            if (uzytkownik == null)
                return principal;

            var clone = principal.Clone();

            Claim[] claims = new Claim[]
            {
                new Claim("usrID", uzytkownik.usrID.ToString()),
                new Claim("usrPracownikID", uzytkownik.usrPracownikID.ToString()),
                new Claim("usrImie", $"{uzytkownik.usrImie}"),
                new Claim("usrNazwisko", $"{uzytkownik.usrNazwisko}"),
                new Claim("usrNazwa", $"{uzytkownik.usrImie} {uzytkownik.usrNazwisko}"),
                new Claim("appFolder", $"{uzytkownik.appFolder}"),
                new Claim("usrFolder", $"{uzytkownik.usrFolder}"),
                new Claim("usrZdjecie", $"{uzytkownik.usrZdjecie}"),
                new Claim("wdzKod", $"{uzytkownik.wdzKod}"),
                new Claim("wdzSymbol", $"{uzytkownik.wdzSymbol}"),
                new Claim("wdzNazwa", $"{uzytkownik.wdzNazwa}"),
                new Claim("stnKod", $"{uzytkownik.stnKod}"),
                new Claim("stnNazwa", $"{uzytkownik.stnNazwa}"),
                new Claim("usrLogin", $"{uzytkownik.usrNazwa}"),
                new Claim("usrEmail", $"{uzytkownik.usrEmail}"),
                new Claim("accessToken", $"{uzytkownik.usrToken}")
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims);
            clone.AddIdentity(identity);

            return clone;
        }
    }
}
