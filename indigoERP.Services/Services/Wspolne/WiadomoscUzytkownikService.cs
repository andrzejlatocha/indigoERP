using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Service.Extensions;
using indigoERP.Service.Interfaces._Wspolne;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace indigoERP.Service.Services._Wspolne
{
    public class WiadomoscUzytkownikService : IWiadomoscUzytkownikService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public WiadomoscUzytkownikService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<WiadomoscUzytkownikDto> Get(int wuzID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            return await httpClient.GetFromJsonAsync<WiadomoscUzytkownikDto>($"WiadomoscUzytkownik/Get/{wuzID}");
        }

        public async Task<ResponseMessage> Insert(WiadomoscUzytkownikDto wiadomoscUzytkownik)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(wiadomoscUzytkownik), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("WiadomoscUzytkownik/Insert", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newObject = JsonConvert.DeserializeObject<WiadomoscUzytkownikDto>(apiResponse);

                    return new ResponseMessage((int)response.StatusCode, apiResponse, newObject?.wuzID);
                }

                return new ResponseMessage(600, apiResponse, null);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }

        public async Task<ResponseMessage> Update(int wuzID, WiadomoscUzytkownikDto wiadomoscUzytkownik)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(wiadomoscUzytkownik), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"WiadomoscUzytkownik/Update/{wuzID}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, wuzID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, wuzID);
            }
        }

        public async Task<ResponseMessage> Delete(int wuzID)
        {
            var httpClient = await _httpClientFactory.CreateClientWithTokenAsync(_authenticationStateProvider);

            try
            {
                var response = await httpClient.DeleteAsync($"WiadomoscUzytkownik/Delete/{wuzID}");
                string apiResponse = await response.Content.ReadAsStringAsync();

                return new ResponseMessage((int)response.StatusCode, apiResponse, wuzID);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(600, ex.Message, null);
            }
        }
    }
}
