using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Wspolne;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace indigoERP.Service.Services._Wspolne
{
    public class WiadomoscService : IWiadomoscService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public WiadomoscService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<IEnumerable<WiadomoscViewModel>> Select(int wiaAplikacjaID, string wiaStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscViewModel>>($"Wiadomosc/Select?wiaAplikacjaID={wiaAplikacjaID}&wiaStatus={wiaStatus}");

            return result ?? Enumerable.Empty<WiadomoscViewModel>();
        }

        public async Task<IEnumerable<WiadomoscHistoriaViewModel>> SelectHistoria(int wiaAplikacjaID, int? wiaUzytkownikID, string wiaStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var url = $"Wiadomosc/SelectHistoria?wiaAplikacjaID={wiaAplikacjaID}&wiaStatus={wiaStatus}";

            if (wiaUzytkownikID != null)
            {
                url += $"&wiaUzytkownikID={wiaUzytkownikID}";
            }

            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscHistoriaViewModel>>(url);

            return result ?? Enumerable.Empty<WiadomoscHistoriaViewModel>();
        }

        public async Task<IEnumerable<WiadomoscViewModel>> SelectByUzytkownik(int appID, int usrID, string wiaStatus)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<WiadomoscViewModel>>($"Wiadomosc/SelectByUzytkownik/{appID}/{usrID}?wiaStatus={wiaStatus}");

            return result ?? Enumerable.Empty<WiadomoscViewModel>();
        }

        public async Task<int> SelectCountByUzytkownik(int appID, int usrID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<int>($"Wiadomosc/SelectCountByUzytkownik/{appID}/{usrID}");
        }

        public async Task<WiadomoscViewModel?> Detail(int wiaID, int usrID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<WiadomoscViewModel>($"Wiadomosc/Detail/{wiaID}/{usrID}");
        }

        public async Task<WiadomoscDto> Get(int wiaID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<WiadomoscDto>($"Wiadomosc/Get/{wiaID}");
        }

        public async Task<ResponseMessage> Update(int wiaID, WiadomoscDto wiadomosc)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(wiadomosc), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"Wiadomosc/Update/{wiaID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, wiaID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, wiaID);
            }
        }

        public async Task<ResponseMessage> Insert(WiadomoscDto wiadomosc)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(wiadomosc), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("Wiadomosc/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<WiadomoscDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.wiaID);
                }

                return new ResponseMessage(600, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> Delete(int wiaID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"Wiadomosc/Delete/{wiaID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, wiaID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }
    }
}
