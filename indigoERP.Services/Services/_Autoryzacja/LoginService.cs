using indigoERP.Domain.Models._Autoryzacja;
using indigoERP.Domain.ViewModels._Autoryzacja;
using indigoERP.Service.Interfaces._Autoryzacja;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace indigoERP.Service.Services._Autoryzacja
{
    public class LoginService : ILoginService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<UzytkownikViewModel> Get(UzytkownikModel uzytkownikModel)
        {
            var httpClient = _httpClientFactory.CreateClient("indigoERPClient");

            try
            {
                var uzytkownik = await httpClient.GetFromJsonAsync<UzytkownikViewModel>($"Login/Get?usrNazwa={uzytkownikModel.usrNazwa}&usrHaslo={uzytkownikModel.usrHaslo}");

                return uzytkownik;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
