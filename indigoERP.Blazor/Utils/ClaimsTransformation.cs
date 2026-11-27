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

            string appID = _configuration["appSettings:appID"];

            UzytkownikViewModel uzytkownik;

            try
            {
                uzytkownik = await _loginService.GetSso(principal.Identity.Name, "WebBrowser", appID);
            }
            catch (HttpRequestException ex)
            {
                // Błąd gateway (np. 404 „Nie znaleziono użytkownika”, 423 konto zablokowane) nie może przerwać potoku:
                // zapisujemy kod w claimie, a strona pokazuje odmowę dostępu zamiast 500.
                var failure = principal.Clone();
                failure.AddIdentity(new ClaimsIdentity(
                    new[] { new Claim("ssoFailureStatusCode", ((int?)ex.StatusCode ?? 0).ToString()) }, "jwtAuthType"));
                return failure;
            }

            if (uzytkownik == null)
                return principal;

            var clone = principal.Clone();
            var claims = new List<Claim>();

            claims.Add(new Claim("usrID", uzytkownik.usrID.ToString()));
            claims.Add(new Claim("usrPracownikID", uzytkownik.usrPracownikID.ToString()));
            claims.Add(new Claim("usrImie", $"{uzytkownik.usrImie}"));
            claims.Add(new Claim("usrNazwisko", $"{uzytkownik.usrNazwisko}"));
            claims.Add(new Claim("usrNazwa", $"{uzytkownik.usrImie} {uzytkownik.usrNazwisko}"));
            claims.Add(new Claim("appFolder", $"{uzytkownik.appFolder}"));
            claims.Add(new Claim("usrFolder", $"{uzytkownik.usrFolder}"));
            claims.Add(new Claim("usrZdjecie", $"{uzytkownik.usrZdjecie}"));
            claims.Add(new Claim("wdzKod", $"{uzytkownik.wdzKod}"));
            claims.Add(new Claim("wdzSymbol", $"{uzytkownik.wdzSymbol}"));
            claims.Add(new Claim("wdzNazwa", $"{uzytkownik.wdzNazwa}"));
            claims.Add(new Claim("stnKod", $"{uzytkownik.stnKod}"));
            claims.Add(new Claim("stnNazwa", $"{uzytkownik.stnNazwa}"));
            claims.Add(new Claim("usrLogin", $"{uzytkownik.usrNazwa}"));
            claims.Add(new Claim("usrEmail", $"{uzytkownik.usrEmail}"));
            claims.Add(new Claim("accessToken", $"{uzytkownik.usrToken}"));

            string[] roles = uzytkownik.srlNazwa.Split('|');
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            ClaimsIdentity identity = new ClaimsIdentity(claims, "jwtAuthType");
            clone.AddIdentity(identity);

            return clone;
        }
    }
}
