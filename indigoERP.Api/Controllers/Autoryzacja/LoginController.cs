using indigoERP.Api.Data;
using indigoERP.Domain.ViewModels._Autoryzacja;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.Common;
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
        /// Pobiera dane logującego się użytkownika
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
        public ActionResult<UzytkownikViewModel> Get(string usrNazwa, string usrHaslo, string? usrKomputer, string? appID)
        {
            _logger.LogInformation($"LoginControler [Get] - parameters {usrNazwa}, {appID}");

            if (usrKomputer == null)
            {
                usrKomputer = "WebBrowser";
            }

            if (appID == null)
            {
                appID = _configuration["appSettings:appID"];

                if (appID == null)
                {
                    appID = "900";
                }
            }

            var uzytkownik = Authenticate(usrNazwa, usrHaslo, usrKomputer, appID);

            if (uzytkownik != null)
            {
                var token = GenerateToken(uzytkownik);

                if (token != null)
                {
                    uzytkownik.usrToken = token.ToString();
                    return uzytkownik;
                }
                else
                {
                    _logger.LogInformation("User token is null");
                }
            }
            else
            {
                _logger.LogInformation("User authenticate object is null");
            }

            return NotFound("Błędne hasło lub nazwa użytkownika.");
        }

        /// <summary>
        /// Loguje uzytkownika bez hasla - wylacznie dla trusted subsystem (SSO z Windows Authentication).
        /// Chronione sekretem w naglowku X-Internal-Sso-Key, nie haslem uzytkownika.
        /// </summary>
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public ActionResult<UzytkownikViewModel> GetSso(string usrNazwa, string? usrKomputer, string? appID)
        {
            string? providedKey = Request.Headers["X-Internal-Sso-Key"];
            string? expectedKey = _configuration["InternalSso:Key"];

            if (string.IsNullOrWhiteSpace(expectedKey) || !string.Equals(providedKey, expectedKey, StringComparison.Ordinal))
            {
                _logger.LogWarning($"LoginControler [GetSso] - nieautoryzowana proba, usrNazwa {usrNazwa}");
                return Unauthorized();
            }

            _logger.LogInformation($"LoginControler [GetSso] - parameters {usrNazwa}, {appID}");

            if (usrKomputer == null)
            {
                usrKomputer = "WebBrowser";
            }

            if (appID == null)
            {
                appID = _configuration["appSettings:appID"];

                if (appID == null)
                {
                    appID = "900";
                }
            }

            var uzytkownik = AuthenticateSso(usrNazwa, usrKomputer, appID);

            if (uzytkownik != null)
            {
                var token = GenerateToken(uzytkownik);

                if (token != null)
                {
                    uzytkownik.usrToken = token.ToString();
                    return uzytkownik;
                }
                else
                {
                    _logger.LogInformation("User token is null (SSO)");
                }
            }
            else
            {
                _logger.LogInformation("User authenticate object is null (SSO)");
            }

            return NotFound("Nie znaleziono użytkownika.");
        }

        private object? GenerateToken(UzytkownikViewModel uzytkownikViewModel)
        {
            SymmetricSecurityKey? securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:Key"]));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, uzytkownikViewModel.usrNazwa));
            claims.Add(new Claim("usrID", uzytkownikViewModel.usrID.ToString()));
            claims.Add(new Claim("usrImie", uzytkownikViewModel.usrImie));
            claims.Add(new Claim("usrNazwisko", uzytkownikViewModel.usrNazwisko));
            claims.Add(new Claim("usrNazwa", uzytkownikViewModel.usrNazwa));
            claims.Add(new Claim("appFolder", uzytkownikViewModel.appFolder));
            claims.Add(new Claim("usrFolder", uzytkownikViewModel.usrFolder));
            claims.Add(new Claim("usrZdjecie", uzytkownikViewModel.usrZdjecie));
            claims.Add(new Claim("wdzKod", uzytkownikViewModel.wdzKod));
            claims.Add(new Claim("wdzSymbol", uzytkownikViewModel.wdzSymbol));
            claims.Add(new Claim("wdzNazwa", uzytkownikViewModel.wdzNazwa));
            claims.Add(new Claim("stnKod", uzytkownikViewModel.stnKod));
            claims.Add(new Claim("stnNazwa", uzytkownikViewModel.stnNazwa));
            claims.Add(new Claim("usrLogin", $"{uzytkownikViewModel.usrNazwa}"));
            claims.Add(new Claim("usrEmail", $"{uzytkownikViewModel.usrEmail}"));
            claims.Add(new Claim("usrTelefon", $"{uzytkownikViewModel.usrTelefon}"));
            claims.Add(new Claim("usrKomorka", $"{uzytkownikViewModel.usrKomorka}"));
            claims.Add(new Claim("accessToken", $"{uzytkownikViewModel.usrToken}"));


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

        private UzytkownikViewModel? Authenticate(string usrNazwa, string usrHaslo, string usrComputer, string appID)
        {
            try
            {
                if (!int.TryParse(appID, out int applicationID))
                {
                    _logger.LogInformation($"User authenticate appID is invalid {appID}");
                    return null;
                }

                List<UzytkownikViewModel> uzytkownik = ReadLoginUsers(usrNazwa, usrHaslo, usrComputer, applicationID);

                if (uzytkownik != null)
                {
                    if (uzytkownik.Count == 1)
                    {
                        return uzytkownik.FirstOrDefault();
                    }
                    else
                    {
                        _logger.LogInformation("User exists more than one times");
                    }
                }
                else
                {
                    _logger.LogInformation("User object is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"User authenticate error {ex.Message}");
            }

            return null;
        }

        private UzytkownikViewModel? AuthenticateSso(string usrNazwa, string usrComputer, string appID)
        {
            try
            {
                if (!int.TryParse(appID, out int applicationID))
                {
                    _logger.LogInformation($"User authenticate SSO appID is invalid {appID}");
                    return null;
                }

                List<UzytkownikViewModel> uzytkownik = ReadSsoLoginUsers(usrNazwa, usrComputer, applicationID);

                if (uzytkownik != null)
                {
                    if (uzytkownik.Count == 1)
                    {
                        return uzytkownik.FirstOrDefault();
                    }
                    else
                    {
                        _logger.LogInformation("User exists more than one times (SSO)");
                    }
                }
                else
                {
                    _logger.LogInformation("User object is null (SSO)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"User authenticate SSO error {ex.Message}");
            }

            return null;
        }

        private List<UzytkownikViewModel> ReadSsoLoginUsers(string usrNazwa, string usrComputer, int appID)
        {
            var connection = _context.Database.GetDbConnection();
            var closeConnection = connection.State != ConnectionState.Open;
            var users = new List<UzytkownikViewModel>();

            if (closeConnection)
            {
                connection.Open();
            }

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "EXEC uspGetUzytkownikSso @usrNazwa, @sesKomputer, @appID";
                command.CommandType = CommandType.Text;

                AddParameter(command, "@usrNazwa", usrNazwa);
                AddParameter(command, "@sesKomputer", usrComputer);
                AddParameter(command, "@appID", appID);

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(MapLoginUser(reader));
                }
            }
            finally
            {
                if (closeConnection)
                {
                    connection.Close();
                }
            }

            return users;
        }

        private List<UzytkownikViewModel> ReadLoginUsers(string usrNazwa, string usrHaslo, string usrComputer, int appID)
        {
            var connection = _context.Database.GetDbConnection();
            var closeConnection = connection.State != ConnectionState.Open;
            var users = new List<UzytkownikViewModel>();

            if (closeConnection)
            {
                connection.Open();
            }

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "EXEC uspGetUzytkownik @usrNazwa, @usrHaslo, @usrKomputer, @appID";
                command.CommandType = CommandType.Text;

                AddParameter(command, "@usrNazwa", usrNazwa);
                AddParameter(command, "@usrHaslo", usrHaslo);
                AddParameter(command, "@usrKomputer", usrComputer);
                AddParameter(command, "@appID", appID);

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(MapLoginUser(reader));
                }
            }
            finally
            {
                if (closeConnection)
                {
                    connection.Close();
                }
            }

            return users;
        }

        private static void AddParameter(DbCommand command, string name, object? value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        private static UzytkownikViewModel MapLoginUser(DbDataReader reader)
        {
            return new UzytkownikViewModel
            {
                usrID = GetInt32(reader, "usrID") ?? 0,
                usrPracownikID = GetInt32(reader, "usrPracownikID"),
                usrNazwa = GetString(reader, "usrNazwa"),
                usrImie = GetString(reader, "usrImie"),
                usrNazwisko = GetString(reader, "usrNazwisko"),
                usrInicjal = GetString(reader, "usrInicjal"),
                usrTelefon = GetString(reader, "usrTelefon"),
                usrKomorka = GetString(reader, "usrKomorka"),
                usrEmail = GetString(reader, "usrEmail"),
                usrKlientID = GetInt32(reader, "usrKlientID"),
                usrDostawcaID = GetInt32(reader, "usrDostawcaID"),
                usrOdbiorcaID = GetInt32(reader, "usrOdbiorcaID"),
                usrSpedytorID = GetInt32(reader, "usrSpedytorID"),
                appID = GetInt32(reader, "appID") ?? 0,
                appFolder = GetString(reader, "appFolder"),
                usrFolder = GetString(reader, "usrFolder"),
                usrZdjecie = GetString(reader, "usrZdjecie"),
                wdzID = GetInt32(reader, "wdzID"),
                wdzKod = GetString(reader, "wdzKod"),
                wdzSymbol = GetString(reader, "wdzSymbol"),
                wdzNazwa = GetString(reader, "wdzNazwa"),
                stnKod = GetString(reader, "stnKod"),
                stnSymbol = GetString(reader, "stnSymbol"),
                stnNazwa = GetString(reader, "stnNazwa"),
                usrDataWaznoscHaslo = GetDateTime(reader, "usrDataWaznoscHaslo"),
                srlID = GetString(reader, "srlID"),
                srlNazwa = GetString(reader, "srlNazwa"),
                usrGuid = GetGuid(reader, "usrGuid"),
                usrToken = GetString(reader, "usrToken"),
                sesGuid = GetGuid(reader, "sesGuid")
            };
        }

        private static object? GetValue(DbDataReader reader, string fieldName)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
            }

            return null;
        }

        private static string? GetString(DbDataReader reader, string fieldName)
        {
            return GetValue(reader, fieldName)?.ToString();
        }

        private static int? GetInt32(DbDataReader reader, string fieldName)
        {
            var value = GetValue(reader, fieldName);

            if (value == null)
            {
                return null;
            }

            return int.TryParse(value.ToString(), out var result) ? result : null;
        }

        private static DateTime? GetDateTime(DbDataReader reader, string fieldName)
        {
            var value = GetValue(reader, fieldName);

            if (value is DateTime dateTime)
            {
                return dateTime;
            }

            return DateTime.TryParse(value?.ToString(), out var result) ? result : null;
        }

        private static Guid? GetGuid(DbDataReader reader, string fieldName)
        {
            var value = GetValue(reader, fieldName);

            if (value is Guid guidValue)
            {
                return guidValue;
            }

            var textValue = value?.ToString();
            var valueWithoutSeparators = textValue?.Length == 32
                ? textValue.Insert(20, "-").Insert(16, "-").Insert(12, "-").Insert(8, "-")
                : textValue;

            if (Guid.TryParse(valueWithoutSeparators, out Guid guid))
            {
                return guid;
            }

            return null;
        }
    }
}
