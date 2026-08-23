using indigoERP.Domain.Models._Autoryzacja;
using indigoERP.Domain.ViewModels._Autoryzacja;
using indigoERP.Service.Interfaces._Autoryzacja;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace indigoERP.Service.Services._Autoryzacja
{
    public class LoginService : ILoginService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public LoginService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<UzytkownikViewModel> Get(UzytkownikLoginModel uzytkownikModel)
        {
            var httpClient = _httpClientFactory.CreateClient("indigoApiClient");

            try
            {
                var uzytkownik = await httpClient.GetFromJsonAsync<UzytkownikViewModel>
                    ($"Login/Get?usrNazwa={uzytkownikModel.usrNazwa}&usrHaslo={uzytkownikModel.usrHaslo}&usrKomputer={uzytkownikModel.usrKomputer}&appID={uzytkownikModel.appID}");

                return uzytkownik;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UzytkownikViewModel> GetSso(string usrNazwa, string usrKomputer, string appID)
        {
            var httpClient = _httpClientFactory.CreateClient("indigoApiClient");
            httpClient.DefaultRequestHeaders.Add("X-Internal-Sso-Key", _configuration["InternalSso:Key"]);

            try
            {
                var uzytkownik = await httpClient.GetFromJsonAsync<UzytkownikViewModel>
                    ($"Login/GetSso?usrNazwa={Uri.EscapeDataString(usrNazwa)}&usrKomputer={Uri.EscapeDataString(usrKomputer)}&appID={appID}");

                return uzytkownik;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
