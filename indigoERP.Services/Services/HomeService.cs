using indigoERP.Domain.ViewModels;
using indigoERP.Service.Interfaces;
using System.Net.Http.Json;

namespace indigoERP.Service.Services
{
    public class HomeService : IHomeService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<AplikacjaViewModel>> SelectAplikacja(string appStatus)
        {
            var httpClient = _httpClientFactory.CreateClient("baseApiClient");
            var result = await httpClient.GetFromJsonAsync<IEnumerable<AplikacjaViewModel>>($"Home/SelectAplikacja?appStatus={appStatus}");

            return result ?? Enumerable.Empty<AplikacjaViewModel>();
        }

        public async Task<IEnumerable<AplikacjaViewModel>> SelectAplikacjaByUzytkownik(int usrID, string appStatus)
        {
            var httpClient = _httpClientFactory.CreateClient("baseApiClient");
            var result = await httpClient.GetFromJsonAsync<IEnumerable<AplikacjaViewModel>>(
                $"Home/SelectAplikacjaByUzytkownik?usrID={usrID}&appStatus={appStatus}");

            return result ?? Enumerable.Empty<AplikacjaViewModel>();
        }
    }
}
