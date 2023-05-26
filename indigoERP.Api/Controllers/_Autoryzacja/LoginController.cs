using indigoERP.Api.Data;
using indigoERP.Domain.ViewModels._Autoryzacja;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace indigoERP.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly indigoERPContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public LoginController(indigoERPContext context, IConfiguration configuration, [NotNull] ILogger<LoginController> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Pobiera użytkownika
        /// </summary>
        /// <param name="usrNazwa">
        /// </param>
        /// <param name="usrHaslo">
        /// </param>
        /// <returns>
        /// Klasa UzytkownikViewModel
        /// </returns>
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public ActionResult<UzytkownikViewModel> Get(string usrNazwa, string usrHaslo)
        {
            _logger.LogInformation($"LoginControler [Get] - parameters {usrNazwa}");

            var uzytkownik = Authenticate(usrNazwa, usrHaslo);

            if (uzytkownik != null)
            {
                var token = GenerateToken(uzytkownik);

                uzytkownik.usrToken = token.ToString();

                return uzytkownik;
            }

            return NotFound("Błędne hasło lub nazwa użytkownika.");
        }

        private object GenerateToken(UzytkownikViewModel uzytkownikViewModel)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            claims.Add(new Claim("usrID", uzytkownikViewModel.usrID.ToString()));
            claims.Add(new Claim("usrImie", uzytkownikViewModel.usrImie));
            claims.Add(new Claim("usrNazwisko", uzytkownikViewModel.usrNazwisko));

            claims.Add(new Claim("appFolder", uzytkownikViewModel.appFolder));
            claims.Add(new Claim("usrFolder", uzytkownikViewModel.usrFolder));
            claims.Add(new Claim("usrZdjecie", uzytkownikViewModel.usrZdjecie));
            claims.Add(new Claim("wdzKod", uzytkownikViewModel.wdzKod));
            claims.Add(new Claim("wdzSymbol", uzytkownikViewModel.wdzSymbol));
            claims.Add(new Claim("wdzNazwa", uzytkownikViewModel.wdzNazwa));
            claims.Add(new Claim("stnKod", uzytkownikViewModel.stnKod));
            claims.Add(new Claim("stnNazwa", uzytkownikViewModel.stnNazwa));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, uzytkownikViewModel.usrNazwa));
            claims.Add(new Claim(ClaimTypes.Email, uzytkownikViewModel.usrEmail));
            //claims.Add(new Claim(ClaimTypes.Role, uzytkownikViewModel.srlNazwa));

            string[] roles = uzytkownikViewModel.srlNazwa.Split('|');

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims,
              expires: DateTime.Now.AddMinutes(60), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UzytkownikViewModel Authenticate(string usrNazwa, string usrHaslo)
        {
            var usrComputer = "WebBrowser";
            var appID = _configuration["appSettings:appID"];

            IEnumerable<UzytkownikViewModel> uzytkownik = _context.UzytkownikViewModel
                .FromSqlRaw($"EXEC uspGetUzytkownik '{usrNazwa}', '{usrHaslo}', '{usrComputer}', {appID}")
                .AsEnumerable<UzytkownikViewModel>();

            if (uzytkownik != null)
            {
                if (uzytkownik.Count() == 1)
                {
                    return uzytkownik.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
