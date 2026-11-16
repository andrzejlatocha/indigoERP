using indigoERP.Domain.ViewModels._Wspolne;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Wspolne;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace indigoERP.Service.Services._Wspolne
{
    // Liczniki z indigoERP.Api (SelectCountByUzytkownik). Błąd jednego licznika zwraca 0 i nie psuje nagłówka.
    public class LicznikNaglowkaService : ILicznikNaglowkaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public LicznikNaglowkaService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<LicznikNaglowkaViewModel> Pobierz(int appID, int usrID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return new LicznikNaglowkaViewModel
            {
                Wiadomosci = await PobierzLicznik(httpClient, "WiadomoscUzytkownik", appID, usrID),
                Zadania = await PobierzLicznik(httpClient, "Zadanie", appID, usrID),
                Zdarzenia = await PobierzLicznik(httpClient, "Zdarzenie", appID, usrID),
            };
        }

        private static async Task<int> PobierzLicznik(HttpClient httpClient, string kontroler, int appID, int usrID)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<int>($"{kontroler}/SelectCountByUzytkownik/{appID}/{usrID}");
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
