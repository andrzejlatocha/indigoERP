using indigoERP.Domain.ViewModels._Autoryzacja;
using indigoERP.Service.Interfaces;
using System.Net.Http.Json;

namespace indigoERP.Service.Services
{
    public class UzytkownikAplikacjaService : IUzytkownikAplikacjaService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UzytkownikAplikacjaService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<UzytkownikAplikacjaViewModel>> Select(int usrID)
        {
            var httpClient = _httpClientFactory.CreateClient("indigoApiClient");

            try
            {
                var aplikacja = await httpClient.GetFromJsonAsync<IEnumerable<UzytkownikAplikacjaViewModel>>($"UzytkownikAplikacja/Select?usrID={usrID}");

                return aplikacja;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
