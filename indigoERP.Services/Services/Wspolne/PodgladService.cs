using indigoERP.Domain.ViewModels._Wspolne;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Wspolne;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace indigoERP.Service.Services._Wspolne
{
    public class PodgladService : IPodgladService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public PodgladService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<WiadomoscUzytkownikViewModel>> WiadomosciNieprzeczytane(int appID, int usrID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscUzytkownikViewModel>>(
                $"WiadomoscUzytkownik/SelectBrakOdczytByUzytkownik/{appID}/{usrID}?wiaStatus=*");

            return result ?? Enumerable.Empty<WiadomoscUzytkownikViewModel>();
        }

        public async Task<IEnumerable<ZadanieViewModel>> ZadaniaNieprzeczytane(int appID, int usrID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<ZadanieViewModel>>(
                $"Zadanie/SelectByUzytkownik/{appID}/{usrID}?tskStatus=*");

            // Endpoint zwraca wszystkie zadania użytkownika; podgląd pokazuje tylko nieprzeczytane (tusOdczyt = false).
            return (result ?? Enumerable.Empty<ZadanieViewModel>()).Where(z => z.tusOdczyt != true);
        }

        public async Task<IEnumerable<ZdarzenieViewModel>> ZdarzeniaNieprzeczytane(int appID, int usrID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<ZdarzenieViewModel>>(
                $"Zdarzenie/SelectBrakOdczytByUzytkownik/{appID}/{usrID}?zdaStatus=*");

            return result ?? Enumerable.Empty<ZdarzenieViewModel>();
        }
    }
}
