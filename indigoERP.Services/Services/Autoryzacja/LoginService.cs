using indigoERP.Domain.Models._Autoryzacja;
using indigoERP.Domain.ViewModels._Autoryzacja;
using indigoERP.Service.Interfaces._Autoryzacja;
using System.Net.Http.Json;

namespace indigoERP.Service.Services._Autoryzacja
{
    public class LoginService : ILoginService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
    }
}
